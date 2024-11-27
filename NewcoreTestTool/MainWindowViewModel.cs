using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using static NewcoreTestTool.SystemInfoTool;

namespace NewcoreTestTool
{
    public class MainWindowViewModel: ObservableObject
    {
        private SystemInfo sysInfo = new ();
        private NewCoreClient _client = null;
        private ConfigReader _configReader = null;
        private HardwareSerialNumber _newCoreHardwareSerialNumber = new HardwareSerialNumber();
        private string macs = string.Empty;
        private string snNumber = string.Empty;
        private TaskCompletionSource<bool> _systemInfoTaskCompletionSource = new TaskCompletionSource<bool>();

        public SystemInfo SysInfo
        {
            get => sysInfo;
            set => SetProperty(ref sysInfo, value);
        }
        public string Macs
        {
            get=> macs;
            set=> SetProperty(ref macs, value);
        }

        public string SnNumber
        {
            get => snNumber;
            set => SetProperty(ref snNumber, value);
        }

        public MainWindowViewModel()
        {
            Inital();
            Task.Run(GetSystemInfo);
        }

        public void Inital()
        {
            try
            {
                _configReader = new ConfigReader();
                string baseUrl = _configReader.Read<string>("mesUrl");
                NewCoreKey newCoreKey = _configReader.Read<NewCoreKey>("openApi");
                if (!NewCoreKey.Vaild(newCoreKey))
                {
                    newCoreKey = new NewCoreKey
                    {
                        appKey = "746fd4ec-aa1d-4912-ba2a-de4a269dc67e",
                        appSecret = "3rTe4216*UbI1-Y*"
                    };

                }
                _client = new NewCoreClient(baseUrl, newCoreKey);
                _client.RefreshToken();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void GetSystemInfo()
        {
            SysInfo.CpuInformation = SystemInfoTool.GetCpuInfo();
            SysInfo.DiskInformation = SystemInfoTool.GetDiskInfo();
            SysInfo.MemoryInformation = SystemInfoTool.GetMemoryInfo();
            SysInfo.GpuInformations = new ObservableCollection<GpuInfo>(SystemInfoTool.GetGpuInfoList());
            List<GPU> gpus = GetAllGPUs();
            //foreach (var gpu in gpus)
            //{
            //    MessageBox.Show($"ID: {gpu.Id}, Name: {gpu.Name}, Vendor: {gpu.Vendor}, Driver Version: {gpu.DriverVersion}, Memory: {gpu.MemoryBytes} bytes, Vendor ID: {gpu.VendorId}, Device ID: {gpu.DeviceId}");
            //}
            SystemInfoTool.GetChargeInfo();
            _systemInfoTaskCompletionSource.SetResult(true);
        }

        private void DownLoadMeiGaoTestParameter(string snText)
        {
            if (string.IsNullOrEmpty(snText))
            {
                return;
            }
            var result = _client.QueryMeiGaoImeiParam(snText);
            MessageBox.Show(JsonConvert.SerializeObject(result));
            _newCoreHardwareSerialNumber = (result == null || result.Count == 0) ? null : result[0];
            Macs = _newCoreHardwareSerialNumber == null ? string.Empty : _newCoreHardwareSerialNumber.GetMacsString();
            return;
        }

        private ICommand mesCommand;
        public ICommand MesCommand
        {
            get => mesCommand ??= new RelayCommand<string>(ExecuteMesCommand);
        }

        private void ExecuteMesCommand(string snText)
        {
            DownLoadMeiGaoTestParameter(snText);
            //string info = JsonConvert.SerializeObject(SysInfo);
            //MessageBox.Show(info);
            //var result = _client.PushCPS<NewCoreResponse<object>, object>(info, "/api/metadata-flow/trigger/webhook/6880832f-db56-48ab-9ad5-5bcdd888be10" );
            Task.Run(() => {
                _systemInfoTaskCompletionSource.Task.ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        string info = JsonConvert.SerializeObject(SysInfo);
                        MessageBox.Show(info);
                    }
                    else if (task.IsFaulted)
                    {
                        MessageBox.Show("Error occurred while fetching system info.");
                    }
                });
            });
        }

    }
}
