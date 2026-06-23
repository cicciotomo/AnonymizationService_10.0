using AnonymizationService.IntensiveCareData;
using AnonymizationService.Jobs.UpsertFhirIntensiveCarePatientData;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.IntensiveCare.States
{
    public class UploadIntensiveCareDataToFhirState : State
    {
        

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var jobScheduler = serviceProvider.GetService<IJobScheduler>();

            if (stateMachine is not IntensiveCareStateMachine intensiveCareStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for SendIntensiveCareEncounterToFHIRState", stateMachine.Id));
            }

            var intensiveCareRepository = serviceProvider.GetService<IRepository<IntensiveCarePatientData>>();
            var patientData = await intensiveCareRepository.GetListAsync(p => 
                p.CloudPatientId== intensiveCareStateMachine.ContextData.CloudPatientId
                && p.CloudUploadDate == null
                && p.ExamStartDate > intensiveCareStateMachine.ContextData.StartDate
                );

            foreach (var patient in patientData) {
                var s = await intensiveCareRepository.GetListAsync(p =>
                    p.CloudPatientId == patient.CloudPatientId
                    && p.PatientId == patient.PatientId
                    && p.NosologicalCode == patient.NosologicalCode
                    && p.ExamStartDate == patient.ExamStartDate
                    && p.ExamEndDate == patient.ExamEndDate
                    && p.CloudUploadDate.HasValue
                    );
                if (s.Any()) { 
                    patient.CloudUploadDate= s.FirstOrDefault().CloudUploadDate;
                }
            }

            await jobScheduler.EnqueueJob<UpsertFhirIntensiveCareDataJob, UpsertFhirIntensiveCareDataJobArgs, JobResult>(new UpsertFhirIntensiveCareDataJobArgs()
            { 
                StateMachineId= stateMachine.Id,
                CloudPatientId = intensiveCareStateMachine.ContextData.CloudPatientId,
                PatientData = patientData,
                FhirPatientId = intensiveCareStateMachine.ContextData.FhirPatientId
            });
        }
    }
}
