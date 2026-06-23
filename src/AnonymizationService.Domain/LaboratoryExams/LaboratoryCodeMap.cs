using Volo.Abp.Domain.Entities.Auditing;

namespace AnonymizationService.LaboratoryExams
{
    public class LaboratoryCodeMap : AuditedEntity
    {
        public string SourceCode { get; set; }
        public string DestinationSystem { get; set; }
        public string DestinationCode { get; set; }

        public override object[] GetKeys()
        {
            return new[] { SourceCode, DestinationSystem };
        }
    }
}
