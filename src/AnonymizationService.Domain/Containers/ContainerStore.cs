using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Containers
{
    public class ContainerStore : ISingletonDependency, IContainerStore
    {
        private readonly List<Container> _runningContainers;

        public ContainerStore()
            => _runningContainers = new List<Container>();

        public void Add(Container containerToAdd)
            => _runningContainers.Add(containerToAdd);

        public int RemoveById(string containerId)
            => _runningContainers.RemoveAll(c => c.ContainerId.StartsWith(containerId));

        public int CpuInUseByContainers()
            => _runningContainers?.Sum(c => c.AllocatedCpus) ?? 0;

        public int RamInUseByContainers()
            => _runningContainers?.Sum(c => c.AllocatedRam) ?? 0;

        public int RunningContainerCountByImage(string imageName)
        => _runningContainers.Count(x => x.ImageName == imageName);

        public void Clear()
            => _runningContainers.Clear();

        public List<int> PortsInUseByRunningContainers()
            => _runningContainers.Where(c => c.PortMapping is not null).Select(c => c.PortMapping.HostPort).ToList();

        public List<Container> GetRunningContainersByImageName(string imageName)
            => _runningContainers.Where(c => c.ImageName == imageName).ToList();
    }
}
