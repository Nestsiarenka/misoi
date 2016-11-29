using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Resizers
{
    public class SeamCarving
    {
        private readonly Image<Gray> _image;
        private readonly double[,] energy;
        private readonly double[,] sum;

        private int i;
        int j;


        public SeamCarving(Image<Gray> image)
        {
            _image = image;
            energy = new double[image.N, image.M];
            sum = new double[image.N, image.M];
        }

        private void FindEnergy()
        {
            for (i = 0; i < _image.M; i++)
            {
                for (j = 0; j < _image.N; j++)
                {
                    energy[i, j] = 0;

                    int sum = 0, count = 0;

                    if (i != _image.M - 1)
                    {
                        count++;
                        sum += Math.Abs((int)_image[i, j].G - (int)_image[i + 1, j].G);
                    }

                    if (j != _image.N - 1)
                    {
                        count++;
                        sum += Math.Abs((int)_image[i, j].G - (int)_image[i, j + 1].G);
                    }

                    if (count != 0)
                    {
                        energy[i, j] = (double)sum / count;
                    }
                }
            }
        }

        private void FindSum()
        {
            for (j = 0; j < _image.N; j++)
            {
                sum[0, j] = energy[0, j];
            }

            for (i = 1; i < _image.M; i++)
            {
                for ( j = 0; j < _image.N; j++)
                {
                    sum[i, j] = sum[i - 1, j];

                    if (j > 0 && sum[i - 1, j - 1] < sum[i, j])
                    {
                        sum[i, j] = sum[i - 1, j - 1];
                    }

                    if (j < _image.N - 1 && sum[i - 1, j + 1] < sum[i, j])
                    {
                        sum[i, j] = sum[i - 1, j + 1];
                    }

                    sum[i, j] += energy[i, j];
                }
            }
        }

        private int[] FindShrinkedPixels()
        {
            int last = _image.M - 1;
            int[] res = new int[_image.M];

            res[last] = 0;

            for (j = 1; j < _image.N; j++)
            {
                if (sum[last, j] < sum[last, res[last]])
                {
                    res[last] = j;
                }
            }

            for (i = last - 1; i >= 0; i--)
            {
                int prev = res[i + 1];

                res[i] = prev;
                if (prev > 0 && sum[i, res[i]] > sum[i, prev - 1])
                {
                    res[i] = prev - 1;
                }
                if (prev < _image.N - 1 && sum[i, res[i]] > sum[i, prev + 1])
                {
                    res[i] = prev + 1;
                }
            }

            return res;
        }

        private void DecWidth()
        {
            FindEnergy();
            FindSum();
            FindShrinkedPixels();

            int[] cropPixels = FindShrinkedPixels();

            for (int i = 0; i < _image.M; i++)
            for (int j = cropPixels[i]; j < _image.N; j++)
            {
                _image[i, j] = new Gray(_image[i, j + 1].G);
            }
        }

        public Image<Gray> ChangeSize(int decWidth, int decHeight)
        {
            for (int i = 0; i < decWidth; i++)
            {
                DecWidth();
                _image.M--;
            }
            return _image;
        }
    }
}
