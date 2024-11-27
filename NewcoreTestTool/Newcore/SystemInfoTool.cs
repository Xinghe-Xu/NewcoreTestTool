using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;

namespace NewcoreTestTool
{
    public static class SystemInfoToolOld
    {
        public static HardwareSerialNumber GetHardwareSerialNumber()
        {
            return new HardwareSerialNumber()
            {
                //BtNos = GetBluetoothAddresses(),
                Macs = GetMacAddresses(),
            };
        }
        public static string GetMainboardSerialNumber()
        {
            string serialNumber = string.Empty;

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");

                foreach (ManagementObject obj in searcher.Get())
                {
                    serialNumber = obj["SerialNumber"].ToString();
                    break; // Assuming there's only one motherboard
                }
            }
            catch (ManagementException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return serialNumber;
        }

        //public static string GetBluetoothMacAddress()
        //{
        //    string macAddress = string.Empty;

        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE Manufacturer != 'Microsoft'");

        //        ManagementObjectCollection adapterCollection = searcher.Get();

        //        foreach (ManagementObject adapter in adapterCollection)
        //        {
        //            if (adapter["Name"] != null && adapter["Name"].ToString().Contains("Bluetooth"))
        //            {
        //                string deviceId = adapter["PNPDeviceID"].ToString();
        //                string[] split = deviceId.Split('&');
        //                if (split.Length >= 8)
        //                {
        //                    macAddress = split[8].Substring(4).Replace('_', ':');
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error: " + ex.Message);
        //    }

        //    return macAddress;
        //}

