using AnonymizationService.Jobs.SendClinicalDataToAzure;
using AnonymizationService.Services.FhirResourcesFilterService;
using AnonymizationService.Services.FhirService;
using AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter;
using AnonymizationService.StateMachines.Clinical;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc;
using AnonymizationService.Services.DbUri;
using AnonymizationService.StateMachines.Redcap;
using AnonymizationService.Services.Redcap;
using AnonymizationService.Redcap;
using Microsoft.Extensions.Azure;

namespace AnonymizationService.Jobs.UpsertFhirRedcapPatientData
{
    internal class UpsertFhirRedcapPatientDataJob : Job<UpsertFhirRedcapPatientDataArgs, JobResult>
    {
        private readonly IFhirServiceClient _fhirServiceClient;
        private readonly IFhirResourcesFilterService _fhirResourcesFilterService;
        private readonly ILogger<UpsertFhirRedcapPatientDataJob> _logger;

        public UpsertFhirRedcapPatientDataJob(
            IFhirServiceClient fhirServiceClient
            , IFhirResourcesFilterService fhirResourcesFilterService
            , ILogger<UpsertFhirRedcapPatientDataJob> logger)
        {
            _fhirServiceClient = fhirServiceClient;
            _fhirResourcesFilterService = fhirResourcesFilterService;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(UpsertFhirRedcapPatientDataArgs args)
        {
            _logger.LogInformation($"[{args.StateMachineId}] Upserting RedCap data on FHIR - FhirID {args.FhirPatientId} CPI {args.CloudPatientId}");
            /*  Mandare su FHIR
             *  Encounter
             *      Start/End Date DateTime.Now [Alternativa non trovata]
             *      patientIdentifier = CPI
             *      patientId = FhirPatientId
             *      identifier = ???
             *      Composition
             *          DocumentReference
             *              RelatesTo(target = DocumentReference dei metadata)
             *              Content(attachment = args.Data)
            */
            var encounterIdentifier = Guid.NewGuid();
            var encounterDate = DateTime.Now;
            var compositionIdentifier = Guid.NewGuid();
            var compositionTitle = args.StudyName;
            var compositionType = new CodeableConcept("http://loinc.org", "75218-8", "Case report");
            var compositionAuthor = new List<ResourceReference>()
                    { new() { 
                        Identifier = new Identifier() { Value = "filterManipulationData.DepartmentIdentifier" },
                        Type = "Organization" 
                        }
                    };
            var compositionSectionTitle = "RedCap Data";
            var documentReferenceIdentifier = Guid.NewGuid().ToString();
            var metadataFhirId = args.MetadataFhirId;
            #region Crea e trasmetti encounter
            try
            {
                var encounter = await _fhirServiceClient.UpsertEncounter(
                    startDate: encounterDate
                    , endDate: encounterDate
                    , patientIdentifier: args.CloudPatientId.ToString()
                    , patientId: args.FhirPatientId
                    , identifier: encounterIdentifier.ToString());
                _logger.LogInformation("[{0}] Upserted encounter {1} for redcap data", args.StateMachineId, encounterIdentifier);

            }
            catch (Exception ex)
            {
                _logger.LogError("[{0}] Errpr upserting encounter {1}", args.StateMachineId, ex.Message);
                _logger.LogError($"{ex.Message}", ex);
                throw;
            }
            #endregion
            
            #region Se non trasmessi, trasmetti metadata *********** TUTTO COMMENTATO
            //if (string.IsNullOrEmpty(metadataFhirId))
            //{
            //    var metadataReferenceIdentifier = Guid.NewGuid();
            //    var metadataDate = DateTime.Now;
            //    var serializedMetaData = args.Metadata;
            //    string metadataBase64String;

            //    try
            //    {
            //        var utf8SerializedDataBytes = System.Text.Encoding.UTF8.GetBytes(serializedMetaData);
            //        metadataBase64String = Convert.ToBase64String(utf8SerializedDataBytes);
            //        _logger.LogInformation("[{0}] Redcap MetaData encoded", args.StateMachineId);
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError("[{0}] Error encoding MetaData ", args.StateMachineId);
            //        throw;
            //    }

            //    var metadataDocumentReference = new DocumentReference()
            //    {
            //        Identifier = new List<Identifier>() { new() { Value = metadataReferenceIdentifier.ToString() } },
            //        Date = metadataDate,
            //        Status = DocumentReferenceStatus.Current,
            //        Content = new List<DocumentReference.ContentComponent>()
            //        {
            //            new()
            //            {
            //                Attachment = new Attachment()
            //                {
            //                    ContentType = "application/json",
            //                    Data = Convert.FromBase64String(metadataBase64String)
            //                }
            //            }
            //        }
            //    };
            //    await _fhirServiceClient.UpsertDocumentReference(metadataDocumentReference);

            //    var studyConfig = await _redcapService.GetStudyConfigurationByIdAsync(args.RedcapStudyConfigurationID);
            //    studyConfig.SetMetadataFihrId(metadataReferenceIdentifier);
            //    studyConfig.SetTrasmissionDate(metadataDate);
            //    await _redcapService.UpdateAsync(studyConfig);
            //}
            #endregion

            #region crea composition
            var composition = new Composition()
            {
                Subject = new ResourceReference() { 
                      Type = "Patient"
                    , Reference = $"Patient/{args.FhirPatientId}"
                    , Identifier = new Identifier() 
                        { Value = args.CloudPatientId.ToString() } },
                Identifier = new Identifier() { Value = compositionIdentifier.ToString() },
                Section = new List<Composition.SectionComponent>(),
                Title = compositionTitle,
                Type = compositionType,
                Status = CompositionStatus.Final,
                Date = encounterDate.ToString("yyyy-MM-ddThh:mm:sszzzz"),
                Author = compositionAuthor,
                Encounter = new ResourceReference() 
                    { Identifier = new Identifier() 
                    { Value = encounterIdentifier.ToString() } 
                }
            };
            #endregion

            #region prepara il dato 
            string base64String;
            try
            {
                var serializedData = args.Data;
                var utf8SerializedDataBytes = System.Text.Encoding.UTF8.GetBytes(serializedData);
                base64String = Convert.ToBase64String(utf8SerializedDataBytes);
                _logger.LogInformation("[{0}] Redcap Data encoded", args.StateMachineId);
            }
            catch (Exception ex)
            {
                _logger.LogError("[{0}] Error encoding data ", args.StateMachineId);
                _logger.LogError(ex.Message);
                throw;
            }
            #endregion

            #region Crea riferimento a document dei metadata
            var metadataReference = new DocumentReference.RelatesToComponent()
            {
                Code = DocumentRelationshipType.Appends,
                Target = new ResourceReference()
                {
                    Identifier = new Identifier()
                    {
                        Value = metadataFhirId
                    }
                }
            };
            #endregion
            
            #region crea documentreference
            var documentReference = new DocumentReference()
            {
                Identifier = new List<Identifier>() { new() { Value = documentReferenceIdentifier } },
                Subject = new ResourceReference() 
                    { Type = "Patient"
                    , Reference = $"Patient/{args.FhirPatientId}"
                    , Identifier = new Identifier()  { Value = args.CloudPatientId.ToString() } 
                },
                Context = new DocumentReference.ContextComponent()
                {
                    Encounter = new List<ResourceReference>()
                    {
                        new() { Identifier = new Identifier() { Value = encounterIdentifier.ToString() } }
                    }
                },
                Date = encounterDate,
                Status = DocumentReferenceStatus.Current,
                RelatesTo = new List<DocumentReference.RelatesToComponent>() { 
                    metadataReference
                },
                Content = new List<DocumentReference.ContentComponent>()
                {
                    new()
                    {
                        Attachment = new Attachment()
                        {
                            ContentType = "application/json",
                            Data = Convert.FromBase64String(base64String)
                        }
                    }
                }
            };
            #endregion

            #region crea composition section e aggiunge a composition
            var compositionSection = new Composition.SectionComponent()
            {
                Title = compositionSectionTitle,
                Entry = new List<ResourceReference>()
            };

            compositionSection.Entry.Add(new ResourceReference()
            {
                Type = documentReference.TypeName,
                Identifier = new Identifier() { Value = documentReference.Identifier.First().Value }
            });
            
            composition.Section.Add(compositionSection);
            #endregion

            #region upserting bundle data
            try
            {
                var entitiesForFhir = new List<Resource>();
                entitiesForFhir.Add(documentReference);
                composition.Section.Add(compositionSection);
                entitiesForFhir.Add(composition);
                await _fhirServiceClient.CreateTransactionBundlesForResourcesAsync(entitiesForFhir);
                _logger.LogInformation("[{0}] Upserted bundle data ", args.StateMachineId);

            }
            catch (Exception ex)
            {
                _logger.LogError("[{0}] Error upserting bundle data", args.StateMachineId);
                _logger.LogError($"{ex.Message}", ex);
                throw;
            }
            #endregion

            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new RedcapPatientDataSentEvent(encounterIdentifier)
            };
        }
    }
}
