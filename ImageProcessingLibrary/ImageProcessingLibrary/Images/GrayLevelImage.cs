using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Exceptions;

namespace ImageProcessingLibrary.Images
{
    public class GrayLevelImage : IEnumerable
    {
        private readonly byte[][] _imageMatrix;

        public byte this[int i, int j]
        {
            get { return _imageMatrix[j][i]; }
            set { _imageMatrix[j][i] = value; }
        }

        public int Count => _imageMatrix.Length;
        public int N { get; }
        public int M { get; }

        public GrayLevelImage(int n, int m)
        {
            _imageMatrix = new byte[m][];

            for (int i = 0; i < m; i++)
            {
               _imageMatrix[i] = new byte[n]; 
            }

            N = n;
            M = m;
        }

        public void Add(params byte[] values)
        {
            if (values.Length == N)
            {
                int i = FindNonZeroRaw();
                _imageMatrix[i] = (byte[])values.Clone();
            }
            else
            {
                throw new ImageException("Number of arguments not meet image dimension, use count of N");
            }
        }

        private int FindNonZeroRaw()
        {
            for (int i = 0; i < M; i++)
            {
                if (!_imageMatrix[i].Any(v => v != 0))
                {
                    return i;
                }
            }
            throw new ImageException("No zero lines in image");
        }

        public IEnumerator GetEnumerator()
        {
            for (int j = 0; j < M; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    yield return this[i, j];
                }
            }
        }
    }
}
