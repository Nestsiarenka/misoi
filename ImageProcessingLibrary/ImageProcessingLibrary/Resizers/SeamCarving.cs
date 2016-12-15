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
        private readonly int[,] energy;
        private readonly int[,] sum;

        private int i;
        private int j;


        public SeamCarving(Image<Gray> image)
        {
            _image = image;
            energy = new int[image.N, image.M];
            sum = new int[image.N, image.M];
        }

        private void FindEnergy()
        {
            for (i = 0; i < _image.N; i++)
            {
                for (j = 0; j < _image.M; j++)
                {
                    energy[i, j] = 0;

                    int sum = 0, count = 0;

                    if (i != _image.N - 1)
                    {
                        count++;
                        sum += Math.Abs((int)_image[i, j].G - (int)_image[i + 1, j].G);
                    }

                    if (j != _image.M - 1)
                    {
                        count++;
                        sum += Math.Abs((int)_image[i, j].G - (int)_image[i, j + 1].G);
                    }

                    if (count != 0)
                    {
                        energy[i, j] = sum / count;
                    }
                }
            }
        }

        /*
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
        }*/

        private void FindSum()
        {
            for (j = 0; j < _image.N; j++)
            {
                sum[j, 0] = energy[j, 0];
            }

            for (i = 1; i < _image.M; i++)
            {
                for ( j = 0; j < _image.N; j++)
                {
                    sum[j, i] = sum[j, i - 1];

                    if (j > 0 && sum[j - 1, i - 1] < sum[j, i])
                    {
                        sum[j, i] = sum[j - 1, i - 1];
                    }

                    if (j < _image.N - 1 && sum[j + 1, i - 1] < sum[j, i])
                    {
                        sum[j, i] = sum[j + 1, i - 1];
                    }

                    sum[j, i] += energy[j, i];
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
                if (sum[j, last] < sum[res[last], last])
                {
                    res[last] = j;
                }
            }

            for (i = last - 1; i >= 0; i--)
            {
                int prev = res[i + 1];

                res[i] = prev;
                if (prev > 0 && sum[res[i], i] > sum[prev - 1, i])
                {
                    res[i] = prev - 1;
                }
                if (prev < _image.N - 1 && sum[res[i], i] > sum[prev + 1, i])
                {
                    res[i] = prev + 1;
                }
            }

            return res;
        }

        private void FindSumHorizontal()
        {
            for (j = 0; j < _image.M; j++)
            {
                sum[0, j] = energy[0, j];
            }

            for (i = 1; i < _image.N; i++)
            {
                for (j = 0; j < _image.M; j++)
                {
                    sum[i, j] = sum[i - 1, j];

                    if (j > 0 && sum[i - 1, j - 1] < sum[i, j])
                    {
                        sum[i, j] = sum[i - 1, j - 1];
                    }

                    if (j < _image.M - 1 && sum[i - 1, j + 1] < sum[i, j])
                    {
                        sum[i, j] = sum[i - 1, j + 1];
                    }

                    sum[i, j] += energy[i, j];
                }
            }
        }

        private int[] FindShrinkedPixelsHorizontal()
        {
            int last = _image.N - 1;
            int[] res = new int[_image.N];

            res[last] = 0;

            for (j = 1; j < _image.M; j++)
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
                if (prev < _image.M - 1 && sum[i, res[i]] > sum[i, prev + 1])
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

            int[] cropPixels = FindShrinkedPixels();

            for (i = 0; i < _image.M; i++)
            {
                for (j = cropPixels[i]; j < _image.N; j++)
                {
                    if (j + 1 < _image.N - 1 )
                    {
                        _image[j, i] = new Gray(_image[j + 1, i].G);
                    }
                }
            }
        }

        private void DecHeight()
        {
            FindEnergy();
            FindSumHorizontal();

            int[] cropPixels = FindShrinkedPixelsHorizontal();

            for (i = 0; i < _image.N; i++)
            {
                for (j = cropPixels[i]; j < _image.M; j++)
                {
                    if (j + 1 < _image.M - 1)
                    {
                        _image[i, j] = new Gray(_image[i, j + 1].G);
                    }
                }
            }
        }

        public Image<Gray> ChangeSize(int decWidth, int decHeight)
        {
            var maxChange = Math.Max(decHeight, decWidth);
            for (int i = 0; i < maxChange; i++)
            {
                if (i < decWidth)
                {
                    DecWidth();
                    _image.N--;
                } 

                if (i < decHeight)
                {
                    DecHeight();
                    _image.M--;
                }
            }

            return _image;
        }
    }
}
