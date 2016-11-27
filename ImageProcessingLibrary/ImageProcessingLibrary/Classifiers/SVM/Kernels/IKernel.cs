using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Classifiers.SVM.Kernels
{
    public interface IKernel
    {
        double Process(double[] x1, double[] x2);
    }
}
