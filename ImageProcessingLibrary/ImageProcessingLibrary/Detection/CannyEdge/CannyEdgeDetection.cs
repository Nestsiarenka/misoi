using System;
using System.Windows.Forms.Design;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Filters.SpatialFilters;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Utilities;

namespace ImageProcessingLibrary.Detection.CannyEdge
{
    public class CannyEdgeDetection
    {

        public Image<Gray> AdoptiveBluring(Image<Gray> image, int iterations, double amplitude)
        {
            //var GyConvolution = new double[,]
            //    {{1,0,-1},
            //     {1,0,-1},
            //     {1,0,-1}};
            //var GxConvolution = new double[,]
            //    {{-1,-1,-1},
            //     {0,0,0},
            //     {1,1,1}};

            var GyConvolution = new double[,]
                {{1, 2,  0, -2, -1},
                 {4, 8,  0, -8, -4},
                 {6, 12, 0,-12, -6},
                 {4, 8,  0, -8, -4},
                 {1, 2,  0, -2, -1}};

            var GxConvolution = new double[,]
                {{-1,-4, -6,  -4,-1},
                 {-2,-8, -12, -8,-2},
                 { 0, 0,  0,   0, 0},
                 { 2, 8,  12,  8, 2},
                 { 1, 4,  6,   4, 1} };

            Image<Gray> resultImage = image.Clone();

            for (int iterationsCounter = 0; iterationsCounter < iterations; iterationsCounter++)
            {
                var Gx = Convolution(image, GxConvolution, false);
                var Gy = Convolution(image, GyConvolution, false);
                
                double[,] w = new double[image.N, image.M];

                for (int y = 0; y < image.M; y++)
                {
                    for (int x = 0; x < image.N; x++)
                    {
                        var d = Math.Sqrt((double)(Gx[x, y].G * Gx[x, y].G +
                            Gy[x, y].G * Gy[x, y].G));

                        w[x, y] = Math.Exp(Math.Sqrt(d)/(2*amplitude*amplitude));
                    }
                }

                for (int x = 1; x < image.N - 1; x++)
                {
                    for (int y = 1; y < image.M - 1; y++)
                    {
                        double n = 0;

                        for (int fx = -1; fx <= 1; fx++)
                        {
                            for (int fy = -1; fy <= 1; fy++)
                            {
                                n += w[x + fx, y + fy];
                            }
                        }

                        double pointValue = 0;
                        for (int fx = -1; fx <= 1; fx++)
                        {
                            for (int fy = -1; fy <= 1; fy++)
                            {
                                pointValue += w[x + fx, y + fy] * (image[x + fx, y + fy].G.Value / n);
                            }
                        }

                        resultImage[x, y] = new Gray((byte)pointValue);
                    }
                }
            }

            return resultImage;
        }

