namespace AnonymizationService.Services.Dicom
{
    internal class DicomSettings
    {
        public const string SettingsName = "Settings:DicomSettings";
        public string ServerHost { get; set; }
        public int ServerPort { get; set; }
        public string ServerAet { get; set; }
        public bool UseTls { get; set; }
        public string Aet { get; set; }
        public string CMoveDestination { get; set; }


        public string ServerHost_Ricerca { get; set; }
        public int ServerPort_Ricerca { get; set; }
        public string ServerAet_Ricerca { get; set; }
        public bool UseTls_Ricerca { get; set; }
        public string Aet_Ricerca { get; set; }
        public string CMoveDestination_Ricerca { get; set; }
    }
}
