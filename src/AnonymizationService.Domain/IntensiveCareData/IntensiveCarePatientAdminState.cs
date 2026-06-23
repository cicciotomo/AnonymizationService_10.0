using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnonymizationService.IntensiveCareData
{
    [NotMapped]
    public class IntensiveCarePatientAdminState
    {
        public string Id { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int AdmitState { get; set; }
    }
}
