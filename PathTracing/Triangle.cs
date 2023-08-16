using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PathTracing
{
    internal class Triangle
    {
        public Vector3 vertex_A;
        public Vector3 vertex_B;
        public Vector3 vertex_C;
        public Vector3 normal;

        public Triangle(Vector3 vertex_A, Vector3 vertex_B, Vector3 vertex_C, Vector3 normal)
        {
            this.vertex_A = vertex_A;
            this.vertex_B = vertex_B;
            this.vertex_C = vertex_C;
            this.normal = normal;
        }
    }
}
