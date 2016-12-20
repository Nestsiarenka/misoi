using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Detection.CannyEdge;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Filters.SpatialFilters;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;
using ImageProcessingLibrary.Resizers;
using ImageProcessingLibrary.Segmentation;
using ImageProcessingLibrary.Utilities;
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

        private void DrawImage(Image<RGB> image, PictureBox pictureBox, Rectangle region = new Rectangle())
        {
            var bitmap = Converter.ToBitmap(image);

            if (!region.IsEmpty)
            {
                using (Graphics gr = Graphics.FromImage(bitmap))
                {
                    using (Pen thick_pen = new Pen(Color.Blue, 5))
                    {
                        gr.DrawRectangle(thick_pen, region);
                    }
                }
            }

            pictureBox.Image?.Dispose();
            pictureBox.Image = bitmap;

            pictureBox.Refresh();
        }

        private void DrawImage(Image<RGB> image, PictureBox pictureBox, IEnumerable<Rectangle> regions)
        {
            var bitmap = Converter.ToBitmap(image);
            
            using (Graphics gr = Graphics.FromImage(bitmap))
            {
                using (Pen thick_pen = new Pen(Color.Blue, 5))
                {
                    foreach (var region in regions)
                    {
                        gr.DrawRectangle(thick_pen, region);
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
            FiltersComboBox.Items.Insert(4, "Sobel");
            FiltersComboBox.Items.Insert(5, "Otsu");
            FiltersComboBox.Items.Insert(6, "Real gaussian");

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
                case 4:
                    filter = new SobelFilter();
                    break;
                case 5:
                    filter = new OtsuBinarization();
                    break;
                case 6:
                    filter = new RealGaussianFilter(1);
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
            var region = segmentator.CropFace(image, _inputImage.N/3, _inputImage.M/3);

            image = _inputImage.Clone();
            image.SetRegionOfInterest(region);
            BicubicResizer bicubicResizer = new BicubicResizer();
            var grayImage = bicubicResizer.Resize(new RGBtoGrayFilter().Filter(image), 60, 120);
            
            var regions = segmentator.SegmentateEyes(grayImage);
            regions.Add(segmentator.SegmentateLips(grayImage));

            var eye1 = grayImage.Clone();
            eye1.SetRegionOfInterest(regions[0]);

            var eye2 = grayImage.Clone();
            eye2.SetRegionOfInterest(regions[1]);

            var lips = grayImage.Clone();
            lips.SetRegionOfInterest(regions[2]);

            var eye1Histo = new float[20];
            var eye2Histo = new float[20];
            var lipsHistoM = new float[20];
            var lipsHistoN = new float[20];

            OtsuBinarization binarizator = new OtsuBinarization();

            eye1 = binarizator.Filter(eye1);
            eye2 = binarizator.Filter(eye2);
            lips = binarizator.Filter(lips);

            for (int x = 0; x < 10; x++)
            {
                if (eye1.N <= x + eye2.N / 2 - 5 || x + eye1.N / 2 - 5 < 0)
                {
                    eye1Histo[x] = 0;
                    continue;
                }

                for (int y = 0; y < eye1.M; y++)
                {
                    eye1Histo[x] += eye1[x + eye1.N/2 - 5, y].G.Value;
                }
            }

            for (int x = 0; x < 10; x++)
            {
                if (eye2.N <= x + eye2.N / 2 - 5 || x + eye2.N / 2 - 5 < 0)
                {
                    eye2Histo[x] = 0;
                    continue;
                }    

                for (int y = 0; y < eye2.M; y++)
                {
                    eye2Histo[x] += eye2[x + eye2.N / 2 - 5, y].G.Value;
                }
            }

            for (int x = 0; x < 10; x++)
            {
                if (lips.N <= x + lips.N / 2 - 5 || x + lips.N / 2 - 5 < 0)
                {
                    lipsHistoN[x] = 0;
                    continue;
                }

                for (int y = 0; y < lips.M; y++)
                {
                    lipsHistoN[x] += lips[x + lips.N / 2 - 5, y].G.Value;
                }
            }

            for (int y = 0; y < 10; y++)
            {
                if (lips.M <= y + lips.M / 2 - 5 ||  y + lips.M / 2 - 5 < 0)
                {
                    lipsHistoM[y] = 0;
                    continue;
                }

                for (int x = 0; x < lips.M; x++)
                {
                    lipsHistoM[y] += lips[x, y + lips.M / 2 - 5].G.Value;
                }
            }



            DrawImage(lips, OutputPictureBox);
        }

        //private float Normalize()
        //{
            
        //}


        private void CannysButton(object sender, EventArgs e)
        {
            var grayFilter = new RGBtoGrayFilter();
            var grayImage = grayFilter.Filter(_inputImage);

            var detector = new CannyEdgeDetection();

            grayImage = detector.MakeDetection(grayImage);

            DrawImage(grayImage, OutputPictureBox);
        }
    }
}
