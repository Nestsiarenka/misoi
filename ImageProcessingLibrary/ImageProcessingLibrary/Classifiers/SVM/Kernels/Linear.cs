using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Classifiers.SVM.Kernels
{
    public class Linear: IKernel
    {
        public double Process(double[] x1, double[] x2)
        {
            double result = 0;

            for (int i = 0; i < x1.Length; i++)
            {
                result += x1[i]*x2[i];
            }

            return result;
        }
    }
}
