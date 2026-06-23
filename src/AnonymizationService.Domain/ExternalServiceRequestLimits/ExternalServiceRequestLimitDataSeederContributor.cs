using AnonymizationService.Enums;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.ExternalServiceRequestLimits
{
    public class ExternalServiceRequestLimitDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IExternalServiceRequestLimitRepository _externalServiceRequestLimitRepository;

        public ExternalServiceRequestLimitDataSeederContributor(
            IExternalServiceRequestLimitRepository externalServiceRequestLimitRepository
            )
        {
            _externalServiceRequestLimitRepository = externalServiceRequestLimitRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var dbExternalServiceRequestLimits = await _externalServiceRequestLimitRepository.GetListAsync();

            if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.ValidateMasterPatientIndex))
            {
                await _externalServiceRequestLimitRepository.InsertAsync(
                   new ExternalServiceRequestLimit(
                   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.ValidateMasterPatientIndex,
                   dailyNumberRequestsLimitInInterval: 5,
                   nightlyNumberRequestsLimitInInterval: 5,
                   startDailyRange: new TimeSpan(6, 0, 0),
                   startNightlyRange: new TimeSpan(22, 0, 0),
                   requestsNumberIntervalMinutes: 1
                   ),
                   autoSave: true
                );
            }
            if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.GetPatientLaboratoryExamResults))
            {
                await _externalServiceRequestLimitRepository.InsertAsync(
                   new ExternalServiceRequestLimit(
                   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.GetPatientLaboratoryExamResults,
                   dailyNumberRequestsLimitInInterval: 5,
                   nightlyNumberRequestsLimitInInterval: 5,
                   startDailyRange: new TimeSpan(6, 0, 0),
                   startNightlyRange: new TimeSpan(22, 0, 0),
                   requestsNumberIntervalMinutes: 1
                   ),
                   autoSave: true
                );
            }
            if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.GetPatientClinicalDataResults))
            {
                await _externalServiceRequestLimitRepository.InsertAsync(
                   new ExternalServiceRequestLimit(
                   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.GetPatientClinicalDataResults,
                   dailyNumberRequestsLimitInInterval: 5,
                   nightlyNumberRequestsLimitInInterval: 5,
                   startDailyRange: new TimeSpan(6, 0, 0),
                   startNightlyRange: new TimeSpan(22, 0, 0),
                   requestsNumberIntervalMinutes: 1
                   ),
                   autoSave: true
                );
            }
            if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.GetDocument))
            {
                await _externalServiceRequestLimitRepository.InsertAsync(
                   new ExternalServiceRequestLimit(
                   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.GetDocument,
                   dailyNumberRequestsLimitInInterval: 5,
                   nightlyNumberRequestsLimitInInterval: 5,
                   startDailyRange: new TimeSpan(6, 0, 0),
                   startNightlyRange: new TimeSpan(22, 0, 0),
                   requestsNumberIntervalMinutes: 1
                   ),
                   autoSave: true
                );
            }
            if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.GetAuthToken))
            {
                await _externalServiceRequestLimitRepository.InsertAsync(
                   new ExternalServiceRequestLimit(
                   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.GetAuthToken,
                   dailyNumberRequestsLimitInInterval: 5,
                   nightlyNumberRequestsLimitInInterval: 5,
                   startDailyRange: new TimeSpan(6, 0, 0),
                   startNightlyRange: new TimeSpan(22, 0, 0),
                   requestsNumberIntervalMinutes: 1
                   ),
                   autoSave: true
                );
			}
			if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.GetDbUriData))
			{
				await _externalServiceRequestLimitRepository.InsertAsync(
				   new ExternalServiceRequestLimit(
				   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.GetDbUriData,
				   dailyNumberRequestsLimitInInterval: 5,
				   nightlyNumberRequestsLimitInInterval: 5,
				   startDailyRange: new TimeSpan(6, 0, 0),
				   startNightlyRange: new TimeSpan(22, 0, 0),
				   requestsNumberIntervalMinutes: 1
				   ),
				   autoSave: true
				);
			}
			if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.GetPathoxExamList))
			{
				await _externalServiceRequestLimitRepository.InsertAsync(
				   new ExternalServiceRequestLimit(
				   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.GetPathoxExamList,
				   dailyNumberRequestsLimitInInterval: 5,
				   nightlyNumberRequestsLimitInInterval: 5,
				   startDailyRange: new TimeSpan(6, 0, 0),
				   startNightlyRange: new TimeSpan(22, 0, 0),
				   requestsNumberIntervalMinutes: 1
				   ),
				   autoSave: true
				);
			}
			if (!dbExternalServiceRequestLimits.Exists(e => e.ExternalServiceRequestsType == ExternalServiceRequestTypeEnum.GetPathoxExamDetail))
			{
				await _externalServiceRequestLimitRepository.InsertAsync(
				   new ExternalServiceRequestLimit(
				   externalServiceRequestLimit: ExternalServiceRequestTypeEnum.GetPathoxExamDetail,
				   dailyNumberRequestsLimitInInterval: 5,
				   nightlyNumberRequestsLimitInInterval: 5,
				   startDailyRange: new TimeSpan(6, 0, 0),
				   startNightlyRange: new TimeSpan(22, 0, 0),
				   requestsNumberIntervalMinutes: 1
				   ),
				   autoSave: true
				);
			}
		}

    }
}
