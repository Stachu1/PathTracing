using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace PathTracing
{
    internal class Camera
    {
        public Vector3 pos { get; set; }
        public Vector3 rotation { get; set; }
        public Size resolution { get; set; }
        public float FOV { get; set; }
        public float gamma { get; set; }
        public float ray_deviation { get; set; }
        public int samples_per_pixel { get; set; }

        public float aspect_ratio;
        public float near_clip_plane;

        public Camera(Vector3 pos, Vector3 rotation, Size resolution, float FOV, float gamma, float ray_deviation, int samples_per_pixel)
        {
            this.pos = pos;
            this.rotation = rotation * MathF.PI / 180f;
            this.resolution = resolution;
            this.FOV = FOV * MathF.PI / 180f;
            this.gamma = gamma;
            this.ray_deviation = ray_deviation;
            this.samples_per_pixel = samples_per_pixel;

            this.aspect_ratio = (float)resolution.Width / (float)resolution.Height;
            this.near_clip_plane = (resolution.Width / 2) / MathF.Tan(this.FOV / 2);
        }

        public Ray GetRay(int row, int col)
        {
            Ray ray = new Ray(pos, new Vector3(col - resolution.Width / 2, resolution.Height / 2 - row, near_clip_plane));
            return (Ray)ray.Rotate(rotation);
        }
    }
}
