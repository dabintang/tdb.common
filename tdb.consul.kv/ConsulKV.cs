using Consul;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.consul.kv
{
    /// <summary>
    /// 操作 consul key/value store
    /// </summary>
    public class ConsulKV : IDisposable
    {
        /// <summary>
        /// consul
        /// </summary>
        private ConsulClient consulClient;

        /// <summary>
        /// key前缀，一般用来区分不同服务
        /// </summary>
        private readonly string prefixKey;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="consulIP">consul服务IP</param>
        /// <param name="consulPort">consul服务端口</param>
        /// <param name="prefixKey">key前缀，一般用来区分不同服务</param>
        public ConsulKV(string consulIP, int consulPort, string prefixKey)
        {
            this.consulClient = new ConsulClient(p => { p.Address = new Uri($"http://{consulIP}:{consulPort}"); });
            this.prefixKey = CvtHelper.ToStr(prefixKey);
        }

        /// <summary>
        /// 获取指定key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            var pair = this.consulClient.KV.Get(fullKey).Result.Response;
            var value = Encoding.UTF8.GetString(pair.Value);
            return value;
        }

        /// <summary>
        /// 获取所有key/value
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> List()
        {
            //获取所有key/value
            var pairs = this.consulClient.KV.List(this.prefixKey).Result.Response;
            if (pairs == null || pairs.Length == 0)
            {
                return new Dictionary<string, string>();
            }

            var dicPair = pairs.ToDictionary(k => k.Key, v => Encoding.UTF8.GetString(v.Value));
            return dicPair;
        }

        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<WriteResult<bool>> DeleteAsync(string key)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            return await this.consulClient.KV.Delete(fullKey);
        }

        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            var result = this.consulClient.KV.Delete(fullKey).Result.Response;
            return result;
        }

        /// <summary>
        /// 删除指定前缀的所有key
        /// </summary>
        /// <param name="prefix">key前缀</param>
        /// <returns></returns>
        public async Task<WriteResult<bool>> DeleteTreeAsync(string prefix)
        {
            //拼接成完整的key前缀
            var fullKeyPrefix = this.FullKey(prefix);

            return await this.consulClient.KV.DeleteTree(fullKeyPrefix);
        }

        /// <summary>
        /// 删除指定前缀的所有key
        /// </summary>
        /// <param name="prefix">key前缀</param>
        /// <returns></returns>
        public bool DeleteTree(string prefix)
        {
            //拼接成完整的key前缀
            var fullKeyPrefix = this.FullKey(prefix);

            var result = this.consulClient.KV.DeleteTree(fullKeyPrefix).Result.Response;
            return result;
        }

        /// <summary>
        /// 写入单个值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<WriteResult<bool>> PutAsync<T>(string key, T value)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            //保存
            KVPair pair = new KVPair(fullKey)
            {
                Value = this.ToBytes(value)
            };

            return await this.consulClient.KV.Put(pair);
        }

        /// <summary>
        /// 写入单个值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Put<T>(string key, T value)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            //保存
            KVPair pair = new KVPair(fullKey)
            {
                Value = this.ToBytes(value)
            };

            var result = this.consulClient.KV.Put(pair).Result.Response;
            return result;
        }

        /// <summary>
        /// 写入多个值
        /// </summary>
        /// <param name="dicKV">key/value</param>
        /// <returns></returns>
        public async Task<WriteResult<KVTxnResponse>> PutAllAsync(Dictionary<string, object> dicKV)
        {
            var lstTxnOp = new List<KVTxnOp>();
            foreach (var kv in dicKV)
            {
                //拼接成完整的key
                var fullKey = this.FullKey(kv.Key);

                var txnOp = new KVTxnOp(fullKey, KVTxnVerb.Set)
                {
                    Value = this.ToBytes(kv.Value)
                };

                lstTxnOp.Add(txnOp);
            }

            return await this.consulClient.KV.Txn(lstTxnOp);
        }

        /// <summary>
        /// 写入多个值
        /// </summary>
        /// <param name="dicKV">key/value</param>
        /// <returns></returns>
        public bool PutAll(Dictionary<string, object> dicKV)
        {
            var lstTxnOp = new List<KVTxnOp>();
            foreach (var kv in dicKV)
            {
                //拼接成完整的key
                var fullKey = this.FullKey(kv.Key);

                var txnOp = new KVTxnOp(fullKey, KVTxnVerb.Set)
                {
                    Value = this.ToBytes(kv.Value)
                };

                lstTxnOp.Add(txnOp);
            }

            var result = this.consulClient.KV.Txn(lstTxnOp).Result.Response.Success;
            return result;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (this.consulClient != null)
            {
                this.consulClient.Dispose();
            }
        }

        /// <summary>
        /// 拼接成完整的key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string FullKey(string key)
        {
            return $"{prefixKey}{key}";
        }

        /// <summary>
        /// 把值转成字节数组
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        private byte[] ToBytes<T>(T value)
        {
            //转字符串值
            string strValue = "";
            if (value is DateTime)
            {
                strValue = Convert.ToDateTime(value).ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
            else if (value is string)
            {
                strValue = CvtHelper.ToStr(value);
            }
            else
            {
                strValue = JsonConvert.SerializeObject(value, Formatting.Indented);
            }

            return Encoding.UTF8.GetBytes(strValue);
        }
    }
}
