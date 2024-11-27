using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewcoreTestTool
{
    public class SystemInfo : ObservableObject
    {
        private CpuInfo cpuInformation = new CpuInfo();
        private DiskInfo diskInformation = new DiskInfo();
        private MemoryInfo memoryInformation = new MemoryInfo();
        private NetworkInfo networkInformation = new NetworkInfo();
        private ObservableCollection<GpuInfo> gpuInformations = new ();
        public CpuInfo CpuInformation
        {
            get => cpuInformation;
            set => SetProperty(ref cpuInformation, value);
        }
        public DiskInfo DiskInformation
        {
            get => diskInformation;
            set => SetProperty(ref diskInformation, value);
        }
        public MemoryInfo MemoryInformation
        {
            get => memoryInformation;
            set => SetProperty(ref memoryInformation, value);
        }

        public NetworkInfo NetworkInformation
        {
            get => networkInformation;
            set => SetProperty(ref networkInformation, value);
        }

        public ObservableCollection<GpuInfo> GpuInformations
        {
            get => gpuInformations;
            set => SetProperty(ref gpuInformations, value);
        }
    }

    public class CpuInfo : ObservableObject
    {
        private string name;
        private int cores;
        private int threads;
        private int maxClockSpeed; // in MHz
        private string manufacturer;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public int Cores
        {
            get => cores;
            set => SetProperty(ref cores, value);
        }

        public int Threads
        {
            get => threads;
            set => SetProperty(ref threads, value);
        }

        public int MaxClockSpeed
        {
            get => maxClockSpeed;
            set => SetProperty(ref maxClockSpeed, value);
        }

        public string Manufacturer
        {
            get => manufacturer;
            set => SetProperty(ref manufacturer, value);
        }
    }

    public class DiskInfo : ObservableObject
    {
        private string model;
        private string interfaceType;
        private long size; // in GB
        private int partitions;
        private long diskReadsPerSec;
        private long diskWritesPerSec;

        public string Model
        {
            get => model;
            set => SetProperty(ref model, value);
        }

        public string InterfaceType
        {
            get => interfaceType;
            set => SetProperty(ref interfaceType, value);
        }

        public long Size
        {
            get => size;
            set => SetProperty(ref size, value);
        }

        public int Partitions
        {
            get => partitions;
            set => SetProperty(ref partitions, value);
        }

        public long DiskReadsPerSec
        {
            get => diskReadsPerSec;
            set => SetProperty(ref diskReadsPerSec, value);
        }

        public long DiskWritesPerSec
        {
            get => diskWritesPerSec;
            set => SetProperty(ref diskWritesPerSec, value);
        }
    }


    public class MemoryInfo : ObservableObject

    {
        private long totalPhysicalMemory;
        public long TotalPhysicalMemory
        {
            get => totalPhysicalMemory;
            set => SetProperty(ref totalPhysicalMemory, value);
        } // Memory in MB}
    }

    public class NetworkInfo : ObservableObject
    {
        private string localIpAddress;
        private string macAddress;
        private string networkInterfaceName;
        private string connectionStatus;

        public string LocalIpAddress
        {
            get => localIpAddress;
            set => SetProperty(ref localIpAddress, value);
        }

        public string MacAddress
        {
            get => macAddress;
            set => SetProperty(ref macAddress, value);
        }

        public string NetworkInterfaceName
        {
            get => networkInterfaceName;
            set => SetProperty(ref networkInterfaceName, value);
        }

        public string ConnectionStatus
        {
            get => connectionStatus;
            set => SetProperty(ref connectionStatus, value);
        }
    }

    public class GpuInfo : ObservableObject
    {
        private string name;
        private string vendor;
        private float usage;
        private float temperature;
        private float memoryUsage;
        private float totalMemory;
        private string identifier;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string Vendor
        {
            get => vendor;
            set => SetProperty(ref vendor, value);
        }
        public float Usage
        {
            get => usage;
            set => SetProperty(ref usage, value);
        }
        public float Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }
        public float MemoryUsage
        {
            get => memoryUsage;
            set => SetProperty(ref memoryUsage, value);
        }
        public float TotalMemory
        {
            get => totalMemory;
            set => SetProperty(ref totalMemory, value);
        }
        public string Identifier
        {
            get => identifier;
            set => SetProperty(ref identifier, value);
        }
    }
}
