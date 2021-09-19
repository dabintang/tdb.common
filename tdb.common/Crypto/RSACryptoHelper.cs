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
		/// <summary>
		/// 取得私钥和公钥 XML 格式,返回数组第一个是私钥,第二个是公钥.
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static string[] CreateKeyXml(int size = 1024)
		{
			//密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
			RSACryptoServiceProvider sp = new RSACryptoServiceProvider(size);
			string privateKey = sp.ToXmlString(true);//private key
			string publicKey = sp.ToXmlString(false);//public  key
			return new string[] { privateKey, publicKey };
		}

		/// <summary>
		/// 取得私钥和公钥 CspBlob 格式,返回数组第一个是私钥,第二个是公钥.
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static string[] CreateKeyCspBlob(int size = 1024)
		{
			//密钥格式要生成pkcs#1格式的  而不是pkcs#8格式的
			RSACryptoServiceProvider sp = new RSACryptoServiceProvider(size);
			string privateKey = Convert.ToBase64String(sp.ExportCspBlob(true));//private key
			string publicKey = Convert.ToBase64String(sp.ExportCspBlob(false));//public  key 

			return new string[] { privateKey, publicKey };
		}

		/// <summary>
		/// 导出PEM PKCS#1格式密钥对，返回数组第一个是私钥,第二个是公钥.
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static string[] CreateKey_PEM_PKCS1(int size = 1024)
		{
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
			string privateKey = RSA_PEM.ToPEM(rsa, false, false);
			string publicKey = RSA_PEM.ToPEM(rsa, true, false);
			return new string[] { privateKey, publicKey };
		}

		/// <summary>
		/// 导出PEM PKCS#8格式密钥对，返回数组第一个是私钥,第二个是公钥.
		/// </summary>
		/// <param name="size">密钥长度,默认1024,可以为2048</param>
		/// <returns></returns>
		public static string[] CreateKey_PEM_PKCS8(int size = 1024)
		{
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(size);
			string privateKey = RSA_PEM.ToPEM(rsa, false, true);
			string publicKey = RSA_PEM.ToPEM(rsa, true, true);
			return new string[] { privateKey, publicKey };
		}

		/// <summary>
		/// 加密 用的是PEM格式的密钥
		/// </summary>
		/// <param name="plaintext">要加密的数据</param>
		/// <param name="publicPEMKey">PEM格式的公钥</param>
		/// <returns></returns>
		public static string Encrypt_PEMKey(string plaintext, string publicPEMKey)
		{
			using (RSACryptoServiceProvider RSA = RSA_PEM.FromPEM(publicPEMKey))
			{
				return Encrypt(plaintext, RSA);
			}
		}

		/// <summary>
		/// 加密 用的是Xml格式的密钥
		/// </summary>
		/// <param name="plaintext">要加密的数据</param>
		/// <param name="publicXmlKey">Xml格式的公钥</param>
		/// <returns></returns>
		public static string Encrypt_XmlKey(string plaintext, string publicXmlKey)
		{
			using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
			{
				RSA.FromXmlString(publicXmlKey);//载入公钥
				return Encrypt(plaintext, RSA);
			}
		}

		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="plaintext">要加密的数据</param>
		/// <param name="RSA">RSA服务</param>
		/// <returns></returns>
		private static string Encrypt(string plaintext, RSACryptoServiceProvider RSA)
		{
			var data = Encoding.UTF8.GetBytes(plaintext);
			int buffersize = (RSA.KeySize / 8) - 11;
			var buffer = new byte[buffersize];
			using (MemoryStream input = new MemoryStream(data), output = new MemoryStream())
			{
				while (true)
				{
					int readsize = input.Read(buffer, 0, buffersize);
					if (readsize <= 0)
					{
						break;
					}
					var temp = new byte[readsize];
					Array.Copy(buffer, 0, temp, 0, readsize);
					var EncBytes = RSA.Encrypt(temp, false);
					output.Write(EncBytes, 0, EncBytes.Length);
				}
				return Convert.ToBase64String(output.ToArray());
			}
		}

		/// <summary>
		/// 解密 用的是Xml格式的密钥
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="privateXmlKey">密钥</param>
		/// <returns></returns>
		public static string Decrypt_XmlKey(string ciphertext, string privateXmlKey)
		{
			using (var RSA = new RSACryptoServiceProvider())
			{
				RSA.FromXmlString(privateXmlKey);
				return Decrypt(ciphertext, RSA);
			}
		}

		/// <summary>
		/// 解密 用的是PEM格式的密钥
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="privatePEMKey">密钥</param>
		/// <returns></returns>
		public static string Decrypt_PEMKey(string ciphertext, string privatePEMKey)
		{
			//using (var RSA = new RSACryptoServiceProvider())
			using (var RSA = RSA_PEM.FromPEM(privatePEMKey))
			{
				return Decrypt(ciphertext, RSA);
			}
		}

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="ciphertext">密文</param>
		/// <param name="RSA">RSA服务</param>
		/// <returns></returns>
		private static string Decrypt(string ciphertext, RSACryptoServiceProvider RSA)
		{
			var data = Convert.FromBase64String(ciphertext);

			//RSA.FromXmlString(str_Private_Key);
			int buffersize = RSA.KeySize / 8;
			var buffer = new byte[buffersize];
			using (MemoryStream input = new MemoryStream(data),
				 output = new MemoryStream())
			{
				while (true)
				{
					int readsize = input.Read(buffer, 0, buffersize);
					if (readsize <= 0)
					{
						break;
					}

					var temp = new byte[readsize];
					Array.Copy(buffer, 0, temp, 0, readsize);
					var DecBytes = RSA.Decrypt(temp, false);
					output.Write(DecBytes, 0, DecBytes.Length);
				}
				return Encoding.UTF8.GetString(output.ToArray());
			}
		}
	}
}
