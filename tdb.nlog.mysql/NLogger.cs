using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.nlog.mysql
{
    /// <summary>
    /// Nlog日志帮助类
    /// Trace 包含大量的信息，例如 protocol payloads。一般仅在开发环境中启用, 仅输出不存文件。
    /// Debug  比 Trance 级别稍微粗略，一般仅在开发环境中启用, 仅输出不存文件。
    /// Info 一般在生产环境中启用。
    /// Warn 一般用于可恢复或临时性错误的非关键问题。
    /// Error 一般是异常信息。
    /// Fatal - 非常严重的错误！
    /// </summary>
    public class NLogger : tdb.nlog.NLogger
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public NLogger() : base()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="serviceCode">服务编码</param>
        public NLogger(string connectionString, string serviceCode = "") : base()
        {
            GlobalDiagnosticsContext.Set("dbConnectionString", connectionString);
            if (string.IsNullOrWhiteSpace(serviceCode) == false)
            {
                LogManager.Configuration.Variables["serviceCode"] = serviceCode;
            }

            this.logger = NLog.LogManager.GetCurrentClassLogger();
        }
    }
}
