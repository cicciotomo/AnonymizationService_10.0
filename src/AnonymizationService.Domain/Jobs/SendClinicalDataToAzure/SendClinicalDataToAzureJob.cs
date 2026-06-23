using AnonymizationService.Services.FhirResourcesFilterService;
using AnonymizationService.Services.FhirService;
using AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter;
using AnonymizationService.StateMachines.Clinical;
using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.SendClinicalDataToAzure
{
    internal class SendClinicalDataToAzureJob : Job<SendClinicalDataToAzureArgs, JobResult>
    {
        private readonly IFhirServiceClient _fhirServiceClient;
        private readonly ITextAnalyticsResponseToFhirDataAdapter _textAnalyticsResponseToFhirDataAdapter;
        private readonly IFhirResourcesFilterService _fhirResourcesFilterService;
        private readonly ILogger<SendClinicalDataToAzureJob> _logger;

        public SendClinicalDataToAzureJob(IFhirServiceClient fhirServiceClient, ITextAnalyticsResponseToFhirDataAdapter textAnalyticsResponseToFhirDataAdapter, IFhirResourcesFilterService fhirResourcesFilterService, ILogger<SendClinicalDataToAzureJob> logger)
        {
            _fhirServiceClient = fhirServiceClient;
            _textAnalyticsResponseToFhirDataAdapter = textAnalyticsResponseToFhirDataAdapter;
            _fhirResourcesFilterService = fhirResourcesFilterService;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(SendClinicalDataToAzureArgs args)
        {
            var documentIds = new List<int>();

            _logger.LogInformation($"Sending clinical data to Azure FHIR Service for patient {args.CloudPatientId}");

            foreach (var documentParsedResult in args.SerializedAnalyticsResults.Where(x => x.Success))
            {
                try
                {
                    _logger.LogInformation($"Extracting FHIR Resources for document {documentParsedResult.Id}");

                    var documentSections = documentParsedResult.Result.Where(x => x.Success).Select(r => new DocumentSection() { SectionName = r.PipelineName, SerializedFhirBundle = r.Result}).ToList();
                    var parsedResults = _textAnalyticsResponseToFhirDataAdapter.ExtractFhirResources(documentSections);
                    
                    var department = await
                        _fhirServiceClient.UpsertHospitalDepartment(documentParsedResult.IssuingDepartment);
                    
                    _logger.LogInformation($"Upsert FHIR Encounter for document {documentParsedResult.Id}");

                    var encounter = await _fhirServiceClient.UpsertEncounter(
                        startDate: documentParsedResult.EncounterStartDate
                        , endDate: documentParsedResult.EncounterEndDate
                        , patientIdentifier: args.CloudPatientId.ToString()
                        , patientId: args.FhirPatientId
                        , identifier: documentParsedResult.EncounterId.ToString());
                    
                    _logger.LogInformation($"Filtering FHIR resources for document {documentParsedResult.Id}");

                    var entitiesForFhir = _fhirResourcesFilterService.FilterFhirResources(parsedResults, new FilterManipulationData()
                    {
                        FhirPatientId = args.FhirPatientId,
                        EncounterEndDate = documentParsedResult.EncounterEndDate,
                        EncounterId = documentParsedResult.EncounterId,
                        PatientIdentifier = args.CloudPatientId,
                        DocumentName = documentParsedResult.Description,
                        DepartmentIdentifier = department.Identifier.FirstOrDefault().Value,
                        DocumentType = documentParsedResult.DocumentType,
                        CreationDate = documentParsedResult.CreationDate
                    }) ;
                    
                    _logger.LogInformation($"Uploading FHIR resources for document {documentParsedResult.Id}");
                    await _fhirServiceClient.CreateTransactionBundlesForResourcesAsync(entitiesForFhir);
                    documentIds.Add(documentParsedResult.Id);
                }
                catch (Exception ex)
                {
                    //TODO DoSomething
                    _logger.LogError(ex.Message, ex);
                }
            }

            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new ClinicalDataSentToAzureEvent(documentIds)
            };
        }
    }
}
