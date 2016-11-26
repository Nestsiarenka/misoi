using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Detection.HOG;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Utilities;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
       {
            var rgbToGrayFilter = new RGBtoGrayFilter();
            var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(
                "C:\\Users\\VAN\\Documents\\BSUIR2016SUM\\MISOI\\projects\\ImageProcessingLibrary\\Images\\ducks.jpg"));
            Image<Gray> image2 = new Image<Gray>(5, 5)
            {
                {56, 63, 57, 49, 72},
                {79, 255, 128, 74, 33},
                {40, 65, 49, 97, 44},
                {42, 67, 39, 54, 99},
                {44, 87, 65, 63, 120}
            };
            var classifier = new HogClassifier(image, 64, 128);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var hogDescriptor = classifier.ComputeHogDescriptor(1, 1);
            watch.Stop();
            Console.WriteLine("Not async: {0}" , watch.ElapsedMilliseconds);

           Console.ReadLine();
       }
    }
}
