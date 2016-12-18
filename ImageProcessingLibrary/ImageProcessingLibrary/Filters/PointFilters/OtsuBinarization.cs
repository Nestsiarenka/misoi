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
    public class OtsuBinarization : HistogramBasedFilters<Gray>
    {
        private int threshHold = 0;

        protected override Histogram HistogramCreation(Image<Gray> image)
        {
            var histogram = new Histogram(image);
            var pixelsCount = image.Count;
            double n;
            double maxN = double.MinValue;

            for (int i = 0; i < 256; i++)
            {
                double wCurrent = 0;
                double mCurrent = 0;

                for (int j = 0; j <= i; j++)
                {
                    double p = (double) histogram[j] / pixelsCount;
                    wCurrent += p;
                    mCurrent += j * p;
                }

                if (wCurrent > 0 + 1e8)
                {
                    mCurrent /= wCurrent;
                }

                double w1Current = 1 - wCurrent;
                double m1Current = 0;

                for (int j = i + 1; j < 256; j++)
                {
                    double p = (double) histogram[j]/pixelsCount;
                    
                    m1Current += j*p;
                }

                if (w1Current > 0 + 1e8)
                {
                    m1Current /= w1Current;
                }

                n = wCurrent*w1Current*(m1Current - mCurrent)*(m1Current - mCurrent);

                if (n > maxN)
                {
                    maxN = n;
                    threshHold = i;
                }
            }
            return histogram;
        }

        protected override Gray ProcessPixel(Gray pixel)
        {
            return pixel.G < threshHold ? new Gray(0) : new Gray(255);
        }
    }
}
