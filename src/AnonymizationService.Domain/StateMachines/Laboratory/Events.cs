using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using AnonymizationService.LaboratoryExams;
using Porini.Abp.StateMachineEngine.Events;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.Laboratory
{
    public class LaboratoryExamsRetrievedEvent : JobEvent
    {
        public List<LaboratoryExam> LaboratoryExamsRetrieved { get; }
        public LaboratoryExamsRetrievedEvent(List<LaboratoryExam> laboratoryExamsRetrieved)
        {
            LaboratoryExamsRetrieved = laboratoryExamsRetrieved;
        }
    }

    public class LaboratoryExamsPersistedEvent : JobEvent { }

    public class LaboratoryExamsCheckUpdateCompletedEvent : JobEvent { }
    public class LaboratoryExamsUploadToFhirCompletedEvent : JobEvent
    {
        public List<FhirLaboratoryDataDto> FhirLaboratoryDataDtos { get; }
        public LaboratoryExamsUploadToFhirCompletedEvent(List<FhirLaboratoryDataDto> fhirLaboratoryDataDtos)
        {
            FhirLaboratoryDataDtos = fhirLaboratoryDataDtos;
        }
    }

    public class LaboratoryExamsStateMachineCompletedEvent : StateMachineEvent { }
}
