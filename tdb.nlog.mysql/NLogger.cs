﻿using NLog;
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
        /// <param name="configFile">配置文件</param>
        public NLogger(string configFile = "") : base(configFile)
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        /// <param name="serviceID">服务ID</param>
        /// <param name="serviceAddress">服务地址</param>
        /// <param name="logConfigFile">日志配置文件</param>
        public NLogger(string connectionString, int serviceID = 0, string serviceAddress = "", string logConfigFile = "") : base(logConfigFile)
        {
            GlobalDiagnosticsContext.Set("dbConnectionString", connectionString);
            LogManager.Configuration.Variables["serviceID"] = Convert.ToString(serviceID);
            if (string.IsNullOrWhiteSpace(serviceAddress) == false)
            {
                LogManager.Configuration.Variables["serviceAddress"] = serviceAddress;
            }
        }
    }
}
