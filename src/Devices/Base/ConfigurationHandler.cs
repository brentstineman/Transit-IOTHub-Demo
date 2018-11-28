using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Transportation.Demo.Devices.Base;
using Microsoft.Azure.Devices.Client;
using System.Threading.Tasks;
using System.Text;
using Transportation.Demo.Shared.Models;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace Transportation.Demo.Devices.Base
{
    public class ConfigurationHandler
    {
        public static string getConfig(string section, string key)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            IConfigurationSection configurationSection = configuration.GetSection(section).GetSection(key);
            return configurationSection.Value;

        }

        public static string GetDeviceRuntimeSettings(string configurationSetting)
        {
            string configFile = ConfigurationHandler.getConfig("AppSettings", configurationSetting);
            string configFileContents = string.Empty;

            // if we have a config file setting and its not empty
            if (configFile != null && configFile.Length > 0)
            {
                if (File.Exists(configFile)) // if the file exists
                    configFileContents = System.IO.File.ReadAllText(configFile);
            }

            if (String.IsNullOrEmpty(configFileContents))
            {
                throw new ArgumentException("Invalid device configuration file. File is either missing or empty");
            }

            return configFileContents;
        }
    }

}