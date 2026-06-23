using AnonymizationService.DbUriData;
using AnonymizationService.HospitalPatients;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.Redcap;
using AnonymizationService.StateMachines.DbUri;
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

namespace AnonymizationService.StateMachines.Redcap.States
{
    internal class UpdateRedcapPatientDataUploadDateState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not RedcapStateMachine redcapStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for UpdateRedcapPatientDataUploadDateState", stateMachine.Id));
            }
            try
            {
                var cloudUploadDate = DateTime.Now;

                #region aggiorna RedcapPatientData
                
                var repo = serviceProvider.GetService<IRepository<RedcapPatientData>>();
                var record = await repo.GetAsync(e =>
                        e.CloudPatientId == redcapStateMachine.ContextData.CloudPatientId
                     && e.RedcapStudyId.ToString() == redcapStateMachine.ContextData.RedcapStudyDefinition);
                record.SetCloudUploadDate(cloudUploadDate);
                await repo.UpdateAsync(record);

                #endregion
                #region aggiorna paziente
                
                var patientService = serviceProvider.GetService<HospitalPatientService>();
                await patientService.UpdateRedCapLastUpdateDateForPatientAsync(redcapStateMachine.ContextData.CloudPatientId, cloudUploadDate);
                
                #endregion

                await PublishEventOnBusAsync(new ExecutedTaskEvent(redcapStateMachine.Id, new RedcapPatientDataUpdateCompletedEvent()));

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("[{0}] Error in UpdateRedcapPatientDataUploadDateState {1}", stateMachine.Id, ex.Message), ex);
            }

        }
    }
}
