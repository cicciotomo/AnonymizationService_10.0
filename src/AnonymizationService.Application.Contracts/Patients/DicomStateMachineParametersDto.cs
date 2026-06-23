using System;

namespace AnonymizationService.StateMachines.Dicom
{
    public class DicomParametersDto
    {
        public bool ShouldStart { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string[] DicomModalities { get; set; }
        public string DicomPacsSource { get; set; }
        public DicomParametersJsonDto DicomParametersJson { get; set; }

    }

    public class DicomParametersJsonDto
    {
        public Boolean MoveToPacsRicerca { get; set; }
        public string StudyDescription { get; set; }
    }
}
