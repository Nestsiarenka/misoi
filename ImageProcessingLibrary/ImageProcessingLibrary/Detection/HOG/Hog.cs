using System;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using ImageProcessingLibrary.Utilities;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Classifiers.SVM.Kernels;
using Gray = ImageProcessingLibrary.Capacities.Structures.Gray;
using ImageProcessingLibrary.Resizers;
using ImageProcessingLibrary.Filters.SpatialFilters;

namespace ImageProcessingLibrary.Detection.HOG
{
    [DataContract]
    public class Hog
    {
        [DataMember]
        private const double scale = 1.2;
        //private readonly Image<Gray> _image;
        [DataMember]
        private readonly int _windowWidth;
        [DataMember]
        private readonly int _windowHeight;
        [DataMember]
        private const int CellSize = 8;
        [DataMember]
        private const int BlockSize = 4;
        [DataMember]
        private const double Eps = 1;
        [DataMember]
        private readonly int _cellsCountInWindow;
        /*
        private const int CellsForWorker = 32;
        private const int WorkersCount = 4;
        */

        [DataMember]
        private readonly int _sizeOfHogFeature;
        [DataMember]
        private readonly int _blocksInRow;
        [DataMember]
        private readonly int _blocksInColumn;

        [DataMember]
        private Smo svm = new Smo();

        [DataMember]
        readonly double[] _bins =
        {
            -10, 10, 30, 50, 70, 90, 110, 130, 150, 170, 190
        };

        public Hog(int windowWidth, int windowHeight)
        {
            _windowHeight = windowHeight;
            _windowWidth = windowWidth;
            _cellsCountInWindow = _windowWidth/CellSize*_windowHeight/CellSize;
            _blocksInColumn = _windowHeight/CellSize - 1;
            _blocksInRow = _windowWidth/CellSize - 1;
            _sizeOfHogFeature = 4*9*_blocksInRow*_blocksInColumn;
        }

        private Hog()
        { }

        public struct PredictionRectangle
        {
            public Rectangle rectangle { get; set; }
            public double prediction { get; set; }
        }

        //private List<PredictionRectangle> NonMaximaSupression(List<PredictionRectangle> srcRectangels, double trashHold)
        //{

        //}

        public List<PredictionRectangle> FindFaces (Image<Gray> image)
        {
            var resizer = new BicubicResizer();
            var tempImage = image;

            List<Image<Gray>> scaledImages = new List<Image<Gray>>();

            while(tempImage.N >= 36 && tempImage.M >= 36)
            {               
                var bitmap = Converter.ToBitmap(tempImage);                

                scaledImages.Add(tempImage);
                tempImage = resizer.Resize(image, (int)(tempImage.N / scale), (int)(tempImage.M / scale));
            }

            var rectangles = new List<PredictionRectangle>();
            

            for (int i = 0; i < scaledImages.Count; i++)
            { 
                int windowSizeScaled = (int)(32 * Math.Pow(scale, i));
               
                Image<Gray> currentImage = scaledImages[i];

                for (int y = 2; y < currentImage.M - 32; y += 4)
                {
                    int yScaled = (int)(y * Math.Pow(scale, i));

                    for (int x = 2; x < currentImage.N - 32;  x += 4)
                    {
                        var predictionResult = Predict(currentImage, x, y);
                        if (predictionResult > 1.1)
                        {
                            int xScaled = (int)(x * Math.Pow(scale, i));
                            var predictionRectangle = new PredictionRectangle();
                            predictionRectangle.prediction = predictionResult;
                            predictionRectangle.rectangle = new Rectangle(xScaled, yScaled, windowSizeScaled, windowSizeScaled);
                            rectangles.Add(predictionRectangle);
                        }                       
                    }
                }
            }

            //var max = rectangles.Max(x => x.prediction);

            //return NonMaximaSuppression(rectangles.Where(x => x.prediction > max / 1.4 && x.prediction < max).ToList());
            return NonMaximaSuppression(rectangles);
        }

