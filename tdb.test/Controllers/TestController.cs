using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tdb.common;
using tdb.common.Crypto;

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

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet]
        public string EncryptAES(string key, string iv, string text)
        {
            try
            {
                return EncryptHelper.EncryptAES(key, iv, text);
            }
            catch
            {
                return "加密失败";
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        [HttpGet]
        public string DecryptAES(string key, string iv, string text)
        {
            try
            {
                return EncryptHelper.DecryptAES(key, iv, text);
            }
            catch
            {
                return "解密失败";
            }
        }

        /// <summary>
        /// 获取本地IP
        /// </summary>
        [HttpGet]
        public string GetLocalIP()
        {
            return CommHelper.GetLocalIP();
        }
    }
}
