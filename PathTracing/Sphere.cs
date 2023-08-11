using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;


namespace PathTracing
{
    internal class Sphere
    {
        public Vector3 pos;
        public float radius;
        public Material material;

        public Sphere(Vector3 pos, float radius, Material material)
        {
            this.pos = pos;
            this.radius = radius;
            this.material = material;
        }
    }
}
