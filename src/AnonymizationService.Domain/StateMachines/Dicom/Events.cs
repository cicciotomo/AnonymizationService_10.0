using AnonymizationService.DicomData;
using Porini.Abp.StateMachineEngine.Events;
using System.Collections.Generic;

namespace AnonymizationService.StateMachines.Dicom
{
    public class DicomDataRetrievedEvent : JobEvent
    {
        public DicomDataRetrievedEvent(List<DicomStudy> dicomStudies)
        {
            DicomStudies = dicomStudies;
        }
        public List<DicomStudy> DicomStudies { get; }
    }

    public class DicomDataAnonymizedEvent : JobEvent
    {
        public DicomDataAnonymizedEvent(List<DicomStudy> dicomStudies)
        {
            DicomStudies = dicomStudies;
        }
        public List<DicomStudy> DicomStudies { get; }
    }

    public class DicomDataSentToAzureEvent : JobEvent { } 

    public class DicomDataCheckUpdateCompletedEvent : JobEvent { }
    public class PersistDicomDataStateEvent : JobEvent
    {
        public PersistDicomDataStateEvent() { }
    }

    public class DicomStateMachineCompletedEvent : StateMachineEvent { }

    public class DicomDataMovedToPacsRicercaEvent : JobEvent { }
}
