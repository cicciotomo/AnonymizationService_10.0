using AnonymizationService.IntensiveCareData;
using AnonymizationService.Services.SynapsePipeline;
using AnonymizationService.StateMachines.IntensiveCare;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using System.Text.Json;
using AnonymizationService.IntensiveCare;
using Microsoft.Extensions.Logging;
using AnonymizationService.Services.DbUri;

namespace AnonymizationService.StateMachines.IntensiveCare.States
{
    public class CallSynapsePipelineState : State
    {
        
        public CallSynapsePipelineState()
        {
        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var _logger = serviceProvider.GetService<ILogger<IntensiveCarePatientData>>();
            _logger.LogDebug($"CallSynapsePipelineState - RunAsync");
            try
            {
                if (stateMachine is not IntensiveCareStateMachine intensiveCareStateMachine)
                {
                    throw new Exception(string.Format("[{0}] Invalid state machine for IntensiveCareStateMachine", stateMachine.Id));
                }
                _logger.LogDebug($"CallSynapsePipelineState - Inizio richieste istanze pipeline per il paziente {intensiveCareStateMachine.ContextData.CloudPatientId}");
                var patientDataRepository = serviceProvider.GetService<IRepository<IntensiveCarePatientData>>();
                var synapsePipelinesService = serviceProvider.GetService<ISynapsePipelinesService>();
                var patientData = await patientDataRepository.GetListAsync();
                patientData = patientData.Where(x => x.CloudUploadDate.HasValue
                                                && !x.PipelineId.HasValue
                                                && !x.PipelineRunCreationTime.HasValue
                                                && x.CloudPatientId == intensiveCareStateMachine.ContextData.CloudPatientId).ToList();
                _logger.LogDebug($"CallSynapsePipelineState - Inizio richieste di [{patientData.Count}] istanze di pipeline per il paziente {intensiveCareStateMachine.ContextData.CloudPatientId}");
                var count = 0;
                foreach (var item in patientData)
                {
                    _logger.LogDebug($"Creazione istanza {++count} di {patientData.Count}");
                    PipelineParameterDto plParameters = new PipelineParameterDto();
                    plParameters.CloudPatientId = item.CloudPatientId;
                    plParameters.EndDate = item.ExamEndDate != null ? item.ExamEndDate.Value.ToString("yyyy-MM-dd HH:mm:sszzz") : string.Empty;
                    plParameters.StartDate = item.ExamStartDate.ToString("yyyy-MM-dd HH:mm:sszzz");
                    plParameters.PatientId = Guid.Parse(item.PatientId);
                    var response = await synapsePipelinesService.CreatePipelineRunAsync(plParameters);
                    _logger.LogDebug($"CreatePipelineRunAsync response: {response}");
                    CreatePipelineRunAsyncResponse resp = JsonSerializer.Deserialize<CreatePipelineRunAsyncResponse>(response);
                    if (resp != null && !string.IsNullOrEmpty(resp.runId))
                    {
                        item.PipelineId = Guid.Parse(resp.runId);
                        item.PipelineRunCreationTime = DateTime.Now;
                        _logger.LogDebug($"PipelineID: {item.PipelineId} - PipelineRunCreationTime: {item.PipelineRunCreationTime}");
                        await patientDataRepository.UpdateAsync(item);
                    }
                    else
                    {          
                        _logger.LogWarning($"Impossibile istanziare una pipeline con il seguente body: CPI={plParameters.CloudPatientId} - StartDate={plParameters.StartDate} - EndDate={plParameters.EndDate} - PatientId={plParameters.PatientId}");
                    }
                    
                }

                await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new SynapsePipelineCalledEvent()));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
                _logger.LogException(ex, LogLevel.Error);
                throw new Exception(ex.Message);
            }

        }

        public class CreatePipelineRunAsyncResponse
        {
            public string runId { get; set; }
        }
    }
}
