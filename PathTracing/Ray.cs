using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace PathTracing
{
    internal class Ray
    {
        public Vector3 pos;
        public Vector3 dir;

        public Ray(Vector3 pos, Vector3 dir)
        {
            this.pos = pos;
            this.dir = Vector3.Normalize(dir);
        }
    }
}
