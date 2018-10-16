using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Mockable contract for ConfigurationManager
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Get an appsetting by key name
        /// </summary>
        /// <param name="key">the key name</param>
        /// <returns>the setting value</returns>
        string GetAppSetting(string key);

        /// <summary>
        /// Get a connectionstring by key name
        /// </summary>
        /// <param name="key">the key name</param>
        /// <returns>the connection string</returns>
        string GetConnectionString(string key);
    }
}
