using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Images;
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

                InputPictureBox.Image?.Dispose();
                InputPictureBox.Image = _inputBitmap;
                InputPictureBox.Refresh();
            }
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
        
    }
}
