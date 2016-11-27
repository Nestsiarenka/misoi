using System;
using System.Linq;

namespace ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO
{
    public class Smo : SvmClassifier
    {
        private double[][] _examples;
        private double[] _examplesClasses;
        private double[] _alphas;
        private double[] _errorCache;
        private double _c;
        private double _tolerance;
        Random random = new Random();
        
        public override void Train(TrainingData data)
        {
            var trainingData = data as SmoTrainingData;
            if (trainingData == null) return;

            _examples = trainingData.Examples;
            _examplesClasses = trainingData.ExamplesClasses;
            _c = trainingData.C;
            _tolerance = trainingData.Tolerance;
            Kernel = trainingData.Kernel;

            Weights = new double[_examples[0].Length];
            _alphas = new double[_examples.Length];
            _errorCache = new double[_examples.Length];

            Training();
        }

        private void Training()
        {
            int numberOfChanged = 0;
            bool terminate = false;
            bool examineAll = true;

            while (!terminate)
            {
                if (examineAll)
                {
                    for (int i = 0; i < _examples.Length; i++)
                    {
                        numberOfChanged += ExamineExample(i);
                    }

                    examineAll = false;
                    terminate = numberOfChanged == 0;
                    numberOfChanged = 0;
                }
                else
                {
                    for (int i = 0; i < _examples.Length; i++)
                    {
                        if (_alphas[i] > 0 && _alphas[i] < _c)
                        {
                            ExamineExample(i);
                        }
                    }

                    examineAll = true;
                }
            }
        }

        private int ExamineExample(int indexA2)
        {
            double examplesClass = _examplesClasses[indexA2];
            double alpha2 = _alphas[indexA2];
            double error2 = ComputeErrorWithCache(indexA2);

            double r2 = error2*examplesClass;

            if ((r2 < -_tolerance && alpha2 < _c) ||
                (r2 > _tolerance && alpha2 > 0))
            {
                var errorsSub = _errorCache.Select(t => Math.Abs(error2 - t)).ToList();
                int indexA1 = errorsSub.IndexOf(errorsSub.Max());
                if (Optimize(indexA2, indexA1))
                {
                    return 1;
                }

                var startingPoint = random.Next(_alphas.Length - 1);
                var i = startingPoint;
                do
                {
                    if (_alphas[i] > 0 && _alphas[i] < _c && Optimize(indexA2, indexA1))
                    {
                        return 1;
                    }

                    i++;

                } while (startingPoint != i % _alphas.Length);

                startingPoint = random.Next(_alphas.Length - 1);
                i = startingPoint;
                do
                {
                    if (Optimize(indexA2, indexA1))
                    {
                        return 1;
                    }

                    i++;

                } while (startingPoint != i % _alphas.Length);
            }

            return 0;
        }

        private bool Optimize(int indexA2, int indexA1)
        {
            if (indexA1 == indexA2) return false;

            double examplesClass1 = _examplesClasses[indexA1];
            double alpha1 = _alphas[indexA1];
            double error1 = ComputeErrorWithCache(indexA1);

            double examplesClass2 = _examplesClasses[indexA2];
            double alpha2 = _alphas[indexA2];
            double error2 = ComputeErrorWithCache(indexA2);

            double s = examplesClass1*examplesClass2;
            double L;
            double H;

            if (s < 0)
            {
                L = Math.Max(0, alpha2 - alpha1);
                H = Math.Min(_c, _c + alpha2 - alpha1);
            }
            else
            {
                L = Math.Max(0, alpha2 + alpha1 - _c);
                H = Math.Min(_c, alpha2 + alpha1);
            }

            if (L == H) return false;

            double k11 = Kernel.Process(_examples[indexA1], _examples[indexA1]);
            double k12 = Kernel.Process(_examples[indexA1], _examples[indexA2]);
            double k22 = Kernel.Process(_examples[indexA2], _examples[indexA2]);

            double eps = 0.001;

            double eta = 2*k12 - k11 - k22;
            double a2;

            if (eta < 0)
            {
                a2 = alpha2 - examplesClass2*(error1 - error2)/eta;
                if (a2 < L)
                {
                    a2 = L;
                } else if (a2 > H)
                {
                    a2 = H;
                }
            }
            else
            {
                double c1 = eta/2;
                double c2 = examplesClass2*(error1 - error2) - eta*alpha2;

                double L1 = c1*L*L + c2*L;
                double H1 = c1*H*H + c2*H;

                if (L1 > H1 + eps)
                {
                    a2 = L;
                }
                else if (L1 < H1 - eps)
                {
                    a2 = H;
                }
                else
                {
                    a2 = alpha2;
                }
            }

            if (Math.Abs(a2 - alpha2) < eps*(a2 + alpha2 + eps))
            {
                return false;
            }

            double a1 = alpha1 + s*(alpha2 - a2);

            double b1 = error1 + examplesClass1*(a1 - alpha1)*
                        Kernel.Process(_examples[indexA1], _examples[indexA1]) +
                        examplesClass2*(a2 - alpha2)
                        *Kernel.Process(_examples[indexA1], _examples[indexA2]) + B;

            double b2 = error2 + examplesClass1 * (a1 - alpha1) *
                        Kernel.Process(_examples[indexA1], _examples[indexA2]) +
                        examplesClass2 * (a2 - alpha2)
                        * Kernel.Process(_examples[indexA2], _examples[indexA2]) + B;

            if (a1 > 0 && a1 < _c)
            {
                B = b1;
            }
            else if (a2 > 0 && a2 < _c)
            {
                B = b2;
            }
            else
            {
                B = (b1 + b2)/2;
            }

            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] +=
                    examplesClass1 * (a1 - alpha1) * _examples[indexA1][i] +
                    examplesClass2 * (a2 - alpha2) * _examples[indexA2][i];
            }

            _alphas[indexA1] = a1;
            _alphas[indexA2] = a2;

            UpdateErrorCache();

            return true;
        }

        private double ComputeErrorWithCache(int index)
        {
            double result = _errorCache[index];

            if (result == 0)
            {
                result = ComputeError(index);
            }

            return result;
        }

        private double ComputeError(int index)
        {
            return Predict(_examples[index]) - _examplesClasses[index];
        }

        private void UpdateErrorCache()
        {
            for (int i = 0; i < _examples.Length; i++)
            {
                if (_alphas[i] > 0 && _alphas[i] < _c)
                {
                    _errorCache[i] = ComputeError(i);
                }
                else
                {
                    _errorCache[i] = 0;
                }
            }
        }
    }
}
