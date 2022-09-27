using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using tdb.common.Enums;

namespace tdb.common
{
    /// <summary>
    /// 转换帮助类
    /// </summary>
    public static class CvtHelper
    {
        /// <summary>
        /// 对象转换为字典
        /// </summary>
        /// <param name="obj">待转化的对象</param>
        /// <param name="isIgnoreNull">是否忽略NULL</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this object obj, bool isIgnoreNull = false)
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
        public static long ToTimeStamp(this DateTime clientTime, EnumAccurateUTC accurate = EnumAccurateUTC.Second)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            switch (accurate)
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
        public static DateTime TimeStampToTime(this long timeStamp, EnumAccurateUTC accurate = EnumAccurateUTC.Second)
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
        public static long DayToSecond(this decimal day)
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
        public static string ToStr<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }

        /// <summary>
        /// 深度复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T DeepClone<T>(this T value)
        {
            var jsonStr = JsonSerializer.Serialize(value, new JsonSerializerOptions() { IncludeFields = true });
            return JsonSerializer.Deserialize<T>(jsonStr, new JsonSerializerOptions() { IncludeFields = true });
        }

        /// <summary>
        /// 默认配置
        /// </summary>
        public static JsonSerializerOptions DefaultOptions = new JsonSerializerOptions()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, //编码：汉子、HTML代码等原样序列化
            IncludeFields = true, //包含变量字段
            PropertyNameCaseInsensitive = true, //属性名不区分大小写
            PropertyNamingPolicy = null, //属性名原样输出（不改变大小写）
        };

        /// <summary>
        /// 序列化为json字符串
        /// </summary>
        /// <typeparam name="TValue">要序列化的值的类型</typeparam>
        /// <param name="value">需要序列号的值</param>
        /// <param name="options">用于控制序列化行为的选项</param>
        /// <returns>josn字符串</returns>
        public static string SerializeJson<TValue>(this TValue value, JsonSerializerOptions options = null)
        {
            //如果未指定序列化选项，使用默认序列化选项
            options = options ?? DefaultOptions;

            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// json字符串反序列化为对象
        /// </summary>
        /// <typeparam name="TValue">反序列化目标类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <param name="options">用于控制反序列化行为的选项</param>
        /// <returns>目标对象</returns>
        public static TValue DeserializeJson<TValue>(this string json, JsonSerializerOptions? options = null)
        {
            //如果未指定序列化选项，使用默认序列化选项
            options = options ?? DefaultOptions;

            return JsonSerializer.Deserialize<TValue>(json, options);
        }

        /// <summary>
        /// json字符串反序列化为对象
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <param name="returnType">反序列化目标类型</param>
        /// <param name="options">用于控制反序列化行为的选项</param>
        /// <returns>目标对象</returns>
        public static object DeserializeJson(this string json, Type returnType, JsonSerializerOptions options = null)
        {
            //如果未指定序列化选项，使用默认序列化选项
            options = options ?? DefaultOptions;

            return JsonSerializer.Deserialize(json, returnType, options);
        }
    }
}


#region 用表达式实现高效深度复制设想（目前数组列表字典等集合类型未实现）
//    /// <summary>
//    /// 
//    /// </summary>
//    public class CvtHelper2
//    {
//        /// <summary>
//        /// 深度克隆
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="value">需复制对象</param>
//        /// <returns>复制后新对象</returns>
//        public static T DeepCopy<T>(T value)
//        {
//            //null直接返回
//            if (value == null)
//            {
//#pragma warning disable CS8603 // 可能返回 null 引用。
//                return default;
//#pragma warning restore CS8603 // 可能返回 null 引用。
//            }

//            //泛型类型
//            var type = typeof(T);

//            #region string、基元类型、decimal、DateTime、enum 及这些类型的可空类型 直接返回即可

//            //（基元类型为bool、byte、sbyte、short、ushort、int、uint、long、ulong、IntPtr、UIntPtr、char、double、float）
//            if (value is string || type.IsPrimitive || value is decimal || value is DateTime || type.IsEnum || value is decimal? || value is DateTime?)
//            {
//                return value;
//            }

//            //可空类型
//            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) && type is TypeInfo typeInfo)
//            {
//                //内部类型
//                var valueType = typeInfo.DeclaredProperties.FirstOrDefault(m => m.Name == "Value");
//                if (valueType != null && (valueType.PropertyType.IsPrimitive || valueType.PropertyType.IsEnum))
//                {
//                    return value;
//                }
//            }

//            #endregion

//            var copyValue = TransExp<T, T>.Trans(value);
//            return copyValue;
//        }
//    }

//    /// <summary>
//    /// 把TFrom转换为TTo的表达式
//    /// </summary>
//    /// <typeparam name="TFrom">转换前类型</typeparam>
//    /// <typeparam name="TTo">转换后类型（注：需有无参构造函数）</typeparam>
//    public static class TransExp<TFrom, TTo>
//    {
//        /// <summary>
//        /// 转换方法
//        /// </summary>
//        private static readonly Func<TFrom, TTo> funcCvt = GetFunc();

