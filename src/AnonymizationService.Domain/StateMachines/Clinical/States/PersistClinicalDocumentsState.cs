using AnonymizationService.ClinicalDocuments;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.Clinical.States
{
    public class PersistClinicalDocumentsState : State
    {
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not ClinicalStateMachine clinicalStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var retrievedClinicalDocuments = clinicalStateMachine.ContextData.ClinicalDocuments;

            if (retrievedClinicalDocuments.Count > 0)
            {
                var clinicalDocumentsRepository = serviceProvider.GetService<IRepository<ClinicalDocument>>();

                var existingDocuments = await clinicalDocumentsRepository
                    .GetListAsync(e => e.CloudPatientId == clinicalStateMachine.ContextData.CloudPatientId);

                var clinicalDocuments = retrievedClinicalDocuments
                    .Select(e => new ClinicalDocument(
                        id: e.Id,
                        name: e.Name,
                        description: e.Description,
                        creationDate: e.CreationDate,
                        cloudPatientId: clinicalStateMachine.ContextData.CloudPatientId
                    )).ToList();

                clinicalDocuments = clinicalDocuments.ExceptBy(existingDocuments.Select(e => e.Id), e => e.Id).ToList();

                await clinicalDocumentsRepository.InsertManyAsync(clinicalDocuments);
            }

            await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new ClinicalDocumentsPersistedEvent(retrievedClinicalDocuments)));
        }
    }
}
