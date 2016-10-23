using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Filters.SpatialFilters
{

    public class ConvolutionFilter : SpatialFilter<Gray, Gray>
    {
        private readonly int[,] _convolutionMatrix;
        private readonly int _divisor;
        private readonly int _kernelWidth;
        private readonly int _kernelHeight;
        private readonly int _widthOffset;
        private readonly int _heightOffset;

        public ConvolutionFilter(int[,] convolutionMatrix)
        {
            _convolutionMatrix = convolutionMatrix;

            _kernelHeight = _convolutionMatrix.GetLength(0);
            _kernelWidth = _convolutionMatrix.GetLength(1);

            _widthOffset = (_kernelWidth - 1)/2;
            _heightOffset = (_kernelHeight - 1)/2;

            if (_kernelHeight % 2 == 0 ||
                _kernelWidth % 2 == 0)
            {
                throw new InvalidFilterCriteriaException("The convolution matrix dimensions have to be odd");
            }

            _divisor = 0;

            foreach (var weight in _convolutionMatrix)
            {
                _divisor += weight;
            }

            if (_divisor == 0)
            {
                throw new InvalidFilterCriteriaException("The sum of matrix elements equel zero");
            }
        }

        public override Image <Gray> Filter(Image<Gray> image)
        {
            image.ReturnZeroIfOutOfBounds = true;

            return base.Filter(image);
        }

        protected override Gray ProcessPixel(int i, int j, Image<Gray> image)
        {
            Gray resultGray = new Gray();
            int convolutionValue = 0;

            for (int x = 0; x < _kernelHeight; x++)
            {
                for (int y = 0; y < _kernelWidth; y++)
                {
                    convolutionValue += (int)image[i - _widthOffset + y, j - _heightOffset + x].G *
                                        _convolutionMatrix[x, y];
                }
            }

            convolutionValue /= _divisor;

            if (convolutionValue > 255)
            {
                convolutionValue = 255;
            }

            if (convolutionValue < 0)
            {
                convolutionValue = 0;
            }

            resultGray.G = Convert.ToByte(convolutionValue);

            return resultGray;
        }
    }
}
