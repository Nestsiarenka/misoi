namespace ImageProcessingLibrary.Capacities.Interface
{
    public interface ICapacity
    {
        bool IsEmpty();
        void Initialize(object value);
        void SetZero();
    }
}