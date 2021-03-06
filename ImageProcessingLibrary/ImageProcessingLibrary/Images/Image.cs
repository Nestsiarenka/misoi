﻿using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Exceptions;

namespace ImageProcessingLibrary.Images
{
    public class Image<T> : IEnumerable<T>
    where T : struct, ICapacity
    {
        private readonly T[][] _imageMatrix;
        private int startingX = 0;
        private int startingY = 0;
        

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

                return _imageMatrix[startingY + j][startingX + i];
            }
            set {
                if (!(ReturnZeroIfOutOfBounds && (j < 0 || j >= M || i < 0 || i >= N)))
                {                    _imageMatrix[startingY + j][startingX + i] = value;

                }
            }
        }

        public int Count => N * M;
        public int N { get; set; }
        public int M { get; set; }
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

        private Image(T[][] imageMatrix, int n, int m)
        {
            _imageMatrix = imageMatrix;

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

        public Image<T> Clone()
        {
            var clonedImage = new Image<T>((T[][])_imageMatrix.Clone(), N, M);
            clonedImage.startingX = startingX;
            clonedImage.startingY = startingY;

            return clonedImage;
        }

        public void SetRegionOfInterest(Rectangle rect)
        {
            startingX += rect.X;
            startingY += rect.Y;

            N = rect.Width;
            M = rect.Height;
        }

        public Rectangle GetRegionOfInterest()
        {
            return new Rectangle(startingX, startingY, N, M);
        }

        public void ResetRegionOfInterest(Rectangle rect)
        {
            startingX = rect.X;
            startingY = rect.Y;

            N = rect.Width;
            M = rect.Height;
        }
    }
}
