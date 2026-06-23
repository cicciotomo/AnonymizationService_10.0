using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Studies
{
    public class PatientsListToAddToStudyRequestDto
    {
        public Guid StudyFhirId { get; set; }

        public List<string> MasterPatientIds { get; set; }
    }
}
