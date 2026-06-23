using AnonymizationService.ClinicalDocuments;
using AnonymizationService.HospitalPatients;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Clinical.States
{
    internal class UpdateClinicalUploadedState : StatefulState<List<int>>
    {
        public UpdateClinicalUploadedState() { }

        public UpdateClinicalUploadedState(List<int> clinicalDocumentsUploaded)
        {
            this.StateData = clinicalDocumentsUploaded ?? new List<int>();
        }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var jobScheduler = serviceProvider.GetService<IJobScheduler>();

            if (stateMachine is not ClinicalStateMachine clinicalStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            if (this.StateData.Count > 0)
            {
                var cloudUploadDate = DateTime.Now;
                var clinicalDocumentsRepository = serviceProvider.GetService<IRepository<ClinicalDocument>>();
                var existingClinicalDocuments = await clinicalDocumentsRepository.GetListAsync(x => this.StateData.Contains(x.Id));

                existingClinicalDocuments.ForEach(x => x.SetCloudUploadDate(cloudUploadDate));
                await clinicalDocumentsRepository.UpdateManyAsync(existingClinicalDocuments);

                var patientService = serviceProvider.GetService<HospitalPatientService>();
                await patientService.UpdateClinicalLastUpdateDateForPatientAsync(clinicalStateMachine.ContextData.CloudPatientId, cloudUploadDate);
            }

            await PublishEventOnBusAsync(new ExecutedTaskEvent(clinicalStateMachine.Id, new ClinicalDataCheckUpdateCompletedEvent()));
        }

    }
}
