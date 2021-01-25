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
                throw new ArgumentNullException("dataType");

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
                   );
        }
    }
}
