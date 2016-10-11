using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Utilities;

namespace ImageProcessingLibrary.Filters.PointFilters
{
    public class FSHS : PointFilter<Gray, Gray>
    {
        private Histogram _histogram;

        public override Image<Gray> Filter(Image<Gray> image)
        {
            _histogram = new Histogram(image);
            return base.Filter(image);
        }

        protected override Gray ProcessPixel(Gray pixel)
        {
            double stagingG = (255.0/(_histogram.LastNonZero - _histogram.FirstNonZero))*
                              ((double)pixel.G - _histogram.FirstNonZero);
            return new Gray(Convert.ToByte(stagingG));
        }
    }
}
