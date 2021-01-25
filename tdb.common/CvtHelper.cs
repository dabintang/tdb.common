using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using tdb.common.Enums;

namespace tdb.common
{
    /// <summary>
    /// 转换帮助类
    /// </summary>
    public class CvtHelper
    {
        /// <summary>
        /// 对象转换为字典
        /// </summary>
        /// <param name="obj">待转化的对象</param>
        /// <param name="isIgnoreNull">是否忽略NULL</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(object obj, bool isIgnoreNull)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型
            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    var value = m.Invoke(obj, new object[] { });

                    // 进行判NULL处理 
                    if (value != null || !isIgnoreNull)
                    {
                        map.Add(p.Name, value); // 向字典添加元素
                    }
                }
            }
            return map;
        }

        /// <summary>
        /// 转UTC时间戳，单位（秒）
        /// </summary>
        /// <param name="clientTime">本地时间</param>
        /// <param name="accurate">时间戳精度（默认：秒）</param>
        /// <returns></returns>
        public static long ToTimeStamp(DateTime clientTime, EnumAccurateUTC accurate = EnumAccurateUTC.Second)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            switch(accurate)
            {
                case EnumAccurateUTC.Second:
                    return Convert.ToInt64((clientTime - startTime).TotalSeconds);
                case EnumAccurateUTC.Millisecond:
                    return Convert.ToInt64((clientTime - startTime).TotalMilliseconds);
                default:
                    throw new Exception($"无法识别的UTC时间戳精度：{accurate}");
            }
        }

        /// <summary>
        /// UTC时间戳转为时间
        /// </summary>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="accurate">时间戳精度（默认：秒）</param>
        /// <returns>返回一个日期时间</returns>
        public static DateTime TimeStampToTime(long timeStamp, EnumAccurateUTC accurate = EnumAccurateUTC.Second)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            switch (accurate)
            {
                case EnumAccurateUTC.Second:
                    return startTime.AddTicks(timeStamp * 10000000);
                case EnumAccurateUTC.Millisecond:
                    return startTime.AddTicks(timeStamp * 10000);
                default:
                    throw new Exception($"无法识别的UTC时间戳精度：{accurate}");
            }
        }

        /// <summary>
        /// 天数转秒数
        /// </summary>
        /// <param name="day">天数</param>
        /// <returns></returns>
        public static long DayToSecond(decimal day)
        {
            return Convert.ToInt64(day * 86400);
        }

        /// <summary>
        /// 转字符串
        /// （如为null，返回空字符串）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToStr<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }
    }
}
