using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Versioning;
using System.Text;

namespace tdb.common
{
    /// <summary>
    /// 通用帮助类
    /// </summary>
    public class CommHelper
    {
        /// <summary>
        /// 自动帮助拼接上程序根路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFullFileName(string fileName)
        {
            string appDataPath = AppDomain.CurrentDomain.BaseDirectory;
            string fullFileName = Path.Combine(appDataPath, fileName);

            return fullFileName;
        }

        ///// <summary>
        ///// 获取本地IP
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> GetLocalIP()
        //{
        //    var ip = NetworkInterface.GetAllNetworkInterfaces()
        //                .Select(p => p.GetIPProperties())
        //                .SelectMany(p => p.UnicastAddresses)
        //                .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))
        //                //.OrderByDescending(p => p.DuplicateAddressDetectionState)
        //                .FirstOrDefault()?.Address.ToString() ?? "";

        //    return ip;
        //}

        /// <summary>
        /// 反射方式给公开属性或公开字段赋值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyOrFieldName">属性或字段名（区分大小写）</param>
        /// <param name="value">值</param>
        public static void ReflectSet(object obj, string propertyOrFieldName, object? value)
        {
            var type = obj.GetType();

            //先尝试获取属性
            var propertyInfo = type.GetProperty(propertyOrFieldName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value, null);
                return;
            }

            //如果属性获取不到，尝试获取字段
            var fieldInfo = type.GetField(propertyOrFieldName);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
                return;
            }

            throw new Exception($"对象没有名为{propertyOrFieldName}的属性或字段");
        }

        /// <summary>
        /// 反射方式获取公开属性或公开字段值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyOrFieldName">属性或字段名（区分大小写）</param>
        public static object? ReflectGet(object obj, string propertyOrFieldName)
        {
            var type = obj.GetType();

            //先尝试获取属性
            var propertyInfo = type.GetProperty(propertyOrFieldName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj);
            }

            //如果属性获取不到，尝试获取字段
            var fieldInfo = type.GetField(propertyOrFieldName);
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }

            throw new Exception($"对象没有名为{propertyOrFieldName}的属性或字段");
        }

        /// <summary>
        /// emit方式给公开属性赋值（注：只支持属性）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名（区分大小写）</param>
        /// <param name="value">值</param>
        public static void EmitSet<T>(T obj, string propertyName, object? value) where T : class
        {
            var setter = EmitHelper<T>.EmitSetter(propertyName);
            setter(obj, value);
        }

        /// <summary>
        /// emit方式获取公开属性值（注：只支持属性）
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名（区分大小写）</param>
        /// <returns></returns>
        public static object? EmitGet<T>(T obj, string propertyName) where T : class
        {
            var getter = EmitHelper<T>.EmitGetter(propertyName);
            return getter(obj);
        }

        /// <summary>
        /// emit方式获取公开属性值（注：只支持属性）
        /// </summary>
        /// <typeparam name="ObjectT">对象类型</typeparam>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名（区分大小写）</param>
        /// <returns></returns>
        public static ReturnT? EmitGet<ObjectT, ReturnT>(ObjectT obj, string propertyName) where ObjectT : class
        {
            var getter = EmitHelper<ObjectT>.EmitGetter<ReturnT>(propertyName);
            return getter(obj);
        }

        /// <summary>
        /// 对象是否存在某属性或字段
        /// key1：对象类型；key2：属性名
        /// </summary>
        private static readonly Dictionary<string, HashSet<string>> _dicObjProperty = new();
        /// <summary>
        /// 判断对象是否存在某公开属性或公开字段
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyOrFieldName">属性或字段名（区分大小写）</param>
        /// <returns></returns>
        public static bool IsExistPropertyOrField(object obj, string propertyOrFieldName)
        {
            var type = obj.GetType();
            var typeName = type.FullName ?? "";
            if (_dicObjProperty.ContainsKey(typeName) == false)
            {
                //所有属性名
                var proSet = type.GetProperties().Select(m => m.Name).ToHashSet();
                //所有字段名
                var fieldSet = type.GetFields().Select(m => m.Name).ToHashSet();
                //属性名+字段名
                proSet.UnionWith(fieldSet);

                _dicObjProperty[typeName] = proSet;
            }

            var proNames = _dicObjProperty[typeName];
            return proNames.Contains(propertyOrFieldName);
        }
    }
}
