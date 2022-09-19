using JWT.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using tdb.jwt;

namespace tdb.test.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestJWTController : ControllerBase
    {
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        public string CreateToken(JWTUserInfo info)
        {
            var token = JWTHelper.Encode(info, "12345678901234567890", 30);
            return token;
        }

        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public string CheckToken(string token)
        {
            try
            {
                var user = JWTHelper.Decode<JWTUserInfo>(token, "12345678901234567890");
                var result = JsonSerializer.Serialize(user);
                return result;
            }
            catch (TokenExpiredException)
            {
                return "token已过期";
            }
            catch (SignatureVerificationException)
            {
                return "token签名不对";
            }
            catch (Exception)
            {
                return "非法token";
            }
        }
    }

    public class JWTUserInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

}
