using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using AnonymizationService.LaboratoryExams;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace AnonymizationService.StateMachines.Laboratory.States
{
    public class UploadDataToFhirServiceState : State
    {
        public UploadDataToFhirServiceState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LaboratoryStateMachine laboratoryDataStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var examRepository = serviceProvider.GetRequiredService<IRepository<LaboratoryExam>>();

            var alreadyUploadedExams = await examRepository.GetListAsync(e =>
                e.CloudPatientId == laboratoryDataStateMachine.ContextData.CloudPatientId &&
                e.CloudUploadDate != null,
                includeDetails: true
            );

            var examsToUpload = laboratoryDataStateMachine.ContextData.LaboratoryExamsResultList.ExceptBy(
                alreadyUploadedExams.Select(alreadyUploadedExam => alreadyUploadedExam.Id),
                e => e.Id).ToList();

            var objectMapper = serviceProvider.GetRequiredService<IObjectMapper>();

            var examsToUploadDtos = objectMapper.Map<List<LaboratoryExam>, List<FhirLaboratoryDataDto>>(examsToUpload);

            await JobScheduler.EnqueueJob<UpsertFhirLaboratoryDataJob, UpsertFhirLaboratoryDataArgs, JobResult>(new UpsertFhirLaboratoryDataArgs
            {
                FhirPatientId = laboratoryDataStateMachine.ContextData.FhirPatientId,
                Exams = examsToUploadDtos,
                StateMachineId = laboratoryDataStateMachine.Id
            });
        }
    }
}
