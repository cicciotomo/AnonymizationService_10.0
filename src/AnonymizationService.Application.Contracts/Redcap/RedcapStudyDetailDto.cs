using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Redcap
{
    public class RedcapStudyDetailDto
    {
        public string Name { get; set; }
        public string Modality { get; set; }
        public string Endpoint { get; set; }
        public string RedcapToken { get; set; }
        public string MpiColumnName { get; set; }
        public string Fields { get; set; }

        public RedcapStudyDetailDto(string name, string endpoint, string redcapToken, string modality, string mpiColumnName, string fields)
        {
            Name = name;
            Endpoint = endpoint;
            RedcapToken = redcapToken;
            Modality = modality;
            MpiColumnName = mpiColumnName;
            Fields = fields;
        }

        public RedcapStudyDetailDto() { }
    }
}
