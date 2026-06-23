using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Redcap
{
    public class RedcapMetadataDto
    {
        public string Fields { get; set; }

        public RedcapMetadataDto() { }
        public RedcapMetadataDto(string Fields)
        {
            this.Fields = Fields;
        }
    }
}
