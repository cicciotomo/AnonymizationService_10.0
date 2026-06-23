using AnonymizationService.Enums;

namespace AnonymizationService.PodDefinitions
{
    public class PodEnvironmentVariableDto
    {
        public string Name { get; set; }
        public string ProvidedByContainer { get; set; }
        public PodEnvironmentVariableProvisionMode ProvisionMode { get; set; }
    }
}