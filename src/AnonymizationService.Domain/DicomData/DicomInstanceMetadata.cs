using FellowOakDicom;
using System;

namespace AnonymizationService.DicomData;

public class DicomInstanceMetadata
{
    public Guid PatientId { get; set; }
    public string StudyUid { get; set; }
    public string SerieUid { get; set; }
    public string SopInstanceUid { get; set; }
    public DicomFile DicomFile { get; set; }
}