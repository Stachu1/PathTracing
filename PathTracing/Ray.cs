using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;

namespace PathTracing
{
    internal class Ray: ICloneable
    {
        public Vector3 pos;
        public Vector3 dir;

        public Ray(Vector3 pos, Vector3 dir)
        {
            this.pos = pos;
            this.dir = Vector3.Normalize(dir);
        }

        public object Clone()
        {
            return new Ray(pos, dir);
        }

        public object Rotate(Vector3 angles)
        {
            Quaternion rotation = Quaternion.CreateFromYawPitchRoll(angles.X, -angles.Y, -angles.Z);
            this.dir = Vector3.Normalize(Vector3.Transform(this.dir, rotation));
            return this;
        }
    }
}
