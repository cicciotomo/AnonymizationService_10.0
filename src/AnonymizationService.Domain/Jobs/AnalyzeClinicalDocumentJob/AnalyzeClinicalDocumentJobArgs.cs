using AnonymizationService.StateMachines.Clinical;
using Porini.Abp.StateMachineEngine.Jobs;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.AnalyzeClinicalDocumentJob;

public class AnalyzeClinicalDocumentJobArgs : JobArgs
{
    public List<ClinicalDocumentSummary> Documents { get; set; }
}