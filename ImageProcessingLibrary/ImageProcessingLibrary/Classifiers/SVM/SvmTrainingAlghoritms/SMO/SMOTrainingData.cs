using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO
{
    public class SMOTrainingData : TrainingData
    {
        public double[][] Examples { get; set; }
        public double C { get; set;}

        public SMOTrainingData (double[][] examples, double c)
        {
            Examples = examples;
            C = c;
        }
    }
}
