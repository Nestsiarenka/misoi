﻿namespace ImageProcessingLibraryUser
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea7 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea8 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.InputPictureBox = new System.Windows.Forms.PictureBox();
            this.OutputPictureBox = new System.Windows.Forms.PictureBox();
            this.InputHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.OutputHistogram = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.FiltersComboBox = new System.Windows.Forms.ComboBox();
            this.PickFilterLabel = new System.Windows.Forms.Label();
            this.loadPictureDialog = new System.Windows.Forms.OpenFileDialog();
            this.UndoAll = new System.Windows.Forms.Button();
            this.WidthTextBox = new System.Windows.Forms.TextBox();
            this.HeightTextBox = new System.Windows.Forms.TextBox();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.pxLabel = new System.Windows.Forms.Label();
            this.pxLabel1 = new System.Windows.Forms.Label();
            this.ResizeButton = new System.Windows.Forms.Button();
            this.SkinSegment = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.InputPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // InputPictureBox
            // 
            this.InputPictureBox.BackColor = System.Drawing.SystemColors.Desktop;
            this.InputPictureBox.Location = new System.Drawing.Point(11, 26);
            this.InputPictureBox.Name = "InputPictureBox";
            this.InputPictureBox.Size = new System.Drawing.Size(430, 360);
            this.InputPictureBox.TabIndex = 0;
            this.InputPictureBox.TabStop = false;
            // 
            // OutputPictureBox
            // 
            this.OutputPictureBox.BackColor = System.Drawing.SystemColors.Highlight;
            this.OutputPictureBox.Location = new System.Drawing.Point(11, 419);
            this.OutputPictureBox.Name = "OutputPictureBox";
            this.OutputPictureBox.Size = new System.Drawing.Size(430, 360);
            this.OutputPictureBox.TabIndex = 1;
            this.OutputPictureBox.TabStop = false;
            // 
            // InputHistogram
            // 
            chartArea7.Name = "ChartArea1";
            this.InputHistogram.ChartAreas.Add(chartArea7);
            this.InputHistogram.Location = new System.Drawing.Point(491, 26);
            this.InputHistogram.Name = "InputHistogram";
            this.InputHistogram.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SemiTransparent;
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series7.Name = "Histogram";
            this.InputHistogram.Series.Add(series7);
            this.InputHistogram.Size = new System.Drawing.Size(436, 361);
            this.InputHistogram.TabIndex = 2;
            this.InputHistogram.Text = "Histogram of input image";
            // 
            // OutputHistogram
            // 
            chartArea8.Name = "ChartArea1";
            this.OutputHistogram.ChartAreas.Add(chartArea8);
            this.OutputHistogram.Location = new System.Drawing.Point(491, 419);
            this.OutputHistogram.Name = "OutputHistogram";
            this.OutputHistogram.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series8.Name = "Histogram";
            this.OutputHistogram.Series.Add(series8);
            this.OutputHistogram.Size = new System.Drawing.Size(436, 361);
            this.OutputHistogram.TabIndex = 3;
            this.OutputHistogram.Text = "Histogram of output image";
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadFileButton.Location = new System.Drawing.Point(940, 26);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(172, 47);
            this.LoadFileButton.TabIndex = 4;
            this.LoadFileButton.Text = "Upload a picture";
            this.LoadFileButton.UseVisualStyleBackColor = true;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // ProcessButton
            // 
            this.ProcessButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProcessButton.Location = new System.Drawing.Point(940, 340);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(172, 47);
            this.ProcessButton.TabIndex = 5;
            this.ProcessButton.Text = "Process filter";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButton_Click);
            // 
            // FiltersComboBox
            // 
            this.FiltersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FiltersComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FiltersComboBox.FormattingEnabled = true;
            this.FiltersComboBox.Location = new System.Drawing.Point(940, 286);
            this.FiltersComboBox.Name = "FiltersComboBox";
            this.FiltersComboBox.Size = new System.Drawing.Size(172, 26);
            this.FiltersComboBox.TabIndex = 6;
            // 
            // PickFilterLabel
            // 
            this.PickFilterLabel.AutoSize = true;
            this.PickFilterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PickFilterLabel.Location = new System.Drawing.Point(937, 261);
            this.PickFilterLabel.Name = "PickFilterLabel";
            this.PickFilterLabel.Size = new System.Drawing.Size(75, 16);
            this.PickFilterLabel.TabIndex = 7;
            this.PickFilterLabel.Text = "Pick a filter:";
            // 
            // UndoAll
            // 
            this.UndoAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UndoAll.Location = new System.Drawing.Point(940, 419);
            this.UndoAll.Name = "UndoAll";
            this.UndoAll.Size = new System.Drawing.Size(172, 47);
            this.UndoAll.TabIndex = 8;
            this.UndoAll.Text = "Undo all";
            this.UndoAll.UseVisualStyleBackColor = true;
            this.UndoAll.Click += new System.EventHandler(this.UndoAll_Click);
            // 
            // WidthTextBox
            // 
            this.WidthTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.WidthTextBox.Location = new System.Drawing.Point(940, 506);
            this.WidthTextBox.Name = "WidthTextBox";
            this.WidthTextBox.Size = new System.Drawing.Size(110, 22);
            this.WidthTextBox.TabIndex = 9;
            // 
            // HeightTextBox
            // 
            this.HeightTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.HeightTextBox.Location = new System.Drawing.Point(940, 554);
            this.HeightTextBox.Name = "HeightTextBox";
            this.HeightTextBox.Size = new System.Drawing.Size(110, 22);
            this.HeightTextBox.TabIndex = 10;
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.WidthLabel.Location = new System.Drawing.Point(937, 484);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(100, 16);
            this.WidthLabel.TabIndex = 11;
            this.WidthLabel.Text = "Down width with";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.HeightLabel.Location = new System.Drawing.Point(937, 535);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(106, 16);
            this.HeightLabel.TabIndex = 12;
            this.HeightLabel.Text = "Down height with";
            // 
            // pxLabel
            // 
            this.pxLabel.AutoSize = true;
            this.pxLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.pxLabel.Location = new System.Drawing.Point(1054, 508);
            this.pxLabel.Name = "pxLabel";
            this.pxLabel.Size = new System.Drawing.Size(22, 16);
            this.pxLabel.TabIndex = 13;
            this.pxLabel.Text = "px";
            // 
            // pxLabel1
            // 
            this.pxLabel1.AutoSize = true;
            this.pxLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.pxLabel1.Location = new System.Drawing.Point(1054, 556);
            this.pxLabel1.Name = "pxLabel1";
            this.pxLabel1.Size = new System.Drawing.Size(22, 16);
            this.pxLabel1.TabIndex = 14;
            this.pxLabel1.Text = "px";
            // 
            // ResizeButton
            // 
            this.ResizeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResizeButton.Location = new System.Drawing.Point(940, 598);
            this.ResizeButton.Name = "ResizeButton";
            this.ResizeButton.Size = new System.Drawing.Size(172, 47);
            this.ResizeButton.TabIndex = 15;
            this.ResizeButton.Text = "Apply resize";
            this.ResizeButton.UseVisualStyleBackColor = true;
            this.ResizeButton.Click += new System.EventHandler(this.ResizeButton_Click);
            // 
            // SkinSegment
            // 
            this.SkinSegment.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SkinSegment.Location = new System.Drawing.Point(940, 667);
            this.SkinSegment.Name = "SkinSegment";
            this.SkinSegment.Size = new System.Drawing.Size(172, 47);
            this.SkinSegment.TabIndex = 16;
            this.SkinSegment.Text = "Face features";
            this.SkinSegment.UseVisualStyleBackColor = true;
            this.SkinSegment.Click += new System.EventHandler(this.SkinSegment_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(940, 732);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 47);
            this.button1.TabIndex = 17;
            this.button1.Text = "Canny\'s edges";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CannysButton);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1124, 793);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SkinSegment);
            this.Controls.Add(this.ResizeButton);
            this.Controls.Add(this.pxLabel1);
            this.Controls.Add(this.pxLabel);
            this.Controls.Add(this.HeightLabel);
            this.Controls.Add(this.WidthLabel);
            this.Controls.Add(this.HeightTextBox);
            this.Controls.Add(this.WidthTextBox);
            this.Controls.Add(this.UndoAll);
            this.Controls.Add(this.PickFilterLabel);
            this.Controls.Add(this.FiltersComboBox);
            this.Controls.Add(this.ProcessButton);
            this.Controls.Add(this.LoadFileButton);
            this.Controls.Add(this.OutputHistogram);
            this.Controls.Add(this.InputHistogram);
            this.Controls.Add(this.OutputPictureBox);
            this.Controls.Add(this.InputPictureBox);
            this.Name = "Form1";
            this.Text = "Image processing";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.InputPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputHistogram)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox InputPictureBox;
        private System.Windows.Forms.PictureBox OutputPictureBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart InputHistogram;
        private System.Windows.Forms.DataVisualization.Charting.Chart OutputHistogram;
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.ComboBox FiltersComboBox;
        private System.Windows.Forms.Label PickFilterLabel;
        private System.Windows.Forms.OpenFileDialog loadPictureDialog;
        private System.Windows.Forms.Button UndoAll;
        private System.Windows.Forms.TextBox WidthTextBox;
        private System.Windows.Forms.TextBox HeightTextBox;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label HeightLabel;
        private System.Windows.Forms.Label pxLabel;
        private System.Windows.Forms.Label pxLabel1;
        private System.Windows.Forms.Button ResizeButton;
        private System.Windows.Forms.Button SkinSegment;
        private System.Windows.Forms.Button button1;
    }
}

