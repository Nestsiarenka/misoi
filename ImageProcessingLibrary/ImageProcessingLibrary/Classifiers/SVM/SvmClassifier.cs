using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessingLibrary.Classifiers.SVM.Kernels;
using ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms;

namespace ImageProcessingLibrary.Classifiers.SVM
{
    public abstract class SvmClassifier
    {
        protected double[] Weights;
        protected double B;
        protected IKernel Kernel;

        public double Predict(double[] example)
        {
            return Kernel.Process(Weights, example) - B;
        }

        public abstract void Train(TrainingData data);
    }
}
