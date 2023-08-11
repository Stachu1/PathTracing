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
        public string name;
        public Vector3 color;
        public float shininess;
        public float transparency;
        public float refractivity;
        public float glow;

        public Material(string name, Vector3 color, float shininess, float transparency, float refractivity, float glow)
        {
            this.name = name;
            this.color = color;
            this.shininess = shininess;
            this.transparency = transparency;
            this.refractivity = refractivity;
            this.glow = glow;
        }
    }
}
