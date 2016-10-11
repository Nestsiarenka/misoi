using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Interfaces
{
    public interface IFilter
    {
        GrayLevelImage Filter(GrayLevelImage grayLevelImage);
    }
}
