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
        public string? name { get; set; }
        public Vector3 color { get; set; }
        public float smoothness { get; set; }
        public float specular_reflection_probability { get; set; }
        public float transparency { get; set; }
        public float refractive_index { get; set; }
        public float light_emission { get; set; }
    }
}
