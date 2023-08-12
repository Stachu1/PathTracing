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

        Size winSize;

        Bitmap picBoxImage;


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
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Size = new Size(1600, 791);
            this.BackColor = Color.FromArgb(38, 38, 38);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Width < 2 || pictureBox1.Height < 2) return;

            Graphics g = e.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            g.DrawRectangle(new Pen(Color.White, 1), 0, 0, pictureBox1.Width - 1, pictureBox1.Height - 1);

            if (scene.img_to_show != null)
            {
                using (picBoxImage = new Bitmap(scene.img_to_show, pictureBox1.Width - 2, pictureBox1.Height - 2))
                {
                    g.DrawImage(picBoxImage, 1, 1);
                }
            }

            scene.imgChanged = false;
        }


        // Reset
        private void ResetButton_Click(object sender, EventArgs e)
        {
            scene.total_iterations = 0;
            scene.img = new Vector3[cam_resolution.Height, cam_resolution.Width];
            scene.FillArray(ref scene.img, Vector3.Zero);
            scene.LoadMaterials();
            scene.LoadSpheres();
        }


        // Render
        private void RenderButton_Click(object sender, EventArgs e)
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
            scene.max_ray_reflections = (int)MaxReflectionsnumericUpDown.Value;
            scene.iteretions_per_render = (int)IterationsPerRendernumericUpDown.Value;
            IterationsTextBox.Text = "Total iterations: " + Convert.ToString(scene.total_iterations);

            // Updating image on windows size change
            if (this.Size != winSize)
            {
                winSize = this.Size;
                scene.imgChanged = true;
            }

            // Updating image
            if (scene.imgChanged)
            {
                pictureBox1.Invalidate();
            }

            // Updating ETA
            if (scene.etaUpdated)
            {
                ETATextBox.Text = $"ETA: {TimeSpan.FromSeconds((long)scene.eta)}";
                scene.etaUpdated = false;
            }
        }


        // Save
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!scene.rendering && scene.loaded)
            {
                if (!Directory.Exists("Renders"))
                {
                    Directory.CreateDirectory("Renders");
                }
                string path;
                if (FileNameTextBox.Text.Contains("."))
                {
                    path = "Renders\\" + FileNameTextBox.Text;
                }
                else
                {
                    path = "Renders\\" + FileNameTextBox.Text + ".bmp";
                }

                scene.SaveToFile(path);
            }
        }

        private void KeepImgUpdated_CheckedChanged(object sender, EventArgs e)
        {
            scene.keepImgUpdated = KeepImgUpdated.Checked;
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            string path;
            if (FileNameTextBox.Text.Contains("."))
            {
                path = "Renders\\" + FileNameTextBox.Text;
            }
            else
            {
                path = "Renders\\" + FileNameTextBox.Text + ".bmp";
            }
            scene.LoadFromFile(path);
        }
    }
}