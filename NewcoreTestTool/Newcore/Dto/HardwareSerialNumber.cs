using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;

namespace NewcoreTestTool
{
    public class HardwareSerialNumber
    {
        public string SN { get; set; }
        //public string Imei { get; set; }
        //public string[] BtNos { get; set; }
        public List<string> Macs { get; set; }

        //public string GetBtNosString() => BtNos == null ? string.Empty : string.Join(",", BtNos);
        public string GetMacsString() => Macs == null ? string.Empty : string.Join(",", Macs);
        public static HardwareSerialNumber valueOf(Dictionary<string, object> dict)
        {
            string sn = dict.ContainsKey(Constant.SN) ? (string)dict[Constant.SN] : null;
            //string imei = dict.ContainsKey(Constant.IMEI) ? (string)dict[Constant.IMEI] : null;
            //string btNo = dict.ContainsKey(Constant.BTNO) ? (string)dict[Constant.BTNO] : null;
            //string[] btNos = GetSpeciadlValueOrdered(dict, Constant.BTNO);
            List<string> macs = GetSpeciadlValueOrdered(dict, Constant.MAC);
            return new HardwareSerialNumber
            {
                SN = sn,
                //BtNos = btNos,
                Macs = macs
            };
        }

        public static List<string> GetSpeciadlValueOrdered(Dictionary<string, object> dict, string key)
        {
            var extractedMacValues = dict
                .Where(kv => kv.Key.Contains(key) && kv.Key.StartsWith(Constant.FIELDPREFIX) && kv.Key.EndsWith(Constant.FIELDSUFFIX))
                .ToDictionary(kv => kv.Key.Substring(Constant.FIELDPREFIX.Length, kv.Key.Length - Constant.FIELDPREFIX.Length - Constant.FIELDSUFFIX.Length), kv => kv.Value);
            string[] values = new string[extractedMacValues.Count];
            foreach (var item in extractedMacValues)
            {
                string macNo = item.Key.Substring(Constant.MAC.Length, item.Key.Length - Constant.MAC.Length);
                int num = int.Parse(macNo);
                values[num - 1] = (string)item.Value;
            }
            Array.Sort(values, StringComparer.OrdinalIgnoreCase);
            return new List<string> (values);
        }

        public bool ContainsMacs(HardwareSerialNumber other)
        {
            if (other == null || other.Macs == null || other.Macs.Count == 0)
            {
                return true;
            }

            if (Macs == null || Macs.Count == 0)
            {
                return false;
            }

            foreach (var mac in other.Macs)
            {
                if(!Macs.Contains(mac))
                {
                    return false;
                }
            }

            return true;
        }

        public static class Constant
        {
            public static string FIELDPREFIX = "field_";
            public static string FIELDSUFFIX = "__c";
            public static string SN = "SN";
            public static string MAC = "MAC";
            public static string BTNO = "BTNO";
            public static string WIFI = "WIFI";

            public static string[] FieldName =
            {
                "MAC",
                "",
                "蓝牙",
                "MAC1",
                "MAC2",
                "MAC3",
                "MAC4",
                "MAC5",
                "MAC6",
            };
        }


    }
}
