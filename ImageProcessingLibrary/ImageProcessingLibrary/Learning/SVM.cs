using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Learning
{
    class SVM
    {
        public double[] LearnSMO(List<double[]> trainingSet, int numberOfProperties, double constantCValue = 10)
        {
            var weights = new double[numberOfProperties];
            var vectorsAlphas = new List<double>(trainingSet.Count);
            var alpha1 = 0;
            var alpha2 = 0;

            while()
            {
                var tempTrainingSet = new List<double[]>();
                var tempVectorsAlphas = new List<double>();

                foreach (double[] trainingVector in trainingSet)
                {
                    double vectorsAlpha = processTrainingVector(trainingVector);

                    if (vectorsAlpha >= constantCValue || vectorsAlpha <= 0)
                    {
                        tempVectorsAlphas.Add(vectorsAlpha);
                        tempTrainingSet.Add(trainingVector);
                    }
                }


            }

            return weights;
        }

        private double processTrainingVector(double[] trainingVector)
        {
            
        }
    }
}
