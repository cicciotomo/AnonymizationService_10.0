using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using AnonymizationService.Services.FhirService;
using AnonymizationService.Services.FhirService.Adapters.DbUri;
using AnonymizationService.StateMachines.DbUri;
using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Jobs;
using System.Linq;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.UpsertFhirDbUriData
{
    internal class UpsertFhirDbUriDataJob : Job<UpsertFhirDbUriDataArgs, JobResult>
    {
        private readonly IFhirServiceClient _fhirServiceClient;
        private readonly ILogger<UpsertFhirLaboratoryDataJob> _logger;
        private readonly IDbUriFhirDataAdapter _dbUriFhirDataAdapter;

        public UpsertFhirDbUriDataJob(IFhirServiceClient fhirServiceClient, ILogger<UpsertFhirLaboratoryDataJob> logger, IDbUriFhirDataAdapter dbUriFhirDataAdapter)
        {
            _fhirServiceClient = fhirServiceClient;
            _logger = logger;
            _dbUriFhirDataAdapter = dbUriFhirDataAdapter;
        }

        public override async Task<JobResult> ExecuteJobAsync(UpsertFhirDbUriDataArgs args)
        {
            _logger.LogInformation($"Uploading FHIR Resources for db Uri data of patients with FHIR {args.FhirPatientId}");

            var urologyOrganization = await _fhirServiceClient.UpsertHospitalDepartment("Urology");

            var dbUriTransformationAdditionalInfo = new DbUriTransformationAdditionalInfo()
            {
                CloudPatientId = args.CloudPatientId,
                FhirPatientId = args.FhirPatientId,
                UrologyOrganizationId = urologyOrganization.Identifier.FirstOrDefault().Value
            };

            var fhirResources = _dbUriFhirDataAdapter.Transform(args.DbUriEventToUpload,
                args.DbUriFUpItemToUpload,
                dbUriTransformationAdditionalInfo);

            var (patientEncounterIdentifier, patientEncounterResources) = _dbUriFhirDataAdapter.TransformPatientEncounter(args.Patient, dbUriTransformationAdditionalInfo);

            var patientDataEncounter = await _fhirServiceClient.GetEncounterByIdentifier(patientEncounterIdentifier);

            if (patientDataEncounter == null)
            {
                fhirResources.AddRange(patientEncounterResources);
            }

            var (fupEncounterIdentifier, fupEncounterResources) = _dbUriFhirDataAdapter.TransformFupEncounter(args.FupData, dbUriTransformationAdditionalInfo);

            var fupDataEncounter = await _fhirServiceClient.GetEncounterByIdentifier(fupEncounterIdentifier);

            if (fupDataEncounter == null)
            {
                fhirResources.AddRange(fupEncounterResources);
            }

            await _fhirServiceClient.CreateTransactionBundlesForResourcesAsync(fhirResources);

            

            var param_1 = args.DbUriEventToUpload.Select(e => e.generalInfo.id).ToList();
            var param_2 = args.DbUriFUpItemToUpload != null ? args.DbUriFUpItemToUpload.Select(f => f.id).ToList() : new System.Collections.Generic.List<long>();
            return new JobResult()
            {
                JobResultEvent = new DbUriDataUploadToFhirCompletedEvent(
                    param_1,
                    param_2),
                StateMachineId = args.StateMachineId
            };
        }
    }
}
