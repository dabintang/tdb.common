﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tdb.test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestNLogController : ControllerBase
    {
        /// <summary>
        /// 写文件日志
        /// </summary>
        [HttpGet]
        public void WriteFileLog()
        {
            var logger = new tdb.nlog.NLogger();

            logger.Trace("NLogger.Ins.Trace");
            logger.Debug("NLogger.Ins.Debug");
            logger.Info("NLogger.Ins.Info");
            logger.Warn("NLogger.Ins.Warn");
            logger.Error(new Exception("抛出error") ,"NLogger.Ins.Error");
            logger.Fatal(new Exception("抛出fatal"), "NLogger.Ins.Fatal");
        }

        /// <summary>
        /// 写数据库日志
        /// </summary>
        [HttpGet]
        public void WriteDBLog()
        {
            var logger = new tdb.nlog.mysql.NLogger(
                "Server=10.1.49.45;Port=3306;Database=tdb.logs;Uid=root;Password=123456;Charset=utf8;Pooling=True;Allow User Variables=True;SslMode=none;",
                "TestAPI");

            logger.Trace("NLogger.Ins.Trace");
            logger.Debug("NLogger.Ins.Debug");
            logger.Info("NLogger.Ins.Info");
            logger.Warn("NLogger.Ins.Warn");
            logger.Error(new Exception("抛出error"), "NLogger.Ins.Error");
            logger.Fatal(new Exception("抛出fatal"), "NLogger.Ins.Fatal");
        }
    }
}
