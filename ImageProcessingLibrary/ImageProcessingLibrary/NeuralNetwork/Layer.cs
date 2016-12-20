using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.NeuralNetwork
{
    class Layer
    {
        Neuron[] neurons;

        public Layer(int n, int nPrevious)
        {
            neurons = new Neuron[n];
            for (int i = 0; i >= n; i++)
            {
                neurons[i] = new Neuron(nPrevious);
            }
        }

        public double[] Compute(double[] x)
        {
            double[] temp = new double[neurons.Length];
            for (int i = 0; i < neurons.Length; i++)
            {
                temp[i] = neurons[i].Compute(x);
            }
            return temp;
        }

    }
}
