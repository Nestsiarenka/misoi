using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Filters.SpatialFilters
{
    public class RealGaussianFilter : ConvolutionFilter
    {

        public RealGaussianFilter(double sigma) : base(ComputeGaussionKernel(sigma))
        {
            
        }

        public static double[,] ComputeGaussionKernel(double sigma)
        {
            int filterSize = 2 * (int)(2 * sigma) + 3;
            double[,] kernel = new double[filterSize, filterSize];
            double mean = Math.Floor(filterSize / 2.0);

            for (int y = 0; y < filterSize; y++)
            {
                for (int x = 0; x < filterSize; x++)
                {
                    var yMean = (y - mean) / sigma;
                    yMean *= yMean;
                    var xMean = (x - mean) / sigma;
                    xMean *= xMean;

                    kernel[x, y] = Math.Exp(-0.5 * (yMean + xMean)) / (2 * Math.PI * sigma * sigma);
                }
            }

            return kernel;
        }
    }
}
