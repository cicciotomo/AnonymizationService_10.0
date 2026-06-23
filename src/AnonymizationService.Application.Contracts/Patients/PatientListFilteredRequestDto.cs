using AnonymizationService.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Patients
{
    public class  PatientListFilteredRequestDto : PagedAndSortedResultRequestDto
    {
        public DateTime? CreationTime { get; set; }
        public DateTime? StartCreationTime { get; set; }
        public DateTime? EndCreationTime { get; set; }
        public string? MasterPatientIndex { get; set; }
        public bool? ClinicalStateMachineShouldStart { get; set; }
        public bool? DicomStateMachineShouldStart { get; set; }
        public bool? LaboratoryStateMachineShouldStart { get; set; }
        public bool? DbUriStateMachineShouldStart { get; set; }
        public bool? PathoxStateMachineShouldStart { get; set; }
        public bool? IntensiveCareStateMachineShouldStart { get; set; }
        public bool? RedcapStateMachineShouldStart { get; set; }

    }
}
