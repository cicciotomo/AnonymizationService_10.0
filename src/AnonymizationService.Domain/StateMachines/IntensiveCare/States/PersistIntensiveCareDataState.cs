using AnonymizationService.ClinicalDocuments;
using AnonymizationService.IntensiveCareData;
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

namespace AnonymizationService.StateMachines.IntensiveCare.States
{
    public class PersistIntensiveCareDataState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not IntensiveCareStateMachine intensiveCareStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for PersistIntensiveCareDataState", stateMachine.Id));
            }

            var retrievedIntensiveCareData = intensiveCareStateMachine.ContextData.RetrievedData.ToList();

            if (retrievedIntensiveCareData.Any())
            {
                var intensiveCareRepository = serviceProvider.GetService<IRepository<IntensiveCarePatientData>>();

                var existingPatientData = await intensiveCareRepository.GetListAsync(e =>
                    e.CloudPatientId == intensiveCareStateMachine.ContextData.CloudPatientId
                    && !e.PipelineRunEndTime.HasValue && !e.PipelineRunResult.HasValue);

                var retrievedPatientData = retrievedIntensiveCareData.SelectMany(a => a.Encounters.Select(e =>
                    new IntensiveCarePatientData(
                        intensiveCareStateMachine.ContextData.CloudPatientId
                        , a.Value
                        , a.PatientId
                        , e.ExamStartDate
                        , e.ExamEndDate
                        , stateMachine.Id
                    )
                )).ToList();

                var retrievedItemToAdd = retrievedPatientData.ExceptBy(
                        existingPatientData.Select(e => new { e.PatientId, e.NosologicalCode, e.ExamStartDate, e.ExamEndDate }),
                        e => new { e.PatientId, e.NosologicalCode, e.ExamStartDate, e.ExamEndDate }
                    ).ToList();

                await intensiveCareRepository.InsertManyAsync(retrievedItemToAdd,true);

            }

            await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new IntensiveCareDocumentsPersistedEvent(retrievedIntensiveCareData)));
        }
    }
}
