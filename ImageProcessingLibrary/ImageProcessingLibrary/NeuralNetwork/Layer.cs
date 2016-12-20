using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.NeuralNetwork
{
    [DataContract]
    public class Layer
    {
        [DataMember] Neuron[] neurons;
        [DataMember] public float[] myPreviousResult;
        [DataMember] public float[] currentLevelErrorsTemp;

        public Layer() { }

        public Layer(int n, int nPrevious, float alpha, float lambda)
        {
            neurons = new Neuron[n];
            for (int i = 0; i < n; i++)
            {
                neurons[i] = new Neuron(nPrevious, alpha , lambda);
            }
        }

        public float[] Compute(float[] x)
        {
            float[] temp = new float[neurons.Length];
            for (int i = 0; i < neurons.Length; i++)
            {
                temp[i] = neurons[i].Compute(x);
            }

            myPreviousResult = temp;
            return temp;
        }

        public float[] ComputeErrorsUpdateDeltas(float[] nextLayerErrors, Layer nextLayer)
        {
            var nextLevelNeurons = nextLayer.neurons;
            var currentLevelErrors = new float[neurons.Length];

            for (int i = 0; i < neurons.Length; i++)
            {
                float errorTemp = 0.0f;

                for (int j = 0; j < nextLevelNeurons.Length; j++)
                {
                    var error = nextLayerErrors[j];
                    var Wji = nextLevelNeurons[j].w[i];

                    errorTemp += Wji*error;

                    //deltaW and b update code works only for sigma function
                    nextLevelNeurons[j].deltaW[i] += myPreviousResult[i]* nextLayerErrors[j];
                    nextLevelNeurons[j].deltaB = nextLayerErrors[j];
                }
                //only for sigma function, need to use another formula for other functons
                currentLevelErrors[i] = errorTemp * myPreviousResult[i] * (1 - myPreviousResult[i]);
            }

            currentLevelErrorsTemp = currentLevelErrors;
            return currentLevelErrors;
        }

        public void UpdateWeights(int trainingSetCount)
        {
            foreach (var neuron in neurons)
            {
                neuron.UpdateWeights(trainingSetCount);
            }
        }
    }
}
