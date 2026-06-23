using System;

namespace AnonymizationService.Services.FhirService.Adapters.DbUri
{

    public class DbUriTransformationAdditionalInfo
    {
        public Guid CloudPatientId { get; set; }
        public string FhirPatientId { get; set; }
        public string UrologyOrganizationId { get; set; }
    }
}




