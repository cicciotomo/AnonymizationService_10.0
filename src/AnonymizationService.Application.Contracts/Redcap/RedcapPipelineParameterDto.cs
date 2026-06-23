using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Redcap
{
    [Serializable]
    public class RedcapPipelineParameterDto
    {
        public Guid TransmissionId { get; set; }
        
    }
}
