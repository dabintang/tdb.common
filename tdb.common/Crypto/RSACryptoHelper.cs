using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace tdb.common.Crypto
{
	/// <summary>
	/// RAS加解密算法 
	/// 先用 RSACryptoHelper.CreateKey(); 创建一对密钥,第一个是私钥,第二个是公钥. 公钥发给客户端,客户端用js和公钥加密,密文到服务器 用私钥解密
	/// 对应的前台js加密算法库为jsencrypt.js 
	/// 参考 https://cdn.bootcss.com/jsencrypt/3.0.0-beta.1/jsencrypt.js
	/// 参考 https://blog.csdn.net/zhangjianying/article/details/79873392
	/// </summary>
	/// <example>
	/// var keys= RSACryptoHelper.CreateKey();//创建一对密钥,第一个是私钥,第二个是公钥.公钥发给客户端,密文到服务器 用私钥解密
	/// var miwen =   RSACryptoHelper.RSA_Encrypt("被加密的文本",keys[0]);//公钥加密
	/// var yuanwen = RSACryptoHelper.RSA_Decrypt(miwen,keys[1]);//私钥解密
	/// </example> 
	class RSACryptoHelper
	{
		#region 生成秘钥

		/// <summary>
		/// 生成秘钥 XML 格式
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static RSAKey CreateKeyXml(int size = 1024)
		{
			//密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
			var sp = new RSACryptoServiceProvider(size);

            var key = new RSAKey
            {
                PrivateKey = sp.ToXmlString(true),//private key
                PublicKey = sp.ToXmlString(false)//public  key
            };
            return key;
		}

		/// <summary>
		/// 生成秘钥 CspBlob 格式
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static RSAKey CreateKeyCspBlob(int size = 1024)
		{
			//密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
			var sp = new RSACryptoServiceProvider(size);

            var key = new RSAKey
            {
                PrivateKey = Convert.ToBase64String(sp.ExportCspBlob(true)),//private key
                PublicKey = Convert.ToBase64String(sp.ExportCspBlob(false))//public  key 
            };
            return key;
		}

		/// <summary>
		/// 生成秘钥 PEM PKCS#1格式
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static RSAKey CreateKey_PEM_PKCS1(int size = 1024)
		{
			var rsa = new RSACryptoServiceProvider(size);

            var key = new RSAKey
            {
                PrivateKey = RSA_PEM.ToPEM(rsa, false, false),
                PublicKey = RSA_PEM.ToPEM(rsa, true, false)
            };
            return key;
		}

		/// <summary>
		/// 生成秘钥 PEM PKCS#8格式
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static RSAKey CreateKey_PEM_PKCS8(int size = 1024)
		{
			var rsa = new RSACryptoServiceProvider(size);

            var key = new RSAKey
            {
                PrivateKey = RSA_PEM.ToPEM(rsa, false, true),
                PublicKey = RSA_PEM.ToPEM(rsa, true, true)
            };
            return key;
		}

		#endregion

		#region 加密

		/// <summary>
		/// 加密 用的是PEM格式的公钥
		/// </summary>
		/// <param name="plaintext">要加密的数据</param>
		/// <param name="publicPEMKey">公钥</param>
		/// <returns></returns>
		public static string Encrypt_PEMKey(string plaintext, string publicPEMKey)
		{
            using var rsa = RSA_PEM.FromPEM(publicPEMKey);
            return Encrypt(plaintext, rsa);
        }

		/// <summary>
		/// 加密 用的是Xml格式的公钥
		/// </summary>
		/// <param name="plaintext">要加密的数据</param>
		/// <param name="publicXmlKey">公钥</param>
		/// <returns></returns>
		public static string Encrypt_XmlKey(string plaintext, string publicXmlKey)
		{
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicXmlKey);//载入公钥
            return Encrypt(plaintext, rsa);
        }

		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="plaintext">要加密的数据</param>
		/// <param name="rsa">RSA服务</param>
		/// <returns></returns>
		private static string Encrypt(string plaintext, RSACryptoServiceProvider rsa)
		{
			var plainBytes = Encoding.UTF8.GetBytes(plaintext);
			var encBytes = rsa.Encrypt(plainBytes, false);
			return Convert.ToBase64String(encBytes);
		}

		#endregion

		#region 解密

		/// <summary>
		/// 解密 用的是Xml格式的私钥
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="privateXmlKey">私钥</param>
		/// <returns></returns>
		public static string Decrypt_XmlKey(string ciphertext, string privateXmlKey)
		{
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateXmlKey);
            return Decrypt(ciphertext, rsa);
        }

		/// <summary>
		/// 解密 用的是PEM格式的私钥
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="privatePEMKey">私钥</param>
		/// <returns></returns>
		public static string Decrypt_PEMKey(string ciphertext, string privatePEMKey)
		{
            using var rsa = RSA_PEM.FromPEM(privatePEMKey);
            return Decrypt(ciphertext, rsa);
        }

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="rsa">RSA服务</param>
		/// <returns></returns>
		private static string Decrypt(string ciphertext, RSACryptoServiceProvider rsa)
		{
			var cipherBytes = Convert.FromBase64String(ciphertext);
			var decBytes = rsa.Decrypt(cipherBytes, false);
			return Encoding.UTF8.GetString(decBytes);
		}

		#endregion

		#region 签名

		/// <summary>
		/// 签名 用的是PEM格式的私钥 SHA256
		/// </summary>
		/// <param name="text">待签名的数据</param>
		/// <param name="privatePEMKey">私钥</param>
		/// <returns></returns>
		public static string Sign_PEMKey(string text, string privatePEMKey)
		{
            using var rsa = RSA_PEM.FromPEM(privatePEMKey);
            return Sign(text, rsa);
        }

		/// <summary>
		/// 签名 用的是Xml格式的私钥 SHA256
		/// </summary>
		/// <param name="text">待签名的数据</param>
		/// <param name="privateXmlKey">私钥</param>
		/// <returns></returns>
		public static string Sign_XmlKey(string text, string privateXmlKey)
		{
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateXmlKey);//载入私钥
            return Sign(text, rsa);
        }

		/// <summary>
		/// 签名 SHA256
		/// </summary>
		/// <param name="text">待签名的数据</param>
		/// <param name="rsa">RSA服务</param>
		/// <returns></returns>
		private static string Sign(string text, RSACryptoServiceProvider rsa)
		{
			var data = Encoding.UTF8.GetBytes(text);
			var sha = SHA256.Create();
			var signBytes = rsa.SignData(data, sha);
			return Convert.ToBase64String(signBytes);
		}

		#endregion

		#region 验证签名

		/// <summary>
		/// 验证签名 用的是Xml格式的公钥 SHA256
		/// </summary>
		/// <param name="text">未签名的数据</param>
		/// <param name="signedText">已签名文本</param>
		/// <param name="publicXmlKey">公钥</param>
		/// <returns></returns>
		public static bool Verify_XmlKey(string text, string signedText, string publicXmlKey)
		{
            using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicXmlKey);
            return Verify(text, signedText, rsa);
        }

		/// <summary>
		/// 验证签名 用的是PEM格式的公钥 SHA256
		/// </summary>
		/// <param name="text">未签名的数据</param>
		/// <param name="signedText">已签名文本</param>
		/// <param name="publicPEMKey">公钥</param>
		/// <returns></returns>
		public static bool Verify_PEMKey(string text, string signedText, string publicPEMKey)
		{
            using var rsa = RSA_PEM.FromPEM(publicPEMKey);
            return Verify(text, signedText, rsa);
        }

		/// <summary>
		/// 验证签名 SHA256
		/// </summary>
		/// <param name="text">未签名的数据</param>
		/// <param name="signedText">已签名文本</param>
		/// <param name="rsa">RSA服务</param>
		/// <returns></returns>
		private static bool Verify(string text, string signedText, RSACryptoServiceProvider rsa)
		{
			var data = Encoding.UTF8.GetBytes(text);
			var signedData = Convert.FromBase64String(signedText);
			var sha = SHA256.Create();
			return rsa.VerifyData(data, sha, signedData);
		}

		#endregion
	}
}