        private List<PredictionRectangle> NonMaximaSuppression(List<PredictionRectangle> inputRectangles)
        {
            List<PredictionRectangle> pick = new List<PredictionRectangle>();
            List<PredictionRectangle> suppressions = new List<PredictionRectangle>();

            inputRectangles.Sort((x, y) => y.rectangle.Y + y.rectangle.Height - (x.rectangle.Y + x.rectangle.Height));

            while (inputRectangles.Count > 0)
            {
                var last = inputRectangles[inputRectangles.Count - 1];
                pick.Add(last);
                suppressions.Add(last);

                for (int i = 0; i < inputRectangles.Count; i++)
                {
                    var x1 = Math.Max(last.rectangle.X, inputRectangles[i].rectangle.X);
                    var y1 = Math.Max(last.rectangle.Y, inputRectangles[i].rectangle.Y);
                    var x2 = Math.Min(last.rectangle.X + last.rectangle.Width,
                        inputRectangles[i].rectangle.X + inputRectangles[i].rectangle.Width);
                    var y2 = Math.Min(last.rectangle.Y + last.rectangle.Height,
                        inputRectangles[i].rectangle.Y + inputRectangles[i].rectangle.Height);

                    var width = Math.Max(0, x2 - x1 + 1);
                    var height = Math.Max(0, y2 - y1 + 1);

                    var overlap = (double) (width*height)/
                                  (inputRectangles[i].rectangle.Width*inputRectangles[i].rectangle.Height);

                    if (overlap > 0.4)
                    {
                        suppressions.Add(inputRectangles[i]);
                    }
                }

                inputRectangles.RemoveAll(x => suppressions.Contains(x));
                suppressions.Clear();
            }

            return pick;
        }


        public double Predict(Image<Gray> image, int windowsX, int windowsY)
        {
            var descriptor = ComputeHogDescriptor(image, windowsX, windowsY);

            return svm.Predict(descriptor);
        }

        public void TrainHog(string trueExamplesFolderPath, string falseExamplesFolderPath)
        {
            var trueExamplesFolderEnumeration = Directory.EnumerateFiles(trueExamplesFolderPath);
            var falseExamplesFolderEnumeration = Directory.EnumerateFiles(falseExamplesFolderPath);

            int trueFilesCount = trueExamplesFolderEnumeration.Count();
            int falseFilesCount = falseExamplesFolderEnumeration.Count();

            var examples = new List<double[]>(trueFilesCount + falseFilesCount);
            var classes = new List<double>(trueFilesCount + falseFilesCount);

            int workersCount = 8;
            int countFilesForWorker = 100;
            var taskArray = new Task[workersCount];
            int taskIndex = 0;
            int taskCounter = 0;

            for (int i = 0; i < trueFilesCount; i += countFilesForWorker)
            {
                taskArray[taskIndex] = Task.Factory.StartNew(
                    obj =>
                    {
                        var data = (DataTrainHog) obj;
                        TrainFromFiles(data.Examples, data.Enumeration, data.OffsetEnumerator,
                            data.Count, data.ClassValue, data.Classes);
                    }, new DataTrainHog
                    {
                        Examples = examples,
                        Enumeration = trueExamplesFolderEnumeration,
                        OffsetEnumerator = i,
                        Count = (trueFilesCount - i < countFilesForWorker) ? trueFilesCount - i : countFilesForWorker,
                        Classes = classes,
                        ClassValue = 1
                    });

                if (taskCounter == workersCount - 1)
                {
                    taskIndex = Task.WaitAny(taskArray);
                }
                else
                {
                    taskCounter++;
                    taskIndex++;
                }
            }

            for (int i = 0; i < falseFilesCount; i += countFilesForWorker)
            {
                taskArray[taskIndex] = Task.Factory.StartNew(
                    obj =>
                    {
                        var data = (DataTrainHog) obj;
                        TrainFromFiles(data.Examples, data.Enumeration, data.OffsetEnumerator,
                            data.Count, data.ClassValue, data.Classes);
                    }, new DataTrainHog
                    {
                        Examples = examples,
                        Enumeration = falseExamplesFolderEnumeration,
                        OffsetEnumerator = i,
                        Count = (falseFilesCount - i < countFilesForWorker) ? falseFilesCount - i : countFilesForWorker,
                        Classes = classes,
                        ClassValue = -1
                    });

                if (taskCounter == workersCount - 1)
                {
                    taskIndex = Task.WaitAny(taskArray);
                }
                else
                {
                    taskCounter++;
                    taskIndex++;
                }
            }

            foreach (var task in taskArray)
            {
                task?.Wait();
            }

            var trainingData = new SmoTrainingData(examples.ToArray(), classes.ToArray(), 0.1, 1e-3, new Linear());

            svm.Train(trainingData);
        }

