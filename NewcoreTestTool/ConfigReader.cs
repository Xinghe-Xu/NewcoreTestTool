using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewcoreTestTool
{
    public class ConfigReader
    {
        public static string configPath = @"appSetting.json";
        private JObject _configObject;

        public ConfigReader()
        {
            string fullPath = Path.GetFullPath(configPath);
            if (!File.Exists(fullPath))
            {
                throw new ArgumentNullException(nameof(fullPath));
            }
            string config = File.ReadAllText(configPath);
            _configObject = JObject.Parse(config);
        }

        public T Read<T>(string propertyPath) where T : class
        {
            if (_configObject == null)
            {
                return null;
            }

            JToken token = _configObject.SelectToken(propertyPath);
            if (token != null)
            {
                return token.ToObject<T>();
            }

            return null;
        }

    }
}

