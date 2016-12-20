using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace ImageProcessingLibrary.NeuralNetwork
{
    [DataContract]
    public class NeuralNetwork
    {
        [DataMember] Layer[] layers;
        [DataMember] private int inputCount;
        [DataMember] private int outputCount;

        public NeuralNetwork() { }

        public NeuralNetwork(int [] layersDescription, float weightDecay, float lambda)
        {
            layers = new Layer[layersDescription.Length - 1];
            inputCount = layersDescription[0];
            outputCount = layersDescription[layersDescription.Length - 1];

            for (int i = 1; i < layersDescription.Length; i++)
            {
                layers[i - 1] = new Layer(layersDescription[i], layersDescription[i-1], weightDecay, lambda);
            }
        }

        public float[] Compute(float[] vector)
        {
            float[] temp = vector;
            foreach (var layer in layers)
            {
                temp = layer.Compute(temp);
            }
            return temp;
        }

        public void TrainOne(float[] vector, float[] expectedResult, int trainingSetCount)
        {
            var computation = Compute(vector);
            var outputErrors = new float[computation.Length];

            for (int i = 0; i < computation.Length; i++)
            {
                //this code only useful for sigma function
                var neuronOut = layers[layers.Length - 1].myPreviousResult[i];
                outputErrors[i] = (neuronOut
                                   - expectedResult[i])*neuronOut*(1 - neuronOut);
            }

            
            var currentErrors = outputErrors;
            for (int i = computation.Length - 2; i >= 0; i--)
            {
                var nextLayer = layers[i + 1];
                currentErrors = layers[i].ComputeErrorsUpdateDeltas(currentErrors, nextLayer);
            }

            foreach (var layer in layers)
            {
                layer.UpdateWeights(trainingSetCount);
            }
        }

        public void Train(float[][] trainingSet, float[][] expectedValues)
        {
            for (int i = 0; i < trainingSet.Length; i++)
            {
                TrainOne(trainingSet[i], expectedValues[i], trainingSet.Length);
            }
        }

        public void Save(string filePath)
        {
            var serializer = new DataContractSerializer(typeof(NeuralNetwork));
            FileStream writer = new FileStream(filePath, FileMode.Create);

            serializer.WriteObject(writer, this);

            writer.Close();
        }

        public static NeuralNetwork Load(string filePath)
        {
            NeuralNetwork network;

            FileStream fs = new FileStream(filePath,
            FileMode.Open);
            XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            DataContractSerializer ser = new DataContractSerializer(typeof(NeuralNetwork));

            network = (NeuralNetwork)ser.ReadObject(reader, true);
            return network;
        }
    }
}