        class DataTrainHog
        {
            public List<double[]> Examples { get; set; }
            public IEnumerable<string> Enumeration { get; set; }
            public int OffsetEnumerator { get; set; }
            public int Count { get; set; }
            public double ClassValue { get; set; }
            public List<double> Classes { get; set; }
        }

        private Object outputLock = new Object();

        public void TrainFromFiles(List<double[]> examples, IEnumerable<string> enumeration,
            int offsetEnumerator, int count, double classValue, List<double> classes)
        {
            for (int i = 0; i < count; i++)
            {
                var rgbToGrayFilter = new RGBtoGrayFilter();
                var image = rgbToGrayFilter.Filter(FileLoader.LoadFromFile(enumeration.ElementAt(offsetEnumerator + i)));
                for (int j = 2; j < image.N - _windowWidth; j += _windowWidth)
                {
                    for (int k = 2; k < image.M - _windowHeight; k += _windowHeight)
                    {
                        Monitor.Enter(outputLock);
                        try
                        {
                            
                            examples.Add(ComputeHogDescriptor(image, j, k));
                            classes.Add(classValue);
                        }
                        finally
                        {
                            Monitor.Exit(outputLock);
                        }
                    }
                }
                
            }
        }

        public double[] ComputeHogDescriptor(Image<Gray> image, int windowx, int windowy)
        {
            return ComputeBlockNormalization(ComputeAllCellsInWindow(image, windowx, windowy));
        }

        private double[] ComputeBlockNormalization(double[,][] computedCells)
        {
            var hogFeature = new double[_sizeOfHogFeature];
            int evaluatedBlocks = 0;
            
            for (int y = 0; y < _blocksInColumn; y++)
            {
                for (int x = 0; x < _blocksInRow; x++)
                {
                    Array.Copy(NormalizeBlock(x, y, computedCells), 0, hogFeature, BlockSize * 9 * evaluatedBlocks, BlockSize * 9);

                    evaluatedBlocks++;
                }
            }
            return hogFeature;
        }

        private double[] NormalizeBlock(int offsetx, int offsety, double[,][] computedCells)
        {
            var result = new double[BlockSize * 9];

            double normalizationValue = ComputeSumOfBlock(offsetx, offsety, computedCells);

            for (int i = 0; i < BlockSize; i++)
            {
                int offsetByBlockX = i % (BlockSize / 2);
                int offsetByBlockY = i / (BlockSize / 2);
                int x = offsetByBlockX + offsetx;
                int y = offsetByBlockY + offsety;

                for (int j = 0; j < computedCells[x,y].Length; j++)
                {
                    result[i * 9 + j] = computedCells[x, y][j]/normalizationValue;
                }
            }

            return result;
        }

        private double ComputeSumOfBlock(int offsetx, int offsety, double[,][] computedCells)
        {
            double result = 0;

            for (int i = 0; i < BlockSize; i++)
            {
                int offsetByBlockX = i % (BlockSize / 2);
                int offsetByBlockY = i / (BlockSize / 2);
                int x = offsetByBlockX + offsetx;
                int y = offsetByBlockY + offsety;

                for (int j = 0; j < computedCells[x, y].Length; j++)
                {
                    var coord = computedCells[x, y][j];
                    result += coord*coord;
                }
            }

            return Math.Sqrt(result + Eps);
        }

        public double[,][] ComputeAllCellsInWindow(Image<Gray> image, int windowx, int windowy)
        {
            return ComputeNumberOfCels(image, windowx, windowy, 0, 0, _cellsCountInWindow);
        }

