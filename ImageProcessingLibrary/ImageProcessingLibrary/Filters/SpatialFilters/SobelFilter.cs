using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Filters.SpatialFilters
{
    public class SobelFilter : SpatialFilter<Gray, Gray>
    {
        public override Image<Gray> Filter(Image<Gray> image)
        {
            image.ReturnZeroIfOutOfBounds = true;

            return base.Filter(image);
        }

        protected override Gray ProcessPixel(int i, int j, Image<Gray> image)
        {
            int P1 = (int)image[i - 1, j - 1].G;
            int P2 = (int)image[i, j - 1].G;
            int P3 = (int)image[i + 1, j - 1].G;
            int P8 = (int)image[i - 1, j].G;
            int P4 = (int)image[i + 1, j].G;
            int P7 = (int)image[i - 1, j + 1].G;
            int P6 = (int)image[i, j + 1].G;
            int P5 = (int)image[i + 1, j + 1].G;

            int G = Math.Abs(P1 + 2*P2 + P3 - P7 - 2*P6 - P5) +
                    Math.Abs(P3 + 2*P4 + P5 - P1 - 2*P8 - P7);

            if (G > 255)
            {
                G = 255;
            }
            else if (G < 0)
            {
                G = 0;
            }

            return new Gray((byte)G);
        }
    }
}
