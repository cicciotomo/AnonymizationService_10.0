using AnonymizationService.LaboratoryExams;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Laboratory.States
{
    internal class PersistLaboratoryExamsState : State
    {
        public PersistLaboratoryExamsState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LaboratoryStateMachine laboratoryStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            if (laboratoryStateMachine.ContextData.LaboratoryExamsResultList?.Count > 0)
            {
                var laboratoryExamRepository = serviceProvider.GetService<IRepository<LaboratoryExam>>();

                var existingExams = await laboratoryExamRepository
                    .GetListAsync(e => e.CloudPatientId == laboratoryStateMachine.ContextData.CloudPatientId);

                var laboratoryExams = laboratoryStateMachine.ContextData.LaboratoryExamsResultList.ExceptBy(existingExams.Select(e => e.Id), e => e.Id);

                await laboratoryExamRepository.InsertManyAsync(laboratoryExams);
            }

            await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new LaboratoryExamsPersistedEvent()));
        }
    }
}