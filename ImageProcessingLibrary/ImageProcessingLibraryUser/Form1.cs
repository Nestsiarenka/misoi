﻿using System;
using System.Windows.Forms;
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
            var grayLevelImage = new GrayLevelImage(4, 5)
            {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, 16},
                {16, 17, 18, 19}
            };

            grayLevelImage[3, 4] = 99;

            var variable = new byte[2, 3] {{1, 2, 3}, {1, 2, 3}};

            var count = grayLevelImage.Count;

            var n = grayLevelImage.N;
            var m = grayLevelImage.M;
        }
    }
}
