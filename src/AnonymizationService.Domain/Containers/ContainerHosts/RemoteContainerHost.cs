using Docker.DotNet;
using Microsoft.Extensions.Logging;
using System;

namespace AnonymizationService.Containers.ContainerHosts
{
    internal class RemoteContainerHost : ContainerHost
    {
        public RemoteContainerHost(ContainerHostSettings containerHostSettings, ILogger<ContainerHost> logger) : base(containerHostSettings, logger)
        {
            var dockerUri = containerHostSettings.DockerEnginePort.HasValue
                ? $"{containerHostSettings.RemoteUriScheme}://{containerHostSettings.RemoteUri}:{containerHostSettings.DockerEnginePort}"
                : $"{containerHostSettings.RemoteUriScheme}://{containerHostSettings.RemoteUri}";

            logger.LogInformation($"Docker Uri: {dockerUri}");

            _dockerClient = new DockerClientConfiguration(new Uri(dockerUri)).CreateClient();
            hostUrl = containerHostSettings.RemoteEndpoint;
        }

        private readonly string hostUrl;

        public override string HostUrl { get => hostUrl; }
    }
}
