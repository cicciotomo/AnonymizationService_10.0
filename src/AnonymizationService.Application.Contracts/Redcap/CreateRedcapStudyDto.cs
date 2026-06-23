using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Redcap
{
    public class CreateRedcapStudyDto
    {
        public string Name { get; set; }
        public string Modality { get; set; }
        public string Endpoint { get; set; }
        public string RedcapToken { get; set; }
        public string MpiColumnName { get; set; }

        public CreateRedcapStudyDto(string name, string endpoint, string redcapToken, string modality, string mpiColumnName)
        {
            Name = name;
            Endpoint = endpoint;
            RedcapToken = redcapToken;
            Modality = modality;
            MpiColumnName = mpiColumnName;
        }

        public CreateRedcapStudyDto() { }
    }
}
