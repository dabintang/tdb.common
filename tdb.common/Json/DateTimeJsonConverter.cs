using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace tdb.common.Json
{
    /// <summary>
	/// json日期格式化
	/// </summary>
	public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 序列化格式
        /// </summary>
        private readonly string format;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_format">序列化格式</param>
        public DateTimeJsonConverter(string _format)
        {
            this.format = _format;
        }

        /// <summary>
        /// 序列化为json字符串
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="date"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
        {
            writer.WriteStringValue(date.ToString(format));
        }

        /// <summary>
        /// 读取json字符串并将其转换为日期类型
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateTime.TryParseExact(reader.GetString(), format, null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return date;
            }

            return reader.GetDateTime();
        }
    }
}
