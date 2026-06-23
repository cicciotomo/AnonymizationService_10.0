using AnonymizationService.HospitalPatients;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.LoadPatientData.States
{
    internal class UpdatePatientUploadedState : State
    {
        public UpdatePatientUploadedState()
        {

        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LoadPatientDataStateMachine patientStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var patientService = serviceProvider.GetService<HospitalPatientService>();
            await patientService.UpdateMedicalDataLastUpdateDateForPatientAsync(patientStateMachine.ContextData.CloudPatientId.Value, DateTime.Now);

            await PublishEventOnBusAsync(new ExecutedTaskEvent(patientStateMachine.Id, new PatientDataCheckUpdateCompletedEvent()));
        }
    }
}
