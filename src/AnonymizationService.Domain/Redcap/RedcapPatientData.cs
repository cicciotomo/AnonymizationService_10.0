using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.Redcap
{
    public class  RedcapPatientData : Entity<Guid>
    {
        public Guid RedcapStudyId { get; protected set; }
        public string RedcapRecordId { get; protected set; }
        public Guid CloudPatientId { get; protected set; }
        public DateTime? CloudUploadDate { get; protected set; }

        public RedcapPatientData(Guid id, Guid redcapStudyId, string redcapRecordId, Guid cloudPatientId) : base(id)
        {
            RedcapStudyId = redcapStudyId;
            RedcapRecordId = redcapRecordId;
            CloudPatientId = cloudPatientId;
        }

        private RedcapPatientData() { }

        public void SetCloudUploadDate(DateTime uploadDate)
        {
            CloudUploadDate = uploadDate;
        }
    }
}
