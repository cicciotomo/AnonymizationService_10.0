using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.ContainerImages
{
    public class AnonymizationServiceContainerImagesDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IContainerImageRepository _containerImageRepository;

        public AnonymizationServiceContainerImagesDataSeederContributor(IContainerImageRepository containerImageRepository)
        {
            _containerImageRepository = containerImageRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var containerImageToSeed = new List<ContainerImage>()
            {
                new()
                {
                    Name = "mcr.microsoft.com/azure-cognitive-services/textanalytics/healthcare:latest",
                    MinRam = 0,
                    MinCpu = 0,
                    MaxAllowedContainers = 1,
                    ExposedPort = 5000,
                    ApiActionUrl = "http://text-analytics-for-health:5000/language/analyze-text/jobs?api-version=2022-04-01-preview",
                    ShortName = "text-analytics-for-health",
                    Variables = new List<ContainerImageVariable>()
                    {
                        new() { Name = "Eula", Value = "accept"},
                        new() { Name = "rai_terms", Value = "accept"},
                        new() { Name = "Billing", Value = "Settings:Containers:TextAnalyticsUrl", FromAppConfiguration = true},
                        new() { Name = "ApiKey", Value = "Settings:Containers:TextAnalyticsApiKey", FromAppConfiguration = true},
                    }
                },
                new()
                {
                    Name = "mcr.microsoft.com/azure-cognitive-services/translator/text-translation:latest",
                    MinRam = 0,
                    MinCpu = 0,
                    MaxAllowedContainers = 1,
                    ExposedPort = 5000,
                    ApiActionUrl = "http://text-translator:5000/translate?api-version=3.0&from=it&to=en",
                    ShortName = "text-translator",
                    Variables = new List<ContainerImageVariable>()
                    {
                        new() { Name = "eula", Value = "accept"},
                        new() { Name = "Languages", Value = "it,en"},
                        new() { Name = "billing", Value = "Settings:Containers:TranslatorUrl", FromAppConfiguration = true},
                        new() { Name = "apiKey", Value = "Settings:Containers:TranslatorApiKey", FromAppConfiguration = true}
                    },
                    Volumes = new List<ContainerImageVolume>()
                    {
                        new() { ContainerPath = "/usr/local/models", HostPath = @"C:\Temp\Containers\translator\models" },
                    }
                },
                new()
                {
                    Name = "nginx:alpine",
                    MinRam = 0,
                    MinCpu = 0,
                    MaxAllowedContainers = 1,
                    ExposedPort = 5000,
                    ApiActionUrl = "http://form-recognizer:5000/formrecognizer/v2.1/custom/models/7aed1acc-861b-4b49-81f8-445ee37ac903/analyze?includeTextDetails=true",
                    ShortName = "form-recognizer",
                    Volumes= new List<ContainerImageVolume>()
                    {
                        new() { ContainerPath = "/etc/nginx/nginx.conf", HostPath = @"form recognizer\nginx.conf" }
                    },
                    InvocationHeaders= new List<ContainerInvocationHeader>()
                    {
                        new() { FromAppConfiguration = false, Name = "Content-Type", Value = "application/pdf" }
                    },
                    IsBinaryDataContent = true
                },
                new()
                {
                    Name = "ai-text-analyzer:latest",
                    MinRam = 0,
                    MinCpu = 0,
                    MaxAllowedContainers = 1,
                    ExposedPort = 3000,
                    ApiActionUrl = "http://ai-text-analyzer:3000/analyze",
                    ShortName = "ai-text-analyzer"
                },
                new()
                {
                    Name = "ai-text-anonymizer:latest",
                    MinRam = 0,
                    MinCpu = 0,
                    MaxAllowedContainers = 1,
                    ExposedPort = 3000,
                    ApiActionUrl = "http://ai-text-anonymizer:3000/anonymize",
                    ShortName = "ai-text-anonymizer"
                },
                new()
                {
                    Name = "ai-document-parser:latest",
                    MinRam = 0,
                    MinCpu = 0,
                    MaxAllowedContainers = 1,
                    ExposedPort = 6868,
                    ApiActionUrl = "http://ai-document-parser:6868/parse",
                    ShortName = "ai-document-parser"
                }
            };

            foreach (var containerImage in containerImageToSeed)
            {
                var persistedContainerImage = await _containerImageRepository.FindAsync(x => x.ShortName == containerImage.ShortName);
                if (persistedContainerImage is null)
                {
                    await _containerImageRepository.InsertAsync(containerImage);
                }
            }
        }
    }
}
