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
        Size winSize;

        Bitmap picBoxImage;




        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Path Tracing";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Size = new Size(1600, 791);
            this.BackColor = Color.FromArgb(38, 38, 38);
        }

        // Display render
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
            scene = new Scene();
            this.Text = "Path Tracing";
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
            // updating progress bar
            if (scene.rendering)
            {
                progressBar1.Value = (int)(scene.render_progress * 100);
                if (progressBar1.Value == 100)
                {
                    scene.rendering = false;
                    scene.SaveToFile("Renders\\render.bmp");
                }
            }
            else
            {   
                // Updating render settings
                scene.max_ray_reflections = (int)MaxReflectionsnumericUpDown.Value;
                scene.iteretions_per_render = (int)IterationsPerRendernumericUpDown.Value;
            }

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
            if (scene.etaChanged)
            {
                ETATextBox.Text = $"ETA: {TimeSpan.FromSeconds((long)scene.eta)}";
                scene.etaChanged = false;

                // Updating Title

                if (scene.rendering)
                {
                    this.Text = $"Path Tracing - {MathF.Round(scene.render_progress * 100.0f, 2)}% | ETA: {TimeSpan.FromSeconds((long)scene.eta)}";
                }
                else
                {
                    this.Text = $"Path Tracing - DONE in {TimeSpan.FromSeconds((long)scene.elapsed_time)}";
                }
            }
        }


        // Save render
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
                    path = $"Renders\\{FileNameTextBox.Text}";
                }
                else
                {
                    path = $"Renders\\{FileNameTextBox.Text}.bmp";
                }

                scene.SaveToFile(path);
            }
        }

        private void KeepImgUpdated_CheckedChanged(object sender, EventArgs e)
        {
            scene.keepImgUpdated = KeepImgUpdated.Checked;
        }

        // Load redner
        private void LoadButton_Click(object sender, EventArgs e)
        {
            string path;
            if (FileNameTextBox.Text.Contains("."))
            {
                path = $"Renders\\{FileNameTextBox.Text}";
            }
            else
            {
                path = $"Renders\\{FileNameTextBox.Text}.bmp";
            }
            scene.LoadFromFile(path);
        }
    }
}