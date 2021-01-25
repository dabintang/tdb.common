using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tdb.test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// 测试异步方法延迟完成是否有问题
        /// </summary>
        /// <param name="sleepSec"></param>
        [HttpGet]
        public void TestAsync(int sleepSec)
        {
            this.SleepWrite(sleepSec);
        }

        /// <summary>
        /// 睡眠后输出
        /// </summary>
        /// <param name="sleepSec"></param>
        private async void SleepWrite(int sleepSec)
        {
            await Task.Delay(sleepSec * 1000);

            Console.WriteLine($"输入：{sleepSec}");
        }
    }
}
