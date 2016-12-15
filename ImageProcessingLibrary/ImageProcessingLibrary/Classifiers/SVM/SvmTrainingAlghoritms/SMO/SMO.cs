using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO
{
    [DataContract]
    public class Smo : SvmClassifier
    {        
        private double[] _errorCache;
        private double _c;
        private double _tolerance;
        [IgnoreDataMember]
        Random random = new Random();

        private double examplesClass2;
        private double alpha2;
        private double error2;

        private double examplesClass1;
        private double alpha1;
        private double error1;

        private double r2;

        private int startingPoint;        

        private double s;
        double L;
        double H;

        private double k11;
        private double k12;
        private double k22;

        double eps = 1e-3;

        private double eta;
        double a2;

        private double c1;
        private double c2;

        private double L1;
        private double H1;

        private double a1;
        private double b1;
        private double b2;
        private double result;
        private double oldB;

        private int indexA2;
        private int indexA1;
        double max;

        private int numberOfChanged;

        public Smo():base()
        { }

        public override void Train(TrainingData data)
        {
            var trainingData = data as SmoTrainingData;
            if (trainingData == null) return;

            _examples = trainingData.Examples;
            _examplesClasses = trainingData.ExamplesClasses;
            _c = trainingData.C;
            _tolerance = trainingData.Tolerance;
            Kernel = trainingData.Kernel;
            KernelType = Kernel.GetType().Name;

            //Weights = new double[_example+s[0].Length];
            _alphas = new double[_examples.Length];
            _errorCache = new double[_examples.Length];

            Training();
            
            //_examples = null;
            //_examplesClasses = null;
            //_alphas = null;
            _errorCache = null;
    }

        private void Training()
        {
            bool terminate = false;
            bool examineAll = true;

            while (!terminate)
            {
                numberOfChanged = 0;

                if (examineAll)
                {
                    for (indexA2 = 0; indexA2 < _examples.Length; indexA2++)
                    {
                        numberOfChanged += ExamineExample();
                    }
                    examineAll = false;

                    terminate = numberOfChanged == 0;
                }
                else
                {
                    for (indexA2 = 0; indexA2 < _examples.Length; indexA2++)
                    {
                        if (_alphas[indexA2] > 0 && _alphas[indexA2] < _c)
                        {
                            numberOfChanged += ExamineExample();
                        }
                    }

                    if (numberOfChanged == 0)
                    {
                        examineAll = true;
                    }
                }
            }
        }

        private int ExamineExample()
        {
            examplesClass2 = _examplesClasses[indexA2];
            alpha2 = _alphas[indexA2];
            error2 = ComputeErrorWithCache(indexA2);

            r2 = error2*examplesClass2;

            if ((r2 < -_tolerance && alpha2 < _c) ||
                (r2 > _tolerance && alpha2 > 0))
            {
                max = double.MinValue;
                bool changed = false;

                for (int i = 0; i < _errorCache.Length; i++)
                {
                    if (_alphas[i] > 0 && _alphas[i] < _c)
                    {
                        var tempError = ComputeErrorWithCache(i);
                        if (Math.Abs(error2 - tempError) > max)
                        {
                            max = Math.Abs(error2 - _errorCache[i]);
                            indexA1 = i;
                            changed = true;
                        }
                    }
                }

                if (changed && Optimize())
                {
                    return 1;
                }

                startingPoint = random.Next(_alphas.Length - 1);
                indexA1 = startingPoint;
                do
                {
                    if (_alphas[indexA1] > 0 && _alphas[indexA1] < _c && Optimize())
                    {
                        return 1;
                    }

                    indexA1 = (indexA1 + 1) % _alphas.Length;

                } while (indexA1 != startingPoint);

                startingPoint = random.Next(_alphas.Length - 1);
                indexA1 = startingPoint;
                do
                {

                    if (Optimize())
                    {
                        return 1;
                    }

                    indexA1 = (indexA1 + 1) % _alphas.Length;

                } while (indexA1 != startingPoint);
            }

            return 0;
        }

        private bool Optimize()
        {
            if (indexA1 == indexA2) return false;

            examplesClass1 = _examplesClasses[indexA1];
            alpha1 = _alphas[indexA1];
            error1 = ComputeErrorWithCache(indexA1);


            s = examplesClass1*examplesClass2;
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

            if (Math.Abs(L - H) < 1e-8)
            {
                return false;
            }

            k11 = Kernel.Process(_examples[indexA1], _examples[indexA1]);
            k12 = Kernel.Process(_examples[indexA1], _examples[indexA2]);
            k22 = Kernel.Process(_examples[indexA2], _examples[indexA2]);

            eta = 2*k12 - k11 - k22;

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
                c1 = eta/2;
                c2 = examplesClass2*(error1 - error2) - eta*alpha2;

                L1 = c1*L*L + c2*L;
                H1 = c1*H*H + c2*H;

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

            a1 = alpha1 + s*(alpha2 - a2);

            b1 = error1 + examplesClass1*(a1 - alpha1)*
                        Kernel.Process(_examples[indexA1], _examples[indexA1]) +
                        examplesClass2*(a2 - alpha2)
                        *Kernel.Process(_examples[indexA1], _examples[indexA2]) + B;

            b2 = error2 + examplesClass1 * (a1 - alpha1) *
                        Kernel.Process(_examples[indexA1], _examples[indexA2]) +
                        examplesClass2 * (a2 - alpha2)
                        * Kernel.Process(_examples[indexA2], _examples[indexA2]) + B;

            oldB = B;

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
            
            //update weights
            /*for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] +=
                    examplesClass1 * (a1 - alpha1) * _examples[indexA1][i] +
                    examplesClass2 * (a2 - alpha2) * _examples[indexA2][i];
            }*/

            //update cache
            for (int k = 0; k < _examples.Length; k++)
            {
                if (_alphas[k] > 0 && _alphas[k] < _c)
                {
                    _errorCache[k] +=
                         examplesClass1 * (a1 - alpha1) * Kernel.Process(_examples[indexA1], _examples[k]) +
                        examplesClass2 * (a2 - alpha2) * Kernel.Process(_examples[indexA2], _examples[k])
                        + oldB - B;
                }
                else
                {
                    _errorCache[k] = 0;
                }
            }

            _errorCache[indexA1] = 0;
            _errorCache[indexA2] = 0;
            //update cache

            _alphas[indexA1] = a1;
            _alphas[indexA2] = a2;

            return true;
        }

        private double ComputeErrorWithCache(int index)
        {
            result = _errorCache[index];

            return result > -1e-8 && result < 1e8 ? ComputeError(index) : result;
        }

        private double ComputeError(int index)
        {
            return Predict(_examples[index]) - _examplesClasses[index];
        }
    }
}
