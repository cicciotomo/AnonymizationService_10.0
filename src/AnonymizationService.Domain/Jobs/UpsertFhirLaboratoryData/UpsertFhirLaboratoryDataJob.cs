using AnonymizationService.Services.FhirService;
using AnonymizationService.StateMachines.Laboratory;
using Porini.Abp.StateMachineEngine.Jobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AnonymizationService.Jobs.UpsertFhirLaboratoryData
{
    internal class UpsertFhirLaboratoryDataJob : Job<UpsertFhirLaboratoryDataArgs, JobResult>
    {
        private readonly IFhirServiceClient _fhirServiceClient;
        private readonly ILogger<UpsertFhirLaboratoryDataJob> _logger;

        public UpsertFhirLaboratoryDataJob(IFhirServiceClient fhirServiceClient, ILogger<UpsertFhirLaboratoryDataJob> logger)
        {
            _fhirServiceClient = fhirServiceClient;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(UpsertFhirLaboratoryDataArgs args)
        {
            _logger.LogInformation("Uploading FHIR Resources for laboratory data of patients with FHIR {id}", args.FhirPatientId);

            await _fhirServiceClient.UpsertEncounterWithObservationsAsync(args.FhirPatientId, args.Exams);

            return new JobResult()
            {
                JobResultEvent = new LaboratoryExamsUploadToFhirCompletedEvent(args.Exams),
                StateMachineId = args.StateMachineId
            };
        }
    }
}
