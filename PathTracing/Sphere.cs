﻿using System;
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
        public Material? material;

        public Sphere(Vector3 pos, float radius, string? material_name)
        {
            this.pos = pos;
            this.radius = radius;
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
