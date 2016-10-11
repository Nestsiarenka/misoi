using ImageProcessingLibrary.Capacities.Interface;
using ImageProcessingLibrary.Images;

namespace ImageProcessingLibrary.Interfaces
{
    public interface IFilter<T, U>
        where T: struct, ICapacity
        where U: struct, ICapacity
    {
        Image<U> Filter(Image<T> image);
    }
}
