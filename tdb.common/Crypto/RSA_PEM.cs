﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace tdb.common.Crypto
{
	/// <summary>
	/// RSA PEM格式秘钥对的解析和导出
	/// </summary>
	public class RSA_PEM
	{
		private static readonly Regex _PEMCode = new(@"--+.+?--+|\s+");
		private static readonly byte[] _SeqOID = new byte[] { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
		private static readonly byte[] _Ver = new byte[] { 0x02, 0x01, 0x00 };

		/// <summary>
		/// 用PEM格式密钥对创建RSA，支持PKCS#1、PKCS#8格式的PEM
		/// </summary>
		/// <param name="pemKey">PEM格式密钥</param>
		/// <returns></returns>
		public static RSACryptoServiceProvider FromPEM(string pemKey)
		{
			//var rsaParams = new CspParameters();
			//rsaParams.Flags = CspProviderFlags.UseMachineKeyStore;
			//var rsa = new RSACryptoServiceProvider(rsaParams);
			var rsa = new RSACryptoServiceProvider();

			var param = new RSAParameters();

			var base64 = _PEMCode.Replace(pemKey, "");
			var data = Base64DecodeBytes(base64);
			if (data == null)
			{
				throw new Exception("PEM内容无效");
			}
			var idx = 0;

            //读取长度
            int readLen(byte first)
            {
                if (data[idx] == first)
                {
                    idx++;
                    if (data[idx] == 0x81)
                    {
                        idx++;
                        return data[idx++];
                    }
                    else if (data[idx] == 0x82)
                    {
                        idx++;
                        return (((int)data[idx++]) << 8) + data[idx++];
                    }
                    else if (data[idx] < 0x80)
                    {
                        return data[idx++];
                    }
                }
                throw new Exception("PEM未能提取到数据");
            }
            //读取块数据
            byte[] readBlock()
            {
                var len = readLen(0x02);
                if (data[idx] == 0x00)
                {
                    idx++;
                    len--;
                }
                var val = Sub(data, idx, len);
                idx += len;
                return val;
            }
            //比较data从idx位置开始是否是byts内容
            bool eq(byte[] byts)
            {
                for (var i = 0; i < byts.Length; i++, idx++)
                {
                    if (idx >= data.Length)
                    {
                        return false;
                    }
                    if (byts[i] != data[idx])
                    {
                        return false;
                    }
                }
                return true;
            }

            if (pemKey.Contains("PUBLIC KEY"))
			{
				/****使用公钥****/
				//读取数据总长度
				readLen(0x30);
				if (!eq(_SeqOID))
				{
					throw new Exception("PEM未知格式");
				}
				//读取1长度
				readLen(0x03);
				idx++;//跳过0x00
					  //读取2长度
				readLen(0x30);

				//Modulus
				param.Modulus = readBlock();

				//Exponent
				param.Exponent = readBlock();
			}
			else if (pemKey.Contains("PRIVATE KEY"))
			{
				/****使用私钥****/
				//读取数据总长度
				readLen(0x30);

				//读取版本号
				if (!eq(_Ver))
				{
					throw new Exception("PEM未知版本");
				}

				//检测PKCS8
				var idx2 = idx;
				if (eq(_SeqOID))
				{
					//读取1长度
					readLen(0x04);
					//读取2长度
					readLen(0x30);

					//读取版本号
					if (!eq(_Ver))
					{
						throw new Exception("PEM版本无效");
					}
				}
				else
				{
					idx = idx2;
				}

				//读取数据
				param.Modulus = readBlock();
				param.Exponent = readBlock();
				param.D = readBlock();
				param.P = readBlock();
				param.Q = readBlock();
				param.DP = readBlock();
				param.DQ = readBlock();
				param.InverseQ = readBlock();
			}
			else
			{
				throw new Exception("pem需要BEGIN END标头");
			}

			rsa.ImportParameters(param);
			return rsa;
		}

		/// <summary>
		/// 将RSA中的密钥对转换成PEM格式，usePKCS8=false时返回PKCS#1格式，否则返回PKCS#8格式，如果convertToPublic含私钥的RSA将只返回公钥，仅含公钥的RSA不受影响
		/// </summary>
		/// <param name="rsa"></param>
		/// <param name="convertToPublic">是否转为公钥</param>
		/// <param name="usePKCS8"></param>
		/// <returns></returns>
		public static string ToPEM(RSACryptoServiceProvider rsa, bool convertToPublic, bool usePKCS8)
		{
			//https://www.jianshu.com/p/25803dd9527d
			//https://www.cnblogs.com/ylz8401/p/8443819.html
			//https://blog.csdn.net/jiayanhui2877/article/details/47187077
			//https://blog.csdn.net/xuanshao_/article/details/51679824
			//https://blog.csdn.net/xuanshao_/article/details/51672547

			var ms = new MemoryStream();
            //写入一个长度字节码
            void writeLenByte(int len)
            {
                if (len < 0x80)
                {
                    ms.WriteByte((byte)len);
                }
                else if (len <= 0xff)
                {
                    ms.WriteByte(0x81);
                    ms.WriteByte((byte)len);
                }
                else
                {
                    ms.WriteByte(0x82);
                    ms.WriteByte((byte)(len >> 8 & 0xff));
                    ms.WriteByte((byte)(len & 0xff));
                }
            }
            //写入一块数据
            void writeBlock(byte[]? byts)
            {
                if (byts == null) throw new Exception("将RSA中的密钥对转换成PEM格式时发生异常.");
                var addZero = (byts[0] >> 4) >= 0x8;
                ms.WriteByte(0x02);
                var len = byts.Length + (addZero ? 1 : 0);
                writeLenByte(len);

                if (addZero)
                {
                    ms.WriteByte(0x00);
                }
                ms.Write(byts, 0, byts.Length);
            }
            //根据后续内容长度写入长度数据
            byte[] writeLen(int index, byte[] byts)
            {
                var len = byts.Length - index;

                ms.SetLength(0);
                ms.Write(byts, 0, index);
                writeLenByte(len);
                ms.Write(byts, index, len);

                return ms.ToArray();
            }

            if (rsa.PublicOnly || convertToPublic)
			{
				/****生成公钥****/
				var param = rsa.ExportParameters(false);

				//写入总字节数，不含本段长度，额外需要24字节的头，后续计算好填入
				ms.WriteByte(0x30);
				var index1 = (int)ms.Length;

				//固定内容
				// encoded OID sequence for PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
				ms.Write(_SeqOID, 0, _SeqOID.Length);

				//从0x00开始的后续长度
				ms.WriteByte(0x03);
				var index2 = (int)ms.Length;
				ms.WriteByte(0x00);

				//后续内容长度
				ms.WriteByte(0x30);
				var index3 = (int)ms.Length;

				//写入Modulus
				writeBlock(param.Modulus);

				//写入Exponent
				writeBlock(param.Exponent);

				//计算空缺的长度
				var byts = ms.ToArray();

				byts = writeLen(index3, byts);
				byts = writeLen(index2, byts);
				byts = writeLen(index1, byts);

				return "-----BEGIN PUBLIC KEY-----\n" + Convert.ToBase64String(byts) + "\n-----END PUBLIC KEY-----";
			}
			else
			{
				/****生成私钥****/
				var param = rsa.ExportParameters(true);

				//写入总字节数，后续写入
				ms.WriteByte(0x30);
				int index1 = (int)ms.Length;

				//写入版本号
				ms.Write(_Ver, 0, _Ver.Length);

				//PKCS8 多一段数据
				int index2 = -1, index3 = -1;
				if (usePKCS8)
				{
					//固定内容
					ms.Write(_SeqOID, 0, _SeqOID.Length);

					//后续内容长度
					ms.WriteByte(0x04);
					index2 = (int)ms.Length;

					//后续内容长度
					ms.WriteByte(0x30);
					index3 = (int)ms.Length;

					//写入版本号
					ms.Write(_Ver, 0, _Ver.Length);
				}

				//写入数据
				writeBlock(param.Modulus);
				writeBlock(param.Exponent);
				writeBlock(param.D);
				writeBlock(param.P);
				writeBlock(param.Q);
				writeBlock(param.DP);
				writeBlock(param.DQ);
				writeBlock(param.InverseQ);

				//计算空缺的长度
				var byts = ms.ToArray();

				if (index2 != -1)
				{
					byts = writeLen(index3, byts);
					byts = writeLen(index2, byts);
				}
				byts = writeLen(index1, byts);

				var flag = " PRIVATE KEY";
				if (!usePKCS8)
				{
					flag = " RSA" + flag;
				}
				return "-----BEGIN" + flag + "-----\n" + Convert.ToBase64String(byts) + "\n-----END" + flag + "-----";
			}
		}

		/// <summary>
		/// base64解码
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static byte[]? Base64DecodeBytes(string str)
		{
			try
			{
				return Convert.FromBase64String(str);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 从数组start开始到指定长度复制一份
		/// </summary>
		public static T[] Sub<T>(T[] arr, int start, int count)
		{
			T[] val = new T[count];
			for (var i = 0; i < count; i++)
			{
				val[i] = arr[start + i];
			}
			return val;
		}
	}
}
