using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.ClinicalDocuments
{
    public class ClinicalDocument : Entity<int>
    {
        public ClinicalDocument(int id, string name, string description, DateTime creationDate, Guid cloudPatientId) : base(id)
        {
            Name = name;
            Description = description;
            CreationDate = creationDate;
            CloudPatientId = cloudPatientId;
        }

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreationDate { get; protected set; }
        public Guid CloudPatientId { get; protected set; }
        public DateTime? CloudUploadDate { get; protected set; }

        public void SetCloudUploadDate(DateTime uploadDate)
        {
            CloudUploadDate = uploadDate;
        }
    }

}
