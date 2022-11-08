using System;
using System.Collections.Generic;
using System.Text;

namespace tdb.common
{
    /// <summary>
    /// 验证帮助类
    /// </summary>
    public class CheckHelper
    {
        /// <summary>
        /// 是否数值类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static bool IsNumeric(Type dataType)
        {
            if (dataType == null)
                throw new ArgumentNullException(nameof(dataType));

            return (dataType == typeof(decimal)
                    || dataType == typeof(short)
                    || dataType == typeof(int)
                    || dataType == typeof(long)
                    || dataType == typeof(ushort)
                    || dataType == typeof(uint)
                    || dataType == typeof(ulong)
                    || dataType == typeof(sbyte)
                    || dataType == typeof(double)
                    || dataType == typeof(float)
                    || dataType == typeof(decimal?)
                    || dataType == typeof(short?)
                    || dataType == typeof(int?)
                    || dataType == typeof(long?)
                    || dataType == typeof(ushort?)
                    || dataType == typeof(uint?)
                    || dataType == typeof(ulong?)
                    || dataType == typeof(sbyte?)
                    || dataType == typeof(double?)
                    || dataType == typeof(float?)
                   );
        }

        /// <summary>
        /// 是否可空类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static bool IsNullableType(Type dataType)
        {
            return (dataType.IsClass || dataType.IsInterface ||
                (dataType.IsGenericType && dataType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))));
        }

        /// <summary>
        /// 判断类型type是否派生自基类baseType
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="baseType">基类</param>
        /// <returns></returns>
        public static bool IsSubclassOf(Type type, Type baseType)
        {
            //如果基类不是泛型类型
            if (baseType.IsGenericType == false)
            {
                return type.IsSubclassOf(baseType);
            }

            if (type.IsGenericType && (type == baseType || type.GetGenericTypeDefinition() == baseType))
            {
                return true;
            }

            if (type.BaseType != null)
            {
                return IsSubclassOf(type.BaseType, baseType);
            }

            return false;
        }
    }
}
