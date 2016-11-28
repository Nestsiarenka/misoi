using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ImageProcessingLibrary.Classifiers.SVM.Kernels;
using ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms;

namespace ImageProcessingLibrary.Classifiers.SVM
{

    [DataContract]
    public abstract class SvmClassifier
    {
        [DataMember]
        protected double[] Weights;
        [DataMember]
        protected double B;

        [DataMember]
        protected string KernelType; 

        protected IKernel Kernel;

        protected SvmClassifier()
        {

        }

        public void InitKernel()
        {
            if (KernelType == typeof(Linear).Name)
            {
                Kernel = new Linear();
            }
            else if (KernelType == typeof(Gaussian).Name)
            {
                Kernel = new Linear();
            }
        }

        public double Predict(double[] example)
        {
            return Kernel.Process(Weights, example) - B;
        }

        public abstract void Train(TrainingData data);
    }
}
