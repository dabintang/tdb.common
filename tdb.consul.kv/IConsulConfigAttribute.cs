using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.consul.kv
{
    /// <summary>
    /// consul配置特性接口
    /// </summary>
    public interface IConsulConfigAttribute
    {
        /// <summary>
        /// consul配置key
        /// </summary>
        string Key { get; set; }
    }
}
