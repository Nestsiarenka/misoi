using System;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Utilities;

namespace ImageProcessingLibrary.Filters.PointFilters
{
    public class LogarithmOperation : HistogramBasedFilters<Gray>
    {
        public override Image<Gray> Filter(Image<Gray> image)
        {
            var newImage = base.Filter(image);

            FSHS fshs = new FSHS();

            return fshs.Filter(newImage);
        }

        protected override Gray ProcessPixel(Gray pixel)
        {
           byte grayValue = Convert.ToByte(255 * Math.Log(1 + (double)pixel.G) / Math.Log(_histogram.LastNonZero));

           return new Gray(grayValue);
        }
    }
}