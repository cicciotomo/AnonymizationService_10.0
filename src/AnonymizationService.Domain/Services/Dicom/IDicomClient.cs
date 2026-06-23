using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Services.Dicom
{
    public interface IDicomClient
    {
        public Task<List<DicomStudy>> RetrieveStudiesByPatientIdAndModalityAsync(string patientId, DateTime startTime, DateTime? endTime, string[] modalities, string pacsSource, string filterTag);
        public Task<List<DicomSerie>> RetrieveSeriesByStudyIdAsync(string studyId, string[] modalities, string pacsSource);
        public Task<bool> RequestMoveForSerieAsync(string studyId, string serieId, string pacsSource);
        public Task RequestMoveForStudyAsync(string studyId, string pacsSource);
    }
}
