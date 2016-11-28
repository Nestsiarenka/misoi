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
            var watch = System.Diagnostics.Stopwatch.StartNew();
            classifier.TrainHog("D:\\Images\\examples\\true", "D:\\Images\\examples\\false");
            Console.WriteLine("Not async: {0}", watch.ElapsedMilliseconds);

            classifier.Save("C:\\Users\\VAN\\Documents\\BSUIR2016SUM\\MISOI\\projects\\ImageProcessingLibrary\\file.xml");

            Console.ReadLine();
       }
    }
}
