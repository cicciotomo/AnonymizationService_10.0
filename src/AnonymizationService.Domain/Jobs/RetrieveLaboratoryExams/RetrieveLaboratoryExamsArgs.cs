using Porini.Abp.StateMachineEngine.Jobs;
using System;

namespace AnonymizationService.Jobs.RetrieveLaboratoryExams
{
    internal class RetrieveLaboratoryExamsArgs : JobArgs
    {
        public string MasterPatientIndex { get; set; }
        public DateTime StartDate { get; set; }
        public Guid CloudPatientId { get; set; }
    }
}
