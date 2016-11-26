using System;
using ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms;

namespace ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms.SMO
{
    public class SMO : ISVMTrainer
    {
        public TrainingResult Train(TrainingData data)
        {
            var trainingData = data as SMOTrainingData;
            var trainingResult = new TrainingResult();




            return trainingResult;
        }
    }
}
