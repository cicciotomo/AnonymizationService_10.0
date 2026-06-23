using AnonymizationService.Containers;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Pods
{
    public class Pod
    {
        public Guid Id { get; set; }
        public string PodDefinitionName { get; set; }
        public bool IsBase { get; set; }
        public int RunningRequests { get; set; }
        public List<Container> RunningContainers { get; set; }
        public Container ContainerEntryPoint { get; set; }
        public ContainerNetworkConfiguration NetworkConfiguration { get; set; }
    }
}
