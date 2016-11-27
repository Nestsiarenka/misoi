using ImageProcessingLibrary.Classifiers.SVM.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO
{
    public class SmoTrainingData : TrainingData
    {
        public double[][] Examples { get; set; }
        public double[] ExamplesClasses { get; set; }
        public double C { get; set;}
        public double Tolerance { get; set; }

        public SmoTrainingData (double[][] examples, double[] examplesClasses, double c, double tolerance, IKernel kernel) : base(kernel)
        {
            Examples = examples;
            C = c;
            Tolerance = tolerance;
            ExamplesClasses = examplesClasses;
        }
    }
}
