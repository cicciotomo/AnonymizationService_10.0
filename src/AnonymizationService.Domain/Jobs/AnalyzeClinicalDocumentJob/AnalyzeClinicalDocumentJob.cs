using AnonymizationService.PipelineExecutor;
using AnonymizationService.StateMachines.Clinical;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace AnonymizationService.Jobs.AnalyzeClinicalDocumentJob
{
    public class AnalyzeClinicalDocumentJob : Job<AnalyzeClinicalDocumentJobArgs, JobResult>
    {
        private readonly IPipelineExecutor _pipelineExecutor;
        private readonly ILogger<AnalyzeClinicalDocumentJob> _logger;

        public AnalyzeClinicalDocumentJob(IPipelineExecutor pipelineExecutor, ILogger<AnalyzeClinicalDocumentJob> logger)
        {
            _pipelineExecutor = pipelineExecutor;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(AnalyzeClinicalDocumentJobArgs args)
        {
            var pipelineResults = new List<ParsedClinicalDocument>();
            var idsDocumentsNotParsed = new List<string>();

            _logger.LogInformation("Analyzing {number} documents via container pipeline", args.Documents.Count);
            var count = 1;
            var numeroTotaleDocumenti = args.Documents.Count;
            foreach (var document in args.Documents)
            {
                _logger.LogInformation("Analyzing document {document}", document.Id);

                var documentContentBytes = Convert.FromBase64String(document.Base64FileContent);

                //TODO: Retrieve pipeline definition from somewhere else
                var pipelineDefinitions = new[]
                {
                    new PipelineDefinition
                    {
                        Name = "Data extraction pipeline",
                        Content = new ContentPipelineDefinition
                        {
                            Data = documentContentBytes,
                            Containers = new[]
                            {
                                new ContainerContentPipelineDefinition
                                {
                                    Step = 1,
                                    ContainerName = "FormRecognizer",
                                    VersionContainer = "v1",
                                },
                                new ContainerContentPipelineDefinition
                                {
                                    Step = 2,
                                    ContainerName = "DocumentParser",
                                    VersionContainer = "v1",
                                    ShouldStartNewPipeline = true
                                }
                            }
                        }
                    }
                };
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - pipelineDefinitions {pipelineDefinitions.ToString()}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - documento {count++} di {numeroTotaleDocumenti}");

                var executionResult = await _pipelineExecutor.ExecuteAsync(pipelineDefinitions, document.Id);

                _logger.LogDebug($"AnalyzeClinicalDocumentJob - executionResults.Count {executionResult.Count}");
                if (executionResult is not { Count: > 0 })
                {
                    // *********** NON CI SONO RISULTATI DA INVIARE SU FHIR, PASSA AL DOCUMENTO SUCCESSIVO
                    //foreach (var er in executionResult) {
                    //    _logger.LogInformation($"executionResults = {er.ErrorMessage} - {er.Success} - {er.PipelineName} - {er.Result}");
                    //}
                    //_logger.LogInformation("executionResult.count non è > 0");
                    _logger.LogDebug($"AnalyzeClinicalDocumentJob - DOCUMENTO NON PARSATO - id = {document.Id}");
                    idsDocumentsNotParsed.Add(document.Id.ToString());
                    continue;
                }

                if (executionResult.Any(x => x.Success == false))
                {
                    //_logger.LogInformation($"AnalyzeClinicalDocumentJob - DOCUMENTO NON PARSATO - id = {document.Id}");
                    //foreach (var er in executionResult)
                    //{
                    //    _logger.LogInformation($"AnalyzeClinicalDocumentJob - executionResults = {er.ErrorMessage} - {er.Success} - {er.PipelineName} ");
                    //}
                    var title_OK = executionResult.Count(x => x.Success == true);
                    var title_NO_OK = executionResult.Where(x => x.Success == false).ToList();
                    var title_Distinct = executionResult.GroupBy(x => x.PipelineName).Select(x => x.First()).ToList();
                    var title_NO_OK_Distinct = title_NO_OK.GroupBy(x => x.PipelineName).Select(x => x.First()).ToList();
                    _logger.LogDebug($"Sono stati elaborati con successo {title_OK} su {title_OK+title_NO_OK_Distinct.Count()}");
                    foreach(var t in title_NO_OK_Distinct)
                    {
                        _logger.LogDebug($"{t.PipelineName} non è stato processato con successo");
                    }
                    //_logger.LogInformation($"Sono stati elaborati con successo {title_OK}");
                    //idsDocumentsNotParsed.Add(document.Id.ToString());
                    //continue;
                }
                _logger.LogDebug("AnalyzeClinicalDocumentJob - executionResult.count è > 0");
                var parsedDocument = new ParsedClinicalDocument()
                {
                    Id = document.Id,
                    FileName = document.Name,
                    Description = document.Description,
                    CreationDate = document.CreationDate,
                    EncounterEndDate = document.EncounterEndDate,
                    EncounterId = document.EncounterId,
                    EncounterStartDate = document.EncounterStartDate,
                    IssuingDepartment = document.IssuingDepartment,
                    Result = executionResult,
                    DocumentType = document.DocumentType,
                };
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - Parsed Document");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - Id : {parsedDocument.Id}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - FileName : {parsedDocument.FileName}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - Description : {parsedDocument.Description}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - CreationDate : {parsedDocument.CreationDate.ToString()}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - EncounterEndDate : {parsedDocument.EncounterId.ToString()}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - EncounterId : {parsedDocument.EncounterId.ToString()}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - EncounterStartDate : {parsedDocument.EncounterStartDate.ToString()}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - IssuingDepartment : {parsedDocument.IssuingDepartment}");
                _logger.LogDebug($"AnalyzeClinicalDocumentJob - DocumentType : {parsedDocument.DocumentType.ToString()}");

                pipelineResults.Add(parsedDocument);

                _logger.LogDebug($"AnalyzeClinicalDocumentJob - Document Id: {document.Id}");
            }

            _logger.LogDebug($"\t AnalyzeClinicalDocumentJob\r\n\t Analyzed document {count - 1} di {numeroTotaleDocumenti} \n\r\t Parsed document {pipelineResults.Count} \n\r\t Not Parsed document {idsDocumentsNotParsed.Count}");
            _logger.LogDebug($"AnalyzeClinicalDocumentJob - Parsed document {pipelineResults.Count} ");
            _logger.LogDebug($"AnalyzeClinicalDocumentJob - Not Parsed document {idsDocumentsNotParsed.Count} ");

            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new DocumentsProcessedEvent(pipelineResults)
            };
        }
    }
}
