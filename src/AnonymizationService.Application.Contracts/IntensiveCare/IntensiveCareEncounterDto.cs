using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.IntensiveCare
{
    [Serializable]
    public class IntensiveCareEncounterDto
    {
        public string TerapiaIntensivaPatientId { get; set; }
        public DateTime ExamStartDate { get; set; }
        public DateTime? ExamEndDate { get; set; }
    }
}
