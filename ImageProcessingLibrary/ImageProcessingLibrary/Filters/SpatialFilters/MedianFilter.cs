using System;
using System.Collections.Generic;
using System.Reflection;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Filters.SpatialFilters
{
    public class MedianFilter : SpatialFilter<Gray, Gray>
    {
        private readonly bool[,] _patternMatrix;
        private readonly int _filterWidth;
        private readonly int _filterHeight;
        private readonly int _widthOffset;
        private readonly int _heightOffset;
        private readonly int _medianIndex;

        public MedianFilter(bool[,] patternMatrix)
        {
            if (patternMatrix == null)
            {
                throw new InvalidFilterCriteriaException("The pattern matrix must be not null");
            }

            _patternMatrix = patternMatrix;

            _filterHeight = _patternMatrix.GetLength(0);
            _filterWidth = _patternMatrix.GetLength(1);

            if (_filterHeight % 2 == 0 ||
                _filterWidth % 2 == 0)
            {
                throw new InvalidFilterCriteriaException("The pattern matrix dimensions have to be odd");
            }

            _widthOffset = (_filterWidth - 1) / 2;
            _heightOffset = (_filterHeight - 1) / 2;

            _medianIndex = _filterHeight*_filterWidth / 2;
        }

        public override Image<Gray> Filter(Image<Gray> image)
        {
            image.ReturnZeroIfOutOfBounds = true;

            return base.Filter(image);
        }

        protected override Gray ProcessPixel(int i, int j, Image<Gray> image)
        {
            Gray resultGray = new Gray();
            var mappedValues = new List<byte>();

            for (int x = 0; x < _filterHeight; x++)
            {
                for (int y = 0; y < _filterWidth; y++)
                {
                    if (_patternMatrix[x, y] == false)
                    {
                        mappedValues.Add((byte)image[i - _widthOffset + y, j - _heightOffset + x].G);  
                    }
                }
            }
            
            mappedValues.Sort();

            resultGray.G = Convert.ToByte(mappedValues[_medianIndex]);

            return resultGray;
        }
    }
}
