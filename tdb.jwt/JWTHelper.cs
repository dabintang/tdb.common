using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using tdb.common;

namespace tdb.jwt
{
    /// <summary>
    /// JWT帮助类
    /// </summary>
    public class JWTHelper
    {
        /// <summary>
        /// 生成token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="payloadInfo">承载信息</param>
        /// <param name="timeoutSeconds">超时时间（单位：秒）</param>
        /// <param name="secret">秘钥</param>
        /// <returns></returns>
        public static string Encode<T>(T payloadInfo, int timeoutSeconds, string secret)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            //用户信息转字段
            var dic = CvtHelper.ToDictionary(payloadInfo, false);
            //超时时间
            var timeOut = CvtHelper.ToTimeStamp(DateTime.Now) + timeoutSeconds;
            dic["exp"] = timeOut;

            var token = encoder.Encode(dic, secret);
            return token;
        }

        /// <summary>
        /// 解析并验证token
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">签名</param>
        /// <param name="secret">秘钥</param>
        /// <returns></returns>
        public static T Decode<T>(string token, string secret)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);

            var json = decoder.Decode(token, secret, true);
            var result = JsonConvert.DeserializeObject<T>(json);

            return result;
        }
    }
}
