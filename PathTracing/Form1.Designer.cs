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
            ResetButton = new Button();
            RenderButton = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            progressBar1 = new ProgressBar();
            SaveButton = new Button();
            FileNameTextBox = new TextBox();
            MaxReflectionsnumericUpDown = new NumericUpDown();
            MaxReflectionsTextBox = new TextBox();
            IterationsPerRendertextBox = new TextBox();
            IterationsPerRendernumericUpDown = new NumericUpDown();
            IterationsTextBox = new TextBox();
            KeepImgUpdated = new CheckBox();
            ETATextBox = new TextBox();
            LoadButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)MaxReflectionsnumericUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)IterationsPerRendernumericUpDown).BeginInit();
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
            // ResetButton
            // 
            ResetButton.BackColor = Color.FromArgb(64, 64, 64);
            ResetButton.Font = new Font("Microsoft Sans Serif", 19.8000011F, FontStyle.Regular, GraphicsUnit.Point);
            ResetButton.ForeColor = Color.White;
            ResetButton.Location = new Point(12, 15);
            ResetButton.Margin = new Padding(3, 4, 3, 4);
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new Size(264, 112);
            ResetButton.TabIndex = 1;
            ResetButton.Text = "Reset";
            ResetButton.UseVisualStyleBackColor = false;
            ResetButton.Click += ResetButton_Click;
            // 
            // RenderButton
            // 
            RenderButton.BackColor = Color.FromArgb(64, 64, 64);
            RenderButton.Font = new Font("Microsoft Sans Serif", 24F, FontStyle.Regular, GraphicsUnit.Point);
            RenderButton.ForeColor = Color.White;
            RenderButton.Location = new Point(12, 135);
            RenderButton.Margin = new Padding(3, 4, 3, 4);
            RenderButton.Name = "RenderButton";
            RenderButton.Size = new Size(264, 112);
            RenderButton.TabIndex = 2;
            RenderButton.Text = "Render";
            RenderButton.UseVisualStyleBackColor = false;
            RenderButton.Click += RenderButton_Click;
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
            // SaveButton
            // 
            SaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            SaveButton.BackColor = Color.FromArgb(64, 64, 64);
            SaveButton.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            SaveButton.ForeColor = Color.White;
            SaveButton.Location = new Point(12, 855);
            SaveButton.Margin = new Padding(3, 4, 3, 4);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(130, 62);
            SaveButton.TabIndex = 6;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = false;
            SaveButton.Click += SaveButton_Click;
            // 
            // FileNameTextBox
            // 
            FileNameTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            FileNameTextBox.BackColor = Color.FromArgb(64, 64, 64);
            FileNameTextBox.BorderStyle = BorderStyle.None;
            FileNameTextBox.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point);
            FileNameTextBox.ForeColor = Color.White;
            FileNameTextBox.Location = new Point(12, 824);
            FileNameTextBox.Margin = new Padding(3, 4, 3, 4);
            FileNameTextBox.Name = "FileNameTextBox";
            FileNameTextBox.Size = new Size(270, 23);
            FileNameTextBox.TabIndex = 7;
            FileNameTextBox.Text = "Render.bmp";
            // 
            // MaxReflectionsnumericUpDown
            // 
            MaxReflectionsnumericUpDown.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            MaxReflectionsnumericUpDown.Location = new Point(218, 391);
            MaxReflectionsnumericUpDown.Margin = new Padding(3, 4, 3, 4);
            MaxReflectionsnumericUpDown.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            MaxReflectionsnumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            MaxReflectionsnumericUpDown.Name = "MaxReflectionsnumericUpDown";
            MaxReflectionsnumericUpDown.Size = new Size(58, 27);
            MaxReflectionsnumericUpDown.TabIndex = 8;
            MaxReflectionsnumericUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // MaxReflectionsTextBox
            // 
            MaxReflectionsTextBox.BackColor = Color.FromArgb(64, 64, 64);
            MaxReflectionsTextBox.BorderStyle = BorderStyle.None;
            MaxReflectionsTextBox.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            MaxReflectionsTextBox.ForeColor = Color.White;
            MaxReflectionsTextBox.ImeMode = ImeMode.NoControl;
            MaxReflectionsTextBox.Location = new Point(12, 391);
            MaxReflectionsTextBox.Margin = new Padding(3, 4, 3, 4);
            MaxReflectionsTextBox.Name = "MaxReflectionsTextBox";
            MaxReflectionsTextBox.ReadOnly = true;
            MaxReflectionsTextBox.Size = new Size(200, 27);
            MaxReflectionsTextBox.TabIndex = 9;
            MaxReflectionsTextBox.Text = "Max ray reflections";
            MaxReflectionsTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // IterationsPerRendertextBox
            // 
            IterationsPerRendertextBox.BackColor = Color.FromArgb(64, 64, 64);
            IterationsPerRendertextBox.BorderStyle = BorderStyle.None;
            IterationsPerRendertextBox.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            IterationsPerRendertextBox.ForeColor = Color.White;
            IterationsPerRendertextBox.ImeMode = ImeMode.NoControl;
            IterationsPerRendertextBox.Location = new Point(12, 426);
            IterationsPerRendertextBox.Margin = new Padding(3, 4, 3, 4);
            IterationsPerRendertextBox.Name = "IterationsPerRendertextBox";
            IterationsPerRendertextBox.ReadOnly = true;
            IterationsPerRendertextBox.Size = new Size(200, 27);
            IterationsPerRendertextBox.TabIndex = 10;
            IterationsPerRendertextBox.Text = "Iterations / render";
            IterationsPerRendertextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // IterationsPerRendernumericUpDown
            // 
            IterationsPerRendernumericUpDown.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            IterationsPerRendernumericUpDown.Location = new Point(218, 426);
            IterationsPerRendernumericUpDown.Margin = new Padding(3, 4, 3, 4);
            IterationsPerRendernumericUpDown.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            IterationsPerRendernumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            IterationsPerRendernumericUpDown.Name = "IterationsPerRendernumericUpDown";
            IterationsPerRendernumericUpDown.Size = new Size(58, 27);
            IterationsPerRendernumericUpDown.TabIndex = 11;
            IterationsPerRendernumericUpDown.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // IterationsTextBox
            // 
            IterationsTextBox.BackColor = Color.FromArgb(64, 64, 64);
            IterationsTextBox.BorderStyle = BorderStyle.None;
            IterationsTextBox.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            IterationsTextBox.ForeColor = Color.White;
            IterationsTextBox.Location = new Point(12, 323);
            IterationsTextBox.Margin = new Padding(3, 4, 3, 4);
            IterationsTextBox.Name = "IterationsTextBox";
            IterationsTextBox.ReadOnly = true;
            IterationsTextBox.Size = new Size(264, 27);
            IterationsTextBox.TabIndex = 12;
            IterationsTextBox.Text = "Total iterations: 0";
            // 
            // KeepImgUpdated
            // 
            KeepImgUpdated.AutoSize = true;
            KeepImgUpdated.BackColor = Color.FromArgb(64, 64, 64);
            KeepImgUpdated.Checked = true;
            KeepImgUpdated.CheckState = CheckState.Checked;
            KeepImgUpdated.Font = new Font("Segoe UI", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            KeepImgUpdated.ForeColor = Color.White;
            KeepImgUpdated.Location = new Point(12, 357);
            KeepImgUpdated.Margin = new Padding(8, 6, 6, 6);
            KeepImgUpdated.Name = "KeepImgUpdated";
            KeepImgUpdated.Size = new Size(194, 27);
            KeepImgUpdated.TabIndex = 13;
            KeepImgUpdated.Text = "Keep Image Updated";
            KeepImgUpdated.UseVisualStyleBackColor = false;
            KeepImgUpdated.CheckedChanged += KeepImgUpdated_CheckedChanged;
            // 
            // ETATextBox
            // 
            ETATextBox.BackColor = Color.FromArgb(64, 64, 64);
            ETATextBox.BorderStyle = BorderStyle.None;
            ETATextBox.Font = new Font("Microsoft Sans Serif", 13.8F, FontStyle.Regular, GraphicsUnit.Point);
            ETATextBox.ForeColor = Color.White;
            ETATextBox.Location = new Point(12, 288);
            ETATextBox.Margin = new Padding(3, 4, 3, 4);
            ETATextBox.Name = "ETATextBox";
            ETATextBox.ReadOnly = true;
            ETATextBox.Size = new Size(264, 27);
            ETATextBox.TabIndex = 14;
            ETATextBox.Text = "ETA: ";
            // 
            // LoadButton
            // 
            LoadButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            LoadButton.BackColor = Color.FromArgb(64, 64, 64);
            LoadButton.Font = new Font("Microsoft Sans Serif", 10.2F, FontStyle.Regular, GraphicsUnit.Point);
            LoadButton.ForeColor = Color.White;
            LoadButton.Location = new Point(152, 855);
            LoadButton.Margin = new Padding(3, 4, 3, 4);
            LoadButton.Name = "LoadButton";
            LoadButton.Size = new Size(130, 62);
            LoadButton.TabIndex = 15;
            LoadButton.Text = "Load";
            LoadButton.UseVisualStyleBackColor = false;
            LoadButton.Click += LoadButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1582, 930);
            Controls.Add(LoadButton);
            Controls.Add(ETATextBox);
            Controls.Add(KeepImgUpdated);
            Controls.Add(IterationsTextBox);
            Controls.Add(IterationsPerRendernumericUpDown);
            Controls.Add(IterationsPerRendertextBox);
            Controls.Add(MaxReflectionsTextBox);
            Controls.Add(MaxReflectionsnumericUpDown);
            Controls.Add(FileNameTextBox);
            Controls.Add(SaveButton);
            Controls.Add(progressBar1);
            Controls.Add(RenderButton);
            Controls.Add(ResetButton);
            Controls.Add(pictureBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)MaxReflectionsnumericUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)IterationsPerRendernumericUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Button RenderButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.TextBox FileNameTextBox;
        private System.Windows.Forms.NumericUpDown MaxReflectionsnumericUpDown;
        private System.Windows.Forms.NumericUpDown IterationsPerRendernumericUpDown;
        internal System.Windows.Forms.TextBox IterationsTextBox;
        public System.Windows.Forms.TextBox MaxReflectionsTextBox;
        public System.Windows.Forms.TextBox IterationsPerRendertextBox;
        private CheckBox KeepImgUpdated;
        internal TextBox ETATextBox;
        private Button LoadButton;
    }
}

