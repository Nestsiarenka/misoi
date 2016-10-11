﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Utilities
{
    public class Histogram : IEnumerable
    {
        public int Count { get; }

        private readonly byte[] _hystogramMatrix = new byte[256];
        public byte this[int i] => _hystogramMatrix[i];

        public byte AOD { get; }
        public int FirstNonZero { get; }
        public int LastNonZero { get; }

        public Histogram(Image<Gray> image)
        {
            Count = image.N * image.M;

            foreach (Gray pixel in image)
            {
                _hystogramMatrix[(int)pixel.G]++;
            }

            double stagingAOD = 0;
            stagingAOD = (double)_hystogramMatrix.Select((v, i) => new {v, i}).Sum(x => x.v * x.i)/Count;
            AOD = Convert.ToByte(stagingAOD);

            FirstNonZero = Array.FindIndex(_hystogramMatrix, v => v > 0);
            LastNonZero = Array.FindLastIndex(_hystogramMatrix, v => v > 0);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _hystogramMatrix.GetEnumerator();
        }
    }
}