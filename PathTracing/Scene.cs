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

        public bool imgChanged = false;
        public bool keepImgUpdated = true;

        public bool etaChanged = false;

        public double eta = 0;
        public double elapsed_time;

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
            Stopwatch st = new Stopwatch();
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

                        //Vector3 new_color = TraceRay(ray);
                        Vector3 old_color = img[row, col];

                        float weight = 1.0f / (total_iterations + 1);
                        Vector3 accum_average_color = old_color * (1 - weight) + new_color * weight;
                        img[row, col] = accum_average_color;
                    }
                });           
                total_iterations++;

                eta = (1.0 - render_progress) * (float)st.Elapsed.TotalSeconds / render_progress;

                elapsed_time = st.Elapsed.TotalSeconds;
                etaChanged = true;

                if (keepImgUpdated)
                {
                    img_to_show = ArrayToImage(img);
                    imgChanged = true;
                }
            }
            
            st.Reset();
            render_progress = 1;
            img_to_show = ArrayToImage(img);
            imgChanged = true;
            eta = 0;
            etaChanged = true;
        }

        private Vector3 TraceRay(Ray ray)
        {
            Vector3 ray_color = Vector3.One;
            Vector3 incoming_light = Vector3.Zero;

            for (int reflection = 0; reflection < max_ray_reflections; reflection++)
            {
                IntersectionInfo info = CalculateRayCollision(ray);
                if (info.isIntersecting)
                {
                    ray.pos = info.pos;
                    ray.dir = CalculateReflection(ray.dir, info.normal, info.material);

                    incoming_light += info.material.color * info.material.glow * ray_color;
                    ray_color *= info.material.color;

                }
                else
                {
                    break;
                }
            }

            return incoming_light;

            ///// Ground
            //if (ray.dir.Y < 0)
            //{
            //    float distance = ray.pos.Y / -ray.dir.Y;
            //    if (distance < intersection_distance || intersection_distance == 0)
            //    {
            //        return Color.DarkCyan;
            //    }
            //}
        }

        private Vector3 CalculateReflection(Vector3 ray_dir, Vector3 normal, Material material)
        {
            float x = RandomValueNormDistr();
            float y = RandomValueNormDistr();
            float z = RandomValueNormDistr();
            Vector3 vec = Vector3.Normalize(new Vector3(x, y, z));
            if (Math.Acos(Vector3.Dot(vec, normal)) > Math.PI / 3.0)
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
            info.isIntersecting = false;
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
                        if (distance < info.dis || !info.isIntersecting)
                        {
                            info.isIntersecting = true;
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
                imgChanged = true;
            }
        }
    }
}
