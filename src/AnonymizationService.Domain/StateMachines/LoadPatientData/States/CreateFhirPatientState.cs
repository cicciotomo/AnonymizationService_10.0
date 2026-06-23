using AnonymizationService.Jobs.CreateFhirPatient;
using Hl7.Fhir.Model;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.LoadPatientData.States
{
    internal class CreateFhirPatientState : State
    {
        public CreateFhirPatientState()
        {
        }

        protected override async System.Threading.Tasks.Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LoadPatientDataStateMachine patientDataStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            Guid CloudPatientId = patientDataStateMachine.ContextData.CloudPatientId!.Value;
            List<string> FhirStudyIds = string.IsNullOrEmpty(patientDataStateMachine.ContextData.FhirStudyId) ?
                    null : patientDataStateMachine.ContextData.FhirStudyId.Split(',').ToList();

            AdministrativeGender gender = AdministrativeGender.Unknown;
            if (patientDataStateMachine.ContextData != null && patientDataStateMachine.ContextData.Gender.StartsWith("M"))
            {
                gender = AdministrativeGender.Male;
            }
            else if (patientDataStateMachine.ContextData != null && patientDataStateMachine.ContextData.Gender.StartsWith("F"))
            {
                gender = AdministrativeGender.Female;
            }
            await JobScheduler.EnqueueJob<CreateFhirPatientJob, CreateFhirPatientArgs, JobResult>(new CreateFhirPatientArgs
            {
                CloudPatientId = CloudPatientId,
                StateMachineId = patientDataStateMachine.Id,
                FhirStudyIds = FhirStudyIds,
                BirthDate = patientDataStateMachine.ContextData.Birthdate,
                Gender = gender
            });
        }
    }
}
