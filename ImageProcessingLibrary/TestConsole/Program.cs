using System;
using System.Drawing;
using System.IO;
using System.Linq;
using ImageProcessingLibrary.Detection.HOG;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.NeuralNetwork;
using ImageProcessingLibrary.Utilities;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var classifier = new Hog(32, 32);

            classifier.TrainHog("D:\\Images\\examples\\trueTmp", "D:\\Images\\examples\\falseTmp");
            classifier.Save("D:\\images\\examples\\file21000.xml");*/

            ////var enumeration = Directory.EnumerateFiles("D:\\Images\\examples\\temp\\false");

            ////for (int i = 0; i < 2; i++)
            ////{
            ////    var rgbToGrayFilter = new RGBtoGrayFilter();
            ////    var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(enumeration.ElementAt(i)));
            ////    image.ReturnZeroIfOutOfBounds = true;

            ////    Console.WriteLine(classifier.Predict(image, 2, 2));
            ////}


            //var classifier = Hog.Load("D:\\images\\examples\\file1.xml");

            //var enumeration = Directory.EnumerateFiles("D:\\Images\\examples\\false");

            //for (int i = 0; i < 8; i++)
            //{

            //    var rgbToGrayFilter = new RGBtoGrayFilter();
            //    var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(enumeration.ElementAt(i)));
            //    image.ReturnZeroIfOutOfBounds = true;

            //    Console.WriteLine(classifier.Predict(image, 2, 2));
            //}

            //Hog hog = new Hog(32, 32);
            //var rgbToGrayFilter = new RGBtoGrayFilter();
            //var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(@"D:\Images\examples\true\1.jpg"));

            //hog.ComputeHogDescriptor(image, 2, 2);

            //SvmClassifier svm = new Smo();
            //double[][] examples = new double[17][];
            //double[] examplesClasses = new double[17];
            //examples[0] = new double[] { 2, 5 }; examplesClasses[0] = 1;
            //examples[1] = new double[] { 3, 8 }; examplesClasses[1] = 1;
            //examples[2] = new double[] { 4, 6 }; examplesClasses[2] = 1;
            //examples[3] = new double[] { 4, 7 }; examplesClasses[3] = 1;
            //examples[4] = new double[] { 5, 7 }; examplesClasses[4] = 1;
            //examples[5] = new double[] { 5, 8 }; examplesClasses[5] = 1;
            //examples[6] = new double[] { 5, 10 }; examplesClasses[6] = 1;
            //examples[7] = new double[] { 6, 5 }; examplesClasses[7] = 1;
            //examples[8] = new double[] { 7, 8 }; examplesClasses[8] = 1;
            //examples[9] = new double[] { 8, 2 }; examplesClasses[9] = -1;
            //examples[10] = new double[] { 8, 4 }; examplesClasses[10] = -1;
            //examples[11] = new double[] { 9, 4 }; examplesClasses[11] = -1;
            //examples[12] = new double[] { 9, 6 }; examplesClasses[12] = -1;
            //examples[13] = new double[] { 10, 2 }; examplesClasses[13] = -1;
            //examples[14] = new double[] { 10, 6 }; examplesClasses[14] = -1;
            //examples[15] = new double[] { 10, 4 }; examplesClasses[15] = -1;
            //examples[16] = new double[] { 11, 5 }; examplesClasses[16] = -1;
            //SmoTrainingData trainingData = new SmoTrainingData(examples, examplesClasses, 0.01, 1e-3, new Linear());
            //svm.Train(trainingData);

            //double[] example = { 4, 6 };

            //Console.WriteLine(svm.Predict(example));
            //var rgbToGrayFilter = new RGBtoGrayFilter();
            //var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile("D:\\Images\\examples\\true\\1.jpg"));
            //var vector1 = new Hog(32, 32).ComputeHogDescriptor(image, 2, 2);

            //Hog hog = Hog.Load("D:\\images\\examples\\file1.xml");
            //var rgbToGrayFilter = new RGBtoGrayFilter();
            //var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(@"D:\Images\examples\1.jpg"));

            //var bitmap = Converter.ToBitmap(image);
            //var faces = hog.FindFaces(image);

            //var newBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            //using (Graphics gr = Graphics.FromImage(newBitmap))
            //{
            //    gr.DrawImage(bitmap, 0, 0);
            //    foreach (var item in faces)
            //    {
            //        using (Pen thick_pen = new Pen(Color.Blue, 5))
            //        {
            //            gr.DrawRectangle(thick_pen, item.rectangle);
            //        }
            //    }
            //}

            //newBitmap.Save(@"D:\Images\examples\faces13.jpg");

            NeuralNetwork neuralNetwork = new NeuralNetwork(new[] {5, 3, 2}, 0.3f, 0.3f);

            float[][] trainingSet = new float[3][];
            float[][] expectedResultes = new float[3][];

            trainingSet[0] = new[] {1.0f, 2.0f, 3.0f};
            trainingSet[1] = new[] { 4.0f, 5.0f, 6.0f };
            trainingSet[2] = new[] { 7.0f, 8.0f, 9.0f };

            expectedResultes[0] = new[] {1.0f, 0};
            expectedResultes[1] = new[] { 0, 1.0f};
            expectedResultes[2] = new[] { 1.0f, 0};

            neuralNetwork.Train(trainingSet, expectedResultes);

            for (int i = 1; i < 10; i++)
            {
                var result = neuralNetwork.Compute(trainingSet[i%3]);
                Console.WriteLine("For {0} is {1} {2}", i, result[0], result[1]);
            }

            Console.WriteLine("Done!!!");
            Console.ReadLine();
        }
    }
}
