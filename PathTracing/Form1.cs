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

namespace PathTracing
{
    public partial class Form1 : Form
    {
        Scene scene = new Scene();
        Vector3 cam_location = new Vector3(0, 10, 0);
        Vector3 cam_direction = new Vector3(0, 0, -1);
        //Size cam_resolution = new Size(400, 225);
        //Size cam_resolution = new Size(1600, 900);
        //Size cam_resolution = new Size(2560, 1440);
        Size cam_resolution = new Size(3200, 1800);
        float FOV = (float)Math.PI / 2;
        float gamma = 2.2F;

        



        public Form1()
        {
            InitializeComponent();
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
            g.DrawRectangle(new Pen(Color.White, 1), 0, 0, pictureBox1.Width-1, pictureBox1.Height-1);

            if (scene.img != null)
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(new Bitmap(scene.img, pictureBox1.Width-2, pictureBox1.Height-2), 1, 1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scene.camera = new Camera(cam_location, cam_direction, cam_resolution, FOV, gamma);
            scene.camera.Load();
            scene.img = new Bitmap(cam_resolution.Width, cam_resolution.Height);
            Graphics g = Graphics.FromImage(scene.img);
            g.FillRectangle(new SolidBrush(Color.Black), 0, 0, cam_resolution.Width, cam_resolution.Height);
            scene.Load_materials();
            scene.Load_spheres();
            progressBar1.Value = 100;
            scene.loaded = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (scene.loaded)
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
                progressBar2.Value = (int)(scene.render_progress * 100);
                if (progressBar2.Value == 100)
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
                if (textBox1.Text.Contains("."))
                {
                    scene.img.Save(textBox1.Text);
                }
                else
                {
                    scene.img.Save(textBox1.Text + ".bmp");
                }
            }
        }
    }
}
