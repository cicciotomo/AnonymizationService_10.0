using AnonymizationService.Services.DbUri;
using AnonymizationService.StateMachines.Clinical.States;
using AnonymizationService.StateMachines.DbUri.States;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.DbUri
{
	public class DbUriStateMachine : ContextStateMachine<UriStateMachineContext>
	{
		public override StateMachineEvent StateMachineCompletedEventType => new DbUriDataStateMachineCompletedEvent();

		private DbUriStateMachine() { }

		public DbUriStateMachine(Guid cloudPatientId, string taxCode, Guid? parentStateMachineId, string FhirPatientId, string MasterPatientIndex, DateTime startDate)
		{
			ParentStateMachineId = parentStateMachineId;
			ContextData = new UriStateMachineContext
			{
				CloudPatientId = cloudPatientId,
				TaxCode = taxCode,
				FhirPatientId = FhirPatientId,
				MasterPatientIndex = MasterPatientIndex,
				StartDate = startDate
			};

			Initialize();
		}

		public override State Initialize()
		{
			CurrentState = new RetrieveDbUriDataState();
			SetCurrentStateValues();
			return CurrentState;
		}

		protected override State ReactToEvent(Porini.Abp.StateMachineEngine.Events.Event receivedEvent)
		{
			switch (CurrentState, receivedEvent)
			{
				case (RetrieveDbUriDataState, DbUriDataRetrievedEvent uriDataRetrievedEvent):
					if (uriDataRetrievedEvent.dbUriPatientData is not null)
					{
						this.ContextData.DbUriPatientData = uriDataRetrievedEvent.dbUriPatientData;
						CurrentState = new PersistDbUriDataState();
					}
					else
					{
						CurrentState = new UpdateCloudUploadDateForDbUriDataState();
					}
					break;
				case (PersistDbUriDataState, PersistedDbUriDataStateEvent persistedUriDataStateEvent):
					CurrentState = new UploadDbUriDataToFhirState();
					break;
				case (UploadDbUriDataToFhirState, DbUriDataUploadToFhirCompletedEvent uploadCompletedEvent):
					CurrentState = new UpdateCloudUploadDateForDbUriDataState(uploadCompletedEvent.DbUriEventUploadedIds, uploadCompletedEvent.DbUriFUpItemUploadedIds);
					break;
				case (UpdateCloudUploadDateForDbUriDataState, UpdatedCloudUploadDateForDbUriDataEvent):
					SetCompleted();
					break;
			}
			SetCurrentStateValues();
			return CurrentState;
		}
	}
	public class UriStateMachineContext
	{
		public Guid CloudPatientId { get; set; }
		public string TaxCode { get; set; }
		public string FhirPatientId { get; set; }
		public string MasterPatientIndex { get; set; }
		public DateTime StartDate { get; set; }
		public DbUriPatientData DbUriPatientData { get; set; }

	}

	public class DbUriDataStateMachineCompletedEvent : StateMachineEvent { }
}
