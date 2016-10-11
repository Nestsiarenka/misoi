using ImageProcessingLibrary.Images;
using ImageProcessingLibrary.Interfaces;

namespace ImageProcessingLibrary.Filters.PointFilters
{
    public abstract class PointFilter : IFilter
    {
        public GrayLevelImage Filter(GrayLevelImage grayLevelImage)
        {
            var exitGrayLevelImage = new GrayLevelImage(grayLevelImage.N, grayLevelImage.M);

            for (int i = 0; i < grayLevelImage.M; i++)
            {
                for (int j = 0; j < grayLevelImage.N; j++)
                {
                    exitGrayLevelImage[j, i] = ProcessPixel(grayLevelImage[j, i]);
                }
            }

            return exitGrayLevelImage;
        }

        public abstract byte ProcessPixel(byte pixel);
    }
}
