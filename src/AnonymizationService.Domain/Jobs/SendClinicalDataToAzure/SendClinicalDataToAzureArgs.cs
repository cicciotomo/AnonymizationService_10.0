using AnonymizationService.StateMachines.Clinical;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.SendClinicalDataToAzure
{
    internal class SendClinicalDataToAzureArgs : JobArgs
    {
        public string FhirPatientId { get; set; }
        public Guid CloudPatientId { get; set; }
        public List<ParsedClinicalDocument> SerializedAnalyticsResults { get; set; }
    }
}
