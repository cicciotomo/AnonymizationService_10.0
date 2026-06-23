using AnonymizationService.Services.Galileo;
using Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.PlatformManagementConsole
{
    public class PlatformManagementConsoleClient : IPlatformManagementConsoleClient, ITransientDependency
    {
        private readonly ILogger<PlatformManagementConsoleClient> _logger;
        private readonly PlatformManagementConsoleClientSettings _settings;

        public PlatformManagementConsoleClient(ILogger<PlatformManagementConsoleClient> logger
            , IOptions<PlatformManagementConsoleClientSettings> platformManagementConsoleOptions)
        {
            _logger = logger;
            _settings = platformManagementConsoleOptions.Value;
        }
        
        public async Task RequestPatientDeletionAsync(Guid cloudPatientId)
        {
            _logger.LogInformation("Requesting patient deletion for patient {cloudPatientId}", cloudPatientId);
            
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_settings.BaseUrl) };

            var patientResourceUrl = $"patient/{cloudPatientId}";
            var response = await httpClient.DeleteAsync(patientResourceUrl);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<StudyMetadata>> GetAvailableStudiesListAsync()
        {
            _logger.LogInformation("Requesting study metadata");
            
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_settings.BaseUrl) };

            const string availableStudiesListUrl = "study/available-studies";
            _logger.LogDebug($"Requesting study metadata at {_settings.BaseUrl}{availableStudiesListUrl}");
            var studiesList = await httpClient.GetAsync(availableStudiesListUrl);

            return await studiesList.Content.ReadFromJsonAsync<List<StudyMetadata>>();
        }

        public async Task<List<StudyMetadata>> GetAvailableStudiesWithCohortStausBozzaListAsync()
        {
            _logger.LogInformation("Requesting study metadata");

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_settings.BaseUrl) };

            const string availableStudiesListUrl = "study/available-studies-with-cohort-staus-bozza";
            _logger.LogDebug($"Requesting study metadata at {_settings.BaseUrl}{availableStudiesListUrl}");
            var studiesList = await httpClient.GetAsync(availableStudiesListUrl);

            return await studiesList.Content.ReadFromJsonAsync<List<StudyMetadata>>();
        }


        public async Task<FullStudyMetadata> GetStudyAsync(Guid studyId)
        {
            _logger.LogInformation($"Requesting study metadata with id={studyId}");

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_settings.BaseUrl) };

            string studyByIdUrl = $"study/{studyId}/study";
            _logger.LogDebug($"Requesting study metadata at {_settings.BaseUrl}{studyByIdUrl}");
            var study = await httpClient.GetAsync(studyByIdUrl);
            return await study.Content.ReadFromJsonAsync<FullStudyMetadata>();
        }

        public async Task AddPatientToStudy(StudyPatient patient)
        {
            _logger.LogInformation($"Requesting Add Patient to study");
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_settings.BaseUrl) };

            string addPatientToStudyUrl = $"study-patient/patient";
            string json = JsonConvert.SerializeObject(patient);
            HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(addPatientToStudyUrl, httpContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemovePatientFromStudy(StudyPatient patient)
        {
            _logger.LogInformation($"Requesting Remove Patient from study");
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
            using var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_settings.BaseUrl) };

            string addPatientToStudyUrl = $"study-patient/patient?CloudPatientIndex={patient.CloudPatientIndex}&StudyId={patient.StudyId}";
            var response = await httpClient.DeleteAsync(addPatientToStudyUrl);
            response.EnsureSuccessStatusCode();
        }
    }
}
