using AnonymizationService.Services.DicomWeb;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.AzureIdentity
{
    public class AzureTokenProvider : IAzureTokenProvider, ISingletonDependency
    {
        private readonly ILogger<DicomWebServiceClient> _logger;

        private ConcurrentDictionary<string, AccessToken> _tokenCache = new ConcurrentDictionary<string, AccessToken>();

        public AzureTokenProvider(
            ILogger<DicomWebServiceClient> logger)
        {
            _logger = logger;
        }


        public async Task<string> GetTokenForScopeAsync(string scope)
        {
            _logger.LogDebug($"GetTokenForScopeAsync - START");
            AccessToken token;
            if (_tokenCache is not null) { _logger.LogDebug($"_tokenCache.Count: {_tokenCache.Count}"); }

            if (_tokenCache.TryGetValue(scope, out token))
            {
                _logger.LogDebug($"token.ExpiresOn.DateTime: {token.ExpiresOn.DateTime}");
                _logger.LogDebug($"System.DateTime.UtcNow: {System.DateTime.UtcNow}");

                if (token.ExpiresOn.DateTime > System.DateTime.UtcNow)
                {
                    _logger.LogDebug($"Restituisce Token precedente");
                    return token.Token;
                }
            }
            _logger.LogDebug($"Scope: {scope}");
            string[] scopes = { scope };
            var azureCreds = new DefaultAzureCredential(); 

            _logger.LogDebug($"Vecchio token: {token.Token}");
            token = await azureCreds.GetTokenAsync(new TokenRequestContext(scopes));
            _logger.LogDebug($"Nuovo   token: {token.Token}");

            _logger.LogDebug($"_tokenCache.Count: {_tokenCache.Count} - Provo a rimuovere il vecchio token");
            if (_tokenCache.TryRemove(scope, out var _)) { _logger.LogDebug($"rimosso vecchio token"); }
            else { _logger.LogDebug($"errore rimozione vecchio token"); }

            _logger.LogDebug($"_tokenCache.Count: {_tokenCache.Count} - Provo ad aggiungere il nuovo token");
            if (_tokenCache.TryAdd(scope, token)) { _logger.LogDebug($"aggiunto nuovo token"); }
            else { _logger.LogDebug($"errore aggiunto nuovo token"); }

            _logger.LogDebug($"_tokenCache.Count: {_tokenCache.Count} - Provo a recuperare il nuovo token aggiunto al dictionary");
            if (_tokenCache.TryGetValue(scope, out var token_1)) {_logger.LogDebug($"Nuovo token: {token_1.Token}");}
            else { _logger.LogDebug($"errore recupero token da dictionary");}

            _logger.LogDebug($"GetTokenForScopeAsync - END");
            return token.Token;
        }
    }
}
