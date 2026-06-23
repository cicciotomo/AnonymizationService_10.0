using AnonymizationService.Jobs.RetrieveDicomData;
using AnonymizationService.Services.DbUri;
using AnonymizationService.Services.HttpOperations;
using AnonymizationService.StateMachines.DbUri;
using AnonymizationService.StateMachines.Dicom;
using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.RetrieveDbUriData
{
	internal class RetrieveDbUriDataJob : Job<RetrieveDbUriDataArgs, JobResult>
	{
		private readonly ILogger<RetrieveDbUriDataJob> _logger;
		private readonly IDbUriService _dbUriService;

		public RetrieveDbUriDataJob(ILogger<RetrieveDbUriDataJob> logger, IDbUriService dbUriService)
		{
			_logger = logger;
			_dbUriService = dbUriService;
		}

		public override async Task<JobResult> ExecuteJobAsync(RetrieveDbUriDataArgs args)
		{
			_logger.LogInformation("Retrieving DbUri Data for patient {id}", args.CloudPatientId);

			var dbUriPatientData = await _dbUriService.GetPatientDataByMasterPatientIndexAsync(args.MasterPatientIndex);

			if (dbUriPatientData == null)
			{
				dbUriPatientData = await _dbUriService.GetPatientDataByTaxCodeAsync(args.TaxCode);
			}			

			_logger.LogInformation("Retrieved DbUri Data for patient {id}", args.CloudPatientId);

			return new JobResult()
			{
				StateMachineId = args.StateMachineId,
				JobResultEvent = new DbUriDataRetrievedEvent(dbUriPatientData)
			};
		}

	}
}
