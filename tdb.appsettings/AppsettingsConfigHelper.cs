﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using tdb.common;

namespace tdb.appsettings
{
    /// <summary>
    /// appsettings.json配置帮助类
    /// </summary>
    public class AppsettingsConfigHelper
    {
        private static IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile(cfg =>
        {
            cfg.Path = "appsettings.json";
            cfg.ReloadOnChange = true;
            cfg.Optional = false;
        }).Build();

        /// <summary>
        /// 获取appsettings.json配置信息
        /// </summary>
        /// <typeparam name="T">appsettings.json配置信息类型</typeparam>
        /// <returns>consul配置信息</returns>
        public static T GetConfig<T>() where T : class, new()
        {
            //创建对象
            var obj = new T();

            //设置appsettings.json配置信息
            SetConfig(obj);

            return obj;
        }

        /// <summary>
        /// 设置appsettings.json配置信息
        /// </summary>
        /// <param name="obj">配置信息对象</param>
        private static void SetConfig(object obj)
        {
            if (obj == null)
            {
                return;
            }

            //获取对象属性
            var pros = obj.GetType().GetProperties();
            //给对象属性赋值
            foreach (var pro in pros)
            {
                //特性
                var attr = pro.GetCustomAttributes<AppsettingsConfigAttribute>().FirstOrDefault();
                if (attr == null)
                {
                    if (pro.PropertyType.IsClass && pro.PropertyType.IsPrimitive == false)
                    {
                        var proVal = pro.GetValue(obj);
                        if (proVal == null)
                        {
                            proVal = Activator.CreateInstance(pro.PropertyType);
                            CommHelper.ReflectSet(obj, pro.Name, proVal);
                        }
                        SetConfig(proVal);
                    }

                    continue;
                }

                //字符串值
                var strValue = configuration[attr.Key];

                //如果是字符串类型
                if (pro.PropertyType == typeof(string))
                {
                    CommHelper.ReflectSet(obj, pro.Name, strValue);
                }
                //如果是日期类型
                else if (pro.PropertyType == typeof(DateTime))
                {
                    var value = Convert.ToDateTime(strValue);
                    CommHelper.ReflectSet(obj, pro.Name, value);
                }
                else
                {
                    var value = JsonConvert.DeserializeObject(strValue, pro.PropertyType);
                    CommHelper.ReflectSet(obj, pro.Name, value);
                }
            }
        }
    }
}
