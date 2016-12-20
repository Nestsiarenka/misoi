using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Schema;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Detection.CannyEdge;
using ImageProcessingLibrary.Filters.PointFilters;
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
                        && !((cb >= 60) && (cr <= 130) && (cb >= 130) && (cr <= 160))
                        )
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

        public List<Rectangle> SegmentateEyes(Image<Gray> grayImage)
        {

            var edgeDetection = new CannyEdgeDetection();
            var edgedImage = edgeDetection.MakeDetection(grayImage);

            edgedImage.SetRegionOfInterest(new Rectangle(grayImage.N * 1 / 10, grayImage.M  * 8 / 40 , grayImage.N * 8 / 10, grayImage.M * 6 / 20));

            var previousRegionOfInterest = edgedImage.GetRegionOfInterest();

            edgedImage.SetRegionOfInterest(new Rectangle(0, 0 , edgedImage.N / 2, edgedImage.M));

            var eyeRegionLeft = ComputeRegion(edgedImage);
            var oldRegionOfInterest = edgedImage.GetRegionOfInterest();

            var newRegionOfInterestLeft = new Rectangle(oldRegionOfInterest.X + eyeRegionLeft.X, 
                oldRegionOfInterest.Y + eyeRegionLeft.Y, 
                eyeRegionLeft.Width, eyeRegionLeft.Height);
            
            edgedImage.ResetRegionOfInterest(previousRegionOfInterest);
            edgedImage.SetRegionOfInterest(new Rectangle(edgedImage.N / 2, 0, edgedImage.N / 2, edgedImage.M));
            
            var eyeRegionRight = ComputeRegion(edgedImage);
            oldRegionOfInterest = edgedImage.GetRegionOfInterest();

            var newRegionOfInterestRight = new Rectangle(oldRegionOfInterest.X + eyeRegionRight.X,
                oldRegionOfInterest.Y + eyeRegionRight.Y,
                eyeRegionLeft.Width, eyeRegionLeft.Height);
            return new List<Rectangle> {newRegionOfInterestLeft, newRegionOfInterestRight};
        }

        public Rectangle SegmentateLips(Image<Gray> grayImage)
        {
            
            var edgeDetection = new CannyEdgeDetection();
            var edgedImage = edgeDetection.MakeDetection(grayImage);

            int croppedX = grayImage.N * 2 / 10;
            int croppedY = grayImage.M * 6 / 10;

            edgedImage.SetRegionOfInterest(new Rectangle(croppedX, croppedY, grayImage.N - croppedX - 2 * grayImage.N / 10, grayImage.M - croppedY - grayImage.M * 2 / 10));
            
            //grayImage.SetRegionOfInterest(new Rectangle(grayImage.N / 4, grayImage.M / 2, grayImage.N / 2, grayImage.M / 2));


            //var grayImage = new RGBtoGrayFilter().Filter(imageClone);
            //var medianImage = new MedianFilter(new bool[5, 5]).Filter(grayImage);
            //var gauss = new GaussianFilter().Filter(medianImage);
            //var sobeledImage = new OtsuBinarization().Filter(gauss);
            //var sobeledImage = new SobelFilter().Filter(binaryImage);

            //var xHisto = new int[edgedImage.N];
            //var yHisto = new int[edgedImage.M];

            //for (int x = 0; x < edgedImage.N; x++)
            //{
            //    int sum = 0;
            //    for (int y = 0; y < edgedImage.M; y++)
            //    {
            //        xHisto[x] += (byte)edgedImage[x, y].G;
            //    }
            //}

            //for (int y = 0; y < edgedImage.M; y++)
            //{
            //    for (int x = 0; x < edgedImage.N; x++)
            //    {
            //        yHisto[y] += (byte)edgedImage[x, y].G;
            //    }
            //}

            //int xMax = 0;
            //List<int> maxIndexes = new List<int>();

            //for (int i = 0; i < xHisto.Length; i++)
            //{
            //    if (xHisto[i] > xMax)
            //    {
            //        xMax = xHisto[i];
            //        maxIndexes.Clear();
            //        maxIndexes.Add(i);
            //    }
            //    else if (xHisto[i] == xMax)
            //    {
            //        maxIndexes.Add(i);
            //    }
            //}

            //xMax = (int)maxIndexes.Average();

            //int[] d1 = new int[yHisto.Length];
            //int[] d2 = new int[yHisto.Length];
            //int[] d = new int[yHisto.Length];

            //for (int y = 1; y < yHisto.Length - 1; y++)
            //{
            //    d1[y] = yHisto[y - 1] - yHisto[y];
            //    d2[y] = yHisto[y] - yHisto[y + 1];
            //    d[y] = yHisto[y] / 2;
            //}

            //int[] w = new int[yHisto.Length];

            //for (int y = 1; y < yHisto.Length - 1; y++)
            //{
            //    if (d1[y] < 0 || d2[y] < 0 && (d1[y] >= d[y] || d2[y] >= d[y]))
            //    {
            //        w[y] = 0;
            //    }
            //    else
            //    {
            //        w[y] = yHisto[y];
            //    }
            //}

            //int yMax = 0;

            //for (int i = 0; i < yHisto.Length; i++)
            //{
            //    if (w[i] > yMax)
            //    {
            //        yMax = w[i];
            //        maxIndexes.Clear();
            //        maxIndexes.Add(i);
            //    }
            //    else if (w[i] == yMax)
            //    {
            //        maxIndexes.Add(i);
            //    }
            //}

            //yMax = (int)maxIndexes.Average();


            //int left = xMax, right = xMax;
            //for (int i = 0, x = xMax; i < xHisto.Length * 8 / 15; i++, x--)
            //{
            //    if (x < 0)
            //    {
            //        break;
            //    }

            //    if (xHisto[x] > 0)
            //    {
            //        left = x;
            //    }
            //}

            //for (int i = 0, x = xMax; i < xHisto.Length * 4 / 10; i++, x++)
            //{
            //    if (x >= xHisto.Length)
            //    {
            //        break;
            //    }

            //    if (xHisto[x] > 0)
            //    {
            //        right = x;
            //    }
            //}

            //int bottom = yMax, top = yMax;
            //for (int i = 0, y = yMax; i < yHisto.Length * 4 / 10; i++, y--)
            //{
            //    if (y < 0)
            //    {
            //        break;
            //    }

            //    if (yHisto[y] > 0)
            //    {
            //        top = y;
            //    }
            //}

            //for (int i = 0, y = yMax; i < yHisto.Length * 4 / 10; i++, y++)
            //{
            //    if (y >= yHisto.Length)
            //    {
            //        break;
            //    }

            //    if (xHisto[y] > 0)
            //    {
            //        bottom = y;
            //    }
            //}

            //do
            //{
            //    top--;
            //    bottom++;
            //} while ((top > 0 && bottom < yHisto.Length) && (yHisto[top] / yHisto[yMax] > 0.2 || yHisto[bottom] / yHisto[yMax] > 0.2));


            //if (xHistoRegions.Count != 0 && yHistoRegions.Count != 0)
            //{
            //    xHistoRegions.Sort((a, b) => (b[1] - b[0]) - (a[1] - a[0]));
            //    yHistoRegions.Sort((a, b) => (b[1] - b[0]) - (a[1] - a[0]));

            //    var x1 = xHistoRegions[0][0];
            //    var x2 = xHistoRegions[0][1];
            //    var y1 = yHistoRegions[0][0];
            //    var y2 = yHistoRegions[0][1];

            //    var oldInterestRegion = imageClone.GetRegionOfInterest();

            //    return new Rectangle(oldInterestRegion.X + x1, oldInterestRegion.Y + y1, x2 - x1, y2 - y1);
            //}



            Rectangle mouthRegion = ComputeRegion(edgedImage);

            var oldInterestRegion = edgedImage.GetRegionOfInterest();

            return new Rectangle(oldInterestRegion.X + mouthRegion.X, oldInterestRegion.Y + mouthRegion.Y, mouthRegion.Width, mouthRegion.Height);
            //return new Rectangle(oldInterestRegion.X + xMax, oldInterestRegion.Y + yMax,  3, 3);
        }

        public Rectangle ComputeRegion(Image<Gray> edgedImage)
        {
            var xHisto = new int[edgedImage.N];
            var yHisto = new int[edgedImage.M];

            for (int x = 0; x < edgedImage.N; x++)
            {
                int sum = 0;
                for (int y = 0; y < edgedImage.M; y++)
                {
                    xHisto[x] += (byte)edgedImage[x, y].G;
                }
            }

            for (int y = 0; y < edgedImage.M; y++)
            {
                for (int x = 0; x < edgedImage.N; x++)
                {
                    yHisto[y] += (byte)edgedImage[x, y].G;
                }
            }

            int xMax = 0;
            List<int> maxIndexes = new List<int>();

            for (int i = 0; i < xHisto.Length; i++)
            {
                if (xHisto[i] > xMax)
                {
                    xMax = xHisto[i];
                    maxIndexes.Clear();
                    maxIndexes.Add(i);
                }
                else if (xHisto[i] == xMax)
                {
                    maxIndexes.Add(i);
                }
            }

            xMax = (int)maxIndexes.Average();

            int[] d1 = new int[yHisto.Length];
            int[] d2 = new int[yHisto.Length];
            int[] d = new int[yHisto.Length];

            for (int y = 1; y < yHisto.Length - 1; y++)
            {
                d1[y] = yHisto[y - 1] - yHisto[y];
                d2[y] = yHisto[y] - yHisto[y + 1];
                d[y] = yHisto[y] / 2;
            }

            int[] w = new int[yHisto.Length];

            for (int y = 1; y < yHisto.Length - 1; y++)
            {
                if (d1[y] < 0 || d2[y] < 0 && (d1[y] >= d[y] || d2[y] >= d[y]))
                {
                    w[y] = 0;
                }
                else
                {
                    w[y] = yHisto[y];
                }
            }

            int yMax = 0;

            for (int i = 0; i < yHisto.Length; i++)
            {
                if (w[i] > yMax)
                {
                    yMax = w[i];
                    maxIndexes.Clear();
                    maxIndexes.Add(i);
                }
                else if (w[i] == yMax)
                {
                    maxIndexes.Add(i);
                }
            }

            yMax = (int)maxIndexes.Average();

            int left = xMax, right = xMax;
            for (int i = 0, x = xMax; i < xHisto.Length * 8 / 15; i++, x--)
            {
                if (x < 0)
                {
                    break;
                }

                if (xHisto[x] > 0)
                {
                    left = x;
                }
            }

            for (int i = 0, x = xMax; i < xHisto.Length * 4 / 10; i++, x++)
            {
                if (x >= xHisto.Length)
                {
                    break;
                }

                if (xHisto[x] > 0)
                {
                    right = x;
                }
            }

            int bottom = yMax, top = yMax;
            for (int i = 0, y = yMax; i < yHisto.Length * 4 / 10; i++, y--)
            {
                if (y < 0)
                {
                    break;
                }

                if (yHisto[y] > 0)
                {
                    top = y;
                }
            }

            for (int i = 0, y = yMax; i < yHisto.Length * 4 / 10; i++, y++)
            {
                if (y >= yHisto.Length)
                {
                    break;
                }

                if (yHisto[y] > 0)
                {
                    bottom = y;
                }
            }
            
            return new Rectangle(left, top, right - left, bottom - top);
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

                var oldInterestRegion = image.GetRegionOfInterest();

                var faceRegion = new Rectangle(oldInterestRegion.X + x1, oldInterestRegion.Y + y1, x2 - x1, y2 - y1);

                return faceRegion;
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