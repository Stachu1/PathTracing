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
        public string? path { get; set; }
        public string? material_name { get; set; }
        public Material? material;
        public Triangle[]? triangles;

        public Mesh(Vector3 pos, Vector3 rotation, float scale, string? path, string? material_name)
        {
            this.pos = pos;
            this.rotation = rotation * MathF.PI / 180f;
            this.scale = scale;
            this.material_name = material_name;
            this.path = path;
            this.path = path;
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

        public void LoadFromSTL(string path)
        {

            if (File.Exists(path))
            {
                Quaternion rotation = Quaternion.CreateFromYawPitchRoll(this.rotation.X, -this.rotation.Y, -this.rotation.Z);
                try
                {
                    using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
                    {
                        reader.BaseStream.Seek(80, SeekOrigin.Begin);
                        uint triangles_count = reader.ReadUInt32();

                        this.triangles = new Triangle[triangles_count];

                        Vector3 pos_max = new Vector3(-1e6f, -1e6f, -1e6f);
                        Vector3 pos_min = new Vector3(1e6f, 1e6f, 1e6f);


                        // Loading raw data
                        for (int i = 0; i < triangles_count; i++)
                        {
                            Vector3 norm = ReadVector3(reader);
                            Vector3 v1 = ReadVector3(reader);
                            Vector3 v2 = ReadVector3(reader);
                            Vector3 v3 = ReadVector3(reader);

                            // Skip attribute byte count (2 bytes)
                            reader.BaseStream.Seek(2, SeekOrigin.Current);

                            this.triangles[i] = new Triangle(v1, v2, v3, norm);

                            pos_max = Vector3.Max(Vector3.Max(Vector3.Max(v1, v2), v3), pos_max);
                            pos_min = Vector3.Min(Vector3.Min(Vector3.Min(v1, v2), v3), pos_min);
                        }

                        Vector3 offset = (pos_max + pos_min) / 2f;

                        // Center mesh
                        Parallel.For(0, triangles_count, i =>
                        {
                            this.triangles[i].vertex_A -= offset;
                            this.triangles[i].vertex_B -= offset;
                            this.triangles[i].vertex_C -= offset;
                        });

                        // Rotate mesh
                        Parallel.For(0, triangles_count, i =>
                        {
                            this.triangles[i].vertex_A = Vector3.Transform(this.triangles[i].vertex_A, rotation);
                            this.triangles[i].vertex_B = Vector3.Transform(this.triangles[i].vertex_B, rotation);
                            this.triangles[i].vertex_C = Vector3.Transform(this.triangles[i].vertex_C, rotation);
                            this.triangles[i].normal = Vector3.Transform(this.triangles[i].normal, rotation);
                        });

                        // Scale mesh
                        Parallel.For(0, triangles_count, i =>
                        {
                            this.triangles[i].vertex_A *= scale;
                            this.triangles[i].vertex_B *= scale;
                            this.triangles[i].vertex_C *= scale;
                        });

                        // Move mesh
                        Parallel.For(0, triangles_count, i =>
                        {
                            this.triangles[i].vertex_A += this.pos;
                            this.triangles[i].vertex_B += this.pos;
                            this.triangles[i].vertex_C += this.pos;
                        });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("File not found.");
            }
        }

        private Vector3 ReadVector3(BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float z = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector3(x, y, z);
        }
    }
}
