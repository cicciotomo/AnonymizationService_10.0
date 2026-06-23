using Porini.Abp.StateMachineEngine.Jobs;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.UpsertFhirLaboratoryData
{
    internal class UpsertFhirLaboratoryDataArgs : JobArgs
    {
        public string FhirPatientId { get; set; }
        public List<FhirLaboratoryDataDto> Exams { get; set; }
    }
}
