using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Patients
{
    public class IntensiveCareParametersDto
    {
        public bool ShouldStart { get; set; }
        public DateTime? StartTime { get; set; }
        public string NosologicalCode { get; set; }
    }
}
