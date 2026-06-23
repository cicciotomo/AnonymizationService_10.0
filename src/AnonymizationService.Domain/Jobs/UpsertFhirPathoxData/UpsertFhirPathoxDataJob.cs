using AnonymizationService.Jobs.UpsertFhirDbUriData;
using AnonymizationService.Services.FhirService;
using AnonymizationService.Services.FhirService.Adapters.DbUri;
using AnonymizationService.Services.FhirService.Adapters.Pathox;
using AnonymizationService.StateMachines.DbUri;
using AnonymizationService.StateMachines.Pathox;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.UpsertFhirPathoxData
{
	internal class UpsertFhirPathoxDataJob : Job<UpsertFhirPathoxDataArgs, JobResult>
	{
		private readonly IFhirServiceClient _fhirServiceClient;
		private readonly ILogger<UpsertFhirPathoxDataJob> _logger;
		private readonly IPathoxFhirDataAdapter _pathoxFhirDataAdapter;
		public UpsertFhirPathoxDataJob(IFhirServiceClient fhirServiceClient, ILogger<UpsertFhirPathoxDataJob> logger, IPathoxFhirDataAdapter pathoxFhirDataAdapter)
		{
			_fhirServiceClient = fhirServiceClient;
			_logger = logger;
			_pathoxFhirDataAdapter = pathoxFhirDataAdapter;
		}

		public override async Task<JobResult> ExecuteJobAsync(UpsertFhirPathoxDataArgs args)
		{
			_logger.LogInformation("Uploading FHIR Resources for Pathox data of patients with FHIR {id}", args.FhirPatientId);

			//Upload To Fhir
			var pathoxOrganization = await _fhirServiceClient.UpsertHospitalDepartment("Pathox");

			var pathoxTransformationAdditionalInfo = new PathoxTransformationAdditionalInfo()
			{
				CloudPatientId = args.CloudPatientId,
				FhirPatientId = args.FhirPatientId,
				PathoxOrganizationId = pathoxOrganization.Identifier.FirstOrDefault().Value
			};

			var fhirResources = new List<Resource>();

			foreach (var pathoxExamResult in args.PathoxExamResults)
			{
				var pathoxExamResultResources = _pathoxFhirDataAdapter.Transform(pathoxExamResult, pathoxTransformationAdditionalInfo);
				fhirResources.AddRange(pathoxExamResultResources);
			}

			await _fhirServiceClient.CreateTransactionBundlesForResourcesAsync(fhirResources);

			return new JobResult()
			{
				JobResultEvent = new PathoxDataUploadToFhirCompletedEvent() { UploadedPathoxExamIds = args.PathoxExamResults.Select(e => e.exam.groupNumberAlt).ToList()},
				StateMachineId = args.StateMachineId
			};
		}
	}
}