        public static string[] GetBluetoothAddresses()
        {
            List<string> bluetoothAddresses = new List<string>();

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%Bluetooth%'");
                foreach (ManagementObject bluetoothDevice in searcher.Get())
                {
                    foreach (PropertyData property in bluetoothDevice.Properties)
                    {
                        if (property.Name == "PNPDeviceID")
                        {
                            string pnpDeviceID = property.Value as string;
                            if (pnpDeviceID != null && pnpDeviceID.Contains("BTHENUM"))
                            {
                                int index = pnpDeviceID.IndexOf('&');
                                if (index >= 0)
                                {
                                    string bluetoothAddress = pnpDeviceID.Substring(index + 1, 12);
                                    bluetoothAddresses.Add(bluetoothAddress);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("错误: " + ex.Message);
            }

            bluetoothAddresses.Sort(StringComparer.OrdinalIgnoreCase);
            return bluetoothAddresses.ToArray();
        }

        public static string[] GetMacAddresses1()
        {
            string[] macAddresses = null;

            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                macAddresses = nics
                    .Where(adapter => (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                                       adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                                       && adapter.OperationalStatus == OperationalStatus.Up)
                    .Select(adapter => adapter.GetPhysicalAddress().ToString())
                    .ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Array.Sort(macAddresses, StringComparer.OrdinalIgnoreCase);
            return macAddresses;
        }

        public static List<string> GetMacAddresses()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "getmac",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process proc = Process.Start(psi);
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();

                string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                var macList = new System.Collections.Generic.List<string>();

                foreach (string line in lines)
                {
                    string[] parts = line.Split();
                    if (parts.Length >= 3)
                    {
                        string macAddress = parts[0];
                        if (IsValidMacAddress(macAddress))
                        {
                            macList.Add(macAddress.Replace("-",""));
                        }
                    }
                }
                macList.Sort(StringComparer.OrdinalIgnoreCase);
                return macList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // 验证 MAC 地址是否有效
        private static bool IsValidMacAddress(string macAddress)
        {
            // 这里可以添加自定义的验证逻辑来确保地址的有效性
            // 示例：检查是否包含 6 组十六进制数，每组两个字符，用连字符 "-" 分隔
            // 示例仅供参考，请根据实际需求进行调整
            return System.Text.RegularExpressions.Regex.IsMatch(macAddress, "^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        }

        private static void Test(string snText)
        {
            // 执行你的 WMI 查询和操作
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm");

            string sn = "";
            string uuid = "";
            string model = "";
            string CPU = "";
            string BiosDate = "";
            string BiosVer = "";

            ManagementObjectSearcher searcher;

            // Get UUID
            searcher = new ManagementObjectSearcher("SELECT IDENTIFYINGNUMBER FROM Win32_ComputerSystemProduct");
            foreach (ManagementObject obj in searcher.Get())
            {
                sn = obj["IDENTIFYINGNUMBER"].ToString();
            }

            // Get UUID
            searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");
            foreach (ManagementObject obj in searcher.Get())
            {
                uuid = obj["UUID"].ToString();
            }

            // Get Model
            searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_ComputerSystemProduct");
            foreach (ManagementObject obj in searcher.Get())
            {
                model = obj["Name"].ToString();
            }

            // Get CPU
            searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                CPU = obj["Name"].ToString();
            }

            // Get BIOS information
            searcher = new ManagementObjectSearcher("SELECT ReleaseDate, SMBIOSBIOSVersion FROM Win32_BIOS");
            foreach (ManagementObject obj in searcher.Get())
            {
                BiosDate = obj["ReleaseDate"].ToString();
                BiosVer = obj["SMBIOSBIOSVersion"].ToString();
            }

            string filename = snText + ".txt";
            string path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Model: " + model);
            sb.AppendLine("CPU: " + CPU);
            sb.AppendLine("BiosVer: " + BiosVer);
            sb.AppendLine("BiosDate: " + BiosDate);
            sb.AppendLine("SN: " + sn);
            sb.AppendLine("UUID: " + uuid);
            sb.AppendLine("Date/Time: " + GetDateTime(DateTime.Now));

            Process process = new Process();
            process.StartInfo.FileName = "getmac";
            process.StartInfo.Arguments = "/v";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            sb.AppendLine(process.StandardOutput.ReadToEnd());

            sb.AppendLine();

            // Get memory information
            searcher = new ManagementObjectSearcher("SELECT DeviceLocator, Capacity, Speed, Manufacturer, PartNumber FROM Win32_PhysicalMemory WHERE NOT DeviceLocator = ''");
            foreach (ManagementObject obj in searcher.Get())
            {
                string deviceLocator = obj["DeviceLocator"].ToString();
                string capacity = obj["Capacity"].ToString();
                string speed = obj["Speed"].ToString();
                string manufacturer = obj["Manufacturer"].ToString();
                string partNumber = obj["PartNumber"].ToString();

                sb.AppendLine($"DIMM {deviceLocator}: Capacity={capacity}, Speed={speed}, Manufacturer={manufacturer}, PartNumber={partNumber}");
            }

            sb.AppendLine();

            // Get disk drive information
            searcher = new ManagementObjectSearcher("SELECT Model, InterfaceType, Size FROM Win32_DiskDrive WHERE InterfaceType = 'IDE' OR InterfaceType = 'SCSI'");
            foreach (ManagementObject obj in searcher.Get())
            {
                string driveModel = obj["Model"].ToString();
                string interfaceType = obj["InterfaceType"].ToString();
                string size = obj["Size"].ToString();

                sb.AppendLine($"Model: {driveModel}, InterfaceType: {interfaceType}, Size: {size}");
            }

            //MessageBox.Show(sb.ToString());

        }

        private static string GetDateTime(DateTime dt)
        {
            string dayOfWeek = dt.ToString("ddd"); // 获取星期的缩写形式
            string date = dt.ToString("MM-dd-yyyy"); // 获取日期的格式
            string time = dt.ToString("hh:mm tt"); // 获取时间的格式，包括AM/PM
            string formattedDateTime = $"Date/Time: {dayOfWeek} {date}  {time}";
            return formattedDateTime;
        }
    }
}
