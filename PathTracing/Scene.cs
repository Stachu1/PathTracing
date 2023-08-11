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

namespace PathTracing
{
    internal class Scene
    {
        public Camera camera;
        public Image img;

        List<Material> materials = new List<Material>();
        List<Sphere> spheres = new List<Sphere>();

        public float render_progress = 0;

        public bool loaded = false;
        public bool rendering = false;
        public int total_iterations = 0;
        public int iteretions_per_render = 1;
        public int max_ray_reflections = 1;

        Random rnd = new Random();


        public void LoadMaterials()
        {
            materials = new List<Material>();
            List<string> materials_string_list = File.ReadAllText(@"Scene/Materials.txt").Replace("\n", "").Replace("\r", "").Split(';').ToList();
            foreach (string material_string in materials_string_list)
            {
                List<string> material_data_list = material_string.Split(':').ToList();
                
                string name = material_data_list[1];
                List<string> color_values = material_data_list[3].Split(',').ToList();
                float r = float.Parse(color_values[0], CultureInfo.InvariantCulture);
                float g = float.Parse(color_values[1], CultureInfo.InvariantCulture);
                float b = float.Parse(color_values[2], CultureInfo.InvariantCulture);
                Vector3 color = new Vector3(r, g, b);
                float shininess = float.Parse(material_data_list[5], CultureInfo.InvariantCulture);
                float transparency = float.Parse(material_data_list[7], CultureInfo.InvariantCulture);
                float refractivity = float.Parse(material_data_list[9], CultureInfo.InvariantCulture);
                float glow = float.Parse(material_data_list[11], CultureInfo.InvariantCulture);
                Material material = new Material(name, color, shininess, transparency, refractivity, glow);
                materials.Add(material);
            }
        }

        public void LoadSpheres()
        {
            spheres = new List<Sphere>();
            List<string> spheres_string_list = File.ReadAllText(@"Scene/Spheres.txt").Replace("\n", "").Replace("\r", "").Split(';').ToList();
            foreach (string sphere_string in spheres_string_list)
            {
                List<string> sphere_data_list = sphere_string.Split(':').ToList();

                List<string> pos_values = sphere_data_list[1].Split(',').ToList();
                Vector3 pos = new Vector3(float.Parse(pos_values[0], CultureInfo.InvariantCulture), float.Parse(pos_values[1], CultureInfo.InvariantCulture), float.Parse(pos_values[2], CultureInfo.InvariantCulture));
                float radius = float.Parse(sphere_data_list[3], CultureInfo.InvariantCulture);
                string material_name = sphere_data_list[5];
                foreach (Material material in materials)
                {
                    if (material.name == material_name)
                    {
                        Sphere sphere = new Sphere(pos, radius, material);
                        spheres.Add(sphere);
                        break;
                    }
                }
            }
        }

        public void Render()
        {
            Graphics g = Graphics.FromImage(img);
            for (int row = 0; row < camera.resolution.Height; row++)
            {
                render_progress = (float)row / (float)camera.resolution.Height;   
                for (int col = 0; col < camera.resolution.Width; col++)
                {
                    Color color = TraceRay(camera.rays[row, col]);
                    g.FillRectangle(new SolidBrush(color), col, row, 1, 1);
                }
            }
            render_progress = 1;
        }

        private Color TraceRay(Ray ray)
        {
            Vector3 ray_color = Vector3.One;
            Vector3 incoming_light = Vector3.Zero;

            for (int relection = 0; relection < max_ray_reflections; relection++)
            {
                IntersectionInfo info = CalculateRayCollision(ray);
                if (info.isIntersecting)
                {
                    ray.pos = info.pos;
                    ray.dir = CalculateReflection(ray.dir, info.normal, info.material);

                    if (info.material.glow > 0)
                    {
                        Vector3 emitted_light = info.material.color * info.material.glow;
                        incoming_light = emitted_light * ray_color;
                        break;
                    }

                    //vector3 emitted_light = info.material.color * info.material.glow;
                    //incoming_light += emitted_light * ray_color;
                    ray_color *= info.material.color;

                }
                else
                {
                    break;
                }
            }

            return Color.FromArgb((int)(incoming_light.X*255), (int)(incoming_light.Y * 255), (int)(incoming_light.Z * 255));

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
            return Vector3.Normalize(new Vector3(x, y, z));
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
                            info.material = sphere.material;
                        }
                    }
                }
            }

            return info;
        }
    }
}
