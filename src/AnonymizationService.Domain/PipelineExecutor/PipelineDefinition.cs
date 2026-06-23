namespace AnonymizationService.PipelineExecutor
{
    public class PipelineDefinition
    {
        public string Name { get; set; }
        public ContentPipelineDefinition Content { get; set; }
    }

    public class ContentPipelineDefinition
    {
        public object Data { get; set; }
        public ContainerContentPipelineDefinition[] Containers { get; set; }
    }

    public class ContainerContentPipelineDefinition
    {
        public int Step { get; set; }
        public string ContainerName { get; set; }
        public string VersionContainer { get; set; }
        public bool? ShouldStartNewPipeline { get; set; }
        public string OutputTransformationMap { get; set; }
        public bool AddOutputToResult { get; set; }
    }
}
