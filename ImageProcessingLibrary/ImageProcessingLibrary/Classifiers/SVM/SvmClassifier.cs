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
        protected double[][] _examples;
        [DataMember]
        protected double[] _examplesClasses;
        [DataMember]
        protected double[] _alphas;
        [DataMember]
        public double B;

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
            double result = 0;

            for (int i = 0; i < _examples.Length; i++)
            {
                result += _alphas[i]*_examplesClasses[i]*Kernel.Process(_examples[i], example);
            }

            return result - B;
        }

        public abstract void Train(TrainingData data);
    }
}