        public Image<Gray> MakeDetection(Image<Gray> image)
        {
            var sobelFilter = new SobelFilter();
            double tmax = 0;
            double tmin = 0;
            
            var GyConvolution = new double[,]
                {{1, 2,  0, -2, -1},
                 {4, 8,  0, -8, -4},
                 {6, 12, 0,-12, -6},
                 {4, 8,  0, -8, -4},
                 {1, 2,  0, -2, -1}};

            var GxConvolution = new double[,]
                {{-1,-4, -6,  -4,-1},
                 {-2,-8, -12, -8,-2},
                 { 0, 0,  0,   0, 0},
                 { 2, 8,  12,  8, 2},
                 { 1, 4,  6,   4, 1} };

            image = AdoptiveBluring(image, 3, 20);

            var Gx = Convolution(image, GxConvolution, false);
            var Gy = Convolution(image, GyConvolution, false);

            //var G = new Image<Gray>(image.N, image.M);

            var G = sobelFilter.Filter(image);

            /*for (int y = 0; y < image.M; y++)
            {
                for (int x = 0; x < image.N; x++)
                {
                    G[x, y] = new Gray((byte)Math.Sqrt((double)(Gx[x, y].G * Gx[x, y].G +
                        Gy[x, y].G * Gy[x, y].G)));

                }
            }*/
            

            int Width = image.N;
            int Height = image.M;

            var nms = G.Clone();

            double angle;

            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    angle = Math.Atan2((byte)Gy[x, y].G, (byte)Gx[x, y].G) * 180 / Math.PI;

                    if (angle < 0)
                    {
                        angle += 180;
                    }
                    
                    if ((angle >= 0 &&  angle < 22.5) || (angle >= 157.5 && angle <= 180))
                    {
                        if ((G[x, y].G < G[x - 1, y].G) || (G[x, y].G < G[x + 1, y].G))
                        {
                            nms[x, y] = new Gray(0);
                        }
                    }
                    
                    if (angle >= 22.5 && angle < 67.5)
                    {
                        if ((G[x, y].G < G[x + 1, y - 1].G) || (G[x, y].G < G[x - 1, y + 1].G))
                        {
                            nms[x, y] = new Gray(0);
                        }
                    }
                    

                    if (angle >= 67.5 && angle < 112.5)
                    {
                        if ((G[x, y].G < G[x, y - 1].G) || (G[x, y].G < G[x, y + 1].G))
                        { 
                            nms[x, y] = new Gray(0);
                        }
                    }
                    

                    if (angle >= 112.5 && angle < 157.5)
                    {
                        if ((G[x, y].G < G[x - 1, y - 1].G) || (G[x, y].G < G[x + 1, y + 1].G))
                        {
                            nms[x, y] = new Gray(0);
                        }
                    }
                }
            }

            //var histogram = new Histogram(nms);
            //var sumB = 0;
            //var maxN = 0.0;
            //int sum = 0;
            //int total = 0;

            tmax = 77;
            tmin = 70;

            //foreach (int i in histogram)
            //{
            //    total += i;
            //}

            //for (int i = 0; i < 256; i++)
            //{
            //    sum += i*histogram[i];
            //}

            //for (int i = 0; i < 256; i++)
            //{
            //    int wCurrent = 0;

            //    wCurrent += histogram[i];
            //    if (wCurrent == 0)
            //    {
            //        continue;
            //    }

            //    int w1Current = total - wCurrent;
            //    if (w1Current == 0)
            //    {
            //        break;
            //    }

            //    sumB += i * histogram[i];

            //    double mCurrent = (double)sumB / wCurrent;
            //    double m1Current = (double)(sum - sumB) / w1Current;

            //    var n = wCurrent * w1Current * (m1Current - mCurrent) * (m1Current - mCurrent);

            //    if (n > maxN)
            //    {
            //        tmax = i;
            //        maxN = n;
            //    }
            //}

            //tmin = tmax / 2;

            Image<Gray> resultImage = new Image<Gray>(image.N, image.M);

            for (int x = 0; x < resultImage.N; x++)
            {
                for (int y = 0; y < resultImage.M; y++)
                {
                    resultImage[x, y] = new Gray(0);
                }
            }

            int[,] edgeMap = new int[nms.N, nms.M];
            int[,] visitedMap = new int[nms.N, nms.M];

