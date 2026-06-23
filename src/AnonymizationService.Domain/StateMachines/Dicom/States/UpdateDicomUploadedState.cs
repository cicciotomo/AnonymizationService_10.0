using AnonymizationService.HospitalPatients;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Dicom.States
{
    internal class UpdateDicomUploadedState : State
    {
        public UpdateDicomUploadedState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not DicomStateMachine dicomStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var patientService = serviceProvider.GetService<HospitalPatientService>();
            await patientService.UpdateDicomLastUpdateDateForPatientAsync(dicomStateMachine.ContextData.CloudPatientId, DateTime.Now);

            await PublishEventOnBusAsync(new ExecutedTaskEvent(dicomStateMachine.Id, new DicomDataCheckUpdateCompletedEvent()));
        }
    }
}
