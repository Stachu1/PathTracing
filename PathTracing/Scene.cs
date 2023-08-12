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

namespace PathTracing
{
    internal class Scene
    {
        public Camera camera;
        public Vector3[,] img;
        public Bitmap img_to_show;

        Dictionary<string, Material> materials = new Dictionary<string, Material>();
        List<Sphere> spheres = new List<Sphere>();

        public float render_progress = 0;

        public bool loaded = false;
        public bool rendering = false;
        public int total_iterations = 0;
        public int iteretions_per_render = 1;
        public int max_ray_reflections = 1;

        public bool imgChanged = false;
        public bool keepImgUpdated = true;

        public bool etaUpdated = false;

        public double eta = 0;

        Random rnd = new Random();

        public void LoadMaterials()
        {

            JsonSerializerOptions opt = new JsonSerializerOptions();
            opt.AllowTrailingCommas = true;
            opt.Converters.Add(new Vector3Converter());

            var potential_materials = JsonSerializer.Deserialize<Dictionary<string, Material>>(File.ReadAllText(@"Scene/Materials.json"), opt);
            if (potential_materials != null)
                materials = potential_materials;
            else
                materials.Clear();
        }

        public void LoadSpheres()
        {
            JsonSerializerOptions opt = new JsonSerializerOptions();
            opt.AllowTrailingCommas = true;
            opt.Converters.Add(new Vector3Converter());

            var potential_spheres = JsonSerializer.Deserialize<List<Sphere>>(File.ReadAllText(@"Scene/Spheres.json"), opt);
            if (potential_spheres != null)
                spheres = potential_spheres;
            else
                spheres.Clear();
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

            render_progress = 0;
            for (int iteration = 0; iteration < iteretions_per_render; iteration++)
            {
                st.Start();

                Parallel.For(0, camera.resolution.Height, row =>
                {
                    render_progress += 1.0f / ((float)camera.resolution.Height * (float)iteretions_per_render);
                    Parallel.For(0, camera.resolution.Width, col =>
                    {
                        Ray ray = camera.GetRay(row, col);

                        //int raysPerPixlRender = 30;
                        //Vector3 total_incoming_light = Vector3.Zero;
                        //for (int ray_index = 0; ray_index < raysPerPixlRender; ray_index++)
                        //{
                        //    total_incoming_light += TraceRay((Ray) ray.Clone());
                        //}
                        //Vector3 new_color = total_incoming_light / raysPerPixlRender;

                        Vector3 new_color = TraceRay(ray);
                        Vector3 old_color = img[row, col];

                        float weight = 1.0f / (total_iterations + 1);
                        Vector3 accum_average_color = old_color * (1 - weight) + new_color * weight;
                        img[row, col] = accum_average_color;
                    });
                });
                st.Stop();
                
                total_iterations++;

                eta = st.Elapsed.TotalSeconds * (double)(iteretions_per_render - iteration);

                st.Reset();

                etaUpdated = true;

                if (keepImgUpdated)
                {
                    img_to_show = ArrayToImage(img);
                    imgChanged = true;
                }
            }
            render_progress = 1;
            eta = 0;
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
            double u1 = rnd.NextDouble();
            double u2 = rnd.NextDouble();

            return (float)(Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2));
        }

        private IntersectionInfo CalculateRayCollision(Ray ray)
        {
            IntersectionInfo info = new IntersectionInfo();

            /// Sphere intersection
            info.isIntersecting = false;
            info.dis = 0;

            foreach (Sphere sphere in spheres)
            {
                float a = (float)Math.Pow(Vector3.Dot(ray.dir, Vector3.Subtract(ray.pos, sphere.pos)), 2);
                float b = (float)Math.Pow(Vector3.Distance(ray.pos, sphere.pos), 2);
                float c = (float)Math.Pow(sphere.radius, 2);
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

    class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                float[] values = JsonSerializer.Deserialize<float[]>(ref reader, options);
                return new Vector3(values[0], values[1], values[2]);
            }

            throw new JsonException("Invalid Vector3 format.");
        }

        public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteNumberValue(value.Z);
            writer.WriteEndArray();
        }
    }

}
