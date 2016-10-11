using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;

namespace ImageProcessingLibrary.Filters.PointFilters
{
    public abstract class PointFilter : IFilter
    {
        public GrayLevelImage Filter(GrayLevelImage grayLevelImage)
        {
            throw new System.NotImplementedException();
        }

        public abstract byte ProcessPixel(byte pixel);
    }
}
