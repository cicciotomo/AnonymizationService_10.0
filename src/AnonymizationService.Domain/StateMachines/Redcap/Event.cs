
using AnonymizationService.PipelineExecutor;
using DemographicWS;
using Hl7.Fhir.Model;
using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnonymizationService.StateMachines.Redcap
{
    public class RedcapPatientDataSummary
    {
        public Guid RedcapStudy { get; set; }
        public string RedcapRecordId { get; set; }
        public string JsonResponseString { get; set; }
        public string MetadataFhirId { get; set; }
        public string Metadata { get; set; }
        public string Study_name {  get; set; }
        public string Token { get; set; }
        /*
                public int EncounterId { get; set; }
                public DateTime EncounterStartDate { get; set; }
                public DateTime EncounterEndDate { get; set; }
        */
    }

    #region Job Events
    public class RedcapPatientDataRetrievedEvent : JobEvent
    {
        public RedcapPatientDataSummary Data { get; set; }

        public RedcapPatientDataRetrievedEvent(RedcapPatientDataSummary data)
        {
            Data = data ?? new RedcapPatientDataSummary();
        }
    }

    public class RedcapPatientDataPersistedEvent : JobEvent
    {
        public bool ItemAlreadyExistent { get; set; }

        public RedcapPatientDataPersistedEvent(bool data)
        {
            ItemAlreadyExistent = data;
        }
    }

    public class RedcapPatientDataSentEvent : JobEvent
    {
        public Guid EncounterIdentifier { get; set; }

        public RedcapPatientDataSentEvent(Guid encounterIdentifier)
        {
            EncounterIdentifier = encounterIdentifier;
        }
    }

    public class RedcapStudyMetadataSentEvent : JobEvent
    {
        public Guid MetadataIdentifier { get; set; }

        public RedcapStudyMetadataSentEvent(Guid metadataIdentifier)
        {
            MetadataIdentifier = metadataIdentifier;
        }
    }

    public class RedcapPatientDataUpdateCompletedEvent : JobEvent { }

    #endregion

    #region State Machine Events
    public class RedcapDataStateMachineCompletedEvent : StateMachineEvent { }

    #endregion



}
