using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Utilities
{
    public class Histogram : IEnumerable
    {
        private int Count { get; }

        private readonly int[] _histogramMatrix = new int[256];
        private byte? _median;
        public int this[int i] => _histogramMatrix[i];

        public byte AOD { get; }
        public int FirstNonZero { get; }
        public int LastNonZero { get; }
        public int MaxMagnitude { get; }

        public byte Median
        {
            get
            {
                if (_median == null)
                {
                    var histogramMatrix = (int[])_histogramMatrix.Clone();
                    Array.Sort(histogramMatrix);
                    var median = histogramMatrix[histogramMatrix.Length / 2];
                    _median = (byte)Array.IndexOf(_histogramMatrix, median);
                }

                return _median.Value;
            }
        }

        public Histogram(Image<Gray> image)
        {
            Count = image.N * image.M;

            int counter = 1;

            foreach (Gray pixel in image)
            {
                _histogramMatrix[(int)pixel.G]++;
                counter++;
            }

            double stagingAOD;
            stagingAOD = (double)_histogramMatrix.Select((v, i) => new { v, i }).Sum(x => x.v * x.i) / Count;
            AOD = Convert.ToByte(stagingAOD);

            FirstNonZero = Array.FindIndex(_histogramMatrix, v => v > 0);
            LastNonZero = Array.FindLastIndex(_histogramMatrix, v => v > 0);

            var maxValue = _histogramMatrix.Max();
            MaxMagnitude = Array.FindIndex(_histogramMatrix, v => v == maxValue);
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return _histogramMatrix.GetEnumerator();
        }
    }
}
