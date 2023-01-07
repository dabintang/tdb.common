using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using tdb.common;

namespace tdb.consul.kv
{
    /// <summary>
    /// consul配置帮助类
    /// </summary>
    public class ConsulConfigHelper
    {
        /// <summary>
        /// 获取consul上的配置信息
        /// </summary>
        /// <typeparam name="TConfig">consul配置信息类型</typeparam>
        /// <typeparam name="TConsulConfigAttribute">consul配置特效接口</typeparam>
        /// <param name="consulIP">consul服务IP</param>
        /// <param name="consulPort">consul服务端口</param>
        /// <param name="prefixKey">key前缀，一般用来区分不同服务</param>
        /// <returns>consul配置信息</returns>
        public static async Task<TConfig> GetConfigAsync<TConfig, TConsulConfigAttribute>(string consulIP, int consulPort, string prefixKey) 
            where TConfig : class, new() 
            where TConsulConfigAttribute: Attribute, IConsulConfigAttribute
        {
            Dictionary<string, string> dicPair;
            using (var kv = new ConsulKV(consulIP, consulPort, prefixKey))
            {
                //获取所有key/value
                dicPair = await kv.ListAsync();
            }

            //创建对象
            var obj = new TConfig();

            Type type = typeof(TConfig);
            //获取对象属性
            var pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //给对象属性赋值
            foreach (var pro in pros)
            {
                //特性
                var attr = pro.GetCustomAttributes<TConsulConfigAttribute>().FirstOrDefault();
                var key = $"{prefixKey}{attr?.Key}";
                if (attr == null || dicPair.ContainsKey(key) == false)
                {
                    continue;
                }

                //字符串值
                var strValue = dicPair[key];

                //如果是字符串类型
                if (pro.PropertyType == typeof(string))
                {
                    CommHelper.EmitSet(obj, pro.Name, strValue);
                }
                //如果是日期类型
                else if (pro.PropertyType == typeof(DateTime))
                {
                    var value = Convert.ToDateTime(strValue);
                    CommHelper.EmitSet(obj, pro.Name, value);
                }
                else
                {
                    var value = strValue.DeserializeJson(pro.PropertyType);
                    CommHelper.EmitSet(obj, pro.Name, value);
                }
            }

            return obj;
        }

        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <typeparam name="TConfig">consul配置信息类型</typeparam>
        /// <typeparam name="TConsulConfigAttribute">consul配置特效接口</typeparam>
        /// <param name="consulIP">consul服务IP</param>
        /// <param name="consulPort">consul服务端口</param>
        /// <param name="config">consul配置信息</param>
        /// <param name="prefixKey">key前缀，一般用来区分不同服务</param>
        /// <returns></returns>
        public static async Task<bool> PutConfig<TConfig, TConsulConfigAttribute>(string consulIP, int consulPort, TConfig config, string prefixKey) 
            where TConfig : class
            where TConsulConfigAttribute : Attribute, IConsulConfigAttribute
        {
            //配置信息字典
            var dicConfig = new Dictionary<string, object?>();

            Type type = typeof(TConfig);
            //获取对象属性
            var pros = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (pros == null)
            {
                return true;
            }

            //找出有配置特性的属性，并把属性对应放入字典
            foreach (var pro in pros)
            {
                //特性
                var attr = pro.GetCustomAttributes<TConsulConfigAttribute>().FirstOrDefault();
                if (attr == null)
                {
                    continue;
                }

                dicConfig[attr.Key] = CommHelper.EmitGet(config, pro.Name);
            }

            using var kv = new ConsulKV(consulIP, consulPort, prefixKey);
            //写入consul
            return await kv.PutAllAsync(dicConfig);
        }
    }
}
