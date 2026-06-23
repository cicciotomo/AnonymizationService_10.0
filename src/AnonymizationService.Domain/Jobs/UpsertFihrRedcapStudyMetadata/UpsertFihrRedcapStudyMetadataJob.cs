using AnonymizationService.Jobs.UpsertFhirRedcapPatientData;
using AnonymizationService.Services.FhirResourcesFilterService;
using AnonymizationService.Services.FhirService;
using Hl7.Fhir.Model;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AnonymizationService.Services.Redcap;
using AnonymizationService.StateMachines.Redcap;

namespace AnonymizationService.Jobs.UpsertFihrRedcapStudyMetadata
{

    public class UpsertFihrRedcapStudyMetadataJob : Job<UpsertFihrRedcapStudyMetadataArgs, JobResult>
    {
        private readonly IFhirServiceClient _fhirServiceClient;
        private readonly IRedcapService _redcapService;
        private readonly IFhirResourcesFilterService _fhirResourcesFilterService;
        private readonly ILogger<UpsertFihrRedcapStudyMetadataJob> _logger;

        public UpsertFihrRedcapStudyMetadataJob(
            IFhirServiceClient fhirServiceClient
            , IRedcapService redcapService
            , IFhirResourcesFilterService fhirResourcesFilterService
            , ILogger<UpsertFihrRedcapStudyMetadataJob> logger)
        {
            _fhirServiceClient = fhirServiceClient;
            _redcapService = redcapService;
            _fhirResourcesFilterService = fhirResourcesFilterService;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(UpsertFihrRedcapStudyMetadataArgs args)
        {
            var metadataReferenceIdentifier = Guid.NewGuid();
            var metadataDate = DateTime.Now;
            var serializedMetaData = args.Metadata;
            string metadataBase64String;

            try
            {
                var utf8SerializedDataBytes = System.Text.Encoding.UTF8.GetBytes(serializedMetaData);
                metadataBase64String = Convert.ToBase64String(utf8SerializedDataBytes);
                _logger.LogInformation("[{0}] Redcap MetaData encoded", args.StateMachineId);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{0}] Error encoding MetaData ", args.StateMachineId);
                throw;
            }

            var metadataDocumentReference = new DocumentReference()
            {
                Identifier = new List<Identifier>() { new() { Value = metadataReferenceIdentifier.ToString() } },
                Date = metadataDate,
                Status = DocumentReferenceStatus.Current,
                Content = new List<DocumentReference.ContentComponent>()
                    {
                        new()
                        {
                            Attachment = new Attachment()
                            {
                                ContentType = "application/json",
                                Data = Convert.FromBase64String(metadataBase64String)
                            }
                        }
                    }
            };
            await _fhirServiceClient.UpsertDocumentReference(metadataDocumentReference);

            var studyConfig = await _redcapService.GetStudyConfigurationByIdAsync(args.RedcapStudyConfigurationId);
            studyConfig.SetMetadataFihrId(metadataReferenceIdentifier);
            studyConfig.SetTrasmissionDate(metadataDate);
            await _redcapService.UpdateAsync(studyConfig);
            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new RedcapStudyMetadataSentEvent(metadataReferenceIdentifier)
            };
        }
    }
}
