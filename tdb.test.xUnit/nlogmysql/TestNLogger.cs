using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.nlog.mysql;
using Xunit.Abstractions;

namespace tdb.test.xUnit.nlogmysql
{
    /// <summary>
    /// 测试Nlog写日志（mysql）
    /// </summary>
    public class TestNLogger
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly NLogger log;

        /// <summary>
        /// 输出
        /// </summary>
        private readonly ITestOutputHelper output;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="output"></param>
        public TestNLogger(ITestOutputHelper output)
        {
            this.log = new tdb.nlog.mysql.NLogger("server=127.0.0.1;database=tdb.logs;user id=root;password=123456;Pooling=True;Allow User Variables=True;", 
                0, "127.0.0.1", @"nlogmysql\NLog.config");
            this.output = output;
        }

        /// <summary>
        /// 写日志
        /// </summary>
        [Fact]
        public void Log()
        {
            var msg = "Log";
            this.log.Log(LogLevel.Trace, msg);
            this.log.Log(LogLevel.Debug, msg);
            this.log.Log(LogLevel.Info, msg);
            this.log.Log(LogLevel.Warn, msg);
            this.log.Log(LogLevel.Error, msg);
            this.log.Log(LogLevel.Fatal, msg);
        }

        /// <summary>
        /// 写日志
        /// </summary>
        [Fact]
        public void LogException()
        {
            var msg = "LogException";
            var ex = new Exception(msg);
            this.log.Log(LogLevel.Trace, ex, msg);
            this.log.Log(LogLevel.Debug, ex, msg);
            this.log.Log(LogLevel.Info, ex, msg);
            this.log.Log(LogLevel.Warn, ex, msg);
            this.log.Log(LogLevel.Error, ex, msg);
            this.log.Log(LogLevel.Fatal, ex, msg);
        }

        /// <summary>
        /// 痕迹日志
        /// </summary>
        [Fact]
        public void Trace()
        {
            this.log.Trace("Trace");
        }

        /// <summary>
        /// 痕迹日志
        /// </summary>
        [Fact]
        public void TraceException()
        {
            var msg = "TraceException";
            var ex = new Exception(msg);
            this.log.Trace(ex, msg);
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        [Fact]
        public void Debug()
        {
            this.log.Trace("Debug");
        }

        /// <summary>
        /// 调试日志
        /// </summary>
        [Fact]
        public void DebugException()
        {
            var msg = "DebugException";
            var ex = new Exception(msg);
            this.log.Debug(ex, msg);
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        [Fact]
        public void Info()
        {
            this.log.Info("Debug");
        }

        /// <summary>
        /// 信息日志
        /// </summary>
        [Fact]
        public void InfoException()
        {
            var msg = "InfoException";
            var ex = new Exception(msg);
            this.log.Info(ex, msg);
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        [Fact]
        public void Warn()
        {
            this.log.Warn("Warn");
        }

        /// <summary>
        /// 警告日志
        /// </summary>
        [Fact]
        public void WarnException()
        {
            var msg = "WarnException";
            var ex = new Exception(msg);
            this.log.Warn(ex, msg);
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        [Fact]
        public void Error()
        {
            this.log.Error("Error");
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        [Fact]
        public void ErrorException()
        {
            var msg = "ErrorException";
            var ex = new Exception(msg);
            this.log.Error(ex, msg);
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        [Fact]
        public void Fatal()
        {
            this.log.Fatal("Fatal");
        }

        /// <summary>
        /// 致命日志
        /// </summary>
        [Fact]
        public void FatalException()
        {
            var msg = "FatalException";
            var ex = new Exception(msg);
            this.log.Fatal(ex, msg);
        }

        /// <summary>
        /// 是否启用指定级别的日志
        /// </summary>
        [Fact]
        public void IsEnabled()
        {
            this.output.WriteLine($"Trace：{this.log.IsEnabled(LogLevel.Trace)}");
            this.output.WriteLine($"Debug：{this.log.IsEnabled(LogLevel.Debug)}");
            this.output.WriteLine($"Info：{this.log.IsEnabled(LogLevel.Info)}");
            this.output.WriteLine($"Warn：{this.log.IsEnabled(LogLevel.Warn)}");
            this.output.WriteLine($"Error：{this.log.IsEnabled(LogLevel.Error)}");
            this.output.WriteLine($"Fatal：{this.log.IsEnabled(LogLevel.Fatal)}");
            this.output.WriteLine($"Off：{this.log.IsEnabled(LogLevel.Off)}");

            this.output.WriteLine($"Trace2：{this.log.IsTraceEnabled}");
            this.output.WriteLine($"Debug2：{this.log.IsDebugEnabled}");
            this.output.WriteLine($"Info2：{this.log.IsInfoEnabled}");
            this.output.WriteLine($"Warn2：{this.log.IsWarnEnabled}");
            this.output.WriteLine($"Error2：{this.log.IsErrorEnabled}");
            this.output.WriteLine($"Fatal2：{this.log.IsFatalEnabled}");
        }
    }
}
