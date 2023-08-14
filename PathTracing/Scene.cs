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

namespace PathTracing
{
    internal class Scene
    {
        public Camera camera;
        public Vector3[,] img;
        public Bitmap? img_to_show;

        public float render_progress = 0;

        public bool loaded = false;
        public bool rendering = false;
        public int total_iterations = 0;
        public int iteretions_per_render = 1;
        public int max_ray_reflections = 1;

        public bool img_changed = false;
        public bool keep_img_updated = true;

        public bool progressed = false;

        public double eta = 0;
        public double elapsed_time;

        public Stopwatch st = new Stopwatch();

        private Random rnd = new Random();

        private JsonSerializerOptions json_opt = new JsonSerializerOptions();

        private Dictionary<string, Material> materials;
        private List<Sphere> spheres;

        public Scene()
        {
            json_opt.AllowTrailingCommas = true;
            json_opt.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            json_opt.Converters.Add(new Vector3Converter());
            json_opt.Converters.Add(new SizeConverter());

            LoadCamera();
            img = new Vector3[camera.resolution.Height, camera.resolution.Width];
            FillArray(ref img, Vector3.Zero);
            LoadMaterials();
            LoadSpheres();
            loaded = true;
        }

        public void LoadCamera()
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

        public void LoadMaterials()
        {
            var potential_materials = JsonSerializer.Deserialize<Dictionary<string, Material>>(File.ReadAllText(@"Scene/Materials.json"), json_opt);
            if (potential_materials != null)
            {
                materials = potential_materials;
            }
            else
            {
                throw new Exception("Failed to load materials!");
            }
        }

        public void LoadSpheres()
        {
            var potential_spheres = JsonSerializer.Deserialize<List<Sphere>>(File.ReadAllText(@"Scene/Spheres.json"), json_opt);
            if (potential_spheres != null)
            {
                spheres = potential_spheres;
            } 
            else
            {
                throw new Exception("Failed to load spheres!");
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
            elapsed_time = 0;

            if (camera == null) 
                return;

            render_progress = 0;
            for (int iteration = 0; iteration < iteretions_per_render; iteration++)
            {
                st.Start();
                Parallel.For(0, camera.resolution.Height, row =>
                {
                    render_progress += 1.0f / ((float)camera.resolution.Height * (float)iteretions_per_render);
                    progressed = true;
                    for (int col = 0; col < camera.resolution.Width; col++)
                    {
                        Ray ray = camera.GetRay(row, col);

                        Vector3 total_incoming_light = Vector3.Zero;
                        for (int ray_index = 0; ray_index < camera.samples_per_pixel; ray_index++)
                        {
                            var r = (Ray)ray.Clone();
                            r.Rotate(new Vector3(((float)rnd.NextDouble()*2-1)* camera.ray_deviation, ((float)rnd.NextDouble() * 2 - 1)*camera.ray_deviation, 0));
                            total_incoming_light += TraceRay(r);
                        }
                        Vector3 new_color = total_incoming_light / camera.samples_per_pixel;

                        Vector3 old_color = img[row, col];

                        float weight = 1.0f / (total_iterations + 1);
                        Vector3 accum_average_color = old_color * (1 - weight) + new_color * weight;
                        img[row, col] = accum_average_color;
                    } 
                });
                total_iterations++;
                
                elapsed_time = st.Elapsed.TotalSeconds;
                
                if (keep_img_updated)
                {
                    img_to_show = ArrayToImage(img);
                    img_changed = true;
                }
            }
            
            st.Reset();
            render_progress = 1;
            img_to_show = ArrayToImage(img);
            img_changed = true;
            eta = 0;
            progressed = true;
        }

        private Vector3 TraceRay(Ray ray)
        {
            Vector3 ray_color = Vector3.One;
            Vector3 incoming_light = Vector3.Zero;

            for (int reflection = 0; reflection < max_ray_reflections + 1; reflection++)
            {
                IntersectionInfo hit = CalculateRayCollision(ray);
                if (hit.is_intersecting)
                {
                    ray.pos = hit.pos;
                    ray.dir = CalculateReflection(ray.dir, hit.normal, hit.material);

                    incoming_light += hit.material.color * hit.material.glow * ray_color;
                    ray_color *= hit.material.color * MathF.Abs(Vector3.Dot(ray.dir, hit.normal));
                    //ray_color *= hit.material.color;
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
                return BlendVectors(diffuse_dir, specular_dir, material.shininess);
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
            Vector3 refracted = eta * vec + (eta * cos_theta1 - cos_theta2) * norm;
            return Vector3.Normalize(refracted);
        }

        private Vector3 BlendVectors(Vector3 vec1, Vector3 vec2, float ratio)
        {
            return Vector3.Normalize(vec1 * (1 - ratio) + vec2 * ratio);
        }

        private Vector3 SpecularReflection(Vector3 vec, Vector3 norm)
        {
            return Vector3.Normalize(vec - 2 * Vector3.Dot(vec, norm) * norm);
        }

        private Vector3 DiffuseReflection(Vector3 norm)
        {
            Vector3 vec = Vector3.Normalize(new Vector3(RandomValueNormDistr(), RandomValueNormDistr(), RandomValueNormDistr()));
            if (Math.Acos(Vector3.Dot(vec, norm)) > Math.PI / 2.0)
            {
                vec = Vector3.Negate(vec);
            }
            return vec;
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

            // Sphere intersection
            info.is_intersecting = false;
            info.dis = 0;

            foreach (Sphere sphere in spheres)
            {
                float a = MathF.Pow(Vector3.Dot(ray.dir, ray.pos - sphere.pos), 2);
                float b = MathF.Pow(Vector3.Distance(ray.pos, sphere.pos), 2);
                float c = MathF.Pow(sphere.radius, 2);
                float discriminant = a - (b - c);

                if (discriminant >= 0)
                {
                    float distance = -(Vector3.Dot(ray.dir, Vector3.Subtract(ray.pos, sphere.pos))) - (float)Math.Sqrt(discriminant);
                    if (distance >= 0)
                    {
                        if (distance < info.dis || !info.is_intersecting)
                        {
                            info.is_intersecting = true;
                            info.dis = distance;
                            info.pos = ray.pos + ray.dir * distance;
                            info.normal = Vector3.Normalize(info.pos - sphere.pos);
                            info.material = materials[sphere.material_name];
                        }
                    }
                }
            }

            return info;
        }

        public void SaveToFile(string path)
        {
            using (Bitmap bmp = ArrayToImage(img))
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
                                img[row, col] = vec_color;
                            }
                        }
                    }
                }
                img_to_show = ArrayToImage(img);
                img_changed = true;
            }
        }
    }
}
