using AnonymizationService.Services.Pathox;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.UpsertFhirPathoxData
{
	internal class UpsertFhirPathoxDataArgs : JobArgs
	{
		public string FhirPatientId { get; set; }
		public Guid CloudPatientId { get; set; }
		public List<PathoxExamResult> PathoxExamResults { get; set; }
	}
}
