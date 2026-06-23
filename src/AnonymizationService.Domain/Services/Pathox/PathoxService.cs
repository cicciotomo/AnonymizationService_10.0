using AnonymizationService.Enums;
using AnonymizationService.Services.DbUri;
using AnonymizationService.Services.Galileo;
using AnonymizationService.Services.HttpOperations;
using AnonymizationService.Services.ThrottlingManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.Pathox
{
	public class PathoxService : IPathoxService, ITransientDependency
	{
		private readonly IHttpOperationsService _httpOperationsService;
		private readonly IConfiguration _configuration;
		private readonly IThrottlingManager _throttlingManager;
		private readonly PathoxSettings _pathoxSettings;

		public PathoxService(IHttpOperationsService httpOperationsService
			, IConfiguration configuration
			, IThrottlingManager throttlingManager
			, IOptionsSnapshot<PathoxSettings> pathoxOptions)
		{
			_httpOperationsService = httpOperationsService;
			_configuration = configuration;	
			_throttlingManager = throttlingManager;
			_pathoxSettings = pathoxOptions.Value;
		}

		public async Task<PathoxListResponse> GetPatientDataByMasterPatientIndexAsync(string masterPatientIndex, DateTime startDate)
		{
			await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetPathoxExamList);
			var url = $"{_pathoxSettings.GetExamListUrl}{masterPatientIndex}/?fromdate={startDate:yyyyMMdd}";
			return await _httpOperationsService.GetFromXmlAsync<PathoxListResponse>(url, _pathoxSettings.Username, _pathoxSettings.Password);
		}

		public async Task<PathoxExamResult> GetExamDetailByExamIdAsync(string examId)
		{
			await _throttlingManager.SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum.GetPathoxExamDetail);
			var url = $"{_pathoxSettings.GetExamDetailUrl}/?ExamId={examId}";
			return await _httpOperationsService.GetFromXmlAsync<PathoxExamResult>(url, _pathoxSettings.Username, _pathoxSettings.Password);
		}
	}
}
