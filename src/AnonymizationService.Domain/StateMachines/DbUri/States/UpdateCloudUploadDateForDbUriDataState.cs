using AnonymizationService.DbUriData;
using AnonymizationService.HospitalPatients;
using AnonymizationService.StateMachines.Laboratory;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.DbUri.States
{
	internal class UpdateCloudUploadDateForDbUriDataState : StatefulState<UploadedToFhirDbUriData>
	{
		public UpdateCloudUploadDateForDbUriDataState() { }
		public UpdateCloudUploadDateForDbUriDataState(List<long> dbUriEventUploadedIds, List<long> dbUriFUpItemUploadedIds)
		{
			this.StateData = new UploadedToFhirDbUriData()
			{
				DbUriEventUploadedIds = dbUriEventUploadedIds,
				DbUriFUpItemUploadedIds = dbUriFUpItemUploadedIds
			};
		}

		protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
		{
			if (stateMachine is not DbUriStateMachine dbUrilStateMachine)
			{
				throw new Exception("Invalid state machine for the given state");
			}

			var cloudUploadDate = DateTime.Now;
						
			if (this.StateData.DbUriFUpItemUploadedIds != null)
			{
				var dbUriFUpItemsRepository = serviceProvider.GetService<IRepository<DbUriFUpItem>>();
				var uploadedDbUriFUpItems = await dbUriFUpItemsRepository.GetListAsync(e => e.CloudPatientId == dbUrilStateMachine.ContextData.CloudPatientId && this.StateData.DbUriFUpItemUploadedIds.Contains(e.Id));
				uploadedDbUriFUpItems.ForEach(x => x.SetCloudUploadDate(cloudUploadDate));

				await dbUriFUpItemsRepository.UpdateManyAsync(uploadedDbUriFUpItems);
			}
		
			if (this.StateData.DbUriEventUploadedIds != null)
			{

				var dbUriEventsRepository = serviceProvider.GetService<IRepository<DbUriEvent>>();
				var uploadedDbUriEvents = await dbUriEventsRepository.GetListAsync(e => e.CloudPatientId == dbUrilStateMachine.ContextData.CloudPatientId && this.StateData.DbUriEventUploadedIds.Contains(e.Id));
				uploadedDbUriEvents.ForEach(x => x.SetCloudUploadDate(cloudUploadDate));

				await dbUriEventsRepository.UpdateManyAsync(uploadedDbUriEvents);
			}

			var patientService = serviceProvider.GetService<HospitalPatientService>();
			await patientService.UpdateDbUriLastUpdateDateForPatientAsync(dbUrilStateMachine.ContextData.CloudPatientId, cloudUploadDate);

			await PublishEventOnBusAsync(new ExecutedTaskEvent(dbUrilStateMachine.Id, new UpdatedCloudUploadDateForDbUriDataEvent()));

		}
	}
	internal class UploadedToFhirDbUriData
	{
		public List<long> DbUriEventUploadedIds { get; set; }
		public List<long> DbUriFUpItemUploadedIds { get; set; }
	}
}
