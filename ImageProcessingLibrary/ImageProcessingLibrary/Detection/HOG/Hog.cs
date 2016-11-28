using System;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessingLibrary.Classifiers.SVM;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using ImageProcessingLibrary.Utilities;
using ImageProcessingLibrary.Filters.PointFilters;
using ImageProcessingLibrary.Classifiers.SVM.Kernels;

namespace ImageProcessingLibrary.Detection.HOG
{
    [DataContract]
    public class Hog
    {
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

        readonly double[] _bins =
        {
            0, 10, 30, 50, 70, 90, 110, 130, 150, 170, 180
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

        public void TrainHog()
        {
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

            var trainingData = new SmoTrainingData(examples.ToArray(), classes.ToArray(), 0.15, 0.001, new Linear());

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
                image.ReturnZeroIfOutOfBounds = true;
                for (int j = 0; j < image.N - _windowWidth; j += _windowWidth)
                {
                    for (int k = 0; k < image.M - _windowHeight; k += _windowHeight)
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

        private double[] ComputeHogDescriptor(Image<Gray> image, int windowx, int windowy)
        {
            return ComputeBlockNormalization(ComputeAllCellsInWindow(image, windowx, windowy));
        }

        private double[] ComputeBlockNormalization(double[,][] computedCells)
        {
            var hogFeature = new double[_sizeOfHogFeature];
            int evaluatedBlocks = 0;
            
            for (int i = 0; i < _blocksInColumn; i++)
            {
                for (int j = 0; j < _blocksInRow; j++)
                {
                    Array.Copy(NormalizeBlock(j, i, computedCells), 0, hogFeature, 36 * evaluatedBlocks, 36);

                    evaluatedBlocks++;
                }
            }
            return hogFeature;
        }

        private double[] NormalizeBlock(int offsetx, int offsety, double[,][] computedCells)
        {
            var result = new double[BlockSize * 9];

            double sumOfBlock = ComputeSumOfBlock(offsetx, offsety, computedCells);

            for (int i = 0; i < BlockSize; i++)
            {
                int offsetByBlockX = i % (BlockSize / 2);
                int offsetByBlockY = i / (BlockSize / 2);
                int x = offsetByBlockX + offsetx;
                int y = offsetByBlockY + offsety;

                for (int j = 0; j < computedCells[x,y].Length; j++)
                {
                    result[i * 9 + j] = computedCells[x, y][j]/sumOfBlock;
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
                    result += computedCells[x, y].Sum();
                }
            }

            return result;
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

            int x = offsetx;
            int y = offsety;
            int xBound = offsetx + CellSize;
            int yBound = offsety + CellSize;

            for (; y < yBound; y++)
            {
                for (; x < xBound; x++)
                {
                    var dx = Math.Abs(Convert.ToInt16(image[x - 1, y].G - image[x + 1, y].G));
                    var dy = Math.Abs(Convert.ToInt16(image[x, y - 1].G - image[x, y + 1].G));

                    double magnitute = Math.Sqrt(dx*dx + dy*dy);
                    double angle = Math.Atan2(dy, dx)*180/Math.PI;

                    var binIndex = (int) Math.Floor(angle/20.0 + 0.5);
                    orientedHistogramTemp[binIndex] += Math.Abs(_bins[binIndex] - angle) / 20.0 * magnitute;
                    binIndex++;
                    orientedHistogramTemp[binIndex] += Math.Abs(_bins[binIndex] - angle) / 20.0 * magnitute;
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
