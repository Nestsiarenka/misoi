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
            return x1.Select((t, i) => t*x2[i]).Sum();
        }
    }
}
