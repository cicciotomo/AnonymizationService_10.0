using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnonymizationService.IntensiveCareData
{
    [NotMapped]
    public class IntensiveCarePatientStringAttribute
    {
        public string PatientId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
