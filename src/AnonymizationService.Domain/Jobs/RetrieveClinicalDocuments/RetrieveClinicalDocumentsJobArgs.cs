using AnonymizationService.ClinicalDocuments;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.RetrieveClinicalDocuments
{
    internal class RetrieveClinicalDocumentsJobArgs : JobArgs
    {
        public string MasterPatientIndex { get; set; }
        public DateTime StartDate { get; set; }
        public ClinicalDocumentTypeFilter[]  DocumentType { get; set; }
        public Guid CloudPatientId { get; set; }
    }
}
