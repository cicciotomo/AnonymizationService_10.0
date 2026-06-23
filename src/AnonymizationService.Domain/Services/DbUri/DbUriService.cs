using AnonymizationService.Enums;
using AnonymizationService.Services.HttpOperations;
using AnonymizationService.Services.ThrottlingManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.DbUri
{
	public class DbUriService : IDbUriService, ITransientDependency
	{
        private readonly IHttpOperationsService _httpOperationsService;
		private readonly IConfiguration _configuration;
		private readonly IThrottlingManager _throttlingManager;
		private readonly DbUriSettings _dbUriSettings;

		public DbUriService(
			   IHttpOperationsService httpOperationsService
			 , IConfiguration configuration
			 , IThrottlingManager throttlingManager
			 , IOptionsSnapshot<DbUriSettings> dbUriOption)
		{
			_configuration = configuration;
			_httpOperationsService = httpOperationsService;
			_throttlingManager = throttlingManager;
			_dbUriSettings = dbUriOption.Value;
		}

		public async Task<DbUriPatientData> GetPatientDataByMasterPatientIndexAsync(string masterPatientIndex)
		{
			await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetDbUriData);

			var app = GetClientApplication();

			var scopes = new[] { $"https://vault.azure.net/.default" };

			var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
			var accessToken = result.AccessToken;
			var url = _dbUriSettings.GetDbUriDataByMasterPatientIndexUrl + masterPatientIndex;
			return await _httpOperationsService.GetAsync<DbUriPatientData>(url, accessToken);
		}

		public async Task<DbUriPatientData> GetPatientDataByTaxCodeAsync(string taxCode)
		{
			await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetDbUriData);

			var app = GetClientApplication();

			var scopes = new[] { $"https://vault.azure.net/.default" };

			var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
			var accessToken = result.AccessToken;
			var url = _dbUriSettings.GetDbUriDataByTaxCodeIndexUrl + taxCode;
			return await _httpOperationsService.GetAsync<DbUriPatientData>(url, accessToken);
		}

		private IConfidentialClientApplication GetClientApplication()
		{
			var clientId = _configuration.GetValue<string>("AnonimyzationService:HSR:ClientId");
			var clientSecret = _configuration.GetValue<string>("AnonimyzationService:HSR:Secret");
			var tenantId = _configuration.GetValue<string>("AnonimyzationService:HSR:TenantId");

			return ConfidentialClientApplicationBuilder.Create(clientId)
						.WithClientSecret(clientSecret)
						.WithAuthority(new Uri($"https://login.microsoftonline.com/" + tenantId))
						.Build();
		}
	}
}
