using Microsoft.Extensions.Logging;

namespace AnonymizationService.Containers.ContainerHosts
{
    public static class ContainerHostFactory
    {
        public static ContainerHost Generate(ContainerHostSettings containerHostSettings, ILogger<ContainerHost> logger)
        {
            ContainerHost containerHost;

            if (containerHostSettings.IsLocalHost)
            {
                containerHost = new LocalContainerHost(containerHostSettings, logger);
            }
            else
            {
                containerHost = new RemoteContainerHost(containerHostSettings, logger);
            }

            return containerHost;
        }
    }
}
