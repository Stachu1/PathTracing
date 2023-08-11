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


        public void Load_materials()
        {
            materials = new List<Material>();
            List<string> materials_string_list = File.ReadAllText(@"Scene/Materials.txt").Replace("\n", "").Replace("\r", "").Split(';').ToList();
            foreach (string material_string in materials_string_list)
            {
                List<string> material_data_list = material_string.Split(':').ToList();
                
                string name = material_data_list[1];
                List<string> color_values = material_data_list[3].Split(',').ToList();
                int r = Convert.ToInt32(float.Parse(color_values[0], CultureInfo.InvariantCulture) * 255);
                int g = Convert.ToInt32(float.Parse(color_values[1], CultureInfo.InvariantCulture) * 255);
                int b = Convert.ToInt32(float.Parse(color_values[2], CultureInfo.InvariantCulture) * 255);
                Color color = Color.FromArgb(255, r, g, b);
                float shininess = float.Parse(material_data_list[5], CultureInfo.InvariantCulture);
                float transparency = float.Parse(material_data_list[7], CultureInfo.InvariantCulture);
                float refractivity = float.Parse(material_data_list[9], CultureInfo.InvariantCulture);
                float glow = float.Parse(material_data_list[11], CultureInfo.InvariantCulture);
                Material material = new Material(name, color, shininess, transparency, refractivity, glow);
                materials.Add(material);
            }
        }

        public void Load_spheres()
        {
            spheres = new List<Sphere>();
            List<string> spheres_string_list = File.ReadAllText(@"Scene/Spheres.txt").Replace("\n", "").Replace("\r", "").Split(';').ToList();
            foreach (string sphere_string in spheres_string_list)
            {
                List<string> sphere_data_list = sphere_string.Split(':').ToList();

                List<string> location_values = sphere_data_list[1].Split(',').ToList();
                Vector3 location = new Vector3(float.Parse(location_values[0], CultureInfo.InvariantCulture), float.Parse(location_values[1], CultureInfo.InvariantCulture), float.Parse(location_values[2], CultureInfo.InvariantCulture));
                float radius = float.Parse(sphere_data_list[3], CultureInfo.InvariantCulture);
                string material_name = sphere_data_list[5];
                foreach (Material material in materials)
                {
                    if (material.name == material_name)
                    {
                        Sphere sphere = new Sphere(location, radius, material);
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
                    Color color = Trace_ray(camera.rays[row, col]);
                    g.FillRectangle(new SolidBrush(color), col, row, 1, 1);
                }
            }
            render_progress = 1;
        }

        private Color Trace_ray(Ray ray, int max_reflections = 8)
        {
            /// Sphere intersection
            float intersection_distance = 0;
            Sphere closest_sphere = null;
            foreach (Sphere sphere in spheres)
            {
                float intersection_check = Check_for_ray_sphere_intersection(ray, sphere);
                if (intersection_check >= 0)
                {
                    float distance = -(Vector3.Dot(ray.direction, Vector3.Subtract(ray.location, sphere.location))) - (float)Math.Sqrt(intersection_check);
                    if (distance > 0)
                    {
                        if (distance < intersection_distance || intersection_distance == 0)
                        {
                            intersection_distance = distance;
                            closest_sphere = sphere;
                        }
                    }
                }
            }

            /// Ground
            if (ray.direction.Y < 0)
            {
                float distance = ray.location.Y / -ray.direction.Y;
                if (distance < intersection_distance || intersection_distance == 0)
                {
                    return Color.DarkCyan;
                }
            }

            if (closest_sphere != null)
            {
                return closest_sphere.material.color;
            }
            else
            {
                return Color.Black;
            }
        }

        private float Check_for_ray_sphere_intersection(Ray ray, Sphere sphere)
        {
            float a = (float)Math.Pow(Vector3.Dot(ray.direction, Vector3.Subtract(ray.location, sphere.location)), 2);
            float b = (float)Math.Pow(Vector3.Distance(ray.location, sphere.location), 2);
            float c = (float)Math.Pow(sphere.radius, 2);
            float intersection_check = a - (b - c);
            return intersection_check;
        }
    }
}
