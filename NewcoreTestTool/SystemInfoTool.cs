using LibreHardwareMonitor.Hardware;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace NewcoreTestTool
{
    public class SystemInfoTool
    {
        public static CpuInfo GetCpuInfo()
        {
            var cpuInfo = new CpuInfo();
            var searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                cpuInfo.Name = obj["Name"]?.ToString();
                cpuInfo.Cores = Convert.ToInt32(obj["NumberOfCores"]);
                cpuInfo.Threads = Convert.ToInt32(obj["NumberOfLogicalProcessors"]);
                cpuInfo.MaxClockSpeed = Convert.ToInt32(obj["MaxClockSpeed"]);
                cpuInfo.Manufacturer = obj["Manufacturer"]?.ToString();
            }
            return cpuInfo;
        }

        public static DiskInfo GetDiskInfo()
        {
            var diskInfo = new DiskInfo();
            var searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            foreach (ManagementObject obj in searcher.Get())
            {
                diskInfo.Model = obj["Model"]?.ToString();
                diskInfo.InterfaceType = obj["InterfaceType"]?.ToString();
                diskInfo.Size = Convert.ToInt64(obj["Size"]) / (1024 * 1024 * 1024); // GB
                diskInfo.Partitions = Convert.ToInt32(obj["Partitions"]);
            }

            // 获取硬盘的读写次数
            searcher = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfDisk_LogicalDisk");
            foreach (ManagementObject obj in searcher.Get())
            {
                if (obj["Name"].ToString() == "_Total")
                {
                    diskInfo.DiskReadsPerSec = Convert.ToInt64(obj["DiskReadsPerSec"]);
                    diskInfo.DiskWritesPerSec = Convert.ToInt64(obj["DiskWritesPerSec"]);
                    break;
                }
            }
            return diskInfo;
        }

        public static MemoryInfo GetMemoryInfo()
        {
            var memoryInfo = new MemoryInfo();
            var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                memoryInfo.TotalPhysicalMemory = Convert.ToInt64(obj["TotalPhysicalMemory"]) / (1024 * 1024); // MB
            }
            return memoryInfo;
        }

        // 获取本地 IP 地址、MAC 地址、网络接口和连接状态
        public static NetworkInfo GetNetworkInformation()
        {
            var networkInfo = new NetworkInfo();
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var netInterface in networkInterfaces)
            {
                if (netInterface.OperationalStatus == OperationalStatus.Up)
                {
                    networkInfo.NetworkInterfaceName = netInterface.Name;
                    networkInfo.ConnectionStatus = "Connected"; 
                    networkInfo.MacAddress = BitConverter.ToString(netInterface.GetPhysicalAddress().GetAddressBytes());
                    foreach (var ipAddress in netInterface.GetIPProperties().UnicastAddresses)
                    {
                        if (ipAddress.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            networkInfo.LocalIpAddress = ipAddress.Address.ToString();
                        }
                    }
                    break;
                }
                else
                {
                    networkInfo.ConnectionStatus = "Disconnected";
                }
            }

            return networkInfo;
        }

        public static List<GpuInfo> GetGpuInfoList()
        {
            var computer = new Computer()
            {
                IsGpuEnabled = true,
            };
            computer.Open();
            var gpuInfoList = new List<GpuInfo>();
            foreach (var hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd)
                {
                    hardware.Update();
                    var gpuInfo = new GpuInfo
                    {
                        Name = hardware.Name,
                        Vendor = GetVendorFromName(hardware.Name)
                    };

                    foreach (var sensor in hardware.Sensors)
                    {
                        switch (sensor.Name)
                        {
                            case "GPU Core":
                                gpuInfo.Usage = sensor.Value ?? 0;
                                break;
                            case "Temperature":
                                gpuInfo.Temperature = sensor.Value ?? 0;
                                break;
                            case "Memory Used":
                                gpuInfo.MemoryUsage = sensor.Value ?? 0;
                                break;
                            case "Memory Total":
                                gpuInfo.TotalMemory = sensor.Value ?? 0;
                                break;
                        }
                    }

                    gpuInfoList.Add(gpuInfo);
                }
            }

            computer.Close();
            return gpuInfoList;
        }
    private static string GetVendorFromName(string name)
    {
        if (name.Contains("NVIDIA", StringComparison.OrdinalIgnoreCase))
        {
            return "NVIDIA";
        }
        else if (name.Contains("AMD", StringComparison.OrdinalIgnoreCase))
        {
            return "AMD";
        }
        return "Unknown";
    }

        public class GPU
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Vendor { get; set; }
            public string DriverVersion { get; set; }
            public ulong MemoryBytes { get; set; }
            public string VendorId { get; set; }
            public string DeviceId { get; set; }
        }

        public static List<GPU> GetAllGPUs()
        {
            List<GPU> gpus = new List<GPU>();

            string queryString = "SELECT Name, AdapterCompatibility, DriverVersion, AdapterRam, PNPDeviceID FROM Win32_VideoController";

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(queryString))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    GPU gpu = new GPU();
                    gpu.Id = gpus.Count;

                    gpu.Name = obj["Name"]?.ToString();
                    gpu.Vendor = obj["AdapterCompatibility"]?.ToString();
                    gpu.DriverVersion = obj["DriverVersion"]?.ToString();
                    gpu.MemoryBytes = Convert.ToUInt64(obj["AdapterRam"]?.ToString() ?? "0");

                    string pnpDeviceId = obj["PNPDeviceID"]?.ToString();
                    if (pnpDeviceId != null && pnpDeviceId.StartsWith("PCI\\"))
                    {
                        pnpDeviceId = pnpDeviceId.Replace("PCI\\", "");
                        var ids = pnpDeviceId.Split('&');
                        gpu.VendorId = ids.Length > 0 ? ids[0].Replace("VEN_", "") : "0";
                        gpu.DeviceId = ids.Length > 1 ? ids[1].Replace("DEV_", "") : "0";
                    }
                    else
                    {
                        gpu.VendorId = "0";
                        gpu.DeviceId = "0";
                    }

                    gpus.Add(gpu);
                }
            }

            return gpus;
        }

        public static string GetChargeInfo()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                // 创建 ManagementObjectSearcher 对象，查询 Win32_Battery 类
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery"))
                {
                    foreach (ManagementObject battery in searcher.Get())
                    {
                        sb.AppendLine("Battery Properties:");
                        sb.AppendLine(new string('-', 40));

                        // 遍历所有属性
                        foreach (PropertyData property in battery.Properties)
                        {
                            sb.AppendLine($"{property.Name}: {JsonConvert.SerializeObject(property.Value)}");
                        }

                        sb.AppendLine(new string('-', 40));
                    }
                    //var batterys = searcher.Get();
                    //foreach (var battery in batterys)
                    //{
                    //    // 获取电池信息
                    //    string deviceName = battery["Name"]?.ToString();
                    //    string manufacturer = battery["Manufacturer"]?.ToString();
                    //    string uniqueId = battery["DeviceID"]?.ToString();
                    //    string designCapacity = battery["DesignCapacity"]?.ToString();
                    //    string fullChargeCapacity = battery["FullChargeCapacity"]?.ToString();
                    //    string estimatedChargeRemaining = battery["EstimatedChargeRemaining"]?.ToString();
                    //    string status = battery["Status"]?.ToString();

                    //    // 计算损耗程度（假设设计容量和当前容量均以毫瓦时为单位）
                    //    if (long.TryParse(designCapacity, out long design) && long.TryParse(fullChargeCapacity, out long fullCharge))
                    //    {
                    //        double wearLevel = design > 0 ? ((double)(design - fullCharge) / design) * 100 : 0;
                    //        sb.AppendLine($"Wear Level: {wearLevel:0.00}%");
                    //    }

                    //    // 输出电池信息
                    //    sb.AppendLine($"Device Name: {deviceName}");
                    //    sb.AppendLine($"Manufacturer: {manufacturer}");
                    //    sb.AppendLine($"Unique ID: {uniqueId}");
                    //    sb.AppendLine($"Design Capacity: {designCapacity} mWh");
                    //    sb.AppendLine($"Full Charge Capacity: {fullChargeCapacity} mWh");
                    //    sb.AppendLine($"Estimated Charge Remaining: {estimatedChargeRemaining}%");
                    //    sb.AppendLine($"Status: {status}");
                    //    sb.AppendLine(new string('-', 40));
                    //}
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"发生异常: {ex.Message}");
            }

            return sb.ToString();
        }
    }

}
