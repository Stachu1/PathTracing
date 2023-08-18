using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;
using System.Numerics;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Timers;
using static System.Formats.Asn1.AsnWriter;
using static System.Windows.Forms.DataFormats;
using System.ComponentModel.Design.Serialization;
using System.DirectoryServices;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace PathTracing
{
    internal class Scene
    {
        public Camera camera;
        public Vector3[,] img_array;
        public Bitmap? img_to_show;

        public float render_progress = 0;

        public bool loaded = false;
        public bool rendering = false;
        public int total_iterations = 0;
        public int iteretions_per_render = 1;
        public int max_ray_reflections = 1;

        public bool abort_render = false;

        public bool img_changed = false;
        public bool keep_img_updated = true;

        public bool progressed = false;

        public double eta = 0;
        public double elapsed_time;

        public Stopwatch st = new Stopwatch();

        private Random rnd = new Random();

        private JsonSerializerOptions json_opt = new JsonSerializerOptions();

        private Material[] materials;
        private Sphere[] spheres;
        private Mesh[] meshes;

        public Scene()
        {
            json_opt.AllowTrailingCommas = true;
            json_opt.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            json_opt.Converters.Add(new Vector3Converter());
            json_opt.Converters.Add(new SizeConverter());

            LoadCamera();
            img_array = new Vector3[camera.resolution.Height, camera.resolution.Width];
            FillArray(ref img_array, Vector3.Zero);
            LoadMaterials();
            LoadSpheres();
            LoadSTLs();
            loaded = true;
        }

        private void LoadCamera()
        {
            var potential_camera = JsonSerializer.Deserialize<Camera>(File.ReadAllText(@"Scene/Camera.json"), json_opt);
            if (potential_camera != null)
            {
                camera = potential_camera;
            }
            else
            {
                throw new Exception("Failed to load camera!");
            }
        }

        private void LoadMaterials()
        {
            Material[] potential_materials = JsonSerializer.Deserialize<Material[]>(File.ReadAllText(@"Scene/Materials.json"), json_opt);
            if (potential_materials != null)
            {
                materials = potential_materials;
            }
            else
            {
                throw new Exception("Failed to load materials!");
            }
        }

        private void LoadSpheres()
        {
            if (!File.Exists(@"Scene/Spheres.json")) { return; }

            Sphere[] potential_spheres = JsonSerializer.Deserialize<Sphere[]>(File.ReadAllText(@"Scene/Spheres.json"), json_opt);
            if (potential_spheres != null)
            {
                spheres = potential_spheres;
                foreach (Sphere sphere in spheres)
                {
                    sphere.SetMaterial(sphere.material_name, materials);
                }
            } 
            else
            {
                throw new Exception("Failed to load spheres!");
            }
        }

        private void LoadSTLs()
        {
            Mesh[] potential_meshes = JsonSerializer.Deserialize<Mesh[]>(File.ReadAllText(@"Scene/STLs.json"), json_opt);
            if (potential_meshes != null)
            {
                meshes = potential_meshes;
                foreach (Mesh mesh in meshes)
                {
                    mesh.SetMaterial(mesh.material_name, materials);
                    mesh.LoadFromSTL(mesh.path);
                }
            }
            else
            {
                throw new Exception("Failed to load camera!");
            }
        }

        public void FillArray(ref Vector3[,] array, Vector3 value)
        {
            int num_rows = array.GetLength(0);
            int num_cols = array.GetLength(1);

            for (int row = 0; row < num_rows; row++)
            {
                for (int col = 0; col < num_cols; col++)
                {
                    array[row, col] = value;
                }
            }
        }

        public Vector3[,] ApplyGammaCorrection(Vector3[,] array, float gamma)
        {
            Vector3[,] array_clone = (Vector3[,])array.Clone();
            float inverter_gamma = 1f / gamma;
            int num_rows = array_clone.GetLength(0);
            int num_cols = array_clone.GetLength(1);
            Parallel.For(0, num_rows, row =>
            {
                for (int col = 0; col < num_cols; col++)
                {
                    array_clone[row, col] = new Vector3(
                        MathF.Pow(array_clone[row, col].X, inverter_gamma), 
                        MathF.Pow(array_clone[row, col].Y, inverter_gamma), 
                        MathF.Pow(array_clone[row, col].Z, inverter_gamma));
                }
            });
            return array_clone;
        }

        public Bitmap ArrayToImage(Vector3[,] array)
        {

            int num_rows = array.GetLength(0);
            int num_cols = array.GetLength(1);

            Bitmap bmp = new Bitmap(num_cols, num_rows);

            for (int row = 0; row < num_rows; row++)
            {
                for (int col = 0; col < num_cols; col++)
                {
                    Vector3 vec_color = array[row, col];

                    float max_value = vec_color.X;
                    if (vec_color.Y > max_value)
                    {
                        max_value = vec_color.Y;
                    }
                    if (vec_color.Z > max_value)
                    {
                        max_value = vec_color.Z;
                    }
                    if (max_value > 1)
                    {
                        vec_color = vec_color / max_value;
                    }

                    Color color = Color.FromArgb((int)(vec_color.X * 255.0f), (int)(vec_color.Y * 255.0f), (int)(vec_color.Z * 255.0f));
                    bmp.SetPixel(col, row, color);
                }
            }
            return bmp;
        }

        public void Render()
        {
            abort_render = false;
            elapsed_time = 0;

            if (camera == null) 
                return;

            render_progress = 0;
            for (int iteration = 0; iteration < iteretions_per_render; iteration++)
            {
                st.Start();
                Parallel.For(0, camera.resolution.Height, row =>
                {
                    for (int col = 0; col < camera.resolution.Width; col++)
                    {
                        // Stop the redner 
                        if (abort_render)
                        {
                            render_progress = 0;
                            total_iterations = 0;
                            progressed = false;
                            return;
                        }

                        Ray ray = camera.GetRay(row, col);

                        bool hit_transparent_maerial = false;
                        int additional_samples = 0;
                        Vector3 total_incoming_light = Vector3.Zero;
                        for (int ray_index = 0; ray_index < camera.samples_per_pixel; ray_index++)
                        {
                            Ray r = (Ray)ray.Clone();
                            r.Rotate(new Vector3(((float)rnd.NextDouble()*2-1)* camera.ray_deviation, ((float)rnd.NextDouble() * 2 - 1)*camera.ray_deviation, 0));
                            total_incoming_light += TraceRay(r, ref hit_transparent_maerial);
                        }

                        if (hit_transparent_maerial)
                        {
                            additional_samples = (int)((float)camera.samples_per_pixel * (camera.SPP_multiplier_for_transparent_materials - 1.0f));
                            for (int ray_index = 0; ray_index < additional_samples; ray_index++)
                            {
                                Ray r = (Ray)ray.Clone();
                                r.Rotate(new Vector3(((float)rnd.NextDouble() * 2 - 1) * camera.ray_deviation, ((float)rnd.NextDouble() * 2 - 1) * camera.ray_deviation, 0));
                                total_incoming_light += TraceRay(r, ref hit_transparent_maerial);
                            }
                        }

                        Vector3 new_color = total_incoming_light / (camera.samples_per_pixel + additional_samples);

                        Vector3 old_color = img_array[row, col];

                        float weight = 1.0f / (total_iterations + 1);
                        Vector3 accum_average_color = old_color * (1 - weight) + new_color * weight;
                        img_array[row, col] = accum_average_color;
                    }
                    render_progress += 1.0f / ((float)camera.resolution.Height * (float)iteretions_per_render);
                    progressed = true;
                });
                total_iterations++;
                
                elapsed_time = st.Elapsed.TotalSeconds;
                
                if (keep_img_updated)
                {
                    // Show mid render image without gamma correction
                    img_to_show = ArrayToImage(img_array);
                    img_changed = true;
                }
            }
            
            st.Reset();
            render_progress = 1;

            // Show final image with gamma correction
            img_to_show = ArrayToImage(ApplyGammaCorrection(img_array, camera.gamma));
            img_changed = true;
            eta = 0;
            progressed = true;
        }

        private Vector3 TraceRay(Ray ray, ref bool hit_transparent_maerial)
        {
            Vector3 ray_color = Vector3.One;
            Vector3 incoming_light = Vector3.Zero;

            for (int reflection = 0; reflection < max_ray_reflections + 1; reflection++)
            {
                IntersectionInfo hit = CalculateRayCollision(ray);
                if (hit.is_intersecting)
                {
                    // Check if the first object the ray hit is transparent
                    if (reflection == 0)
                    {
                        hit_transparent_maerial = hit.material.transparency > 0.0f;
                    }

                    ray.pos = hit.pos;

                    if (hit.material.specular_reflection_probability == 1.0f)
                    {
                        // Specular reflection with no color (Coated object)
                        ray.dir = SpecularReflection(ray.dir, hit.normal);
                        incoming_light += hit.material.color * hit.material.light_emission * ray_color;
                        continue;
                    }
                    
                    if (hit.material.specular_reflection_probability == 0.0f)
                    {
                        // Normar reflection based on the object material and its color
                        ray.dir = CalculateReflection(ray.dir, hit.normal, hit.material);
                        incoming_light += hit.material.color * hit.material.light_emission * ray_color;
                        ray_color *= hit.material.color;
                        continue;
                    }

                    if (hit.material.specular_reflection_probability > rnd.NextDouble())
                    {
                        // Specular reflection with no color (Coated object)
                        ray.dir = SpecularReflection(ray.dir, hit.normal);
                        incoming_light += hit.material.color * hit.material.light_emission * ray_color;
                    }
                    else
                    {
                        // Normar reflection based on the object material and its color
                        ray.dir = CalculateReflection(ray.dir, hit.normal, hit.material);
                        incoming_light += hit.material.color * hit.material.light_emission * ray_color;
                        ray_color *= hit.material.color;
                    }
                }
                else
                {
                    break;
                }
            }

            return incoming_light;
        }

        private Vector3 CalculateReflection(Vector3 ray_dir, Vector3 normal, Material material)
        {   
            if (material.transparency > (float)rnd.NextDouble())
            {
                return SpecularRefraction(ray_dir, normal, material.refractivity);
            }
            else
            {
                Vector3 diffuse_dir = DiffuseReflection(normal);
                Vector3 specular_dir = SpecularReflection(ray_dir, normal);
                return BlendVectors(diffuse_dir, specular_dir, material.smoothness);
            }
        }

        private Vector3 SpecularRefraction(Vector3 vec, Vector3 norm, float material_refractivity)
        {
            // Ray is exiting the object
            float dot = Vector3.Dot(vec, norm);
            float eta;
            float cos_theta1;


            // Ray is entering the object
            if (dot < 0)
            {
                eta = 1f / material_refractivity;
                cos_theta1 = Vector3.Dot(-vec, norm);
            }
            // Ray is exiting the object
            else
            {
                eta = material_refractivity;
                cos_theta1 = Vector3.Dot(vec, norm);
                norm = -norm;
            }

            float sin_theta2_sqr = eta * eta * (1 - cos_theta1 * cos_theta1);

            // Ray reflection
            if (sin_theta2_sqr > 1f)
            {
                return SpecularReflection(vec, norm);
            }

            // Ray refraction
            float cos_theta2 = MathF.Sqrt(1f - sin_theta2_sqr);
            return eta * vec + (eta * cos_theta1 - cos_theta2) * norm;
        }

        private Vector3 BlendVectors(Vector3 vec1, Vector3 vec2, float ratio)
        {
            return Vector3.Normalize(vec1 * (1 - ratio) + vec2 * ratio);
        }

        private Vector3 SpecularReflection(Vector3 vec, Vector3 norm)
        {
            return vec - 2 * Vector3.Dot(vec, norm) * norm;
        }

        private Vector3 DiffuseReflection(Vector3 norm)
        {
            Vector3 vec = Vector3.Normalize(new Vector3(RandomValueNormDistr(), RandomValueNormDistr(), RandomValueNormDistr()));
            if (Math.Acos(Vector3.Dot(vec, norm)) > Math.PI / 2.0)
            {
                vec = Vector3.Negate(vec);
            }
            return Vector3.Normalize(norm + vec);
        }

        private float RandomValueNormDistr()
        {
            float u1 = (float)rnd.NextDouble();
            float u2 = (float)rnd.NextDouble();

            return (float)(MathF.Sqrt(-2 * MathF.Log(u1)) * MathF.Sin(2 * MathF.PI * u2));
        }

        private IntersectionInfo CalculateRayCollision(Ray ray)
        {
            IntersectionInfo info = new IntersectionInfo();
            float distance;

            // Sphere intersection
            if (spheres != null)
            {
                foreach (Sphere sphere in spheres)
                {
                    float discriminant = GetDiscriminantForSphere(ray, sphere);
                    if (discriminant >= 0)
                    {
                        distance = GetDistanceToSphere(ray, sphere, discriminant);
                        if (distance >= 0)
                        {
                            if (distance < info.dis || !info.is_intersecting)
                            {
                                info.is_intersecting = true;
                                info.dis = distance;
                                info.pos = ray.pos + ray.dir * distance;
                                info.normal = Vector3.Normalize(info.pos - sphere.pos);
                                info.material = sphere.material;
                            }
                        }
                    }
                }
            }

            // Mesh intersection
            if (meshes != null)
            {
                foreach (Mesh mesh in meshes)
                {
                    if (mesh == null) continue;

                    foreach (Triangle triangle in mesh.triangles)
                    {
                        if (triangle == null) continue;

                        // If the triangle isn't transparent and the ray hit from behind it should skipped
                        if (mesh.material.transparency == 0.0f)
                        {
                            if (Vector3.Dot(triangle.normal, ray.dir) >= 0.0f) continue;
                        }

                        Vector3 edge_AB = triangle.vertex_B - triangle.vertex_A;
                        Vector3 edge_AC = triangle.vertex_C - triangle.vertex_A;

                        Vector3 cross_dir_edge2 = Vector3.Cross(ray.dir, edge_AC);
                        float determinant = Vector3.Dot(edge_AB, cross_dir_edge2);

                        if (determinant > -float.Epsilon && determinant < float.Epsilon) continue;

                        float inv_determinant = 1.0f / determinant;
                        Vector3 to_origin = ray.pos - triangle.vertex_A;
                        float u = Vector3.Dot(to_origin, cross_dir_edge2) * inv_determinant;

                        if (u < 0.0f || u > 1.0f) continue;

                        Vector3 cross_to_origin_edge1 = Vector3.Cross(to_origin, edge_AB);
                        float v = Vector3.Dot(ray.dir, cross_to_origin_edge1) * inv_determinant;

                        if (v < 0.0f || u + v > 1.0f) continue;

                        distance = Vector3.Dot(edge_AC, cross_to_origin_edge1) * inv_determinant;

                        if (distance <= float.Epsilon) continue;

                        if (distance < info.dis || !info.is_intersecting)
                        {
                            info.is_intersecting = true;
                            info.dis = distance;
                            info.pos = ray.pos + ray.dir * distance;
                            info.normal = triangle.normal;
                            info.material = mesh.material;
                        }
                    }
                }
            }
            

            return info;
        }

        private float GetDistanceToSphere(Ray ray, Sphere sphere, float discriminant)
        {
            return -(Vector3.Dot(ray.dir, Vector3.Subtract(ray.pos, sphere.pos))) - (float)Math.Sqrt(discriminant);
        }

        private float GetDiscriminantForSphere(Ray ray, Sphere sphere)
        {
            float a = MathF.Pow(Vector3.Dot(ray.dir, ray.pos - sphere.pos), 2);
            float b = MathF.Pow(Vector3.Distance(ray.pos, sphere.pos), 2);
            float c = MathF.Pow(sphere.radius, 2);
            return a - (b - c);
        }

        public void SaveToFile(string path, Vector3[,] array)
        {
            using (Bitmap bmp = ArrayToImage(array))
            {
                bmp.Save(path);
            }
        }

        public void LoadFromFile(string path)
        {
            if (File.Exists(path))
            {
                using (Bitmap bmp_og = new Bitmap(path))
                {

                    using (Bitmap bmp = new Bitmap(bmp_og, camera.resolution))
                    {

                        for (int row = 0; row < bmp.Size.Height; row++)
                        {
                            for (int col = 0; col < bmp.Size.Width; col++)
                            {
                                Color pixel_color = bmp.GetPixel(col, row);
                                Vector3 vec_color = new Vector3((float)pixel_color.R / 255.0f, (float)pixel_color.G / 255.0f, (float)pixel_color.B / 255.0f);
                                img_array[row, col] = vec_color;
                            }
                        }
                    }
                }
                string[] ti = path.Split("TI")[0].Split("_");
                int.TryParse(ti[ti.Length - 1], out total_iterations);
                img_to_show = ArrayToImage(ApplyGammaCorrection(img_array, camera.gamma));
                img_changed = true;
            }
        }
    }
}
