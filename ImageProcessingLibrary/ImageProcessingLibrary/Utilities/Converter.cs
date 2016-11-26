using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Exceptions;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Utilities.Enums;

namespace ImageProcessingLibrary.Utilities
{
    public class Converter
    {
        public delegate void BitmapInitializator<T>(int c, T pixel, byte[] array) where T: struct, ICapacity;

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
            Image<T> image, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
            where T: struct, ICapacity
        {
            int n = image.N;
            int m = image.M;

            var bitmap = new Bitmap(n, m, pixelFormat);
            var data = bitmap.LockBits(new Rectangle(0, 0, n, m),
                ImageLockMode.WriteOnly, pixelFormat);

            byte[] imageLikeByteArray;
            int stride = data.Stride;

            imageLikeByteArray = new byte[m * stride];

            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    initializator(j * stride + i * (stride/n), image[i, j], imageLikeByteArray);
                }
            }

            Marshal.Copy(imageLikeByteArray, 0, data.Scan0, imageLikeByteArray.Length);

            bitmap.UnlockBits(data);
            return bitmap;
        }

        public static Bitmap ToBitmap(Image<RGB> image)
        {
            return InitializeBitmapFromImage((c, pixel, array) =>
            {
                array[c] = (byte)pixel.B;
                array[c + 1] = (byte)pixel.G;
                array[c + 2] = (byte)pixel.R;
            }, image);
        }

        public static Bitmap ToBitmap(Image<Gray> image)
        {
            Bitmap bitmap =  InitializeBitmapFromImage((c, pixel, array) =>
            {
               array[c] = (byte)pixel.G;
            }, image, PixelFormat.Format8bppIndexed);

            ColorPalette pal = bitmap.Palette;
            for (int i = 0; i < 256; i++)
                pal.Entries[i] = Color.FromArgb(255, i, i, i);
            bitmap.Palette = pal;

            return bitmap;
        }

        public static List<Image<Gray>> SeparateRgbImageToChannels(Image<RGB> image)
        {
            if (image == null)
            {
                throw new ConverterException("The image must be not null");
            }

            var channelsRgb = new List<Image<Gray>>
            {
                ChannelToGrayImage(image, Channel.R),
                ChannelToGrayImage(image, Channel.G),
                ChannelToGrayImage(image, Channel.B)
            };


            return channelsRgb;
        }

        public static Image<RGB> ChannelsToRgb(Image<Gray> red, Image<Gray> green, Image<Gray> blue)
        {
            if (red == null || green == null || blue == null ||
                red.M != green.M || red.M != blue.M || 
                red.N != green.N || red.N != blue.N
                )
            {
                throw new ConverterException("The channels must be not null and have equal dimensions");
            }

            var imageRgb = new Image<RGB>(red.N, red.M);

            for (int i = 0; i < red.N; i++)
            {
                for (int j = 0; j < red.M; j++)
                {
                    imageRgb[i, j] = new RGB(red[i,j].G, green[i,j].G, blue[i,j].G);
                }
            }

            return imageRgb;
        }

        public static Image<Gray> ChannelToGrayImage(Image<RGB> image, Channel channel)
        {
            if (image == null)
            {
                throw new ConverterException("The image must be not null");
            }

            var imageGray = new Image<Gray>(image.M, image.N);

            for (int i = 0; i < image.N; i++)
            {
                for (int j = 0; j < image.M; j++)
                {
                    Gray pixel;
                    switch (channel)
                    {
                        case Channel.R:
                            pixel = new Gray(image[i, j].R);
                            break;
                        case Channel.G:
                            pixel = new Gray(image[i, j].G);
                            break;
                        case Channel.B:
                            pixel = new Gray(image[i, j].B);
                            break;
                        default:
                            pixel = new Gray();
                            break;
                    }

                    imageGray[i, j] = pixel;
                }
            }

            return imageGray;
        }
    }
}