﻿using System;
using System.Drawing;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Filters.SpatialFilters;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;
using ImageProcessingLibrary.Resizers;
using ImageProcessingLibrary.Segmentation;
using ImageProcessingLibrary.Utilities;
using ImageProcessingLibrary.Utilities.Enums;
using ImageProcessingLibraryUser.Properties;


namespace ImageProcessingLibraryUser
{
    public partial class Form1 : Form
    {
        private Bitmap _inputBitmap;
        private Bitmap _outputBitmap;
        private Image<RGB> _inputImage;
        private Image<RGB> _outputImage;
        private Image<Gray> _inputImageGrayLevel;
        private Image<Gray> _outputImageGrayLevel;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPictureDialogInitialization();
            ComboBoxInitialization();
            HistogramChartsIntialization(InputHistogram);
            HistogramChartsIntialization(OutputHistogram);
        }

        private void LoadFileButton_Click(object sender, EventArgs e)
        {
            if (loadPictureDialog.ShowDialog() == DialogResult.OK)
            {
                _inputBitmap?.Dispose();

                _inputImage = FileLoader.LoadFromFile(loadPictureDialog.FileName);

                var grayFilter = new RGBtoGrayFilter();
                _inputImageGrayLevel = grayFilter.Filter(_inputImage);
                _outputImageGrayLevel = _inputImageGrayLevel.Clone();

                DrawHistagram(new Histogram(_inputImageGrayLevel), InputHistogram);
                DrawHistagram(new Histogram(_outputImageGrayLevel), OutputHistogram);

                DrawImage(_inputImageGrayLevel, InputPictureBox);
                DrawImage(_inputImageGrayLevel, OutputPictureBox);
            }
        }

        private void DrawImage(Image<Gray> image, PictureBox pictureBox)
        {
            var bitmap = Converter.ToBitmap(image);

            pictureBox.Image?.Dispose();
            pictureBox.Image = bitmap;

            pictureBox.Refresh();
        }

        private void DrawImage(Image<RGB> image, PictureBox pictureBox, Point point = new Point())
        {
            var bitmap = Converter.ToBitmap(image);

            if (!point.IsEmpty)
            {
                using (Graphics gr = Graphics.FromImage(bitmap))
                {
                    using (Pen thick_pen = new Pen(Color.Blue, 5))
                    {
                        gr.DrawRectangle(thick_pen, point.X - 2, point.Y - 2, 2, 2);
                    }
                }
            }

            pictureBox.Image?.Dispose();
            pictureBox.Image = bitmap;

            pictureBox.Refresh();
        }

        private void DrawHistagram(Histogram histogram, Chart histogramChart)
        {
            histogramChart.Series[0].Points.Clear();

            for (int i = 0; i < 256; i++)
            {
                histogramChart.Series[0].Points.AddXY(i, histogram[i]);
            }
        }

        private void HistogramChartsIntialization(Chart chart)
        {
            chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
            chart.ChartAreas[0].AxisX.Crossing = 0;
            chart.ChartAreas[0].AxisX.Minimum = 0;
        }

        private void ComboBoxInitialization()
        {
            FiltersComboBox.Items.Insert(0, "FSHS");
            FiltersComboBox.Items.Insert(1, "Log + FSHS");
            FiltersComboBox.Items.Insert(2, "Gaussian");
            FiltersComboBox.Items.Insert(3, "Median");

            FiltersComboBox.SelectedIndex = 0;
        }

        private void LoadPictureDialogInitialization()
        {
            loadPictureDialog.Filter = Resources.FileFormatsForDialog;
            loadPictureDialog.DefaultExt = ".png";
            loadPictureDialog.FileOk += (s, ea) =>
            {
                if (ValidateFileName(loadPictureDialog.FileName)) return;

                MessageBox.Show(Resources.IllegalExtensionMessage, Resources.OpenMessageForDialog, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ea.Cancel = true;
            };
        }

        private static bool ValidateFileName(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && Resources.FileFormatsForValidation.Contains(Path.GetExtension(fileName)); 
        }

        private void ProcessButton_Click(object sender, EventArgs e)
        {
            if (_outputImageGrayLevel == null)
            {
                return;
            }

            IFilter<Gray, Gray> filter;

            switch (FiltersComboBox.SelectedIndex)
            {
                case 0:
                    filter = new FSHS();
                    break;
                case 1:
                    filter = new LogarithmOperation();
                    break;
                case 2:
                    filter = new GaussianFilter();
                    break;
                case 3:
                    filter = new MedianFilter(new bool[3,3]);
                    break;
                default:
                    return;
            }

            _outputImageGrayLevel = filter.Filter(_outputImageGrayLevel);

            DrawHistagram(new Histogram(_outputImageGrayLevel), OutputHistogram);
            DrawImage(_outputImageGrayLevel, OutputPictureBox);
        }

        private void UndoAll_Click(object sender, EventArgs e)
        {
            _outputImageGrayLevel = _inputImageGrayLevel.Clone();

            DrawHistagram(new Histogram(_outputImageGrayLevel), OutputHistogram);
            DrawImage(_outputImageGrayLevel, OutputPictureBox);
        }

        private void ResizeButton_Click(object sender, EventArgs e)
        {
            int widthWith, heightWith;

            if (int.TryParse(WidthTextBox.Text, out widthWith) 
                && int.TryParse(HeightTextBox.Text, out heightWith))
            {

                var resizer = new BicubicResizer();
                var image = resizer.Resize(_inputImageGrayLevel,
                        _inputImageGrayLevel.N - widthWith, _inputImageGrayLevel.M - heightWith);

                DrawImage(image, InputPictureBox);
            }
        }

        private void SkinSegment_Click(object sender, EventArgs e)
        {
            SkinColorSegmentation segmentator = new SkinColorSegmentation();

            var image = segmentator.Segmentate(_inputImage);

            DrawHistagram(new Histogram(new RGBtoGrayFilter().Filter(image)), OutputHistogram);
            DrawImage(image, OutputPictureBox);
        }
    }
}
