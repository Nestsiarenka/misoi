using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO;
using ImageProcessingLibrary.Detection.HOG;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Utilities;
using ImageProcessingLibrary.Classifiers.SVM.Kernels;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
       {
            var classifier = new Hog(32, 32);

            classifier.TrainHog("D:\\Images\\examples\\true", "D:\\Images\\examples\\false");
            classifier.Save("D:\\images\\examples\\file.xml");

            //var classifier =  Hog.Load("D:\\images\\examples\\file.xml");

            //var enumeration = Directory.EnumerateFiles("D:\\Images\\examples\\true");

            //for (int i = 0; i < 100; i++)
            //{

            //    var rgbToGrayFilter = new RGBtoGrayFilter();
            //    var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(enumeration.ElementAt( i)));
            //    image.ReturnZeroIfOutOfBounds = true;


            //    Console.WriteLine(classifier.Predict(image, 0, 0));
            //}

            Console.WriteLine("Done!!!");

            Console.ReadLine();
       }
    }
}
