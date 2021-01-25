using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.appsettings
{
    /// <summary>
    /// appsettings.json配置特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AppsettingsConfigAttribute : Attribute
    {
        /// <summary>
        /// appsettings.json配置key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// appsettings.json配置key特性
        /// </summary>
        /// <param name="key">appsettings.json配置key</param>
        public AppsettingsConfigAttribute(string key)
        {
            this.Key = key;
        }
    }
}
