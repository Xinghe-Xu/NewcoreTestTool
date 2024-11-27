using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NewcoreTestTool
{
    public class Snapshot
    {
        public string SN { get; set; }
        public HardwareSerialNumber CloudHardwareSerialNumber { get; set; }
        public HardwareSerialNumber LocalHardwareSerialNumber { get; set; }
        public MeiGaoTestRecord CurMeiGaoTestRecord { get; set; }
        public int CurrentIndex { get; set; }

        public void Save()
        {
            string curDir = Directory.GetCurrentDirectory();
            string fullPath = Path.GetFullPath(curDir + Config.FILEPATH);
            if (!File.Exists(fullPath))
            {
                string dir = Path.GetFullPath(Path.GetDirectoryName(fullPath));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            File.WriteAllText(fullPath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Snapshot Read()
        {
            try
            {
                string curDir = Directory.GetCurrentDirectory();
                string fullPath = Path.GetFullPath(curDir + Config.FILEPATH);
                if (!File.Exists(fullPath))
                {
                    return null;
                }

                string jsonString = File.ReadAllText(fullPath);
                return JsonConvert.DeserializeObject<Snapshot>(jsonString);
            }
            catch(Exception ex)
            {
                return null;
            }

        }
        public static class Config
        {
            public static string FILENAME = "PCDiagnosticTool.json";
            public static string PATHNAME = @"/record/";
            public static string FILEPATH = PATHNAME + FILENAME;
        }
    }
}
