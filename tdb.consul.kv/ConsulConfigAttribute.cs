using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.consul.kv
{
    /// <summary>
    /// consul配置特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConsulConfigAttribute : Attribute, IConsulConfigAttribute
    {
        /// <summary>
        /// consul配置key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// consul配置key特性
        /// </summary>
        /// <param name="key">consul配置key</param>
        public ConsulConfigAttribute(string key)
        {
            this.Key = key;
        }
    }
}
