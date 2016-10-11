using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

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
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, 16},
                {16, 17, 18, 19}
            };

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
