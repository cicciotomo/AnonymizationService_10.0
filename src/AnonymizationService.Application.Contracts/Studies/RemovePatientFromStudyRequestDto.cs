using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Studies
{
    public class RemovePatientFromStudyRequestDto
    {
        public Guid StudyId { get; set; }

        public string MasterPatientId { get; set; }
    }
}
