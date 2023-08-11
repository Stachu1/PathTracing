using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace PathTracing
{
    internal class Camera
    {
        public Vector3 pos;
        public Vector3 dir;
        public Size resolution;
        public float FOV;
        public float gamma;
        public Ray[,] rays;

        public Camera(Vector3 pos, Vector3 dir, Size resolution, float FOV, float gamma)
        {
            this.pos = pos;
            this.dir = dir;
            this.resolution = resolution;
            this.FOV = FOV;
            this.gamma = gamma;
        }

        public void Load()
        {
            rays = new Ray[resolution.Height, resolution.Width];

            //float z = (float)Math.Cos(FOV / 2) * resolution.Width / 2;
            //tan a = (x / 2) / z
            //z = (x / 2) * cot
            float z = -(resolution.Width / 2) / (float)Math.Tan(FOV / 2);

            for (int row = 0; row < resolution.Height; row++)
            {
                for (int col = 0; col < resolution.Width; col++)
                {
                    rays[row, col] = new Ray(pos, new Vector3(col - resolution.Width / 2, -row + resolution.Height / 2, z));
                }
            }
        }
    }
}
