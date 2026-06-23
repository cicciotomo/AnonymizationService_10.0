using AnonymizationService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class PatientFilerListRequestDto : PagedAndSortedResultRequestDto
    {
     
        public DateTime? CreationTime { get; set; }
        public Guid? CloudPatientId { get; set; }
        public string MasterPatientIndex { get; set; }
        public string FhirPatientId { get; set; }



    }
}
