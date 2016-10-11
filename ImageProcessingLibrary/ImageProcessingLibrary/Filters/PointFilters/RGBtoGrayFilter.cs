using System;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Filters.PointFilters
{
    public class RGBtoGrayFilter : PointFilter<RGB, Gray>
    {
        protected override Gray ProcessPixel(RGB pixel)
        {
            var g = Convert.ToByte(0.21 * pixel.R + 0.72 * pixel.G + 0.07 * pixel.B);
            return new Gray(g);
        }
    }
}
