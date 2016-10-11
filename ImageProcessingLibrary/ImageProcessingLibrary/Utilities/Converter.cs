using System;
using System.Drawing;
using System.Drawing.Imaging;
using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Utilities
{
    public class Converter
    {
        public delegate Color BitmapInitializator<T>(int i, int j, T pixel) where T: struct, ICapacity;

        public static Image<RGB> ToImage(Bitmap bitmap)
        {
            int n = bitmap.Width;
            int m = bitmap.Height;

            Image<RGB> image = new Image<RGB>(n, m);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    image[i, j] = new RGB(pixel.R, pixel.G, pixel.B);
                }
            }

            return image;
        }

        private static Bitmap InitializeBitmapFromImage<T> (BitmapInitializator<T> initializator, 
            Image<T> image, PixelFormat pixelFormat = PixelFormat.Format32bppRgb)
            where T: struct, ICapacity
        {
            int n = image.N;
            int m = image.M;

            var bitmap = new Bitmap(n, m, pixelFormat);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    Color color = initializator(i, j, image[i, j]);
                    bitmap.SetPixel(i, j, color);
                }
            }

            return bitmap;
        }

        public static Bitmap ToBitmap(Image<RGB> image)
        {
            return InitializeBitmapFromImage((i, j, pixel) => Color.FromArgb((byte)pixel.R, (byte)pixel.G, (byte)pixel.B), image);
        }

        public static Bitmap ToBitmap(Image<Gray> image)
        {
            return InitializeBitmapFromImage((i, j, pixel) => Color.FromArgb((byte)pixel.G), image, PixelFormat.Format8bppIndexed);
        }
    }
}