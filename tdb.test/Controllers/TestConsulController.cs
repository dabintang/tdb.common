using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using tdb.common;
using tdb.consul.kv;
using tdb.consul.services;

namespace tdb.test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestConsulController : ControllerBase
    {
        const string consulIP = "127.0.0.1";
        const int consulPort = 8500;
        readonly IHostApplicationLifetime _appLifetime;

        public TestConsulController(IHostApplicationLifetime appLifetime)
        {
            _appLifetime = appLifetime;
        }

        /// <summary>
        /// 心跳检查
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        /// <summary>
        /// 注册到consul
        /// </summary>
        /// <param name="consulUri"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult RegisterToConsul()
        {
            ConsulServicesHelper.RegisterToConsul(consulIP, consulPort, "127.0.0.1", 31000, "TestServiceName", "http://127.0.0.1:31000/api/TestConsul/HealthCheck",
                TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(5), _appLifetime);

            return Ok();
        }

        /// <summary>
        /// 获取已注册到consul的本服务地址
        /// </summary>
        /// <param name="serviceName">服务在consul上的服务名</param>
        /// <returns></returns>
        [HttpGet]
        public string FindServices(string serviceName)
        {
            var lstService = ConsulServicesHelper.FindServices(consulIP, consulPort, serviceName);
            var jsonStr = lstService.SerializeJson();

            return jsonStr;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="prefixKey">key前缀</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ConsulConfig> GetConfig(string prefixKey)
        {
            var config = await ConsulConfigHelper.GetConfigAsync<ConsulConfig, ConsulConfigAttribute>(consulIP, consulPort, prefixKey);
            return config;
        }

        /// <summary>
        /// 备份配置
        /// </summary>
        /// <param name="prefixKey">key前缀</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> BackupConfig(string prefixKey)
        {
            var config = await ConsulConfigHelper.GetConfigAsync<ConsulConfig, ConsulConfigAttribute>(consulIP, consulPort, prefixKey);
            var jsonStr = config.SerializeJson();

            var fullFileName = CommHelper.GetFullFileName($"backup\\kv\\config_{DateTime.Now:yyyyMMddHHmmss}.json");
            var path = Path.GetDirectoryName(fullFileName);
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }
            System.IO.File.WriteAllText(fullFileName, jsonStr);

            return fullFileName;
        }

        /// <summary>
        /// 还原配置
        /// </summary>
        /// <param name="prefixKey">key前缀</param>
        /// <param name="file">配置文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<bool> RestoreConfig(string prefixKey, IFormFile file)
        {
            using var stream = file.OpenReadStream();
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            var json = Encoding.UTF8.GetString(bytes);
            var config = json.DeserializeJson<ConsulConfig>();
            return await ConsulConfigHelper.PutConfig<ConsulConfig, ConsulConfigAttribute>(consulIP, consulPort, config, prefixKey);
        }
    }

    #region class

    /// <summary>
    /// 还原配置条件
    /// </summary>
    public class RestoreConfigReq
    {
        /// <summary>
        /// 完整还原份文件名(.json文件)
        /// </summary>
        public string FullFileName { get; set; }
    }

    /// <summary>
    /// consul配置
    /// </summary>
    public class ConsulConfig
    {
        /// <summary>
        /// 是否已设置consul配置
        /// </summary>
        public bool HadConsulConfig()
        {
            return !string.IsNullOrEmpty(DBLogConnStr);
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        [ConsulConfig("DBConnStr")]
        public string DBConnStr { get; set; }

        /// <summary>
        /// 日志数据库连接字符串
        /// </summary>
        [ConsulConfig("DBLogConnStr")]
        public string DBLogConnStr { get; set; }

        /// <summary>
        /// redis配置
        /// </summary>
        [ConsulConfig("Redis")]
        public RedisConfig Redis { get; set; }

        /// <summary>
        /// 认证相关配置
        /// </summary>
        [ConsulConfig("Token")]
        public TokenConfig Token { get; set; }

        /// <summary>
        /// 默认密码
        /// </summary>
        [ConsulConfig("DefaultPassword")]
        public string DefaultPassword { get; set; }

        #region 内部类

        /// <summary>
        /// redis配置
        /// </summary>
        public class RedisConfig
        {
            /// <summary>
            /// 连接字符串
            /// </summary>
            public List<string> ConnectString { get; set; }
        }

        /// <summary>
        /// 认证相关配置
        /// </summary>
        public class TokenConfig
        {
            /// <summary>
            /// 发行者
            /// </summary>
            public string Issuer { get; set; }

            /// <summary>
            /// 接收者
            /// </summary>
            public string Audience { get; set; }

            /// <summary>
            /// 秘钥（至少16位）
            /// </summary>
            public string SecretKey { get; set; }

            /// <summary>
            /// 超时时间（秒）
            /// </summary>
            public int TimeoutSeconds { get; set; }
        }

        #endregion
    }

    #endregion
}
