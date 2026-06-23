using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.RetrieveIntensiveCare
{
    internal class RetrieveIntensiveCareJobArgs : JobArgs
    {
        public string MasterPatientIndex { get; set; }
        public DateTime StartDate { get; set; }
        public IEnumerable<string> NosologicalCodes { get; set; }
        public Guid CloudPatientId { get; set; }
    }
}
