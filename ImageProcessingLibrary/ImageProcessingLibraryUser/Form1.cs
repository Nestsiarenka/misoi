using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;
using ImageProcessingLibrary.Utilities;

namespace ImageProcessingLibraryUser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var grayLevelImage = new Image<Gray>(4, 5)
            {
                {1, 2, 3, 4},
                {1, 2, 3, 4},
                {4, 3, 2, 1},
                {4, 3, 2, 1},
                {1, 10, 10, 9}
            };
            //1 - 4
            //2 - 4
            //3 - 4
            //4 - 4
            //5 - 1
            //6 - 1
            //7 - 1
            //8 - 1
            // AOD - 3

            FSHS filter = new FSHS();
            grayLevelImage = filter.Filter(grayLevelImage);

            Histogram histogram = new Histogram(grayLevelImage);


            var rgbImage = new Image<RGB>(2, 2)
            {
                {new byte[] {1, 2, 3}, new byte[] {4, 5, 6}},
                {new byte[] {7, 8, 9}, new byte[] {10, 11, 12}}
            };

            foreach (Gray val in grayLevelImage)
            {
                var lol = val;
            }

            foreach (RGB val in rgbImage)
            {
                var lol = val;
            }

            var variable = new byte[2, 3] {{1, 2, 3}, {1, 2, 3}};


            var count = grayLevelImage.Count;

            var n = grayLevelImage.N;
            var m = grayLevelImage.M;
        }
    }
}
