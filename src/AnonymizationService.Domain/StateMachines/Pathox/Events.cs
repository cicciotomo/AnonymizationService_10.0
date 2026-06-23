using AnonymizationService.Services.DbUri;
using AnonymizationService.Services.Pathox;
using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.StateMachines.Pathox
{
	public class PathoxDataRetrievedEvent : JobEvent
	{
		public PathoxDataRetrievedEvent(List<PathoxExamResult> pathoxExamResults)
		{
			PathoxExamResults = pathoxExamResults;
		}
		public List<PathoxExamResult> PathoxExamResults { get; set; }
	}
	public class PersistedPathoxDataStateEvent : JobEvent	{	}

	public class UpdatedCloudUploadDateForPathoxDataEvent : JobEvent { }

	public class PathoxDataUploadToFhirCompletedEvent : JobEvent
	{
		public List<string> UploadedPathoxExamIds { get; set; }
	}

}
