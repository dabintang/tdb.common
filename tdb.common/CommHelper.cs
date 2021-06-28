using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
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

        /// <summary>
        /// 获取本地IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            var ip = NetworkInterface.GetAllNetworkInterfaces()
                        .Select(p => p.GetIPProperties())
                        .SelectMany(p => p.UnicastAddresses)
                        .Where(p => p.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(p.Address))
                        .OrderByDescending(p => p.DuplicateAddressDetectionState)
                        .FirstOrDefault()?.Address.ToString();
            return ip;
        }

        /// <summary>
        /// 反射方式给属性赋值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">值</param>
        public static void ReflectSet(object obj, string propertyName, object propertyValue)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(propertyName);
            propertyInfo.SetValue(obj, propertyValue, null);
        }

        /// <summary>
        /// 反射方式获取属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        public static object ReflectGet(object obj, string propertyName)
        {
            var type = obj.GetType();
            var propertyInfo = type.GetProperty(propertyName);
            return propertyInfo.GetValue(obj);
        }

        /// <summary>
        /// emit方式给属性赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="propertyValue">值</param>
        public static void EmitSet<T>(T obj, string propertyName, object propertyValue) where T : class
        {
            var setter = EmitHelper<T>.EmitSetter(propertyName);
            setter(obj, propertyValue);
        }

        /// <summary>
        /// emit方式获取属性值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object EmitGet<T>(T obj, string propertyName) where T : class
        {
            var getter = EmitHelper<T>.EmitGetter(propertyName);
            return getter(obj);
        }

        /// <summary>
        /// emit方式获取属性值
        /// </summary>
        /// <typeparam name="ObjectT">对象类型</typeparam>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static ReturnT EmitGet<ObjectT, ReturnT>(ObjectT obj, string propertyName) where ObjectT : class
        {
            var getter = EmitHelper<ObjectT>.EmitGetter<ReturnT>(propertyName);
            return getter(obj);
        }

        /// <summary>
        /// 对象是否存在某属性
        /// key1：对象类型；key2：属性名
        /// </summary>
        private static Dictionary<string, HashSet<string>> _dicObjProperty = new Dictionary<string, HashSet<string>>();
        /// <summary>
        /// 判断对象是否存在某属性
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static bool IsExistProperty(object obj, string propertyName)
        {
            var type = obj.GetType();
            var typeName = type.FullName;
            if (_dicObjProperty.ContainsKey(typeName) == false)
            {
                _dicObjProperty[typeName] = type.GetProperties().Select(m => m.Name).ToHashSet();
            }

            var proNames = _dicObjProperty[typeName];
            return proNames.Contains(propertyName);
        }
    }
}
