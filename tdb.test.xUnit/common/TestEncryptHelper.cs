using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using tdb.common.Crypto;
using Xunit.Abstractions;

namespace tdb.test.xUnit.common
{
    /// <summary>
    /// 测试 加解密帮助类
    /// </summary>
    public class TestEncryptHelper
    {
        /// <summary>
        /// 输出
        /// </summary>
        private readonly ITestOutputHelper output;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_output"></param>
        public TestEncryptHelper(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// 测试Md5加密
        /// </summary>
        [Fact]
        public void TestMd5()
        {
            var t1 = "abcdefghijklmn";
            var t2 = "abcdefghijklnm";

            var md51 = EncryptHelper.Md5(t1);
            Assert.NotEqual(t1, md51);

            var md52 = EncryptHelper.Md5(t2);
            Assert.NotEqual(t2, md52);

            var md511 = EncryptHelper.Md5(t1);
            Assert.Equal(md51, md511);
        }

        /// <summary>
        /// 测试AES加解密
        /// </summary>
        [Fact]
        public void TestAES()
        {
            var plainText = "abcdef呵呵ghi娃哈哈jklmn";
            var key = "1234567890223456";
            var iv = "1234567890223456";

            //加密
            var encryptText = EncryptHelper.EncryptAES(key, iv, plainText);
            Assert.NotEqual(plainText, encryptText);

            //解密
            var decryptText = EncryptHelper.DecryptAES(key, iv, encryptText);
            Assert.Equal(plainText, decryptText);
        }

        /// <summary>
        /// 测试RSA加解密
        /// </summary>
        [Fact]
        public void TestRSA()
        {
            var plainText = "abcdefghi娃哈哈jklmn";

            //创建RAS秘钥
            var key = EncryptHelper.CreateRSAKey();

            //公钥加密
            var encryptText = EncryptHelper.EncryptRSA(plainText, key.PublicKey);
            Assert.NotEqual(plainText, encryptText);

            //私钥解密
            var decryptText = EncryptHelper.DecryptRSA(encryptText, key.PrivateKey);
            Assert.Equal(plainText, decryptText);

            //私钥签名
            var signText = EncryptHelper.SignRSA(plainText, key.PrivateKey);
            Assert.NotEqual(plainText, signText);

            //公钥验证签名
            var checkSign = EncryptHelper.VerifyRSA(plainText, signText, key.PublicKey);
            Assert.True(checkSign);
        }

        /// <summary>
        /// 和前端联调
        /// </summary>
        [Fact]
        public void TestRSAWithWebFront()
        {
            //RAS秘钥
            var key = new RSAKey();
            key.PublicKey = "-----BEGIN PUBLIC KEY-----\nMIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDbh2tm2dVAqJn + d5r + Dc / JSzvwiZWwySziGfp9TmegUQJJXwezuP2jLz2JEJFBX + gdjE6ArKem1YO3DdJjg4MiNFopEUj9L1f81ErvFKcnLHXOdmgbncbs6pddufw / zCcCO + MPa8CaKlSILUOkB6ldnhSfNf1QiYuCtd / mMm789QIDAQAB\n-----END PUBLIC KEY-----";
            key.PrivateKey = "-----BEGIN RSA PRIVATE KEY-----\nMIICWwIBAAKBgQDbh2tm2dVAqJn + d5r + Dc / JSzvwiZWwySziGfp9TmegUQJJXwezuP2jLz2JEJFBX + gdjE6ArKem1YO3DdJjg4MiNFopEUj9L1f81ErvFKcnLHXOdmgbncbs6pddufw / zCcCO + MPa8CaKlSILUOkB6ldnhSfNf1QiYuCtd / mMm789QIDAQABAoGAfBRUfjONNxiUwaF0tzezRAEEBfad5uguODWawQx5wcFH25Sc09YxdfSTgU8d6qd8mIbfMBdmQwPXiLWFPr3sdXK1qrFCR1la8YlZov2mPv39lksasva9OSSYPqPP67nJetDnWKJQSxssd783dx4IgwEn4lXsMmbrAHdScq72QxkCQQDnbdTSKbSqA6OxklISivhlpjoDT6L1FzuM6PQhM5cogc321u / AEKGgla9s / atvL8BtTUNWgCZ5asyj84kfsExrAkEA8tYmD3TrzevfzYNo1aZGfmgMzt4nu / BzTkYaHntJGkI4OF8PTr2oplUn + 1c / F472Slf0tLEemMOKVaG1ybA0HwJAVLcZrozhu1J1u2yqamtAnkUI + 2lNZ5ZHkD8 + DmFKNeO + N1tai94KrDPe8XCyLpM2R0x / F8z1SIDVxDZDvbvVXQJAd0wFPqqt / WfXdtsL8YnSL99mC2rQEmA6BUYjJr5iV2gYvnjUyFYcODq1faCK + kPdcwBq0yoAYgQOPatH52GM + QJALvU5ox / re69BAzQg6HP52xeem / nSTq1 + M0Q0i48r9xykGKd + a4TCcBAA7NbW00ijEHP4RItX7qVTLt9i4 + pkWg ==\n-----END RSA PRIVATE KEY-----";
            
            //内容
            var text = "start 123 木头人";

            //公钥加密
            var encryptText = EncryptHelper.EncryptRSA(text, key.PublicKey);
            this.output.WriteLine($"公钥加密：{encryptText}");

            //私钥解密
            var decryptText = EncryptHelper.DecryptRSA(encryptText, key.PrivateKey);
            this.output.WriteLine($"私钥解密：{decryptText}");
            var decryptText2 = EncryptHelper.DecryptRSA("u3OAROSZuHX5UQlB8+JGqeSBVbM5MaSS9IXRofjtsRjFj5oD7vwAwuveWAkFUiWOCBEUp/sICAU3KdfGurdDJQmuR7aUESlj4HGnzm4VLjLC2vKNQJmyU5H1ANxl+XYW13eYHGzYlj2WhLenAW8n3FKLEGFq7/tOBs1JzfLoh2k=", key.PrivateKey);
            Assert.Equal(text, decryptText2);

            //私钥签名
            var signText = EncryptHelper.SignRSA(text, key.PrivateKey);
            this.output.WriteLine($"私钥签名：{signText}");

            //公钥验证签名
            var checkSign = EncryptHelper.VerifyRSA(text, signText, key.PublicKey);
            this.output.WriteLine($"公钥验证签名：{checkSign}");
        }
    }
}
