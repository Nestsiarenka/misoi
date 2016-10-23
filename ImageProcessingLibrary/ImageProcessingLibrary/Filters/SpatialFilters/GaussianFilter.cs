using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Filters.SpatialFilters
{
    public class GaussianFilter : ConvolutionFilter
    {
        public GaussianFilter() : base(new [,] { { -1, -1, -1}, { -1, 9, -1}, { -1, -1, -1} })
        {
        }
    }
}
