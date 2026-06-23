using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Studies
{
    public class StudyPatientDto 
    {
        public string StudyFhirId { get; set; }

        public string MasterPatientIndex { get; set; }

        public string CloudPatientIndex { get; set; }

        public bool IsDeleted { get; set; } 

        public bool IsFromCohortBuilder { get; set; } 
    }
}
