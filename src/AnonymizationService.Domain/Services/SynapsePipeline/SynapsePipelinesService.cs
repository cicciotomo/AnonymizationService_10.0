using AnonymizationService.Services.AzureIdentity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using AnonymizationService.IntensiveCare;
using Volo.Abp.DependencyInjection;
using AnonymizationService.Services.IntensiveCare;
using Microsoft.Extensions.Options;
using AnonymizationService.IntensiveCareData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AnonymizationService.Redcap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace AnonymizationService.Services.SynapsePipeline
{
    [ExposeServices(typeof(ISynapsePipelinesService))]
    public class SynapsePipelinesService : ISynapsePipelinesService, ITransientDependency
    {
        private readonly IAzureTokenProvider _tokenProvider;
        private readonly SynapseSettings _synapseSettings;
        private readonly IServiceProvider _serviceProvider;

        public SynapsePipelinesService(
            IAzureTokenProvider tokenProvider,
            IOptions<SynapseSettings> synapseSettings,
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _tokenProvider = tokenProvider;
            _synapseSettings = synapseSettings.Value;
        }

        public async Task<string> CreatePipelineRunAsync(PipelineParameterDto parameters)
        {
            var _logger = _serviceProvider.GetService<ILogger<IntensiveCarePatientData>>();
            var token = await _tokenProvider.GetTokenForScopeAsync(_synapseSettings.SynapseServiceUrl + "/.default");
            _logger.LogDebug($"Token: {token}");
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"https://{_synapseSettings.Workspace}.dev.azuresynapse.net/pipelines/{_synapseSettings.PipelineName}/createRun?api-version={_synapseSettings.ApiVersion}";
                _logger.LogDebug($"uri: {uri}");
                var jsonParameters = JsonSerializer.Serialize(parameters);

                var content = new StringContent(jsonParameters, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(uri, content);
                _logger.LogDebug($"Response: {response.StatusCode}");

                if (!((int)response.StatusCode >= 200 && (int)response.StatusCode <= 299))
                {
                    _logger.LogError($"Raggiunto il limite massimo della coda - Errore {response.StatusCode} ");
                    throw new Exception($"Raggiunto il limite massimo della coda");
                }
                //response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        public async Task<string> CreateRedcapPipelineRunAsync(RedcapPipelineParameterDto parameters)
        {
            var _logger = _serviceProvider.GetService<ILogger<IntensiveCarePatientData>>();
            var token = await _tokenProvider.GetTokenForScopeAsync(_synapseSettings.SynapseServiceUrl + "/.default");
            _logger.LogDebug($"Token: {token}");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"https://{_synapseSettings.Workspace}.dev.azuresynapse.net/pipelines/{_synapseSettings.RedcapPipelineName}/createRun?api-version={_synapseSettings.ApiVersion}";
                _logger.LogDebug($"uri: {uri}");
                var jsonParameters = JsonSerializer.Serialize(parameters);

                var content = new StringContent(jsonParameters, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        public async Task<string> GetPipelineRunAsync(string runId)
        {
            var token = await _tokenProvider.GetTokenForScopeAsync(_synapseSettings.SynapseServiceUrl + "/.default");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"https://{_synapseSettings.Workspace}.dev.azuresynapse.net/pipelineruns/{runId}?api-version={_synapseSettings.ApiVersion}";

                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }

        public async Task<string> GetPipelinesAsync()
        {
            var token = await _tokenProvider.GetTokenForScopeAsync(_synapseSettings.SynapseServiceUrl + "/.default");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var uri = $"https://{_synapseSettings.Workspace}.dev.azuresynapse.net/pipelines?api-version={_synapseSettings.ApiVersion}";

                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
        }
    }
}
