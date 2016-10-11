using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;

namespace ImageProcessingLibrary.Filters.PointFilters
{
    public abstract class PointFilter : IFilter
    {
        public GrayLevelImage Filter(GrayLevelImage grayLevelImage)
        {
            var exitGrayLevelImage = new GrayLevelImage(grayLevelImage.N, grayLevelImage.M);

            for (int i = 0; i < grayLevelImage.N; i++)
            {
                for (int j = 0; j < grayLevelImage.M; j++)
                {
                    exitGrayLevelImage[i, j] = ProcessPixel(grayLevelImage[i, j]);
                }
            }

            return exitGrayLevelImage;
        }

        public abstract byte ProcessPixel(byte pixel);
    }
}
