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
        public Vector3 pos { get; set; }
        public float radius { get; set; }
        public string? material_name { get; set; }
    }
}
