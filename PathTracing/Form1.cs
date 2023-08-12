using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Numerics;
using System.IO;

namespace PathTracing
{
    public partial class Form1 : Form
    {
        Scene scene = new Scene();
        Vector3 cam_pos = new Vector3(0, 10, 0);
        Vector3 cam_dir = new Vector3(0, 0, -1);

        Size cam_resolution = new Size(400, 225);
        //Size cam_resolution = new Size(1600, 900);
        //Size cam_resolution = new Size(2560, 1440);
        //Size cam_resolution = new Size(3200, 1800);
        float FOV = (float)Math.PI / 2;
        float gamma = 2.2F;

        



        public Form1()
        {
            InitializeComponent();

            scene.camera = new Camera(cam_pos, cam_dir, cam_resolution, FOV, gamma);
            scene.total_iterations = 0;
            scene.img = new Vector3[cam_resolution.Height, cam_resolution.Width];
            scene.FillArray(ref scene.img, Vector3.Zero);
            scene.LoadMaterials();
            scene.LoadSpheres();
            scene.loaded = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Path Tracing";
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(1600, 791);
            //pictureBox1.Size = new Size(1280, 720);
            //pictureBox1.Location = new Point(288, 12);
            this.BackColor = Color.FromArgb(38, 38, 38);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {   
            if (pictureBox1.Width < 2 ||  pictureBox1.Height < 2) return;

            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            g.DrawRectangle(new Pen(Color.White, 1), 0, 0, pictureBox1.Width-1, pictureBox1.Height-1);

            if (scene.img_to_show != null)
            {
                g.DrawImage(new Bitmap(scene.img_to_show, pictureBox1.Width - 2, pictureBox1.Height - 2), 1, 1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scene.total_iterations = 0;
            scene.img = new Vector3[cam_resolution.Height, cam_resolution.Width];
            scene.FillArray(ref scene.img, Vector3.Zero);
            scene.LoadMaterials();
            scene.LoadSpheres();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (scene.loaded && !scene.rendering)
            {
                Thread thread = new Thread(scene.Render);
                thread.Start();
                scene.rendering = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (scene.rendering)
            {
                progressBar1.Value = (int)(scene.render_progress * 100);
                if (progressBar1.Value == 100)
                {
                    scene.rendering = false;
                }
            }

            scene.max_ray_reflections = (int)numericUpDown1.Value;
            scene.iteretions_per_render = (int)numericUpDown2.Value;
            textBox4.Text = "Total iterations: " + Convert.ToString(scene.total_iterations);
            pictureBox1.Invalidate();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!scene.rendering && scene.loaded)
            {
                if (!Directory.Exists("Renders"))
                {
                    Directory.CreateDirectory("Renders");
                }
                if (textBox1.Text.Contains("."))
                {
                    scene.img_to_show.Save("Renders\\" + textBox1.Text);
                }
                else
                {
                    scene.img_to_show.Save("Renders\\" + textBox1.Text + ".bmp");
                }
            }
        }
    }
}
