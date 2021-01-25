using CSRedis;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tdb.csredis
{
    /// <summary>
    /// reids缓存
    /// </summary>
    public class CSRedisCache
    {
        /// <summary>
        /// redis
        /// </summary>
        protected CSRedisClient rds;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionStrings">连接字符串集合</param>
        public CSRedisCache(string[] connectionStrings)
        {
            this.rds = new CSRedis.CSRedisClient((p => { return null; }), connectionStrings);
            RedisHelper.Initialization(this.rds);
        }

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            return RedisHelper.Get(key);
        }

        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return RedisHelper.Get<T>(key);
        }

        /// <summary>
        /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public bool Set(string key, object value, TimeSpan expire, RedisExistence? exists = null)
        {
            return RedisHelper.Set(key, value, expire, exists);
        }

        /// <summary>
        /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        /// <param name="exists"></param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, object value, TimeSpan expire, RedisExistence? exists = null)
        {
            return RedisHelper.SetAsync(key, value, expire, exists);
        }

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long Del(params string[] key)
        {
            return RedisHelper.Del(key);
        }

        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> DelAsync(params string[] key)
        {
            return RedisHelper.DelAsync(key);
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            return RedisHelper.Exists(key);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public bool Expire(string key, TimeSpan expire)
        {
            return RedisHelper.Expire(key, expire);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public Task<bool> ExpireAsync(string key, TimeSpan expire)
        {
            return RedisHelper.ExpireAsync(key, expire);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public bool ExpireAt(string key, DateTime expire)
        {
            return RedisHelper.ExpireAt(key, expire);
        }

        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        public Task<bool> ExpireAtAsync(string key, DateTime expire)
        {
            return RedisHelper.ExpireAtAsync(key, expire);
        }

        /// <summary>
        /// 查找所有分区节点中符合给定模式(pattern)的 key
        /// </summary>
        /// <param name="pattern">如：runoob*</param>
        /// <returns></returns>
        public string[] Keys(string pattern)
        {
            return RedisHelper.Keys(pattern);
        }

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public string HGet(string key, string field)
        {
            return RedisHelper.HGet(key, field);
        }

        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HGet<T>(string key, string field)
        {
            return RedisHelper.HGet<T>(key, field);
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, string> HGetAll(string key)
        {
            return RedisHelper.HGetAll(key);
        }

        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T> HGetAll<T>(string key)
        {
            return RedisHelper.HGetAll<T>(key);
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>如果字段是哈希表中的一个新建字段，并且值设置成功，返回true。如果哈希表中域字段已经存在且旧值已被新值覆盖，返回false。</returns>
        public bool HSet(string key, string field, object value)
        {
            return RedisHelper.HSet(key, field, value);
        }

        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>如果字段是哈希表中的一个新建字段，并且值设置成功，返回true。如果哈希表中域字段已经存在且旧值已被新值覆盖，返回false。</returns>
        public Task<bool> HSetAsync(string key, string field, object value)
        {
            return RedisHelper.HSetAsync(key, field, value);
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        /// <returns></returns>
        public bool HMSet(string key, params object[] keyValues)
        {
            return RedisHelper.HMSet(key, keyValues);
        }

        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        /// <returns></returns>
        public Task<bool> HMSetAsync(string key, params object[] keyValues)
        {
            return RedisHelper.HMSetAsync(key, keyValues);
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public long HDel(string key, params string[] fields)
        {
            return RedisHelper.HDel(key, fields);
        }

        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Task<long> HDelAsync(string key, params string[] fields)
        {
            return RedisHelper.HDelAsync(key, fields);
        }

        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HExists(string key, string field)
        {
            return RedisHelper.HExists(key, field);
        }

        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string[] HKeys(string key)
        {
            return RedisHelper.HKeys(key);
        }

        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long HLen(string key)
        {
            return RedisHelper.HLen(key);
        }

        /// <summary>
        /// 订阅，根据分区规则返回SubscribeObject，Subscribe(("chan1", msg => Console.WriteLine(msg.Body)),("chan2", msg => Console.WriteLine(msg.Body)))
        /// </summary>
        /// <param name="channels">频道和接收器</param>
        /// <returns>返回可停止订阅的对象</returns>
        public CSRedisClient.SubscribeObject Subscribe(params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels)
        {
            return RedisHelper.Subscribe(channels);
        }

        /// <summary>
        /// 使用lpush + blpop订阅端（多端争抢模式），只有一端收到消息
        /// </summary>
        /// <param name="listKeys">支持多个 key（不含prefix前辍）</param>
        /// <param name="onMessage">接收消息委托，参数1：key；参数2：消息体</param>
        /// <returns></returns>
        public CSRedisClient.SubscribeListObject SubscribeList(string[] listKeys, Action<string, string> onMessage)
        {
            return RedisHelper.SubscribeList(listKeys, onMessage);
        }

        /// <summary>
        /// 模糊订阅，订阅所有分区节点(同条消息只处理一次），返回SubscribeObject，PSubscribe(new [] { "chan1*", "chan2*"}, msg => Console.WriteLine(msg.Body))
        /// </summary>
        /// <param name="channelPatterns">模糊频道</param>
        /// <param name="pmessage">接收器</param>
        /// <returns>返回可停止模糊订阅的对象</returns>
        public CSRedisClient.PSubscribeObject PSubscribe(string[] channelPatterns, Action<CSRedisClient.PSubscribePMessageEventArgs> pmessage)
        {
            return RedisHelper.PSubscribe(channelPatterns, pmessage);
        }

        /// <summary>
        /// 用于将信息发送到指定分区节点的频道，最终消息发布格式：1|message
        /// </summary>
        /// <param name="channel">频道名</param>
        /// <param name="message">消息文本</param>
        /// <returns></returns>
        public long Publish(string channel, string message)
        {
            return RedisHelper.Publish(channel, message);
        }

        /// <summary>
        /// 查看服务是否运行
        /// </summary>
        /// <returns></returns>
        public bool Ping()
        {
            return RedisHelper.Ping();
        }

        /// <summary>
        /// 缓存壳
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="expire">过期时间</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        public T CacheShell<T>(string key, TimeSpan expire, Func<T> getData)
        {
            //转成秒数
            int seconds = Convert.ToInt32(expire.TotalSeconds);

            return RedisHelper.CacheShell(key, seconds, getData);
        }

        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field">字段</param>
        /// <param name="expire">过期时间</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        public T CacheShell<T>(string key, string field, TimeSpan expire, Func<T> getData)
        {
            //转成秒数
            int seconds = Convert.ToInt32(expire.TotalSeconds);

            return RedisHelper.CacheShell(key, field, seconds, getData);
        }

        /// <summary>
        /// 缓存壳(哈希表)，将 fields 每个元素存储到单独的缓存片，实现最大化复用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="fields">字段</param>
        /// <param name="expire">过期时间</param>
        /// <param name="getData">获取源数据的函数，输入参数是没有缓存的 fields，返回值应该是 (field, value)[]</param>
        /// <returns></returns>
        public (string key, T value)[] CacheShell<T>(string key, string[] fields, TimeSpan expire, Func<string[], (string, T)[]> getData)
        {
            //转成秒数
            int seconds = Convert.ToInt32(expire.TotalSeconds);

            return RedisHelper.CacheShell(key, fields, seconds, getData);
        }
    }
}
