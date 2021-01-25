using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tdb.csredis;
using static CSRedis.CSRedisClient;

namespace tdb.test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestCSRedisController : ControllerBase
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public Info Get(string key)
        {
            return CSRediser.Ins.Get<Info>(key);
        }

        /// <summary>
        /// 设值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        [HttpPost]
        public bool Set(string key, Info value)
        {
            return CSRediser.Ins.Set(key, value, TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        [HttpGet]
        public Info HGet(string key, string field)
        {
            return CSRediser.Ins.HGet<Info>(key, field);
        }

        /// <summary>
        /// 设值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        [HttpPost]
        public void HSet(string key, string field, Info value)
        {
            CSRediser.Ins.HSet(key, field, value);
            CSRediser.Ins.Expire(key, TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// 获取所有key
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        [HttpGet]
        public string[] Keys(string pattern)
        {
            return CSRediser.Ins.Keys(pattern);
        }

        /// <summary>
        /// 获取所有key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public string[] HKeys(string key)
        {
            return CSRediser.Ins.HKeys(key);
        }

        //只是举例测试，暂不考虑线程安全
        private static Dictionary<string, SubscribeObject> dicSub = new Dictionary<string, SubscribeObject>();
        /// <summary>
        /// 订阅指定频道
        /// </summary>
        /// <param name="channel">频道</param>
        [HttpPost]
        public void Subscribe(string channel)
        {
            if (dicSub.ContainsKey(channel) == false)
            {
                var sub = CSRediser.Ins.Subscribe((channel, args => SubscribeBackFun(args)));
                dicSub[channel] = sub;
            }
        }

        /// <summary>
        /// 取消订阅指定频道
        /// </summary>
        /// <param name="channel">频道</param>
        [HttpPost]
        public string Unsubscribe(string channel)
        {
            if (dicSub.ContainsKey(channel) == false)
            {
                return "未订阅该频道";
            }

            var sub = dicSub[channel];
            sub.Unsubscribe();
            dicSub.Remove(channel);
            return $"已取消订阅频道：{channel}";
        }

        /// <summary>
        /// 处理订阅回报
        /// </summary>
        /// <param name="args"></param>
        private static void SubscribeBackFun(SubscribeMessageEventArgs args)
        {
            Console.WriteLine($"{args.MessageId}.{args.Channel}.{args.Body}");
        }

        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="value"></param>
        [HttpPost]
        public void Publish(string channel, Info value)
        {
            CSRediser.Ins.Publish(channel, JsonConvert.SerializeObject(value));
        }
    }

    public class Info
    {
        public string Content { get; set; }
    }

    public class CSRediser
    {
        private static object _lock = new object();
        private static CSRedisCache _ins;
        public static CSRedisCache Ins
        {
            get
            {
                if (_ins == null)
                {
                    lock(_lock)
                    {
                        if (_ins == null)
                        {
                            _ins = new CSRedisCache(new string[] { "10.1.137.148:6379,password=,defaultDatabase=5,poolsize=50,ssl=false,writeBuffer=10240,prefix=" });
                        }
                    }
                }

                return _ins;
            }
        }


    }
}
