using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.NeuralNetwork
{
    [DataContract]
    public class Neuron
    {
        [DataMember] public float b;
        [DataMember] public float[] w;
        [DataMember] public float[] deltaW;
        [DataMember] public float deltaB;
        [DataMember] private float alpha;
        [DataMember] private float lambda;

        public Neuron(int n, float alpha, float lambda)
        {
            this.alpha = alpha;
            this.lambda = lambda;

            var random = new Random();
            b = (float)random.NextDouble();

            w = new float[n];

            for (int i = 0; i < n; i++)
            {
                w[i] = (float)random.NextDouble();
            }

            deltaW = new float[n];
        }

        public float Compute(float[] x)
        {
            float z = 0.0f;
            for (int i = 0; i < x.Length; i++)
            {
                z += x[i]*w[i] + b;
            }
            z++;
            return ActiveFunction(z);
        }

        public static float ActiveFunction(float z)
        {
            return (float)(1.0f/(1 + Math.Exp(-z)));
        }

        public void UpdateWeights(int trainingSetCount)
        {
            for (int i = 0; i < w.Length; i++)
            {
                w[i] = w[i] - alpha*(deltaW[i]/trainingSetCount + lambda*w[i]);
            }

            b = b - alpha*(deltaB/trainingSetCount);
        }
    }
}
