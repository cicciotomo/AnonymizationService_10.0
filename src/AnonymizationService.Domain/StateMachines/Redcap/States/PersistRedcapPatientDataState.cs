using AnonymizationService.LaboratoryExams;
using AnonymizationService.Redcap;
using AnonymizationService.Services.Galileo;
using AnonymizationService.StateMachines.Laboratory;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Redcap.States
{
    internal class PersistRedcapPatientDataState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not RedcapStateMachine redcapStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for PersistRedcapPatientDataState", stateMachine.Id));
            }
            if (redcapStateMachine.ContextData.Summary is null)
            {
                throw new Exception(string.Format("[{0}] Invalid call to PersistRedcapPatientDataState", stateMachine.Id));
            }
            try
            {
                // Creo un RedcapPatientData dal RedcapPatientDataSummary in contextData.Summary
                var item = new RedcapPatientData(
                      Guid.NewGuid(),
                      Guid.Parse(redcapStateMachine.ContextData.RedcapStudyDefinition),
                      redcapStateMachine.ContextData.Summary.RedcapRecordId,
                      redcapStateMachine.ContextData.CloudPatientId);
                // Se esistesse già un duplicato interrompere StateMachine
                var repo = serviceProvider.GetService<IRepository<RedcapPatientData>>();
                var existingRecord = await repo.FindAsync(e =>
                        e.CloudPatientId == redcapStateMachine.ContextData.CloudPatientId
                    && e.RedcapStudyId.ToString() == redcapStateMachine.ContextData.RedcapStudyDefinition &&
                    e.RedcapRecordId == redcapStateMachine.ContextData.Summary.RedcapRecordId
                    );
                /*
                 * 1)passare da lista a elemento singolo
                 * 
                 * 2)gestione
                 * CloudUploadDate not null -> non mandare
                 * CloudUploadDate null -> manda senza insert
                 * elemento null -> insert e manda
                 *                 
                 */
                if (existingRecord != null)
                {
                    if (existingRecord.CloudUploadDate == null)
                    {
                        await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id
                        , new RedcapPatientDataPersistedEvent(false)));

                    } else
                    {
                        await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id
                        , new RedcapPatientDataPersistedEvent(true)));
                    }
                }
                else
                {
                    // Altrimenti salvare il RedcapPatientData e prosegui con la SM
                    await repo.InsertAsync(item);
                    await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id
                        , new RedcapPatientDataPersistedEvent(false)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("[{0}] Error in PersistRedcapPatientDataState {1}", stateMachine.Id, ex.Message), ex);
            }

        }
    }
}
