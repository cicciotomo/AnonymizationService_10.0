using AnonymizationService.StateMachines.Clinical;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.UpsertFhirRedcapPatientData
{
    internal class UpsertFhirRedcapPatientDataArgs : JobArgs
    {
        public Guid RedcapStudyConfigurationID { get; set; }
        public string FhirPatientId { get; set; }
        public Guid CloudPatientId { get; set; }
        public string Data { get; set; }
        public string MetadataFhirId { get; set; }
        public string Metadata { get; set; }
        public string StudyName { get; set; }
        public string Token { get; set; }
    }
}
