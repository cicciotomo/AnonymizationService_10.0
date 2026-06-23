namespace AnonymizationService.PipelineExecutor
{
    public class PipelineExecutionResult
    {
        public bool Success => string.IsNullOrEmpty(ErrorMessage);
        public string ErrorMessage { get; set; }
        public string PipelineName { get; set; }
        public string Result { get; set; }
    }
}
