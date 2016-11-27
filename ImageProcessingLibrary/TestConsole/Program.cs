using System;
using System.Collections.Generic;
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
            var rgbToGrayFilter = new RGBtoGrayFilter();
            var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(
                "D:\\Images\\1.jpg"));
            Image<Gray> image2 = new Image<Gray>(5, 5)
            {
                {56, 63, 57, 49, 72},
                {79, 255, 128, 74, 33},
                {40, 65, 49, 97, 44},
                {42, 67, 39, 54, 99},
                {44, 87, 65, 63, 120}
            };
            var classifier = new Hog(64, 128);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var hogDescriptor = classifier.ComputeHogDescriptor(image, 1, 1);
            watch.Stop();
            Console.WriteLine("Not async: {0}" , watch.ElapsedMilliseconds);

            Smo smo = new Smo();
            double[][] examples = { new double[]{1,4}, new double[] { 1,6}, new double[] { 2,8}, new double[] { 3,6}, new double[] { 4,1}, new double[] { 6,1}, new double[] { 6,3}, new double[] { 8,2}};
            double[] classes = {-1, -1, -1, -1, 1, 1, 1, 1};
            double c = 0.15;

            SmoTrainingData trainingData = new SmoTrainingData(examples, classes, c, 0.0001, new Linear());

            watch = System.Diagnostics.Stopwatch.StartNew();
            smo.Train(trainingData);
            watch.Stop();
            Console.WriteLine("Training: {0}", watch.ElapsedMilliseconds);

            Console.ReadLine();
       }
    }
}
