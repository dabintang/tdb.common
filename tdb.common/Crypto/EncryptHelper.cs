using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace tdb.common.Crypto
{
    /// <summary>
    /// 加解密帮助类
    /// </summary>
    public class EncryptHelper
    {
        #region MD5

        /// <summary>
        /// MD5　32位加密
        /// </summary>
        /// <param name="str">需要加密的字符串</param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            var val = BitConverter.ToString(s);
            val = val.Replace("-", "");
            return val;
        }

        #endregion

        #region AES

        /// <summary>  
        /// AES加密
        /// </summary>  
        /// <param name="key">秘钥（支持长度：16、24、32）</param>
        /// <param name="iv">向量（支持长度：16）</param>
        /// <param name="plainText">明文字符串</param>
        /// <returns>加密后字符串</returns>
        public static string EncryptAES(string key, string iv, string plainText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt);
            swEncrypt.Write(plainText);
            swEncrypt.Close();
            byte[] bytes = msEncrypt.ToArray();

            return ByteArrayToHexString(bytes);
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="key">秘钥（支持长度：16、24、32）</param>
        /// <param name="iv">向量（支持长度：16）</param>
        /// <param name="text">密文字节数组</param>  
        /// <returns>返回解密后的字符串</returns>  
        public static string DecryptAES(string key, string iv, string text)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using var msEncrypt = new MemoryStream(HexStringToByteArray(text));
            using var csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read);
            using var srEncrypt = new StreamReader(csEncrypt);
            return srEncrypt.ReadToEnd();
        }

        /// <summary>
        /// 将指定的16进制字符串转换为byte数组
        /// </summary>
        /// <param name="s">16进制字符串(如：“7F 2C 4A”或“7F2C4A”都可以)</param>
        /// <returns>16进制字符串对应的byte数组</returns>
        private static byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }

            return buffer;
        }

        /// <summary>
        /// 将一个byte数组转换成一个格式化的16进制字符串
        /// </summary>
        /// <param name="data">byte数组</param>
        /// <returns>格式化的16进制字符串</returns>
        private static string ByteArrayToHexString(byte[] data)
        {
            var sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                //16进制数字
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            }
            return sb.ToString().ToUpper();
        }

        #endregion

        #region RSA

        /// <summary>
        /// 创建RAS秘钥（PEM PKCS#1格式格式秘钥）
        /// </summary>
        /// <param name="size">密钥长度,默认1024,可以为2048</param>
        /// <returns></returns>
        public static RSAKey CreateRSAKey(int size = 1024)
        {
            //生成秘钥 PEM PKCS#1格式
            return RSACryptoHelper.CreateKey_PEM_PKCS1(size);
        }

        /// <summary>
        /// RAS公钥加密（PEM格式格式秘钥）
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static string EncryptRSA(string plaintext, string publicKey)
        {
            return RSACryptoHelper.Encrypt_PEMKey(plaintext, publicKey);
        }

        /// <summary>
        /// RAS私钥解密（PEM格式格式秘钥）
        /// </summary>
        /// <param name="ciphertext">密文</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string DecryptRSA(string ciphertext, string privateKey)
        {
            return RSACryptoHelper.Decrypt_PEMKey(ciphertext, privateKey);
        }

        /// <summary>
        /// RAS私钥签名（PEM格式格式秘钥）
        /// </summary>
        /// <param name="text">待签名的数据</param>
        /// <param name="privateKey">私钥</param>
        /// <returns></returns>
        public static string SignRSA(string text, string privateKey)
        {
            return RSACryptoHelper.Sign_PEMKey(text, privateKey);
        }

        /// <summary>
        /// RAS私钥验证签名（PEM格式格式秘钥）
        /// </summary>
        /// <param name="text">待签名的数据</param>
        /// <param name="signedText">已签名文本</param>
        /// <param name="publicKey">公钥</param>
        /// <returns></returns>
        public static bool VerifyRSA(string text, string signedText, string publicKey)
        {
            return RSACryptoHelper.Verify_PEMKey(text, signedText, publicKey);
        }

        #endregion
    }
}
