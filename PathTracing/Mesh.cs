using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PathTracing
{
    internal class Mesh
    {
        public Vector3 pos { get; set; }
        public Vector3 rotation { get; set; }
        public float scale { get; set; }
        public string? material_name { get; set; }
        public Material? material;
        public Triangle[]? raw_triangles;
        public Triangle[]? triangles;

        public Mesh(Vector3 pos, Vector3 rotation, float scale, string? material_name)
        {
            this.pos = pos;
            this.rotation = rotation;
            this.scale = scale;
            this.material_name = material_name;
        }

        public void SetMaterial(string material_name, Material[] materials)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].name == material_name)
                {
                    this.material = materials[i];
                    return;
                }
            }
            throw new Exception($"No material named {material_name} was found!");
        }
    }
}
