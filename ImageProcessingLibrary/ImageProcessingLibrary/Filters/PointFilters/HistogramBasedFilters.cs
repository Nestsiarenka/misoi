using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Capacities.Structures;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Utilities;

namespace ImageProcessingLibrary.Filters.PointFilters
{
    public abstract class HistogramBasedFilters<U> : PointFilter<Gray, U>
        where U : struct, ICapacity
    {
        protected Histogram _histogram;

        protected virtual Histogram HistogramCreation(Image<Gray> image)
        {
            return new Histogram(image);
        }

        public override Image<U> Filter(Image<Gray> image)
        {
            _histogram = HistogramCreation(image);

            return base.Filter(image);
        }
    }
}