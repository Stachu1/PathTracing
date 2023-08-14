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

        Thread? thread = null;

        const float WINDOW_SIZE_PERCENTAGE = 0.5f;   // Percentage of primary screen width

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Path Tracing";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Size = GetDeafultWindowSize(WINDOW_SIZE_PERCENTAGE, scene.camera.aspect_ratio);
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

            scene.img_changed = false;
        }


        // Reset
        private void ResetButton_Click(object sender, EventArgs e)
        {
            scene = new Scene();
            this.Text = "Path Tracing";
            this.Size = GetDeafultWindowSize(WINDOW_SIZE_PERCENTAGE, scene.camera.aspect_ratio);
        }


        // Render
        private void RenderButton_Click(object sender, EventArgs e)
        {
            if (scene.loaded && !scene.rendering)
            {
                thread = new Thread(scene.Render);
                thread.Start();
                scene.rendering = true;
                this.Text = $"Path Tracing - {MathF.Round(scene.render_progress * 100.0f, 2)}% | ETA: -:-:-";
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
                    // Auto save with no gamma correction
                    scene.SaveToFile($"Renders\\NoGamma_{scene.camera.samples_per_pixel * scene.total_iterations}.bmp", scene.img_array);
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
                scene.img_changed = true;
            }

            // Updating image
            if (scene.img_changed)
            {
                pictureBox1.Invalidate();
            }

            // Updating ETA
            if (scene.progressed)
            {
                scene.eta = (1.0 - scene.render_progress) * (float)scene.st.Elapsed.TotalSeconds / scene.render_progress;
                ETATextBox.Text = $"ETA: {TimeSpan.FromSeconds((long)scene.eta)}";
                scene.progressed = false;

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

                // Save file with gamma correction applyed
                scene.SaveToFile(path, scene.ApplyGammaCorrection(scene.img_array, scene.camera.gamma));
            }
        }

        private void KeepImgUpdated_CheckedChanged(object sender, EventArgs e)
        {
            scene.keep_img_updated = KeepImgUpdated.Checked;
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

        // Return deafult window size
        private Size GetDeafultWindowSize(float size_perc, float ratio)
        {
            int windowWidth = (int)(Screen.PrimaryScreen.Bounds.Width * size_perc);
            int picBoxWidth = windowWidth - 308;   // Magic number (pixel size for the buttons and offsets)
            int picBoxHeight = (int)((float)picBoxWidth / ratio);
            int windowHeight = picBoxHeight + 77;   // Magic number (pixel size for the offsets)
            return new Size(windowWidth, windowHeight);
        }
    }
}