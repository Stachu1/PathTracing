using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace PathTracing
{
    internal class Material
    {
        public Vector3 color { get; set; }
        public float shininess { get; set; }
        public float transparency { get; set; }
        public float refractivity { get; set; }
        public float glow { get; set; }
    }
}
