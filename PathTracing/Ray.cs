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
        public Vector3 location;
        public Vector3 direction;

        public Ray(Vector3 location, Vector3 direction)
        {
            this.location = location;
            this.direction = Vector3.Normalize(direction);
        }
    }
}
