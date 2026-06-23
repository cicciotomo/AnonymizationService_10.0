using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.IntensiveCareData
{
    [NotMapped]
    public class IntensiveCareEncounter : Entity
    {
        public string TerapiaIntensivaPatientId { get; set; }
        public DateTime ExamStartDate { get; set; }
        public DateTime? ExamEndDate { get; set; }

        public IntensiveCareEncounter(string terapiaIntensivaPatientId, DateTime examStartDate, DateTime? examEndDate)
        { 
            TerapiaIntensivaPatientId = terapiaIntensivaPatientId;
            ExamStartDate = examStartDate;
            ExamEndDate = examEndDate;
        }

        public override object[] GetKeys()
        {
            throw new NotImplementedException();
        }
    }
}
