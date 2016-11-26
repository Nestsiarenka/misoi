using System;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Detection.HOG
{
    public class HogClassifier
    {
        private readonly Image<Gray> _image;
        private readonly int _windowWidth;
        private readonly int _windowHeight;
        private const int CellSize = 8;
        private const int BlockSize = 4;
        private readonly int _cellsCountInWindow;
        /*
        private const int CellsForWorker = 32;
        private const int WorkersCount = 4;
        */
        private readonly int _sizeOfHogFeature;
        private readonly int _blocksInRow;
        private readonly int _blocksInColumn;

        readonly double[] _bins = {
                0, 10, 30, 50, 70, 90, 110, 130, 150, 170, 180
            };


        public HogClassifier(Image<Gray> image, int windowWidth, int windowHeight)
        {
            _image = image;
            _windowHeight = windowHeight;
            _windowWidth = windowWidth;
            _cellsCountInWindow = _windowWidth / CellSize * _windowHeight / CellSize;
            _blocksInColumn = _windowHeight/CellSize - 1;
            _blocksInRow = _windowWidth/CellSize - 1;
            _sizeOfHogFeature = 4 * 9 * _blocksInRow * _blocksInColumn;
        }

        public double[] ComputeHogDescriptor(int windowx, int windowy)
        {
            return ComputeBlockNormalization(ComputeAllCellsInWindow(windowx, windowy));
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

        public double[,][] ComputeAllCellsInWindow(int windowx, int windowy)
        {
            return ComputeNumberOfCels(windowx, windowy, 0, 0, _cellsCountInWindow);
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

        public double[,][] ComputeNumberOfCels(
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

                computedCells[cellx, celly] = ComputeOrientedHistogramForCell(offsetx, offsety);

                x += CellSize;
            }

            return computedCells;
        }

        private double[] ComputeOrientedHistogramForCell(int offsetx, int offsety)
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
                    var dx = Math.Abs(Convert.ToInt16(_image[x - 1, y].G - _image[x + 1, y].G));
                    var dy = Math.Abs(Convert.ToInt16(_image[x, y - 1].G - _image[x, y + 1].G));

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
    }
}
