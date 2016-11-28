using System;
using System.Drawing;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Utilities
{
    public static class FileLoader
    {
        public static Image<RGB> LoadFromFile(String pathToFile)
        {
            using (var bitmap = new Bitmap(pathToFile))
            {
                return Converter.ToImage(bitmap);
            }                
        }
    }
}