            var edgePoints = new int[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (nms[x, y].G >= tmax)
                    {
                        edgePoints[x, y] = 1;
                    }
                    else if (nms[x, y].G >= tmin)
                    {
                        edgePoints[x, y] = 2;
                    }
                }
            }

            for (int x = 1; x < Width - 1 ; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    if (edgePoints[x, y] == 1)
                    {
                        edgeMap[x, y] = 1;
                        Travers(x, y, visitedMap, edgeMap, edgePoints);
                        visitedMap[x, y] = 1;
                    }
                }
            }

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    resultImage[x, y] = new Gray((byte)(edgeMap[x, y] * 255));
                }
            }

            return resultImage;
        }

        private void Travers(int x, int y, int[,] visitedMap, int[,] edgeMap, int[,] edgePoints)
        {
            if (visitedMap[x, y] == 1)
            {
                return;
            }
            
            if (edgePoints[x + 1, y] == 2)
            {
                edgeMap[x + 1, y] = 1;
                visitedMap[x + 1, y] = 1;
                Travers(x + 1, y, visitedMap, edgeMap, edgePoints);
                return;
            }

            if (edgePoints[x + 1, y - 1] == 2)
            {
                edgeMap[x + 1, y - 1] = 1;
                visitedMap[x + 1, y - 1] = 1;
                Travers(x + 1, y - 1, visitedMap, edgeMap, edgePoints);
                return;
            }

            if (edgePoints[x, y - 1] == 2)
            {
                edgeMap[x, y - 1] = 1;
                visitedMap[x, y - 1] = 1;
                Travers(x, y - 1, visitedMap, edgeMap, edgePoints);
                return;
            }
            
            if (edgePoints[x - 1, y - 1] == 2)
            {
                edgeMap[x - 1, y - 1] = 1;
                visitedMap[x - 1, y - 1] = 1;
                Travers(x - 1, y - 1, visitedMap, edgeMap, edgePoints);
                return;
            }

            if (edgePoints[x - 1, y] == 2)
            {
                edgeMap[x - 1, y] = 1;
                visitedMap[x - 1, y] = 1;
                Travers(x - 1, y, visitedMap, edgeMap, edgePoints);
                return;
            }
            
            if (edgePoints[x - 1, y + 1] == 2)
            {
                edgeMap[x - 1, y + 1] = 1;
                visitedMap[x - 1, y + 1] = 1;
                Travers(x - 1, y + 1, visitedMap, edgeMap, edgePoints);
                return;
            }
            
            if (edgePoints[x, y + 1] == 2)
            {
                edgeMap[x, y + 1] = 1;
                visitedMap[x, y + 1] = 1;
                Travers(x, y + 1, visitedMap, edgeMap, edgePoints);
                return;
            }

            if (edgePoints[x + 1, y + 1] == 2)
            {
                edgeMap[x + 1, y + 1] = 1;
                visitedMap[x + 1, y + 1] = 1;
                Travers(x + 1, y + 1, visitedMap, edgeMap, edgePoints);
            }
        }

    private Image<Gray> Convolution(Image<Gray> image, double[,] kernel, bool normalize)
        {
            int kernelHalf = kernel.GetLength(0) / 2;
            double min = double.MaxValue;
            double max = double.MinValue;

            Image<Gray> resultImage = new Image<Gray>(image.N, image.M);

            for (int x = 0; x < resultImage.N; x++)
            {
                for (int y = 0; y < resultImage.M; y++)
                {
                    resultImage[x, y] = new Gray(0);
                }
            }

            if (normalize)
            {
                for (int y = kernelHalf; y < image.M - kernelHalf; y++)
                {
                    for (int x = kernelHalf; x < image.N - kernelHalf; x++)
                    {
                        double pixel = 0;

                        for (int fy = -kernelHalf; fy <= kernelHalf; fy++)
                        {
                            for (int fx = -kernelHalf; fx < kernelHalf; fx++)
                            {
                                pixel += (double)(kernel[kernelHalf + fx, kernelHalf + fy] * image[x + fx, y + fy].G);
                            }
                        }

                        if (pixel < min)
                        {
                            min = pixel;
                        }
                        if (pixel > max)
                        {
                            max = pixel;
                        }
                    }
                }
            }

            for (int y = kernelHalf; y < image.M - kernelHalf; y++)
            {
                for (int x = kernelHalf; x < image.N - kernelHalf; x++)
                {
                    double pixel = 0;

                    for (int fy = -kernelHalf; fy <= kernelHalf; fy++)
                    {
                        for (int fx = -kernelHalf; fx < kernelHalf; fx++)
                        {
                            pixel += (double)(kernel[kernelHalf + fx, kernelHalf + fy] * image[x + fx, y + fy].G);
                        }
                    }

                    if (normalize)
                    {
                        pixel = 255 * (pixel - min) / (max - min);
                    }

                    resultImage[x, y] = new Gray((byte)pixel);
                }
            }

            return resultImage;
        }
    }
}
