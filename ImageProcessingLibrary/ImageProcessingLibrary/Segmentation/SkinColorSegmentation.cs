using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Segmentation
{
    public class SkinColorSegmentation
    {
        public Image<RGB> Segmentate(Image<RGB> image)
        {
            Image<RGB> resultImage = new Image<RGB>(image.N, image.M);
            double h;
            double s;
            double cb;
            double cr;

            for (int i = 0; i < image.M; i++)
            {
                for (int j = 0; j < image.N; j++)
                {
                    var pixel = image[j, i];
                    HSVcalculateH(pixel, out h, out s);
                    YCbCbCrCalculateCbCr(pixel, out cb, out cr);
                    
                    if (
                        ((pixel.R > 50 && pixel.G > 40 && pixel.B > 20 && Max(pixel) - Min(pixel) > 10
                            && pixel.R - pixel.G >= 10 && pixel.R > pixel.G && pixel.R > pixel.B) ||
                            (pixel.R > 220 && pixel.G > 210 && pixel.B > 170 && Math.Abs((byte)pixel.R - (byte)pixel.G) <= 15 && pixel.R > pixel.B
                            && pixel.G > pixel.B))
                        && (h >= 0) && (h <= 50)
                        && (s >= 0.1) && (s <= 0.9)
                        && !((cb >= 60) && (cr <= 130) && (cb >= 130) && (cr <= 160)))
                    {
                        resultImage[j, i] = new RGB(255, 255, 255);
                    }
                    else
                    {
                        resultImage[j, i] = new RGB(0, 0, 0);
                    }
                   
                }
            }
            
            return resultImage;
        }

        private void HSVcalculateH(RGB pixel, out double h, out double s)
        {
            if (pixel.B == pixel.G && pixel.B == pixel.R)
            {
               h = 0;
               s = 0;
            }

            double R = (double)pixel.R/byte.MaxValue;
            double G = (double)pixel.G/byte.MaxValue;
            double B = (double)pixel.B/byte.MaxValue;

            if (pixel.R > pixel.G && pixel.R > pixel.B)
            {
                double min = Math.Min(G, B);

                h = 60*((G - B)/(R - min) % 6);
                s = (R - min)/R;
            }
            else
            {
                if (pixel.G > pixel.B)
                {
                    double min = Math.Min(B, R);
                    h =  60 * ((B - R) / (G - min) + 2);
                    s = (G - min) / G;
                }
                else
                {
                    double min = Math.Min(G, R);
                    h = 60 * ((R - G) / (B - min) + 4);
                    s = (B - min)/B;
                }
            }
        }
        
        public Rectangle CropFace(Image<RGB> image, int widthProportion, int heightProportion)
        {
            var xHisto = new int[image.N];
            var yHisto = new int[image.M];

            for (int y = 0; y < image.M; y++)
            {
                for (int x = 0; x < image.N; x++)
                {
                    yHisto[y] += (byte)image[x, y].R;
                }
            }
            for (int x = 0; x < image.N; x++)
            {
                int sum = 0;
                for (int y = 0; y < image.M; y++)
                {
                    xHisto[x] += (byte) image[x, y].R;
                }
            }

            var xHistoRegions = CalculateHisto(xHisto, heightProportion * 255 , widthProportion);
            var yHistoRegions = CalculateHisto(yHisto, widthProportion * 255, heightProportion);

            if (xHistoRegions.Count != 0 && yHistoRegions.Count != 0)
            {
                xHistoRegions.Sort((a, b) => (b[1] - b[0]) - (a[1] - a[0]));
                yHistoRegions.Sort((a, b) => (b[1] - b[0]) - (a[1] - a[0]));

                var x1 = xHistoRegions[0][0];
                var x2 = xHistoRegions[0][1];
                var y1 = yHistoRegions[0][0];
                var y2 = yHistoRegions[0][1];
                
                return new Rectangle(x1, y1, x2 - x1, y2 - y1);
            }

            return new Rectangle();
        }

        private List<int[]> CalculateHisto(int[] histo, int minHistoHeight, int minHistoWidth)
        {
            var regions = new List<int[]>();
            int begin = 0;

            for (int i = 0; i < histo.Length; i++)
            {
                if (histo[i] >= minHistoHeight && begin == 0)
                {
                    begin = i;
                }

                if (histo[i] < minHistoHeight && begin != 0 || i == histo.Length - 1)
                {
                    if (i - begin >= minHistoWidth)
                    {
                        regions.Add(new[] { begin, i });
                        begin = i;
                    }
                    else
                    {
                        begin = i;
                    }
                }
            }

            return regions;
        }


        private void YCbCbCrCalculateCbCr(RGB pixel, out double cb, out double cr)
        {
            double R = (double)pixel.R;
            double G = (double)pixel.G;
            double B = (double)pixel.B;

            //cb = 0.564*(B - Y);
            cb = -0.148 * R - 0.291 * G + 0.439 * B + 128;
            //cr = 0.713*(R - Y);
            cr = 0.439*R - 0.368*G - 0.071*B + 128;
        }

        private byte Max(RGB pixel)
        {
            return Convert.ToByte(Math.Max(Math.Max((byte)pixel.R, (byte)pixel.G), (byte)pixel.B));
        }

        private byte Min(RGB pixel)
        {
            return Convert.ToByte(Math.Min(Math.Min((byte)pixel.R, (byte)pixel.G), (byte)pixel.B));
        }
    }

}