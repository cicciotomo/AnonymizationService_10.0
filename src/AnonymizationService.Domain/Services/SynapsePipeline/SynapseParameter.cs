using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.SynapsePipeline
{
    public class SynapseParameter
    {
        public Guid PatientId { get; set; }
        public string MasterPatientIndex { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
