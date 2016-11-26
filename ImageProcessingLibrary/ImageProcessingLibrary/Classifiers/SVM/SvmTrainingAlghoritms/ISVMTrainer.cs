namespace ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms
{
    interface ISVMTrainer
    {
        TrainingResult Train(TrainingData data);
    }
}
