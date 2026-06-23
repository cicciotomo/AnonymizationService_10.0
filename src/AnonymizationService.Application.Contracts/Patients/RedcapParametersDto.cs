using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Patients
{
    public class RedcapParametersDto
    {
        public bool ShouldStart { get; set; }
        public DateTime? StartTime { get; set; }
        public string RedcapStudyConfigurationId { get; set; }  //*****CUSTOM PROPERTIES
        public string RecordId { get; set; }  //*****CUSTOM PROPERTIES
    }
}
