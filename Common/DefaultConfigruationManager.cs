using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Executing class to use the static System.configuration.ConfigurationManager to implement IConfigurationManager
    /// </summary>
    public class DefaultConfigruationManager: IConfigurationManager
    {
        public string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }
    }
}