        /*
        class DataForComputeNumberOfCels
        {
            public int WindowX { get; set; }
            public int WindowY { get; set; }
            public int OffsetByWindowX { get; set; }
            public int OffsetByWindowY { get; set; }
        }

        private double[,][] ComputeAllCellsInWindowAsync()
        {
            int windowX = 1;
            int windowY = 1;

            Task[] taskArray = new Task[WorkersCount];

            for (int cellsNumber = 0, taskCounter = 0, taskIndex = 0; cellsNumber < _cellsCountInWindow; 
                cellsNumber += CellsForWorker)
            {
                int offsetByWindowX = cellsNumber % _cellsCountInRow * CellSize;
                int offsetByWindowY = cellsNumber / _cellsCountInRow * CellSize;
                taskArray[taskIndex] = Task.Factory.StartNew(
                    obj =>
                    {
                        var data = (DataForComputeNumberOfCels) obj;
                        ComputeNumberOfCels(data.WindowX, data.WindowY, data.OffsetByWindowX, data.OffsetByWindowY,
                            CellsForWorker);
                    }, new DataForComputeNumberOfCels
                    {
                        WindowX = windowX, WindowY = windowY, OffsetByWindowX = offsetByWindowX,
                        OffsetByWindowY = offsetByWindowY
                    });

                if (taskCounter == WorkersCount - 1)
                {
                    taskIndex = Task.WaitAny(taskArray);
                }
                else
                {
                    taskCounter++;
                    taskIndex++;
                }

                ComputeNumberOfCels(windowX, windowY, offsetByWindowX, offsetByWindowY, CellsForWorker);
            }

            foreach (var task in taskArray)
            {
                task?.Wait();
            }

            return _computedCells;
        }*/

        public double[,][] ComputeNumberOfCels(Image<Gray> image,
            int windowX, int windowY,
            int offsetByWindowX, int offsetByWindowY, int numberOfCells)
        { 
            var computedCells = new double[_windowWidth / CellSize, _windowHeight / CellSize][];

            int x = offsetByWindowX;
            int y = offsetByWindowY;

            for (int cellsCounter = 0; cellsCounter < numberOfCells; cellsCounter++)
            {
                if (x == _windowWidth)
                {
                    x = 0;
                    y += CellSize;
                }

                int offsetx = windowX + x;
                int offsety = windowY + y;

                int cellx = x/CellSize;
                int celly = y/CellSize;

                computedCells[cellx, celly] = ComputeOrientedHistogramForCell(image, offsetx, offsety);

                x += CellSize;
            }

            return computedCells;
        }

        private double[] ComputeOrientedHistogramForCell(Image<Gray> image, int offsetx, int offsety)
        {
            var orientedHistogramTemp = new double[11];
            
            int xBound = offsetx + CellSize;
            int yBound = offsety + CellSize;

            for (int y = offsety; y < yBound; y++)
            {
                for (int x = offsetx; x < xBound; x++)
                {
                    var dx = Convert.ToInt16(image[x + 1, y].G - image[x - 1, y].G);
                    var dy = Convert.ToInt16(image[x, y + 1].G - image[x, y - 1].G);

                    double magnitute = Math.Sqrt(dx*dx + dy*dy);
                    double angle = Math.Atan2(dy, dx)*180/Math.PI;

                    if (angle < 0)
                    {
                        angle += 180;
                    }

                    var firstBin = (int) Math.Floor(angle/20.0 + 0.5);
                    var secondBin = firstBin + 1;
                    orientedHistogramTemp[firstBin] += Math.Abs(_bins[secondBin] - angle) / 20.0 * magnitute;
                    orientedHistogramTemp[secondBin] += Math.Abs(_bins[firstBin] - angle) / 20.0 * magnitute;
                }
            }

            var orientedHistogram = new double[9];
            Array.Copy(orientedHistogramTemp, 1, orientedHistogram, 0, 9);
            return orientedHistogram;
        }

        public void Save(string filePath)
        {
            var serializer = new DataContractSerializer(typeof(Hog));
            FileStream writer = new FileStream(filePath, FileMode.Create);

            serializer.WriteObject(writer, this);

            writer.Close();
        }

        public static Hog Load(string filePath)
        {
            Hog hog;

            FileStream fs = new FileStream(filePath,
            FileMode.Open);
            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(Hog));

            hog = (Hog)ser.ReadObject(reader, true);
            hog.svm.InitKernel();
            return hog;
        }
    }
}
