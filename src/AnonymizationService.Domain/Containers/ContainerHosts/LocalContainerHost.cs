using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace AnonymizationService.Containers.ContainerHosts
{
    internal class LocalContainerHost : ContainerHost
    {
        public LocalContainerHost(ContainerHostSettings containerHostSettings, ILogger<ContainerHost> logger) : base(containerHostSettings, logger)
        {
            _dockerClient = new DockerClientConfiguration()
                .CreateClient();
        }

        public override string HostUrl => "localhost";
    }
}