//        /// <summary>
//        /// 生成转换方法
//        /// </summary>
//        /// <returns></returns>
//        private static Func<TFrom, TTo> GetFunc()
//        {
//            //参数表达式
//            var parameterExpression = Expression.Parameter(typeof(TFrom), typeof(TFrom).Name);
//            //初始化表达式
//            var memberInitExpression = GetMemberInitExpression(typeof(TFrom), typeof(TTo), parameterExpression);
//            Expression<Func<TFrom, TTo>> lambda = Expression.Lambda<Func<TFrom, TTo>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

//            return lambda.Compile();
//        }

//        /// <summary>
//        /// 获取成员初始化表达式
//        /// </summary>
//        /// <param name="typeFrom">源类型</param>
//        /// <param name="typeTo">目标类型</param>
//        /// <param name="parameterExpression">参数表达式</param>
//        /// <returns></returns>
//        private static MemberInitExpression GetMemberInitExpression(Type typeFrom, Type typeTo, Expression parameterExpression)
//        {
//            //如果目标类型是否集合类型
//            if (typeTo == (typeof(ICollection<>)))
//            {
//                //如果源类型不是集合类型，返回空集合
//                if (typeFrom != (typeof(ICollection<>)))
//                {
//                    return Expression.MemberInit(Expression.New(typeTo));
//                }


//            }

//            //源类型成员
//            var lstMemberFrom = new List<MemberInfo>();
//            lstMemberFrom.AddRange(typeFrom.GetProperties().Where(m => m.CanWrite));
//            lstMemberFrom.AddRange(typeFrom.GetFields().Where(m => m.IsPublic));
//            //目标类型成员
//            var lstMemberTo = new List<MemberInfo>();
//            lstMemberTo.AddRange(typeTo.GetProperties().Where(m => m.CanWrite));
//            lstMemberTo.AddRange(typeTo.GetFields().Where(m => m.IsPublic));

//            //目标成员绑定信息
//            var lstMemberBindingTo = new List<MemberBinding>();
//            foreach (var itemTo in lstMemberTo)
//            {
//                var memberFrom = lstMemberFrom.Find(m => m.Name == itemTo.Name);
//                if (memberFrom == null) continue;

//                //获取源类型访问字段或属性表达式
//                var memberExpressionFrom = Expression.PropertyOrField(parameterExpression, itemTo.Name);

//                //判断成员是否为对象类型
//                var (isItemToObject, itemTypeTo) = IsObject(itemTo);
//                if (isItemToObject) 
//                {
//                    //获取成员初始化表达式
//                    var itemInitExpression = GetObjectBindingExpression(memberExpressionFrom.Type, itemTypeTo, memberExpressionFrom);
//                    lstMemberBindingTo.Add(Expression.Bind(itemTo, itemInitExpression));
//                }
//                else
//                {
//                    lstMemberBindingTo.Add(Expression.Bind(itemTo, memberExpressionFrom));
//                }
//            }

//            var memberInitExpression = Expression.MemberInit(Expression.New(typeTo), lstMemberBindingTo.ToArray());
//            return memberInitExpression;
//        }

//        /// <summary>
//        /// 判断成员是否为对象类型，并返回成员类型
//        /// </summary>
//        /// <param name="member"></param>
//        /// <returns></returns>
//        private static (bool, Type) IsObject(MemberInfo member)
//        {
//            Type type = typeof(object);
//            if (member is PropertyInfo propertyInfo)
//            {
//                type = propertyInfo.PropertyType;
//            }
//            else if (member is FieldInfo fieldInfo)
//            {
//                type = fieldInfo.FieldType;
//            }

//            return (!type.IsValueType && type != typeof(string), type);
//        }

//        /// <summary>
//        /// 获取对象赋值表达式
//        /// </summary>
//        /// <param name="typeFrom">源类型</param>
//        /// <param name="typeTo">目标类型</param>
//        /// <param name="memberExpressionFrom">源类型成员表达式</param>
//        /// <returns></returns>
//        private static Expression GetObjectBindingExpression(Type typeFrom, Type typeTo, MemberExpression memberExpressionFrom)
//        {
//            //字段 输入对象NULL时，对应的输出对像值
//            ConstantExpression nullTinProperty = Expression.Constant(null, typeTo);
//            ConstantExpression nullObeject = Expression.Constant(null, typeof(object));

//            //比较，判断输入对象是否为 null
//            BinaryExpression binaryExpression = Expression.Equal(memberExpressionFrom, nullObeject);

//            //非null时，创建一个新对象
//            MemberInitExpression subMemberInitExpression = GetMemberInitExpression(typeFrom, typeTo, memberExpressionFrom);

//            //if(tin.propertyinfo==null,null,new tin.propertyinfo{...})
//            return Expression.Condition(binaryExpression, nullTinProperty, subMemberInitExpression);
//        }

//        /// <summary>
//        /// 转换
//        /// </summary>
//        /// <param name="from">转换前对象</param>
//        /// <returns>转换后对象</returns>
//        public static TTo Trans(TFrom from)
//        {
//            return funcCvt(from);
//        }
//    }
#endregion