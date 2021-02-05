using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        const string consulIP = "10.1.49.90";
        const int consulPort = 8500;

        readonly IConfiguration _config;
        readonly IHostApplicationLifetime _appLifetime;

        public TestConsulController(IConfiguration config, IHostApplicationLifetime appLifetime)
        {
            _config = config;
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
            ConsulServicesHelper.RegisterToConsul(consulIP, consulPort, "10.1.49.45", 5000, "TestServiceName", "http://10.1.49.45:5000/api/Consul/HealthCheck",
                TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), _config, _appLifetime);

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
            var jsonStr = JsonConvert.SerializeObject(lstService);

            return jsonStr;
        }

        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="consulIP">consul IP</param>
        /// <param name="consulPort">consul Port</param>
        /// <param name="prefixKey">key前缀</param>
        /// <returns></returns>
        [HttpGet]
        public ConsulConfig GetConfig(string consulIP, int consulPort, string prefixKey)
        {
            var config = ConsulConfigHelper.GetConfig<ConsulConfig>(consulIP, consulPort, prefixKey);
            return config;
        }

        /// <summary>
        /// 备份配置
        /// </summary>
        /// <param name="consulIP">consul IP</param>
        /// <param name="consulPort">consul Port</param>
        /// <param name="prefixKey">key前缀</param>
        /// <returns></returns>
        [HttpPost]
        public string BackupConfig(string consulIP, int consulPort, string prefixKey)
        {
            var config = ConsulConfigHelper.GetConfig<ConsulConfig>(consulIP, consulPort, prefixKey);
            var jsonStr = JsonConvert.SerializeObject(config);

            var fullFileName = CommHelper.GetFullFileName($"backup\\kv\\config_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json");
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
        /// <param name="consulIP">consul IP</param>
        /// <param name="consulPort">consul Port</param>
        /// <param name="prefixKey">key前缀</param>
        /// <param name="file">配置文件</param>
        /// <returns></returns>
        [HttpPost]
        public bool RestoreConfig(string consulIP, int consulPort,string prefixKey, IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                stream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);

                var json = Encoding.Default.GetString(bytes);
                var config = JsonConvert.DeserializeObject<ConsulConfig>(json);
                return ConsulConfigHelper.PutConfig(consulIP, consulPort, config, prefixKey);
            }
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
        /// mysql日志数据库连接字符串
        /// </summary>
        [ConsulConfig("MySqlLogConnStr")]
        public string MySqlLogConnStr { get; set; }

        /// <summary>
        /// redis配置
        /// </summary>
        [ConsulConfig("Redis")]
        public RedisConfig Redis { get; set; }

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

        #endregion
    }

    #endregion
}
