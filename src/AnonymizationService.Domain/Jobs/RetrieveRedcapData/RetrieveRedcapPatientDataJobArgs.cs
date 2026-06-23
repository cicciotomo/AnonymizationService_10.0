using Porini.Abp.StateMachineEngine.Jobs;
using System;

namespace AnonymizationService.Jobs.RetrieveRedcapData
{
    internal class RetrieveRedcapPatientDataJobArgs : JobArgs
    {
        public string MasterPatientIndex { get; set; }
        public DateTime StartDate { get; set; }
        public Guid CloudPatientId { get; set; }
        public Guid RedCapStudyId { get; set; }
        public string RedCapRecordId { get; set; }

    }
}
