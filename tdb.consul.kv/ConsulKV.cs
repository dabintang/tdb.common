using Consul;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private readonly ConsulClient consulClient;

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
            this.prefixKey = prefixKey.ToStr();
        }

        /// <summary>
        /// 获取指定key的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string key)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            var pair = (await this.consulClient.KV.Get(fullKey)).Response;
            var value = Encoding.UTF8.GetString(pair.Value);
            return value;
        }

        /// <summary>
        /// 获取所有key/value
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> ListAsync()
        {
            //获取所有key/value
            var pairs = (await this.consulClient.KV.List(this.prefixKey)).Response;
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
        public async Task<bool> DeleteAsync(string key)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            return (await this.consulClient.KV.Delete(fullKey)).Response;
        }

        /// <summary>
        /// 删除指定前缀的所有key
        /// </summary>
        /// <param name="prefix">key前缀</param>
        /// <returns></returns>
        public async Task<bool> DeleteTreeAsync(string prefix)
        {
            //拼接成完整的key前缀
            var fullKeyPrefix = this.FullKey(prefix);

            return (await this.consulClient.KV.DeleteTree(fullKeyPrefix)).Response;
        }

        /// <summary>
        /// 写入单个值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public async Task<bool> PutAsync<T>(string key, T? value)
        {
            //拼接成完整的key
            var fullKey = this.FullKey(key);

            //保存
            KVPair pair = new(fullKey)
            {
                Value = this.ToBytes(value)
            };

            return (await this.consulClient.KV.Put(pair)).Response;
        }

        /// <summary>
        /// 写入多个值
        /// </summary>
        /// <param name="dicKV">key/value</param>
        /// <returns></returns>
        public async Task<bool> PutAllAsync(Dictionary<string, object?> dicKV)
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

            var res = (await this.consulClient.KV.Txn(lstTxnOp)).Response;
            return res.Success;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (this.consulClient != null)
            {
                this.consulClient.Dispose();
                GC.SuppressFinalize(this);
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

        private JsonSerializerOptions? _jsonSerializerOptions;
        /// <summary>
        /// json序列化设置
        /// </summary>
        private JsonSerializerOptions JsonSerializerOptions
        {
            get
            {
                if (this._jsonSerializerOptions == null)
                {
                    this._jsonSerializerOptions = new JsonSerializerOptions(CvtHelper.DefaultOptions);
                    //写入缩进
                    this._jsonSerializerOptions.WriteIndented = true;
                }
                return this._jsonSerializerOptions;
            }
        }

        /// <summary>
        /// 把值转成字节数组
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="value">值</param>
        /// <returns></returns>
        private byte[] ToBytes<T>(T? value)
        {
            if (value == null)
            {
                return Encoding.UTF8.GetBytes(string.Empty);
            }

            //转字符串值
            string strValue;
            if (value is DateTime)
            {
                strValue = Convert.ToDateTime(value).ToString("yyyy/MM/dd HH:mm:ss");
            }
            else if (value is string)
            {
                strValue = value.ToStr();
            }
            else
            {
                strValue = value.SerializeJson(this.JsonSerializerOptions);
            }

            return Encoding.UTF8.GetBytes(strValue);
        }
    }
}
