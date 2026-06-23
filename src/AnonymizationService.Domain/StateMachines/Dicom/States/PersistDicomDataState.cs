using AnonymizationService.DicomData;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Dicom.States
{
    internal class PersistDicomDataState : State
    {
        public PersistDicomDataState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not DicomStateMachine dicomStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var dicomStudyRepository = serviceProvider.GetService<IDicomStudyRepository>();

            await dicomStudyRepository.UpsertDiffAsync(dicomStateMachine.ContextData.DicomStudies);

            await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new PersistDicomDataStateEvent()));
        }
    }
}
