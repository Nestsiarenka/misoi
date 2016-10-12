using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;
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

                _inputBitmap = Converter.ToBitmap(_inputImageGrayLevel);

                DrawHistagram(new Histogram(_inputImageGrayLevel), InputHistogram);

                InputPictureBox.Image?.Dispose();
                InputPictureBox.Image = _inputBitmap;

                InputPictureBox.Refresh();
            }
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
            IFilter<Gray, Gray> filter;

            switch (FiltersComboBox.)
            {
                default:
            }
        }
    }
}
