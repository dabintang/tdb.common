using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tdb.appsettings;

namespace tdb.test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestAppsettingsController : ControllerBase
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public AppsettingsConfig GetConfig()
        {
            var cfg = AppsettingsConfigHelper.GetConfig<AppsettingsConfig>();
            return cfg;
        }
    }

    #region class

    /// <summary>
    /// appsettings.json配置
    /// </summary>
    public class AppsettingsConfig
    {
        public LogLevelConfig LogDefault { get; set; }

        [AppsettingsConfig("Kestrel:EndPoints:Server:Url")]
        public string LocalUrl { get; set; }
    }

    public class LogLevelConfig
    {
        [AppsettingsConfig("Logging:LogLevel:Default")]
        public string Default { get; set; }
        [AppsettingsConfig("Logging:LogLevel:Microsoft")]
        public string Microsoft { get; set; }
        [AppsettingsConfig("Logging:LogLevel:Microsoft.Hosting.Lifetime")]
        public string MicrosoftHostingLifetime { get; set; }
    }

    #endregion
}