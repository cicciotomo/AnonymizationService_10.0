using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Redcap
{
    public class RedcapStudyDto: EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Modality { get; set; }
        public string Endpoint { get; set; }
        public string RedcapToken { get; set; }

        public RedcapStudyDto(string name, string endpoint, string redcapToken, string modality)
        {
            Name = name;
            Endpoint = endpoint;
            RedcapToken = redcapToken;
            Modality = modality;
        }

        public RedcapStudyDto() { }
    }
}
