using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Resizers
{
    public class BicubicResizer
    {

        public Image<Gray> Resize(Image<Gray> image, int destWidth, int destHeight)
        {
            Image<Gray> result = new Image<Gray>(destWidth, destHeight);
            image.ReturnZeroIfOutOfBounds = true;

            double tx = (double)image.N/ destWidth;
            double ty = (double)image.M / destHeight;

            short[] C = new short[4];

            for (int i = 0; i < destHeight; ++i)
            {
                for (int j = 0; j < destWidth; ++j)
                {
                    int x = (int)(tx * j);
                    int y = (int)(ty * i);
                    double dx = tx * j - x;
                    double dy = ty * i - y;

                    for (int jj = 0; jj < 4; ++jj)
                    {
                        int z = y - 1 + jj;
                        short a0 = (short)image[x, z].G;
                        short d0 = (short)(image[x - 1, z].G - a0);
                        short d2 = (short)(image[x + 1, z].G - a0);
                        short d3 = (short)(image[x + 2, z].G - a0);
                        short a1 = (short)(-1.0 / 3 * d0 + d2 - 1.0 / 6 * d3);
                        short a2 = (short)(1.0 / 2 * d0 + 1.0 / 2 * d2);
                        short a3 = (short)(-1.0 / 6 * d0 - 1.0 / 2 * d2 + 1.0 / 6 * d3);
                        C[jj] =  (short)(a0 + a1 * dx + a2 * dx * dx + a3 * dx * dx * dx);

                        d0 = (short)(C[0] - C[1]);
                        d2 = (short)(C[2] - C[1]);
                        d3 = (short)(C[3] - C[1]);
                        a0 = C[1];
                        a1 = (short)(-1.0 / 3 * d0 + d2 - 1.0 / 6 * d3);
                        a2 = (short)(1.0 / 2 * d0 + 1.0 / 2 * d2);
                        a3 = (short)(-1.0 / 6 * d0 - 1.0 / 2 * d2 + 1.0 / 6 * d3);

                        double newPixel = a0 + a1*dy + a2*dy*dy + a3*dy*dy*dy;
                        
                        var newPixelByte = Convert.ToByte(newPixel > 255 ? 255 : newPixel < 0 ? 0 : newPixel);
                        result[j, i] = new Gray(newPixelByte);
                    }
                }
            }

            return result;
      }
    }
}
