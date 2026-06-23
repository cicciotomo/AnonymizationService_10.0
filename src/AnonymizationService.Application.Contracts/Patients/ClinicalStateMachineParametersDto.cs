using AnonymizationService.ClinicalDocumentTypes;
using System;

namespace AnonymizationService.StateMachines.Clinical
{
    public class ClinicalParametersDto
    {
        public bool ShouldStart { get; set; }
        public DateTime? StartTime { get; set; }
        public ClinicalDocumentTypeFilterDto[] DocumentType { get; set; }
    }

    //public class ClinicalDocumentTypeFilterDto
    //{ 
    //    public string Type { get; set; }
    //    public string Filler { get; set; }
    //}
}
