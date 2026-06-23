using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.DicomData
{
    public class DicomStudy : Entity<string>
    {
        protected DicomStudy() { }
        public Guid CloudPatientId { get; private set; }
        public DateTime StudyDate { get; private set; }
        public string StudyFilePath { get; private set; }
        public List<DicomSerie> DicomSeries { get; private set; }

        public DicomStudy(string id, Guid cloudPatientId, DateTime studyDate, string studyFilePath) : base(id)
        {
            CloudPatientId = cloudPatientId;
            StudyDate = studyDate;
            DicomSeries = new List<DicomSerie>();
            StudyFilePath = studyFilePath;
        }

        public void AddDicomSerie(DicomSerie dicomSerie)
        {
            DicomSeries.Add(dicomSerie);
        }

        public void AddDicomSeries(List<DicomSerie> dicomSeries)
        {
            DicomSeries.AddRange(dicomSeries);
        }
        public void UpdateStudyFilePath(string studyFilePath)
        {
            StudyFilePath = studyFilePath;
        }
    }
}
