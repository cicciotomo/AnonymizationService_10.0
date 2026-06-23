using AnonymizationService.PodDefinitions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Pods
{
    public class AnonymizationServicePodDefinitionsDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IPodDefinitionRepository _podDefinitionRepository;

        public AnonymizationServicePodDefinitionsDataSeederContributor(IPodDefinitionRepository podDefinitionRepository)
        {
            _podDefinitionRepository = podDefinitionRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            List<PodDefinition> podDefinitionsToSeed = new List<PodDefinition>()
            {
                new PodDefinition()
                {
                    Name = "FormRecognizer",
                    EntryPoint = "form-recognizer",
                    MaxNumberOfAllowedInstances = 1,
                    EnvironmentVariables = new List<PodEnvironmentVariable>
                    {
                        new() { Name = "Queue:RabbitMQ:HostName", ProvidedByContainer = "rabbitmq", ProvisionMode = Enums.PodEnvironmentVariableProvisionMode.ContainerName},
                        new() { Name = "Queue:RabbitMQ:Port", ProvidedByContainer = "rabbitmq", ProvisionMode = Enums.PodEnvironmentVariableProvisionMode.ContainerPort},
                    },
                    ContainerList = new List<PodDefinitionContainer>()
                    {
                        new PodDefinitionContainer()
                        {
                            ShortName = "form-recognizer",
                            Version = "v1",
                        }
                    },
                    Enabled = true
                },
                new PodDefinition()
                {
                    Name = "TextAnalytics",
                    EntryPoint = "text-analytics-for-health",
                    MaxNumberOfAllowedInstances = 1,
                    ContainerList = new List<PodDefinitionContainer>()
                    {
                        new PodDefinitionContainer()
                        {
                            ShortName = "text-analytics-for-health",
                            Version = "v1"
                        },
                    },
                    Enabled = true
                },
                new PodDefinition()
                {
                    Name = "TextTranslator",
                    EntryPoint = "text-translator",
                    MaxNumberOfAllowedInstances = 1,
                    ContainerList = new List<PodDefinitionContainer>()
                    {
                        new PodDefinitionContainer()
                        {
                            ShortName = "text-translator",
                            Version = "v1"
                        },
                    },
                    Enabled = true
                },
                new PodDefinition()
                {
                    Name = "TextAnalyzer",
                    EntryPoint = "ai-text-analyzer",
                    MaxNumberOfAllowedInstances = 1,
                    ContainerList = new List<PodDefinitionContainer>()
                    {
                        new PodDefinitionContainer()
                        {
                            ShortName = "ai-text-analyzer",
                            Version = "v1"
                        },
                    },
                    Enabled = true
                },
                new PodDefinition()
                {
                    Name = "TextAnonymizer",
                    EntryPoint = "ai-text-anonymizer",
                    MaxNumberOfAllowedInstances = 1,
                    ContainerList = new List<PodDefinitionContainer>()
                    {
                        new PodDefinitionContainer()
                        {
                            ShortName = "ai-text-anonymizer",
                            Version = "v1"
                        },
                    },
                    Enabled = true
                },
                new PodDefinition()
                {
                    Name = "DocumentParser",
                    EntryPoint = "ai-document-parser",
                    MaxNumberOfAllowedInstances = 1,
                    ContainerList = new List<PodDefinitionContainer>()
                    {
                        new PodDefinitionContainer()
                        {
                            ShortName = "ai-document-parser",
                            Version = "v1"
                        },
                    },
                    Enabled = true
                }
            };

            foreach (var podDefinition in podDefinitionsToSeed)
            {
                var persistedPodDefinition = await _podDefinitionRepository.FindAsync(x => x.Name == podDefinition.Name);
                if (persistedPodDefinition is null)
                {
                    await _podDefinitionRepository.InsertAsync(podDefinition);
                }
            }
        }
    }
}
