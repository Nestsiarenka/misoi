using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Exceptions;

namespace ImageProcessingLibrary.Images
{
    public class Image<T> : IEnumerable<T>
    where T : struct, ICapacity
    {
        private readonly T[][] _imageMatrix;
        

        public T this[int i, int j]
        {
            get
            {
                if (ReturnZeroIfOutOfBounds && (j < 0 || j >= M || i < 0 || i >= N))
                {
                    T zeroValue = new T();

                    zeroValue.SetZero();
                    return zeroValue;
                }

                return _imageMatrix[j][i];
            }
            set { _imageMatrix[j][i] = value; }
        }

        public int Count => N * M;
        public int N { get; }
        public int M { get; }
        public bool ReturnZeroIfOutOfBounds { get; set; }

        public Image(int n, int m)
        {
            _imageMatrix = new T[m][];

            for (int i = 0; i < m; i++)
            {
               _imageMatrix[i] = new T[n]; 
            }

            N = n;
            M = m;
        }

        public void Add(params object[] values)
        {
            if (values.Length == N)
            {
                int i = FindNonZeroRaw();
                for (int j = 0; j < values.Length; j++)
                {
                    _imageMatrix[i][j].Initialize(values[j]);
                }
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
                if (!_imageMatrix[i].Any(v => !v.IsEmpty()))
                {
                    return i;
                }
            }

            throw new ImageException("No zero lines in image");
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int j = 0; j < M; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    yield return this[i, j];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
