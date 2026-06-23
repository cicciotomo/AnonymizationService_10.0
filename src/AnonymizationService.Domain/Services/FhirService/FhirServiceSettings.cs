namespace AnonymizationService.Services.FhirService
{
    public class FhirServiceSettings
    {
        public const string SettingsName = "Settings:FhirServiceSettings";
        public string FhirServiceUrl { get; set; }
        public string FhirOrganizationIdentifier { get; set; }
        public string FhirRootOrganizationName { get; set; }
    }
}
