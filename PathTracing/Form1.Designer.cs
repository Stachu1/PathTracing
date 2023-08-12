namespace PathTracing
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            progressBar1 = new ProgressBar();
            button3 = new Button();
            textBox1 = new TextBox();
            numericUpDown1 = new NumericUpDown();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            numericUpDown2 = new NumericUpDown();
            textBox4 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(288, 15);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1280, 900);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(64, 64, 64);
            button1.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            button1.ForeColor = Color.White;
            button1.Location = new Point(12, 15);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(264, 112);
            button1.TabIndex = 1;
            button1.Text = "Reset";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(64, 64, 64);
            button2.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            button2.ForeColor = Color.White;
            button2.Location = new Point(12, 135);
            button2.Margin = new Padding(3, 4, 3, 4);
            button2.Name = "button2";
            button2.Size = new Size(264, 112);
            button2.TabIndex = 2;
            button2.Text = "Render";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Tick += timer1_Tick;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = SystemColors.Control;
            progressBar1.Location = new Point(12, 255);
            progressBar1.Margin = new Padding(3, 4, 3, 4);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(264, 25);
            progressBar1.TabIndex = 4;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button3.BackColor = Color.FromArgb(64, 64, 64);
            button3.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            button3.ForeColor = Color.White;
            button3.Location = new Point(166, 852);
            button3.Margin = new Padding(3, 4, 3, 4);
            button3.Name = "button3";
            button3.Size = new Size(110, 62);
            button3.TabIndex = 6;
            button3.Text = "Save";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBox1.BackColor = Color.FromArgb(64, 64, 64);
            textBox1.BorderStyle = BorderStyle.None;
            textBox1.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(12, 868);
            textBox1.Margin = new Padding(3, 4, 3, 4);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(148, 23);
            textBox1.TabIndex = 7;
            textBox1.Text = "Render.bmp";
            // 
            // numericUpDown1
            // 
            numericUpDown1.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            numericUpDown1.Location = new Point(218, 354);
            numericUpDown1.Margin = new Padding(3, 4, 3, 4);
            numericUpDown1.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(58, 27);
            numericUpDown1.TabIndex = 8;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.FromArgb(64, 64, 64);
            textBox2.BorderStyle = BorderStyle.None;
            textBox2.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            textBox2.ForeColor = Color.White;
            textBox2.ImeMode = ImeMode.NoControl;
            textBox2.Location = new Point(12, 354);
            textBox2.Margin = new Padding(3, 4, 3, 4);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(200, 20);
            textBox2.TabIndex = 9;
            textBox2.Text = "Max ray reflections";
            textBox2.TextAlign = HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.FromArgb(64, 64, 64);
            textBox3.BorderStyle = BorderStyle.None;
            textBox3.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            textBox3.ForeColor = Color.White;
            textBox3.ImeMode = ImeMode.NoControl;
            textBox3.Location = new Point(12, 401);
            textBox3.Margin = new Padding(3, 4, 3, 4);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(200, 20);
            textBox3.TabIndex = 10;
            textBox3.Text = "Iterations per render";
            textBox3.TextAlign = HorizontalAlignment.Center;
            // 
            // numericUpDown2
            // 
            numericUpDown2.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            numericUpDown2.Location = new Point(218, 401);
            numericUpDown2.Margin = new Padding(3, 4, 3, 4);
            numericUpDown2.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(58, 27);
            numericUpDown2.TabIndex = 11;
            numericUpDown2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // textBox4
            // 
            textBox4.BackColor = Color.FromArgb(64, 64, 64);
            textBox4.BorderStyle = BorderStyle.None;
            textBox4.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            textBox4.ForeColor = Color.White;
            textBox4.Location = new Point(12, 308);
            textBox4.Margin = new Padding(3, 4, 3, 4);
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.Size = new Size(264, 27);
            textBox4.TabIndex = 12;
            textBox4.Text = "Total iterations: 0";
            textBox4.TextAlign = HorizontalAlignment.Center;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1582, 930);
            Controls.Add(textBox4);
            Controls.Add(numericUpDown2);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(numericUpDown1);
            Controls.Add(textBox1);
            Controls.Add(button3);
            Controls.Add(progressBar1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(pictureBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        internal System.Windows.Forms.TextBox textBox4;
        public System.Windows.Forms.TextBox textBox2;
        public System.Windows.Forms.TextBox textBox3;
    }
}

