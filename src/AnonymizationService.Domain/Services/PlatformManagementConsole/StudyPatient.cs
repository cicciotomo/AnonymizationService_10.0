using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.PlatformManagementConsole
{
    public class StudyPatient
    {
        public string StudyId { get; set; }

        public string MasterPatientIndex { get; set; }

        public string CloudPatientIndex { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsFromCohortBuilder { get; set; }
    }
}
