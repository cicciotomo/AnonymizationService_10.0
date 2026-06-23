using AnonymizationService.HospitalPatients;
using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using AnonymizationService.LaboratoryExams;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Laboratory.States
{
    public class UpdateCloudUploadDateForLaboratoryExamsState : StatefulState<List<FhirLaboratoryDataDto>>
    {
        public UpdateCloudUploadDateForLaboratoryExamsState() { }
        public UpdateCloudUploadDateForLaboratoryExamsState(List<FhirLaboratoryDataDto> fhirLaboratoryDataDtos)
        {
            this.StateData = fhirLaboratoryDataDtos;
        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not LaboratoryStateMachine laboratoryStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            if (this.StateData.Count > 0)
            {
                var cloudUploadDate = DateTime.Now;
                var laboratoryExamRepository = serviceProvider.GetService<IRepository<LaboratoryExam>>();
                var laboratoryExamIds = this.StateData.Select(x => x.Id).ToList();
                var existLaboratoryExams = await laboratoryExamRepository.GetListAsync(x => laboratoryExamIds.Contains(x.Id));

                existLaboratoryExams.ForEach(x => x.SetCloudUploadDate(cloudUploadDate));

                await laboratoryExamRepository.UpdateManyAsync(existLaboratoryExams);

                var patientService = serviceProvider.GetService<HospitalPatientService>();
                await patientService.UpdateLaboratoryLastUpdateDateForPatientAsync(laboratoryStateMachine.ContextData.CloudPatientId, cloudUploadDate);
            }

            await PublishEventOnBusAsync(new ExecutedTaskEvent(laboratoryStateMachine.Id, new LaboratoryExamsCheckUpdateCompletedEvent()));
        }
    }
}
