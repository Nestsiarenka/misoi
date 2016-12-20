using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.NeuralNetwork
{
    class Neuron
    {
        double b = 0.0;
        double[] w; 

        public Neuron(int n)
        {
            w = new double[n];
        }

        public double Compute(double[] x)
        {
            double z = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                z += x[i]*w[i] + b;
            }
            return ActiveFunction(z);
        }

        private static double ActiveFunction(double z)
        {
            return 1/(1 + Math.Exp(-z));
        }
    }
}
