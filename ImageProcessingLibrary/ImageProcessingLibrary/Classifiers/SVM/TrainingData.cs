using ImageProcessingLibrary.Classifiers.SVM.Kernels;

namespace ImageProcessingLibrary.Classifiers.SVM
{
    public abstract class TrainingData
    {
        public IKernel Kernel { get; set; }

        protected TrainingData(IKernel kernel)
        {
            Kernel = kernel;
        }
    }
}
