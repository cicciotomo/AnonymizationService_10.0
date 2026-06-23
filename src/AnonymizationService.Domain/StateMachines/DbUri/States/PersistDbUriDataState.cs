using Porini.Abp.StateMachineEngine.States;
using Porini.Abp.StateMachineEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnonymizationService.ClinicalDocuments;
using AnonymizationService.StateMachines.Clinical;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine.Events;
using Volo.Abp.Domain.Repositories;
using AnonymizationService.Services.DbUri;
using AnonymizationService.DbUriData;
using static System.Net.WebRequestMethods;


namespace AnonymizationService.StateMachines.DbUri.States
{
    internal class PersistDbUriDataState : State
    {
        public PersistDbUriDataState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {

            if (stateMachine is not DbUriStateMachine dbUrilStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var retrievedDbUriPatientData = dbUrilStateMachine.ContextData.DbUriPatientData;

            if (retrievedDbUriPatientData is not null)
            {
                var dbUriFUpItemsRepository = serviceProvider.GetService<IRepository<DbUriFUpItem>>();

                var existingdbUriFUpItems = await dbUriFUpItemsRepository.GetListAsync(e => e.CloudPatientId == dbUrilStateMachine.ContextData.CloudPatientId);
                var existingdbUriFUpItemsIds = existingdbUriFUpItems.Select(i => i.Id).ToList();

                if (retrievedDbUriPatientData.fup != null)
                {
                    var dbUriFUpItemsToInsert = retrievedDbUriPatientData.fup.items
                                .Where(r => !existingdbUriFUpItemsIds.Contains(r.id))
                                .Select(r => new DbUriFUpItem(
                                    id: r.id,
                                    cloudPatientId: dbUrilStateMachine.ContextData.CloudPatientId))
                                .ToList();

                    await dbUriFUpItemsRepository.InsertManyAsync(dbUriFUpItemsToInsert);
                }


                var dbUriEventsRepository = serviceProvider.GetService<IRepository<DbUriEvent>>();
                var existingdbUriEvents = await dbUriEventsRepository.GetListAsync(e => e.CloudPatientId == dbUrilStateMachine.ContextData.CloudPatientId);
                var existingdbUriEventsIds = existingdbUriEvents.Select(i => i.Id).ToList();

                var dbUriEventsToInsert = retrievedDbUriPatientData.events
                                            .Where(r => !existingdbUriEventsIds.Contains(r.generalInfo.id))
                                            .Select(r => new DbUriEvent(
                                                id: r.generalInfo.id,
                                                cloudPatientId: dbUrilStateMachine.ContextData.CloudPatientId))
                                            .ToList();

                await dbUriEventsRepository.InsertManyAsync(dbUriEventsToInsert);

            }
            await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new PersistedDbUriDataStateEvent()));

        }
    }
}
