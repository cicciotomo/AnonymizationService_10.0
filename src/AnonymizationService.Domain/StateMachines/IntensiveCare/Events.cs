using AnonymizationService.IntensiveCareData;
using AnonymizationService.PipelineExecutor;
using Hl7.Fhir.Model;
using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnonymizationService.StateMachines.IntensiveCare
{
    // Job Events
    public class IntensiveCareDataCheckUpdateCompletedEvent : JobEvent { }

    public class IntensiveCareDocumentsPersistedEvent : JobEvent
    {
        public List<IntensiveCareDocumentSummary> Documents { get; set; }


        public IntensiveCareDocumentsPersistedEvent(List<IntensiveCareDocumentSummary> documents)
        {
            Documents = documents ?? new List<IntensiveCareDocumentSummary>();
        }
    }

    public class IntensiveCareDocumentsRetrievedEvent : JobEvent
    {
        public List<IntensiveCareDocumentSummary> Documents { get; set; }

        public IntensiveCareDocumentsRetrievedEvent(List<IntensiveCareDocumentSummary> documents)
        {
            Documents = documents ?? new List<IntensiveCareDocumentSummary>();
        }
    }

    public class IntensiveCareDataSentToFHIREvent : JobEvent
    {
        public List<IntensiveCarePatientData> IntensiveCarePatientDatas { get; set; }

        public IntensiveCareDataSentToFHIREvent(List<IntensiveCarePatientData> intensiveCarePatientDatas)
        {
            IntensiveCarePatientDatas = intensiveCarePatientDatas ?? new List<IntensiveCarePatientData>();
        }
    }

    public class SynapsePipelineCalledEvent : JobEvent { }

  

    // State Machine Events
    public class IntensiveCareDataStateMachineCompletedEvent : StateMachineEvent { }

    public class IntensiveCareDocumentSummary : IntensiveCarePatientStringAttribute
    {
        public IEnumerable<IntensiveCareEncounter> Encounters { get; set; }
    }


}
