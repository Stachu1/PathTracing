using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PathTracing
{
    internal class Rectangle
    {
        public Vector3 pos;
        public Vector3 vertex_A;
        public Vector3 vertex_B;
        public Vector3 vertex_C;
        public Vector3 vertex_D;
        public Vector3 normal;

        public Triangle triangle_1;
        public Triangle triangle_2;

        public Rectangle(Vector3 pos, Vector3 u, Vector3 v, Vector3 normal)
        {
            this.pos = pos;
            this.vertex_A = pos - u + v;
            this.vertex_B = pos + u + v;
            this.vertex_C = pos + u - v;
            this.vertex_D = pos - u - v;
            this.normal = normal;

            this.triangle_1 = new Triangle(this.vertex_A, vertex_B, vertex_C, normal);
            this.triangle_2 = new Triangle(this.vertex_A, vertex_D, vertex_C, normal);
        }
    }
}
