using System.Collections.Generic;

namespace AnonymizationService.Containers
{
    public interface IContainerStore
    {
        void Add(Container containerToAdd);
        void Clear();
        int CpuInUseByContainers();
        List<Container> GetRunningContainersByImageName(string imageName);
        List<int> PortsInUseByRunningContainers();
        int RamInUseByContainers();
        int RemoveById(string containerId);
        int RunningContainerCountByImage(string imageName);
    }
}