using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.IntensiveCare
{
    [Serializable]
    public class PipelineParameterDto
    {
        public Guid PatientId { get; set; }
        public Guid CloudPatientId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
