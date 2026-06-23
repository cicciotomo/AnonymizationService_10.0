namespace AnonymizationService.Services.Galileo
{
    public class GalileoSettings
    {
        public const string SettingsName = "Settings:GalileoSettings";
        public string PeopleWsBaseUrl { get; set; }
        public string AuthTokenBaseUrl { get; set; }
        public string AuthTokenApi { get; set; }
        public string DocumentsDownloadBaseUrl { get; set; }
        public string DocumentsListDownloadApi { get; set; }
        public string DocumentDownloadApi { get; set; }
        public string DocumentsDownloadStartDate { get; set; }
        public string LabResDownloadBaseUrl { get; set; }
        public string LabResDownloadAction { get; set; }
        public string Application { get; set; }
        public string User { get; set; }
        public string StoragePath { get; set; }
    }
}
