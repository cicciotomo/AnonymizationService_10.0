using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.IntensiveCare
{
    [Serializable]
    public class IntensiveCarePatientStringAttributeDto
    {
        public string PatientId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
