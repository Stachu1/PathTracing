using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathTracing
{
    internal class IntersectionInfo
    {
        public bool is_intersecting;
        public float dis;
        public Vector3 pos;
        public Vector3 normal;
        public Material? material;
    }
}
