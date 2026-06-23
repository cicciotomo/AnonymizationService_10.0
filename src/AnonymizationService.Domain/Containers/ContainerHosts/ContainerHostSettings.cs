namespace AnonymizationService.Containers.ContainerHosts
{
    public class ContainerHostSettings
    {
        public const string SettingsName = "Settings:ContainerHostSettings";

        public bool IsLocalHost { get; set; }
        public string RemoteUri { get; set; }
        public string RemoteEndpoint { get; set; }
        public string RemoteUriScheme { get; set; }
        public int? DockerEnginePort { get; set; }
        public int CpuAvailable { get; set; }
        public int RamAvailable { get; set; }
        public int BaseTcpPort { get; set; }
    }
}