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
    /// consul配置
    /// </summary>
    public class ConsulConfig
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        [ConsulConfig("LogType")]
        public string LogType { get; set; }

        /// <summary>
        /// 数据库连接字符串(VTS_MS_MatchTradeCenter)
        /// </summary>
        [ConsulConfig("VE_DBConnMatchTradeCenter")]
        public string ConnMatchTradeCenter { get; set; }

        /// <summary>
        /// 数据库连接字符串(VTS_MS_MatchHistoryData)
        /// </summary>
        [ConsulConfig("VE_DBConnMatchHistoryData")]
        public string ConnMatchHistoryData { get; set; }

        /// <summary>
        /// redis配置
        /// </summary>
        [ConsulConfig("VE_Redis")]
        public RedisConfig Redis { get; set; }

        /// <summary>
        /// 交易网关地址
        /// </summary>
        [ConsulConfig("VE_TradeGWUrl")]
        public string TradeGWUrl { get; set; }

        /// <summary>
        /// 资源站点地址
        /// </summary>
        [ConsulConfig("VE_ResourcesUrl")]
        public string ResourcesUrl { get; set; }

        /// <summary>
        /// 竞赛服务Api地址
        /// </summary>
        [ConsulConfig("VE_RaceApiUrl")]
        public string RaceApiUrl { get; set; }

        /// <summary>
        /// 红楹Api地址
        /// </summary>
        [ConsulConfig("VE_HYAPIUrl")]
        public string HYAPIUrl { get; set; }

        /// <summary>
        /// 赛组口令长度
        /// </summary>
        [ConsulConfig("VE_TeamCommandLen")]
        public int TeamCommandLen { get; set; }

        /// <summary>
        /// upms配置
        /// </summary>
        [ConsulConfig("VE_Upms")]
        public UpmsConfig Upms { get; set; }

        /// <summary>
        /// 实验报告配置
        /// </summary>
        [ConsulConfig("VE_ExpReport")]
        public ExpReportConfig ExpReport { get; set; }

        /// <summary>
        /// 默认自选外汇代码
        /// </summary>
        [ConsulConfig("VE_DefaultSelfFX")]
        public List<string> LstDefaultSelfFXCode { get; set; }

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
        /// consul服务配置
        /// </summary>
        public class ConsulServerAddressConfig
        {
            /// <summary>
            /// ip
            /// </summary>
            public string Ip { get; set; }

            /// <summary>
            /// 端口
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// KV中的key前缀
            /// </summary>
            public string preInConsul { get; set; }

            /// <summary>
            /// 服务名
            /// </summary>
            public string ServiceName { get; set; }
        }

        /// <summary>
        /// upms配置
        /// </summary>
        public class UpmsConfig
        {
            /// <summary>
            /// 地址
            /// </summary>
            public string Url { get; set; }

            /// <summary>
            /// 产品号
            /// </summary>
            public string ApplicationMark { get; set; }

            /// <summary>
            /// B端管理员类型ID
            /// </summary>
            public int AdminTypeID { get; set; }

            /// <summary>
            /// csmar公司VE产品B端管理员ID
            /// </summary>
            public int CsmarAdminTypeInfoID { get; set; }

            /// <summary>
            /// csmar公司VE产品B端管理员名称
            /// </summary>
            public string CsmarAdminTypeInfoName { get; set; }

            /// <summary>
            /// 管理员接口权限
            /// </summary>
            public List<string> AmdinApiRight { get; set; }

            /// <summary>
            /// 普通用户接口权限
            /// </summary>
            public List<string> NormalUserApiRight { get; set; }

            /// <summary>
            /// 红楹免费流量角色ID
            /// </summary>
            public int HYRoleID { get; set; }

            /// <summary>
            /// upms权限标示配置
            /// </summary>
            public UpmsRightConfig RightMark { get; set; }
        }

        /// <summary>
        /// upms权限标示配置
        /// </summary>
        public class UpmsRightConfig
        {
            /// <summary>
            /// 创建日内回转大赛权限标示
            /// </summary>
            public string CreateRiNei { get; set; }

            /// <summary>
            /// 创建外汇大赛权限标示
            /// </summary>
            public string CreateWaiHui { get; set; }

            /// <summary>
            /// 综合大赛交易品种权限标示映射关系
            /// </summary>
            public Dictionary<string, int> DicZHBreed { get; set; }
        }

        /// <summary>
        /// 实验报告配置
        /// </summary>
        public class ExpReportConfig
        {
            /// <summary>
            /// 报告样例
            /// </summary>
            public ExpReportPdfInfo Examples { get; set; }

            /// <summary>
            /// 优秀实验报告
            /// </summary>
            public List<ExpReportPdfInfo> LstExcellent { get; set; }
        }

        /// <summary>
        /// 实验报告PDF信息
        /// </summary>
        public class ExpReportPdfInfo
        {
            /// <summary>
            /// pdf名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// pdf路径
            /// </summary>
            public string Path { get; set; }
        }

        #endregion
    }

    #endregion
}
