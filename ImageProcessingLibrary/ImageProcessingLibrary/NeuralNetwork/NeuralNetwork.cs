using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.NeuralNetwork
{
    class NeuralNetwork
    {
        List<Layer> layers;

        public double[] Compute(double[] vector)
        {
            double[] temp = vector;
            foreach (var layer in layers)
            {
                temp = layer.Compute(temp);
            }
            return temp;
        }
    }
}
