﻿using System;
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
using System.Runtime.InteropServices;
using static System.Windows.Forms.AxHost;

namespace PathTracing
{
    public partial class Form1 : Form
    {
        Scene scene = new Scene();
        Size winSize;

        Bitmap picBoxImage;

        Thread? thread = null;

        const float WINDOW_SIZE_PERCENTAGE = 0.5f;   // Percentage of primary screen width

        bool do_reset = false;
        bool start_render = false;


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
            do_reset = true;
            scene.abort_render = true;
            this.Text = "Path Tracing";
        }


        // Render
        private void RenderButton_Click(object sender, EventArgs e)
        {
            if (do_reset)
            {
                do_reset = false;
                scene = new Scene();
                this.Text = "Path Tracing";
                this.Size = GetDeafultWindowSize(WINDOW_SIZE_PERCENTAGE, scene.camera.aspect_ratio);
            }

            if (scene.loaded && !scene.rendering)
            {
                start_render = true;
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            // updating progress bar
            if (scene.rendering)
            {
                progressBar1.Value = (int)(scene.render_progress * 100);
                if (scene.render_progress == 1f)
                {
                    scene.rendering = false;
                    // Auto save with no gamma correction
                    scene.SaveToFile(@$"Renders/NoGamma_{scene.camera.samples_per_pixel * scene.total_iterations}SPP_{scene.total_iterations}TI.png", scene.img_array);
                }
            }
            else
            {
                // Updating render settings
                scene.max_ray_reflections = (int)MaxReflectionsnumericUpDown.Value;
                scene.iteretions_per_render = (int)IterationsPerRendernumericUpDown.Value;

                if (start_render)
                {
                    start_render = false;
                    thread = new Thread(scene.Render);
                    thread.IsBackground = true;
                    thread.Start();
                    scene.rendering = true;
                    this.Text = $"Path Tracing - {MathF.Round(scene.render_progress * 100.0f, 2)}% | ETA: -:-:-";
                }
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

                    if (this.WindowState != FormWindowState.Maximized)
                    {
                        this.WindowState = FormWindowState.Minimized;
                        this.Show();
                        this.WindowState = FormWindowState.Normal;
                    }
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
                    path = @$"Renders/{FileNameTextBox.Text}";
                }
                else
                {
                    path = @$"Renders/{FileNameTextBox.Text}.png";
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
            if (FileNameTextBox.Text.Contains(".png"))
            {
                path = @$"Renders/{FileNameTextBox.Text}";
            }
            else
            {
                path = @$"Renders/{FileNameTextBox.Text}.png";
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