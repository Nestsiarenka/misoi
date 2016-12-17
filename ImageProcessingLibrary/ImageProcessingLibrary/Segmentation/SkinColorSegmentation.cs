using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ImageProcessingLibrary.Capacities.Structures;
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
                        && !((cb >= 60) && (cr <= 130) && (cb >= 130) && (cr <= 160)))
                    {
                        resultImage[j, i] = new RGB(255,255, 255);
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