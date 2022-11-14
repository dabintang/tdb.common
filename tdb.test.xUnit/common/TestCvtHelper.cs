using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.test.xUnit.common
{
    /// <summary>
    /// 测试 CvtHelper 类
    /// </summary>
    public class TestCvtHelper
    {
        /// <summary>
        /// 测试方法 ToDictionary
        /// </summary>
        [Fact]
        public void TestToDictionary()
        {
            //获取对象
            var obj = GetObject();

            //对象（公共属性及公共字段）转换为字典
            var dic1 = obj.ToDictionary(true);

            Assert.True(dic1.ContainsKey("Name"));
            Assert.False(dic1.ContainsKey("Age"));
            Assert.True(dic1.ContainsKey("BirthDate"));
            Assert.True(dic1.ContainsKey("Amount"));
            Assert.False(dic1.ContainsKey("Secret"));
            Assert.Equal(dic1["Name"], obj.Name);
            Assert.Equal(dic1["BirthDate"], obj.BirthDate);
            Assert.Equal(dic1["Amount"], obj.Amount);

            obj.Name = String.Empty;
            obj.Amount = null;
            //对象（公共属性及公共字段）转换为字典
            var dic2 = obj.ToDictionary(true);
            Assert.True(dic2.ContainsKey("Name"));
            Assert.False(dic2.ContainsKey("Age"));
            Assert.True(dic2.ContainsKey("BirthDate"));
            Assert.False(dic2.ContainsKey("Amount"));
            Assert.False(dic2.ContainsKey("Secret"));
            Assert.Equal(dic2["Name"], obj.Name);
            Assert.Equal(dic2["BirthDate"], obj.BirthDate);

            obj.Name = null;
            var dic3 = obj.ToDictionary(true);
            Assert.False(dic3.ContainsKey("Name"));
            Assert.False(dic3.ContainsKey("Age"));
            Assert.True(dic3.ContainsKey("BirthDate"));
            Assert.False(dic3.ContainsKey("Amount"));
            Assert.False(dic3.ContainsKey("Secret"));
            Assert.Equal(dic3["BirthDate"], obj.BirthDate);

            var dic4 = obj.ToDictionary(false);
            Assert.True(dic4.ContainsKey("Name"));
            Assert.False(dic4.ContainsKey("Age"));
            Assert.True(dic4.ContainsKey("BirthDate"));
            Assert.True(dic4.ContainsKey("Amount"));
            Assert.False(dic4.ContainsKey("Secret"));
            Assert.Equal(dic4["Name"], obj.Name);
            Assert.Equal(dic4["BirthDate"], obj.BirthDate);
            Assert.Equal(dic4["Amount"], obj.Amount);
        }

        /// <summary>
        /// 测试方法 ToTimeStamp
        /// </summary>
        [Fact]
        public void TestToTimeStamp()
        {
            var time = new DateTime(2022, 10, 13, 19, 53, 36, 258);

            var timeStampMillisecond = time.ToTimeStamp(tdb.common.Enums.EnumAccurateUTC.Millisecond);
            Assert.Equal(1665662016258, timeStampMillisecond);

            var timeStampSecond = time.ToTimeStamp(tdb.common.Enums.EnumAccurateUTC.Second);
            Assert.Equal(1665662016, timeStampSecond);
        }

        /// <summary>
        /// 测试方法 TimeStampToTime
        /// </summary>
        [Fact]
        public void TestTimeStampToTime()
        {
            var timeMillisecond = 1665662016258.TimeStampToTime(tdb.common.Enums.EnumAccurateUTC.Millisecond);
            Assert.Equal(new DateTime(2022, 10, 13, 19, 53, 36, 258), timeMillisecond);

            var timeSecond = 1665662016L.TimeStampToTime(tdb.common.Enums.EnumAccurateUTC.Second);
            Assert.Equal(new DateTime(2022, 10, 13, 19, 53, 36), timeSecond);
        }

        /// <summary>
        /// 测试方法 ToStr
        /// </summary>
        [Fact]
        public void TestToStr()
        {
            string name = null;
            Assert.Equal("", name.ToStr());

            string name2 = "实时sf1313";
            Assert.Equal(name2, name2.ToStr());
        }

        /// <summary>
        /// 测试方法 DeepClone
        /// </summary>
        [Fact]
        public void TestDeepClone()
        {
            //获取对象
            var obj = GetObject();
            //深度复制
            var copyObj = obj.DeepClone();
            Assert.Equal(obj.SerializeJson(), copyObj.SerializeJson());

            var text = "娃哈哈123abc";
            //深度复制
            var copyText = text.DeepClone();
            Assert.Equal(text, copyText);

            var now = DateTime.Now;
            //深度复制
            var copyNow = now.DeepClone();
            Assert.Equal(now.ToString("yyyyMMddHHmmssfff"), copyNow.ToString("yyyyMMddHHmmssfff"));
        }

        /// <summary>
        /// 测试方法 SerializeJson
        /// </summary>
        [Fact]
        public void TestSerializeJson()
        {
            //获取对象
            var obj = GetObject();
            obj.BirthDate = new DateTime(2022, 10, 14, 9, 12, 47);
            //序列化成json字符串
            var jsonStr = obj.SerializeJson();
            var jsonText = "{\"Name\":\"张三\",\"BirthDate\":\"2022-10-14 09:12:47.000\",\"Amount\":44,\"TypeField\":\"System.String, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\"Age\":33}";
            Assert.Equal(jsonStr, jsonText);

            InnerTestCvtHelper obj2 = null;
            var jsonStr2 = obj2.SerializeJson();
            Assert.Equal("null", jsonStr2);
        }

        /// <summary>
        /// 测试方法 DeserializeJson<TValue>
        /// </summary>
        [Fact]
        public void TestDeserializeJson1()
        {
            //json字符串1
            var jsonText1 = "{\"Name\":\"张三\",\"BirthDate\":\"2022/10/14 09:12:47\",\"Amount\":44,\"TypeField\":\"System.String, System.Private.CoreLib, Version=7.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e\",\"Age\":33}";
            //反序列化
            var obj1 = jsonText1.DeserializeJson<InnerTestCvtHelper>();
            Assert.Equal("张三", obj1.Name);
            Assert.Equal(new DateTime(2022, 10, 14, 9, 12, 47), obj1.BirthDate);
            Assert.Equal(44, obj1.Amount);
            Assert.Equal(33, obj1.Age);
            Assert.Equal(typeof(string), obj1.TypeField);

            //json字符串2
            var jsonText2 = "{\"Name\":\"张三\",\"BirthDate\":\"2022-10-14\",\"Amount\":null,\"Age\":22}";
            //反序列化
            var obj2 = jsonText2.DeserializeJson<InnerTestCvtHelper>();
            Assert.Equal("张三", obj2.Name);
            Assert.Equal(new DateTime(2022, 10, 14), obj2.BirthDate);
            Assert.Null(obj2.Amount);
            Assert.Equal(22, obj2.Age);

            var int3 = "".DeserializeJson<int?>();
            Assert.Null(int3);
            var int4 = "null".DeserializeJson<int?>();
            Assert.Null(int4);
            var obj5 = "".DeserializeJson<InnerTestCvtHelper>();
            Assert.Null(obj5);
            var obj6 = "null".DeserializeJson<InnerTestCvtHelper>();
            Assert.Null(obj6);
        }

        /// <summary>
        /// 测试方法 DeserializeJson
        /// </summary>
        [Fact]
        public void TestDeserializeJson2()
        {
            //json字符串1
            var jsonText1 = "{\"Name\":\"张三\",\"BirthDate\":\"2022-10-14 09:12:47\",\"Amount\":44,\"Age\":33}";
            //反序列化
            var obj1 = jsonText1.DeserializeJson(typeof(InnerTestCvtHelper)) as InnerTestCvtHelper;
            Assert.Equal("张三", obj1.Name);
            Assert.Equal(new DateTime(2022, 10, 14, 9, 12, 47), obj1.BirthDate);
            Assert.Equal(44, obj1.Amount);
            Assert.Equal(33, obj1.Age);

            //json字符串2
            var jsonText2 = "{\"Name\":\"张三\",\"BirthDate\":\"2022-10-14\",\"Amount\":null,\"Age\":22}";
            //反序列化
            var obj2 = jsonText2.DeserializeJson(typeof(InnerTestCvtHelper)) as InnerTestCvtHelper;
            Assert.Equal("张三", obj2.Name);
            Assert.Equal(new DateTime(2022, 10, 14), obj2.BirthDate);
            Assert.Null(obj2.Amount);
            Assert.Equal(22, obj2.Age);

            var int3 = "".DeserializeJson<int?>();
            Assert.Null(int3);
            var int4 = "null".DeserializeJson<int?>();
            Assert.Null(int4);
            var obj5 = "".DeserializeJson<InnerTestCvtHelper>();
            Assert.Null(obj5);
            var obj6 = "null".DeserializeJson<InnerTestCvtHelper>();
            Assert.Null(obj6);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        private static InnerTestCvtHelper GetObject()
        {
            return new InnerTestCvtHelper()
            {
                Name = "张三",
                Age = 33,
                BirthDate = DateTime.Now,
                Amount = 44M,
                TypeField = typeof(string)
            };
        }

        #region 定义

        /// <summary>
        /// 
        /// </summary>
        public class InnerTestCvtHelper
        {
            /// <summary>
            /// 
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public int Age;

            /// <summary>
            /// 
            /// </summary>
            public DateTime BirthDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public decimal? Amount { get; set; }

            /// <summary>
            /// 
            /// </summary>
            private string Secret { get; set; } = "sfasfaf胜多负少";

            /// <summary>
            /// 类型字段
            /// </summary>
            public Type TypeField { get; set; }
        }

        #endregion
    }
}

