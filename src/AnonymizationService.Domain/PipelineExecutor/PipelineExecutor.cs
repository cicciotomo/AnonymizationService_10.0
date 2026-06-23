using AnonymizationService.ContainerImages;
using AnonymizationService.Containers;
using AnonymizationService.DefaultImportParameters;
using AnonymizationService.PodDefinitions;
using AnonymizationService.Pods;
using AnonymizationService.Services.FileSystemDocumentLogService;
using DemographicWS;
using FellowOakDicom.Imaging.Render;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SRace.JUST.net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.PipelineExecutor
{
    public class PipelineExecutor : ITransientDependency, IPipelineExecutor
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PipelineExecutor> _logger;
        private readonly IPodOrchestrator _podOrchestrator;
        private readonly IRepository<PodDefinition, Guid> _podDefinitionsRepository;
        private readonly IRepository<ContainerImage, Guid> _containerImagesRepository;
        private readonly IRepository<DefaultImportParameter, Guid> _defaultImportParameterRepository;


        private const int MAX_NUMBER_OF_ITERATION = 10;

        public PipelineExecutor(ILogger<PipelineExecutor> logger,
            IPodOrchestrator podOrchestrator,
            IRepository<PodDefinition, Guid> podDefinitionsRepository,
            IConfiguration configuration,
            IRepository<ContainerImage, Guid> containerImagesRepository,
            IRepository<DefaultImportParameter, Guid> defaultImportParameterRepository
            )
        {
            _logger = logger;
            _podOrchestrator = podOrchestrator;
            _podDefinitionsRepository = podDefinitionsRepository;
            _containerImagesRepository = containerImagesRepository;
            _configuration = configuration;
            _defaultImportParameterRepository = defaultImportParameterRepository;
        }

        public void WriteLogToFile(Serilog.ILogger logger, string message, int documentId, bool isActive)
        {
            if (isActive)
            {
                logger.Information(message, documentId);
            }
        }

        public async Task<List<PipelineExecutionResult>> ExecuteAsync(PipelineDefinition[] pipelineDefinition, int documentId)
        {
            var generateLogValue = _defaultImportParameterRepository.GetListAsync().Result.Where(x => x.Name == "Generate Log").FirstOrDefault().Value;
            var fileSystemLogsActive = generateLogValue == "true" ? true : false;
#if DEBUG
            var path = "As-Logs/";
#else
            var path = _configuration.GetValue<string>("LOGS_FOLDER");
#endif


            var jsonTransformer = new JsonTransformer();

            var executionResults = new List<PipelineExecutionResult>();

            var _fileSystemDocumentLogService = new FileSystemDocumentLogService();

            Serilog.ILogger documentLogger = null;
            if (fileSystemLogsActive)
            {
                documentLogger = _fileSystemDocumentLogService.CreaLoggerPerDocumento(path, documentId);
            }

            foreach (var pipeline in pipelineDefinition)
            {

                _logger.LogInformation($"PipelineExecutor - Start pipeline with name {pipeline.Name} - {documentId}");
                WriteLogToFile(documentLogger, $"PipelineExecutor - Start pipeline with name {pipeline.Name} - {documentId}", documentId, fileSystemLogsActive);
                var jsonDataContent = JsonConvert.SerializeObject(pipeline.Content.Data);
                var byteArrayDataContent = Array.Empty<byte>();

                var podDefinitions = await _podDefinitionsRepository.GetListAsync(includeDetails: true);
                var containerImages = await _containerImagesRepository.GetListAsync(includeDetails: true);

                var isFirstContainer = true;

                foreach (var containerOrchestrationStep in pipeline.Content.Containers)
                {
                    _logger.LogDebug($"PipelineExecutor - Starting container {containerOrchestrationStep.ContainerName} on step {containerOrchestrationStep.Step} out of {pipeline.Content.Containers.Length} - {documentId}");
                    WriteLogToFile(documentLogger, $"PipelineExecutor - Starting container {containerOrchestrationStep.ContainerName} on step {containerOrchestrationStep.Step} out of {pipeline.Content.Containers.Length} - {documentId}", documentId, fileSystemLogsActive);
                    Pod pod;
                    Containers.Container container = null;

                    //POD MANAGER
                    try
                    {
                        var podDefinition = podDefinitions.FirstOrDefault(x => x.Name == containerOrchestrationStep.ContainerName);
                        if (podDefinition == null)
                        {
                            throw new NotImplementedException($"{containerOrchestrationStep.ContainerName} image not found - {documentId}");
                        }
                        pod = await _podOrchestrator.ExecutePodAsync(podDefinition, containerImages);
                    }
                    catch (NotImplementedException ex)
                    {
                        _logger.LogError(ex.Message);
                        _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}");
                        _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - ex.InnerException?.Message: {ex.InnerException?.Message}  - {documentId}");

                        WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}", documentId, fileSystemLogsActive);
                        var executionResult = new PipelineExecutionResult
                        {
                            PipelineName = pipeline.Name,
                            Result = jsonDataContent,
                            ErrorMessage = ex.Message
                        };
                        executionResults.Add(executionResult);
                        continue;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed try run container for {containerOrchestrationStep.ContainerName} image - {documentId}");
                        _logger.LogError($"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}");
                        
                        WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}", documentId, fileSystemLogsActive);

                        var executionResult = new PipelineExecutionResult
                        {
                            PipelineName = pipeline.Name,
                            Result = jsonDataContent,
                            ErrorMessage = ex.Message
                        };
                        executionResults.Add(executionResult);
                        continue;
                        //throw;
                    }
                    //POD MANAGER - END


                    try
                    {
                        container = pod.ContainerEntryPoint;
                        var containerImage = containerImages.SingleOrDefault(i => i.Name == container.ImageName);

                        if (isFirstContainer)
                        {
                            isFirstContainer = false;
                            if (containerImage.IsBinaryDataContent)
                            {
                                byteArrayDataContent = (byte[])pipeline.Content.Data;
                            }
                        }
#if DEBUG
                        var handler = new HttpClientHandler();
                        handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };
                        var httpClient = new HttpClient(handler);
                        httpClient.Timeout = TimeSpan.FromSeconds(60);
#else
                        var httpClient = new HttpClient();
#endif
                        var dataContent = containerImage.IsBinaryDataContent ? new ByteArrayContent(byteArrayDataContent) : new StringContent(jsonDataContent, Encoding.UTF8, "application/json");

                        if (containerImage?.InvocationHeaders?.Any() == true)
                        {
                            foreach (var header in containerImage.InvocationHeaders)
                            {
                                if (!header.FromAppConfiguration)
                                {
                                    dataContent.Headers.Add(header.Name, header.Value);
                                }
                                else
                                {
                                    var value = _configuration.GetValue<string>(header.Value);
                                    dataContent.Headers.Add(header.Name, value);
                                }
                            }
                        }

                        _logger.LogDebug($"Executing POST request to {container.Url} - {documentId}");
                        WriteLogToFile(documentLogger, $"Executing POST request to {container.Url} - {documentId}", documentId, fileSystemLogsActive);
                        
                        var httpResponse = await httpClient.PostAsync(container.Url, dataContent);

                        _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - {DateTime.Now.ToString()} - httpResponse StatusCode = {httpResponse.StatusCode} - IsSuccessStatusCode = {httpResponse.IsSuccessStatusCode} - {documentId}");
                        WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - {DateTime.Now.ToString()} - httpResponse StatusCode = {httpResponse.StatusCode} - IsSuccessStatusCode = {httpResponse.IsSuccessStatusCode} - {documentId}", documentId, fileSystemLogsActive);
                        byte[] contentBytes = await httpResponse.Content.ReadAsByteArrayAsync();
                        string resultString = Encoding.UTF8.GetString(contentBytes);

                        _logger.LogDebug($"PipelineExecutor - Dimensione ByteArray = {contentBytes.Length} - DataContent = {resultString} - {documentId}");
                        _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - Dimensione ByteArray = {contentBytes.Length} - {documentId}");
                        WriteLogToFile(documentLogger, $"PipelineExecutor - Dimensione ByteArray = {contentBytes.Length} - DataContent = {resultString} - {documentId}", documentId, fileSystemLogsActive);
                        WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - Dimensione ByteArray = {contentBytes.Length} - {documentId}", documentId, fileSystemLogsActive);

                        httpResponse.EnsureSuccessStatusCode();

                        if (httpResponse.Headers.TryGetValues("Operation-Location", out var operationLocationHeaders))
                        {
                            var operationLocation = operationLocationHeaders.FirstOrDefault();
                            var uri = new Uri(operationLocation);
                            var operationLocationHost = uri.Host;

                            for (var i = 0; i < MAX_NUMBER_OF_ITERATION; i++)
                            {
                                _logger.LogDebug($"i < MAX_NUMBER_OF_ITERATION - i = {i} - MAX_NUMBER_OF_ITERATION = {MAX_NUMBER_OF_ITERATION} - {documentId}");
                                WriteLogToFile(documentLogger, $"i < MAX_NUMBER_OF_ITERATION - i = {i} - MAX_NUMBER_OF_ITERATION = {MAX_NUMBER_OF_ITERATION} - {documentId}", documentId, fileSystemLogsActive);

                                await Task.Delay(1000 * i * i);

#if DEBUG
                                if (operationLocation.StartsWith("http://srraceasdev.ihsr.dom/language/"))
                                {
                                    operationLocation = operationLocation.Replace("http://srraceasdev.ihsr.dom/language/", "http://srraceasdev.ihsr.dom/text-analytics-for-health/language/");
                                    _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation (LOCALHOST)={operationLocation} - {documentId}");
                                    WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation (LOCALHOST)={operationLocation} - {documentId}", documentId, fileSystemLogsActive);
                                }
                                else {
                                    _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation (LOCALHOST)={operationLocation} - {documentId}");
                                    WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation (LOCALHOST)={operationLocation} - {documentId}", documentId, fileSystemLogsActive);
                                }
#else
                                _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation ={operationLocation} - {documentId}");
                                WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation ={operationLocation} - {documentId}", documentId, fileSystemLogsActive);
#endif

                                    httpResponse = await httpClient.GetAsync(operationLocation);
                                _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation StatusCode = {httpResponse.StatusCode} - {documentId}");
                                WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - operationLocation StatusCode = {httpResponse.StatusCode} - {documentId}", documentId, fileSystemLogsActive);

                                httpResponse.EnsureSuccessStatusCode();
                                var getResultContent = await httpResponse.Content.ReadAsStringAsync();
                                var result = JsonConvert.DeserializeObject<dynamic>(getResultContent);
                                if (result.status == "succeeded")
                                {
                                    _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - BREAK - result.status == \"succeeded\" - {documentId}");
                                    WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - BREAK - result.status == \"succeeded\" - {documentId}", documentId, fileSystemLogsActive);
                                    break;
                                }
                                else if (result.status != "running" && result.status != "notStarted")
                                {
                                    _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - throw new Exception - result.status != \"running\" && result.status != \"notStarted\" - {documentId}");
                                    WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - throw new Exception - result.status != \"running\" && result.status != \"notStarted\" - {documentId}", documentId, fileSystemLogsActive);
                                    throw new Exception("PipelineExecutor {containerOrchestrationStep.Step} - Fail get result from job. - {documentId}");
                                }

                                if (i == MAX_NUMBER_OF_ITERATION - 1)
                                {
                                    WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - Reached max number of iteration on GET request on operation location {operationLocation} - {documentId}", documentId, fileSystemLogsActive);
                                    throw new Exception($"PipelineExecutor {containerOrchestrationStep.Step} - Reached max number of iteration on GET request on operation location {operationLocation} - {documentId}");
                                }
                            }
                        }

                        jsonDataContent = await httpResponse.Content.ReadAsStringAsync();
                        byteArrayDataContent = await httpResponse.Content.ReadAsByteArrayAsync();
                        if (!string.IsNullOrEmpty(containerOrchestrationStep.OutputTransformationMap))
                        {

                            _logger.LogDebug($"PipelineExecutor  {containerOrchestrationStep.Step} - jsonTransformer.Transform - jsonDataContent = {jsonDataContent} - {documentId}");
                            WriteLogToFile(documentLogger, $"PipelineExecutor  {containerOrchestrationStep.Step} - jsonTransformer.Transform - jsonDataContent = {jsonDataContent} - {documentId}", documentId, fileSystemLogsActive);
                            jsonDataContent = jsonTransformer.Transform(containerOrchestrationStep.OutputTransformationMap, jsonDataContent);
                        }
                    }

                    catch (TaskCanceledException ex)
                    {
                        WriteLogToFile(documentLogger, $"PipelineExecutor  {containerOrchestrationStep.Step} - La richiesta è scaduta: {ex.Message} - {documentId}", documentId, fileSystemLogsActive);
                        _logger.LogError($"PipelineExecutor  {containerOrchestrationStep.Step} - La richiesta è scaduta: {ex.Message} - {documentId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"PipelineExecutor {containerOrchestrationStep.Step} - Failed call url {container.Url} - {documentId}");
                        WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - Failed call url {container.Url} - {documentId}", documentId, fileSystemLogsActive);
                        
                        _logger.LogError($"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}");
                        WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}", documentId, fileSystemLogsActive);

                        var executionResult = new PipelineExecutionResult
                        {
                            PipelineName = pipeline.Name,
                            Result = jsonDataContent,
                            ErrorMessage = ex.Message
                        };
                        executionResults.Add(executionResult);
                    }
                    finally
                    {
                        try
                        {
                            await _podOrchestrator.DisposePodAsync(pod);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"PipelineExecutor {containerOrchestrationStep.Step} - Failed stop container with id {container.ContainerId} - {documentId}");
                            WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - Failed stop container with id {container.ContainerId} - {documentId}", documentId, fileSystemLogsActive);
                            
                            _logger.LogError($"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}");
                            
                            WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}", documentId, fileSystemLogsActive);

                            var executionResult = new PipelineExecutionResult
                            {
                                PipelineName = pipeline.Name,
                                Result = jsonDataContent,
                                ErrorMessage = ex.Message
                            };
                            executionResults.Add(executionResult);
                        }
                    }

                    if (containerOrchestrationStep.AddOutputToResult)
                    {
                        var executionResult = new PipelineExecutionResult
                        {
                            PipelineName = pipeline.Name,
                            Result = jsonDataContent
                        };
                        executionResults.Add(executionResult);
                    }
                    if (containerOrchestrationStep.ShouldStartNewPipeline == true)
                    {

                        _logger.LogDebug($"PipelineExecutor {containerOrchestrationStep.Step} - jsonTransformer.Transform - jsonDataContent = {jsonDataContent} - {documentId}");
                        WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - jsonTransformer.Transform - jsonDataContent = {jsonDataContent} - {documentId}", documentId, fileSystemLogsActive);

                        try
                        {
                            var innerPipelineDefinition = JsonConvert.DeserializeObject<PipelineDefinition[]>(jsonDataContent);
                            executionResults.AddRange(await ExecuteAsync(innerPipelineDefinition, documentId));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}");
                            _logger.LogError($"PipelineExecutor {containerOrchestrationStep.Step} - ex.InnerException?.Message: {ex.InnerException?.Message} - {documentId}");

                            WriteLogToFile(documentLogger, $"PipelineExecutor {containerOrchestrationStep.Step} - ex.Message: {ex.Message} - {documentId}", documentId, fileSystemLogsActive);

                            var executionResult = new PipelineExecutionResult
                            {
                                PipelineName = pipeline.Name,
                                Result = jsonDataContent,
                                ErrorMessage = ex.Message
                            };
                            executionResults.Add(executionResult);
                        }

                    }
                }
            }
            _logger.LogDebug($"PipelineExecutor - Esecuzione pipeline completata - {documentId}");
            _logger.LogDebug($"PipelineExecutor - executionResults.Count = {executionResults.Count} - {documentId}");

            WriteLogToFile(documentLogger, $"PipelineExecutor - Esecuzione pipeline completata - {documentId}", documentId, fileSystemLogsActive);
            WriteLogToFile(documentLogger, $"PipelineExecutor - executionResults.Count = {executionResults.Count} - {documentId}", documentId, fileSystemLogsActive);

            foreach (var er in executionResults)
            {
                _logger.LogDebug($"PipelineExecutor - executionResults = {er.ErrorMessage} - {er.Success} - {er.PipelineName} - {documentId}");
                WriteLogToFile(documentLogger, $"PipelineExecutor - executionResults = {er.ErrorMessage} - {er.Success} - {er.PipelineName} - {documentId}", documentId, fileSystemLogsActive);
            }
            return executionResults;
        }
    }

}
