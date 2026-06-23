using System;
using System.Collections.Generic;

namespace AnonymizationService.Containers
{
    public class Container
    {
        public string ContainerId { get; set; }
        public string Name { get; set; }
        public DateTime? StartTime { get; set; }
        public ContainerPortMapping PortMapping { get; set; }
        public int? ExposedPort { get; set; }
        public int? AllocatedRam { get; set; }
        public int? AllocatedCpus { get; set; }
        public string ImageName { get; set; }
        public string ImageShortName { get; set; }
        public int? MaxAllowedContainers { get; set; }
        public string Url { get; set; }
        public string NetworkId { get; set; }
        public string NetworkName { get; set; }
        public Dictionary<string, string> EnvVariables { get; set; }
        public Dictionary<string, string> Volumes { get; set; }
    }

    public class ContainerPortMapping
    {
        public ContainerPortMapping(int hostPort, int containerPort)
        {
            HostPort = hostPort;
            ContainerPort = containerPort;
        }

        public int HostPort { get; set; }
        public int ContainerPort { get; set; }
    }
}
