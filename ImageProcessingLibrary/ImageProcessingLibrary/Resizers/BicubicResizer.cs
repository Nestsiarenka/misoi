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

        public Image<Gray> Resize(Image<Gray> image,
            int srcWidth, int srcHeight, int destWidth, int destHeight)
        {
            Image<Gray> result = new Image<Gray>(destWidth, destHeight);
            image.ReturnZeroIfOutOfBounds = true;

            double tx = (double)srcWidth / destWidth;
            double ty = (double)srcHeight / destHeight;

            byte[] C = new byte[4];

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
                        byte a0 = (byte)image[x, z].G;
                        byte d0 = (byte)(image[x - 1, z].G - a0);
                        byte d2 = (byte)(image[x + 1, z].G - a0);
                        byte d3 = (byte)(image[x + 2, z].G - a0);
                        byte a1 = (byte)(-1.0 / 3 * d0 + d2 - 1.0 / 6 * d3);
                        byte a2 = (byte)(1.0 / 2 * d0 + 1.0 / 2 * d2);
                        byte a3 = (byte)(-1.0 / 6 * d0 - 1.0 / 2 * d2 + 1.0 / 6 * d3);
                        C[jj] =  (byte)(a0 + a1 * dx + a2 * dx * dx + a3 * dx * dx * dx);

                        d0 = (byte)(C[0] - C[1]);
                        d2 = (byte)(C[2] - C[1]);
                        d3 = (byte)(C[3] - C[1]);
                        a0 = C[1];
                        a1 = (byte)(-1.0 / 3 * d0 + d2 - 1.0 / 6 * d3);
                        a2 = (byte)(1.0 / 2 * d0 + 1.0 / 2 * d2);
                        a3 = (byte)(-1.0 / 6 * d0 - 1.0 / 2 * d2 + 1.0 / 6 * d3);

                        var newPixel = a0 + a1*dy + a2*dy*dy + a3*dy*dy*dy;
                        var newPixelByte = Convert.ToByte(newPixel > 255 ? 255 : newPixel);
                        result[j, i] = new Gray(newPixelByte);
                    }
                }
            }

            return result;
      }
    }
}
