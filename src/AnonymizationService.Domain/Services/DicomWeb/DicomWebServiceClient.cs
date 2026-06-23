using AnonymizationService.Services.AzureIdentity;
using FellowOakDicom;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Health.Dicom.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.DicomWeb
{
    public class DicomWebServiceClient : IDicomWebServiceClient, ITransientDependency
    {
        private readonly DicomWebServiceSettings _settings;
        private readonly IAzureTokenProvider _azureTokenProvider;
        private readonly ILogger<DicomWebServiceClient> _logger;
        private IDicomWebClient _dicomWebClient;
        
        public string _token = "";

        public DicomWebServiceClient(
            IOptionsSnapshot<DicomWebServiceSettings> optionsSnapshot
            , IAzureTokenProvider azureTokenProvider
            , ILogger<DicomWebServiceClient> logger)
        {
            _settings = optionsSnapshot.Value;
            _azureTokenProvider = azureTokenProvider;
            _logger = logger;
        }

        public async Task UploadDicomFileAsync(List<DicomFile> dicomFiles)
        {
            await InitializeDicomWebClientAsync();
            string serieInstanceUid = dicomFiles.FirstOrDefault().Dataset.GetString(DicomTag.SeriesInstanceUID);

            _logger.LogInformation($"Uploading {dicomFiles.Count()} DicomFiles for Serie {serieInstanceUid} to Azure DicomService");
            try
            {
                await _dicomWebClient.StoreAsync(dicomFiles);
            }
            catch (DicomWebException e) when (e.Message == "Conflict")
            {
                _logger.LogWarning(e, "Dicom Serie with Id {Id} already existing on Azure Dicom Service", serieInstanceUid);
                _logger.LogWarning(e, $"Exception message: {e.Message}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception in uploading DicomFiles for Serie {Id} to Azure DicomService", serieInstanceUid);
                _logger.LogError(e, $"Exception message: {e.Message}");
                throw;
            }
        }

        private async Task InitializeDicomWebClientAsync()
        {
            _logger.LogDebug($"InitializeDicomWebClientAsync");
            if (_dicomWebClient is not null && ChekToken(_token))
            {
                _logger.LogDebug($"_dicomWebClient is not null && ChekToken(_token)=TRUE");
                return;
            }
            _logger.LogDebug($"Generazione nuovo client e relativo token");
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_settings.DicomServiceUrl);
            var client = new DicomWebClient(httpClient);
            _token = await _azureTokenProvider.GetTokenForScopeAsync(_settings.DicomServiceIdentityScope);
            _logger.LogDebug($"DicomWebClient Token: {_token}");

            client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            _dicomWebClient = client;
        }

        private bool ChekToken(string token)
        {
            _logger.LogDebug($"ChekToken");
            try
            {
                string[] parts = token.Split('.');
                if (parts.Length != 3)
                {
                    _logger.LogDebug("Token non valido.");
                    return false;
                }
                string payload = parts[1];
                string decodedPayload = Base64UrlDecode(payload);
                var jsonDoc = JsonDocument.Parse(decodedPayload);
                var expElement = jsonDoc.RootElement.GetProperty("exp");

                long expTimestamp = expElement.GetInt64();
                DateTime expDate = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).DateTime;
                DateTime currentDate = DateTime.UtcNow;
                double minutesRemaining = (expDate - currentDate).TotalMinutes;

                _logger.LogDebug($"Minuti residui di validità del token: {minutesRemaining}");
                _logger.LogDebug($"Il token scade il: {expDate.ToLocalTime()}");
                if (minutesRemaining < 0) {
                    _logger.LogDebug($"Token SCADUTO ***********************************************************************");
                    return false; 
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                return false;
            }          
        }

        static string Base64UrlDecode(string input)
        {
            string base64 = input.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

    }
}
