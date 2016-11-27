using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Classifiers.SVM.Kernels
{
    public class Gaussian : IKernel
    {
        private readonly Linear _linearKernel = new Linear();
        private double _gaussianParameter;

        public Gaussian(double gaussianParameter)
        {
            _gaussianParameter = gaussianParameter;
        }


        public double Process(double[] x1, double[] x2)
        {
            double result = _linearKernel.Process(x1, x1) - 2*_linearKernel.Process(x1, x2)
                            + _linearKernel.Process(x2, x2);
            result = Math.Exp(result*result/(2*_gaussianParameter*_gaussianParameter));

            return result;
        }
    }
}
