using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Exceptions;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Utilities
{
    public class Converter
    {
        public delegate void BitmapInitializator<T>(int i, int j, T pixel, byte[] array) where T: struct, ICapacity;

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

            byte[] imageLikeByteArray;
            int stride;

            switch (pixelFormat)
            {
                case PixelFormat.Format32bppRgb:
                    imageLikeByteArray = new byte[n * m * 4];
                    stride = n * 4;
                    break;
                case PixelFormat.Format8bppIndexed:
                    imageLikeByteArray = new byte[n * m];
                    stride = n;
                    break;
                default:
                    throw new ConverterException("Unsupported PixelFormat type");
            }

            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n ; i++)
                {
                    initializator(i, j, image[i, j], imageLikeByteArray);
                }
            }
            
            var bitmap = new Bitmap(n, m, pixelFormat);
            var data = bitmap.LockBits(new Rectangle(0, 0, n, m),
                ImageLockMode.WriteOnly, pixelFormat);

            Marshal.Copy(imageLikeByteArray, 0, data.Scan0, imageLikeByteArray.Length);

            bitmap.UnlockBits(data);
            return bitmap;
        }

        public static Bitmap ToBitmap(Image<RGB> image)
        {
            return InitializeBitmapFromImage((i, j, pixel, array) =>
            {
                var currentIndex = (j * image.N + i) * 4;
                array[currentIndex] = (byte)pixel.B;
                array[currentIndex + 1] = (byte)pixel.G;
                array[currentIndex + 2] = (byte)pixel.R;
            }, image);
        }

        public static Bitmap ToBitmap(Image<Gray> image)
        {
            return InitializeBitmapFromImage((i, j, pixel, array) =>
            {
               var currentIndex = j * image.N + i;
               array[currentIndex] = (byte)pixel.G;
            }, image, PixelFormat.Format8bppIndexed);
        }
    }
}