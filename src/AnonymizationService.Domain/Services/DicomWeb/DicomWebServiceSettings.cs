namespace AnonymizationService.Services.DicomWeb
{
    public class DicomWebServiceSettings
    {
        public const string SettingsName = "Settings:DicomWebServiceSettings";
        public string DicomServiceUrl { get; set; }
        public string DicomServiceIdentityScope { get; set; }
    }
}
