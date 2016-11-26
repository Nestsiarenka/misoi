namespace ImageProcessingLibrary.Classifiers.SVM.SvmTrainingAlghoritms
{
    public class TrainingResult
    {
        public double[] Weights { get; set;}
        public double B { get; set;}

        public TrainingResult() {}

        public TrainingResult(double[] weights, double b)
        {
            Weights = weights;
            B = b;
        }
    }
}
