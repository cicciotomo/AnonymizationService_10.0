using AnonymizationService.Services.Galileo;
using AnonymizationService.StateMachines.Clinical;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AnonymizationService.ClinicalDocuments;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using System.Runtime.CompilerServices;
using AnonymizationService.Services.Clinical;

namespace AnonymizationService.Jobs.RetrieveClinicalDocuments
{
    internal class RetrieveClinicalDocumentsJob : Job<RetrieveClinicalDocumentsJobArgs, JobResult>
    {
        private readonly IClinicalDataService _clinicalDataService;
        private readonly IClinicalDocumentTypeService _clinicalDocumentTypeService;
        private readonly ILogger<RetrieveClinicalDocumentsJob> _logger;

        public RetrieveClinicalDocumentsJob(
                IClinicalDataService clinicalDataService, 
                IClinicalDocumentTypeService clinicalDocumentTypeService,
                ILogger<RetrieveClinicalDocumentsJob> logger
            )
        {
            _clinicalDataService = clinicalDataService;
            _clinicalDocumentTypeService = clinicalDocumentTypeService;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(RetrieveClinicalDocumentsJobArgs args)
        {
            _logger.LogInformation("Retrieving documents for patient {id}", args.CloudPatientId);

            var patientDocuments = await _clinicalDataService.GetPatientClinicalDataResultsAsync(args.MasterPatientIndex, args.StartDate);

            var documents = patientDocuments
                .SelectMany(doc => doc.orders?.
                    SelectMany(ord => ord.documents
                        .Select(clinicalDoc => new ClinicalDocumentSummary()
                        {
                            Id = clinicalDoc.documentId,
                            Description = clinicalDoc.description,
                            CreationDate = DateTime.TryParseExact(
                                clinicalDoc.creationDate
                                , "yyyyMMdd"
                                , CultureInfo.InvariantCulture
                                , DateTimeStyles.None
                                , out var creationDate) ? creationDate : DateTime.Now,
                            EncounterStartDate = DateTime.TryParseExact(
                                doc.episodeStartDate
                                , "yyyyMMdd"
                                , CultureInfo.InvariantCulture
                                , DateTimeStyles.None
                                , out var startDate) ? startDate : DateTime.Now,
                            EncounterEndDate = DateTime.TryParseExact(
                                doc.episodeEndDate
                                , "yyyyMMdd"
                                , CultureInfo.InvariantCulture
                                , DateTimeStyles.None
                                , out var endDate) ? endDate : DateTime.Now,
                            EncounterId = doc.episodeId,
                            IssuingDepartment = ord.filler
                        }))).ToList();

            if (patientDocuments.Any(doc => doc.episodeDocuments?.Count > 0))
            {
                documents = documents.Concat(
                    patientDocuments.SelectMany(
                        doc => doc.episodeDocuments.Select(
                            episodeDocument => new ClinicalDocumentSummary()
                            {
                                Id = episodeDocument.documentId,
                                Description = episodeDocument.description,
                                CreationDate = DateTime.TryParseExact(
                                episodeDocument.creationDate
                                , "yyyyMMdd"
                                , CultureInfo.InvariantCulture
                                , DateTimeStyles.None
                                , out var creationDate) ? creationDate : DateTime.Now,
                                EncounterStartDate = DateTime.TryParseExact(
                                    doc.episodeStartDate
                                    , "yyyyMMdd"
                                    , CultureInfo.InvariantCulture
                                    , DateTimeStyles.None
                                    , out var startDate) ? startDate : DateTime.Now,
                                EncounterEndDate = DateTime.TryParseExact(
                                    doc.episodeEndDate
                                    , "yyyyMMdd"
                                    , CultureInfo.InvariantCulture
                                    , DateTimeStyles.None
                                    , out var endDate) ? endDate : DateTime.Now,
                                EncounterId = doc.episodeId,
                            }))).ToList();
            }

          

            _logger.LogInformation("Retrieved {num} documents for patient {id}", documents.Count, args.CloudPatientId);

            //ToDo: Add WHERE to filter DocumentType from args
            //List<string> selectedDocumentTypes = args.DocumentType.Split(',').ToList();
            List<string> selectedDocumentTypes = args.DocumentType.Select(x => x.Type).Distinct().ToList();

            var documentTypes = _clinicalDocumentTypeService.GetDocumentTypesAsync().Result.Where(x => x.Enabled==1).Where(x => selectedDocumentTypes.Any(y => y==x.LoincDescription));

            documents = documents.Where(x => documentTypes.Any(y => y.GalileoCode == x.Description )).ToList();

            var filteredDocuments = new List<ClinicalDocumentSummary>();
            foreach (var filter in args.DocumentType)
            {
                //var docs = documents.Where(x => x.IssuingDepartment.ToLower().Contains(filter.Filler.ToLower()) && x.Description == filter.Type);

                var docs = documents.Where(x => x.IssuingDepartment.ToLower().Contains(filter.Filler.ToLower()) && documentTypes.Any(y => y.GalileoCode == x.Description)); 
                filteredDocuments.AddRange(docs);
            }
            _logger.LogDebug($"retrievedDocuments.Count: {filteredDocuments.Count}");

            var retrievedDocuments = new List<ClinicalDocumentSummary>();
            int counter = 0;           
            foreach (var document in filteredDocuments)
            {
                counter++;
                _logger.LogInformation("Retrieving details for document {documentId} of patient {patientId}", document.Id, args.CloudPatientId);
                _logger.LogDebug($"counter {counter}");
                try
                {
                    var currentTypeDocument = documentTypes.Where(x => x.GalileoCode == document.Description).FirstOrDefault();
                    var patientDocumentFile = await _clinicalDataService.GetDocumentAsync(document.Id);

                    document.Base64FileContent = patientDocumentFile.content;
                    document.Name = $"{patientDocumentFile.documentId}-{document.CreationDate}-{document.Description}.PDF";
                    document.DocumentType = currentTypeDocument;
                    retrievedDocuments.Add(document);
                    _logger.LogDebug($"counter {counter}");
                    _logger.LogDebug($"document.Base64FileContent = {document.Base64FileContent} ");
                    _logger.LogDebug($"document.Name = {document.Name}");
                    _logger.LogDebug($"document.DocumentType = {document.DocumentType}");
                    _logger.LogDebug($"Retrieving details for document {document.Id} of patient {args.CloudPatientId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Unable to retrieve document with Id {document.Id}");
                }
            }
            _logger.LogInformation($"Aggiunti {counter} documenti da elaborare");
            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new ClinicalDocumentsRetrievedEvent(retrievedDocuments)
            };
        }
    }
}
