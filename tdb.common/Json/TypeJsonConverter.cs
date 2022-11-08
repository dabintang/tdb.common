using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace tdb.common.Json
{
    /// <summary>
    /// Type类型的json序列化
    /// </summary>
    public class TypeJsonConverter : JsonConverter<Type>
    {
        /// <summary>
        /// 序列化为json字符串
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.AssemblyQualifiedName);
        }

        /// <summary>
        /// 读取json字符串并将其转换为Type类型
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override Type? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var typeName = reader.GetString();
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }

            var type = Type.GetType(typeName);

            return type;
        }
    }
}
