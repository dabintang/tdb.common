using HashidsNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.common
{
    /// <summary>
    /// 对ID进行编码或解码
    /// </summary>
    public class HashIDService : Hashids
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="salt">秘盐</param>
        /// <param name="minHashLength">最小长度</param>
        /// <param name="alphabet">hash字母表</param>
        /// <param name="seps"></param>
        public HashIDService(string salt, int minHashLength = 6, string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890", string seps = "cfhistuCFHISTU")
            : base(salt, minHashLength, alphabet, seps)
        {
        }
    }
}
