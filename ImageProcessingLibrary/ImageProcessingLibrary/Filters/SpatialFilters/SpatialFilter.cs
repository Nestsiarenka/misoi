using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;

namespace ImageProcessingLibrary.Filters.SpatialFilters
{
    public abstract class SpatialFilter<T, U> : IFilter<T, U>
            where T : struct, ICapacity
            where U : struct, ICapacity
    {
        public virtual Image<U> Filter(Image<T> image)
        {
            var exitGrayLevelImage = new Image<U>(image.N, image.M);

            for (int j = 0; j < image.M; j++)
            {
                for (int i = 0; i < image.N; i++)
                {
                    exitGrayLevelImage[i, j] = ProcessPixel(i, j, image);
                }
            }

            return exitGrayLevelImage;
        }

        protected abstract U ProcessPixel(int i, int j, Image<T> image);
    }
}
