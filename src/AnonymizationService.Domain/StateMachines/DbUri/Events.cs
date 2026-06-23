using AnonymizationService.Services.DbUri;
using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.DbUri
{
	public class DbUriDataRetrievedEvent : JobEvent
	{
		public DbUriDataRetrievedEvent(DbUriPatientData dbUriPatientData)
		{
			this.dbUriPatientData = dbUriPatientData;
		}

		public DbUriPatientData dbUriPatientData { get; set; }
	}
	public class PersistedDbUriDataStateEvent : JobEvent
	{
		public PersistedDbUriDataStateEvent() { }

		public DbUriPatientData dbUriPatientData { get; set; }
	}

	public class UpdatedCloudUploadDateForDbUriDataEvent : JobEvent { }

	public class DbUriDataUploadToFhirCompletedEvent : JobEvent
	{
		public DbUriDataUploadToFhirCompletedEvent(List<long> dbUriEventUploadedIds, List<long> dbUriFUpItemUploadedIds)
		{			
			DbUriEventUploadedIds = dbUriEventUploadedIds;
			DbUriFUpItemUploadedIds = dbUriFUpItemUploadedIds;
		}

		public List<long> DbUriEventUploadedIds { get; set; }
		public List<long> DbUriFUpItemUploadedIds { get; set; }
	}
}
