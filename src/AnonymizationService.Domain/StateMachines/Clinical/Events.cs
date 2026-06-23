using AnonymizationService.ClinicalDocuments;
using AnonymizationService.PipelineExecutor;
using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnonymizationService.StateMachines.Clinical
{
    // Job Events
    public class ClinicalDataSentToAzureEvent : JobEvent
    {
        public List<int> UploadedDocumentsId { get; set; }

        public ClinicalDataSentToAzureEvent(List<int> uploadedDocumentsId)
        {
            UploadedDocumentsId = uploadedDocumentsId;
        }
    }

    public class ClinicalDataCheckUpdateCompletedEvent : JobEvent { }


    public class ClinicalDocumentsPersistedEvent : JobEvent
    {
        public List<ClinicalDocumentSummary> Documents { get; set; }

        public ClinicalDocumentsPersistedEvent(List<ClinicalDocumentSummary> documents)
        {
            Documents = documents ?? new List<ClinicalDocumentSummary>();
        }
    }

    public class ClinicalDocumentsRetrievedEvent : JobEvent
    {
        public List<ClinicalDocumentSummary> Documents { get; set; }

        public ClinicalDocumentsRetrievedEvent(List<ClinicalDocumentSummary> documents)
        {
            Documents = documents ?? new List<ClinicalDocumentSummary>();
        }
    }

    public class DocumentsProcessedEvent : JobEvent
    {
        public List<ParsedClinicalDocument> ProcessResults { get; set; }

        public DocumentsProcessedEvent(List<ParsedClinicalDocument> serializedProcessResults)
        {
            ProcessResults = serializedProcessResults;
        }
    }

    // State Machine Events
    public class ClinicalDataStateMachineCompletedEvent : StateMachineEvent { }

    public class ClinicalDocumentSummary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public string Base64FileContent { get; set; }
        public int EncounterId { get; set; }
        public DateTime EncounterStartDate { get; set; }
        public DateTime EncounterEndDate { get; set; }
        public string IssuingDepartment { get; set; }
        public ClinicalDocumentType DocumentType { get; set; }
    }

    public class ParsedClinicalDocument
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int EncounterId { get; set; }
        public DateTime EncounterStartDate { get; set; }
        public DateTime EncounterEndDate { get; set; }
        public string IssuingDepartment { get; set; }
        public List<PipelineExecutionResult> Result { get; set; } = new List<PipelineExecutionResult>();
        public bool Success { get { return Result.Any(x => x.Success); } }
        public ClinicalDocumentType DocumentType { get; set; }
    }
}
