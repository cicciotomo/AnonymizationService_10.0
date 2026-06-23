namespace AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter
{
    public class DocumentSection
    {
        public string SectionName { get; set; }
        public string SerializedFhirBundle { get; set; }
    }
}
