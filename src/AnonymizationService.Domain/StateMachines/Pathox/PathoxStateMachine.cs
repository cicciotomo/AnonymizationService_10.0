using AnonymizationService.Services.Pathox;
using AnonymizationService.StateMachines.Pathox.States;
using Hl7.Fhir.Model;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using Event = Porini.Abp.StateMachineEngine.Events.Event;

namespace AnonymizationService.StateMachines.Pathox
{
	public class PathoxStateMachine : ContextStateMachine<PathoxStateMachineContext>
	{
		public override StateMachineEvent StateMachineCompletedEventType => new PathoxDataStateMachineCompletedEvent();
		private PathoxStateMachine() { }

		public PathoxStateMachine(Guid cloudPatientId, Guid? parentStateMachineId, string FhirPatientId, string MasterPatientIndex, DateTime startDate)
		{
			ParentStateMachineId = parentStateMachineId;
			ContextData = new PathoxStateMachineContext
			{
				CloudPatientId = cloudPatientId,
				FhirPatientId = FhirPatientId,
				MasterPatientIndex = MasterPatientIndex,
				StartDate = startDate
			};

			Initialize();
		}

		public override State Initialize()
		{
			CurrentState = new RetrievePathoxDataState();
			SetCurrentStateValues();
			return CurrentState;
		}

		protected override State ReactToEvent(Event receivedEvent)
		{
			switch (CurrentState, receivedEvent)
			{
				case (RetrievePathoxDataState, PathoxDataRetrievedEvent pathoxDataRetrievedEvent):
					if (pathoxDataRetrievedEvent.PathoxExamResults.Count > 0)
					{
						ContextData.PathoxExams = pathoxDataRetrievedEvent.PathoxExamResults;
						CurrentState = new PersistPathoxDataState();
					}
					else
					{
						CurrentState = new UpdateCloudUploadDateForPathoxDataState();
					}
					break;
				case (PersistPathoxDataState, PersistedPathoxDataStateEvent persistedPathoxStateEvent):
					CurrentState = new UploadPathoxDataToFhirState();
					break;
				case (UploadPathoxDataToFhirState, PathoxDataUploadToFhirCompletedEvent uploadCompletedEvent):
					CurrentState = new UpdateCloudUploadDateForPathoxDataState(uploadCompletedEvent.UploadedPathoxExamIds);
					break;
				case (UpdateCloudUploadDateForPathoxDataState, UpdatedCloudUploadDateForPathoxDataEvent):
					SetCompleted();
					break;
			}
			SetCurrentStateValues();
			return CurrentState;
		}

	}
	public class PathoxStateMachineContext
	{
		public Guid CloudPatientId { get; set; }
		public string FhirPatientId { get; set; }
		public string MasterPatientIndex { get; set; }
		public DateTime StartDate { get; set; }
		public List<PathoxExamResult> PathoxExams { get; set; }
	}

	public class PathoxDataStateMachineCompletedEvent : StateMachineEvent { }
}
