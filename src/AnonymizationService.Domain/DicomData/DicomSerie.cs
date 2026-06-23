using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.DicomData
{
    public class DicomSerie : Entity<string>
    {

        public string DicomStudyId { get; protected set; }
        public string Modality { get; protected set; }
        public DateTime? CloudUploadDate { get; protected set; }

        private DicomSerie()
        {
        }

        public DicomSerie(string id, string dicomStudyId, string modality) : base(id)
        {
            DicomStudyId = dicomStudyId;
            Modality = modality;
        }

        public void SetCloudUploadDate(DateTime uploadDate)
        {
            CloudUploadDate = uploadDate;
        }
    }
}
