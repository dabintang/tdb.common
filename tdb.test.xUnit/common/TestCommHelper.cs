using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using Xunit.Abstractions;

namespace tdb.test.xUnit.common
{
    /// <summary>
    /// 测试 CommHelper 帮助类
    /// </summary>
    public class TestCommHelper
    {
        /// <summary>
        /// 输出
        /// </summary>
        private readonly ITestOutputHelper output;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_output"></param>
        public TestCommHelper(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// 测试方法 GetFullFileName
        /// </summary>
        [Fact]
        public void TestGetFullFileName()
        {
            var fullFileName = CommHelper.GetFullFileName(@"json\abc.json");
            this.output.WriteLine($"FullFileName：{fullFileName}");
        }

        /// <summary>
        /// 测试方法 GetLocalIP
        /// </summary>
        [Fact]
        public void TestGetLocalIP()
        {
            var ip = CommHelper.GetLocalIP();
            this.output.WriteLine($"LocalIP：{ip}");
        }

        /// <summary>
        /// 测试方法 ReflectSet
        /// </summary>
        [Fact]
        public void TestReflectSet()
        {
            var obj = new TestInfo();

            CommHelper.ReflectSet(obj, "Name", "张三");
            CommHelper.ReflectSet(obj, "Age", 33);

            Assert.Equal("张三", obj.Name);
            Assert.Equal(33, obj.Age);
        }

        /// <summary>
        /// 测试方法 ReflectGet
        /// </summary>
        [Fact]
        public void TestReflectGet()
        {
            var obj = new TestInfo()
            {
                Name = "李四",
                Age = 30
            };

            var name = CommHelper.ReflectGet(obj, "Name");
            var age = CommHelper.ReflectGet(obj, "Age");

            Assert.Equal(name, obj.Name);
            Assert.Equal(age, obj.Age);
        }

        /// <summary>
        /// 测试方法 EmitSet
        /// </summary>
        [Fact]
        public void TestEmitSet()
        {
            var obj = new TestInfo();

            CommHelper.EmitSet(obj, "Name", "张三");
            CommHelper.EmitSet(obj, "Amount", 333.33M);

            Assert.Equal("张三", obj.Name);
            Assert.Equal(333.33M, obj.Amount);
        }

        /// <summary>
        /// 测试方法 EmitGet<T>
        /// </summary>
        [Fact]
        public void TestEmitGet1()
        {
            var obj = new TestInfo()
            {
                Name = "李四",
                Amount = 123.456M
            };

            var name = CommHelper.EmitGet(obj, "Name");
            var amount = CommHelper.EmitGet(obj, "Amount");

            Assert.Equal(name, obj.Name);
            Assert.Equal(amount, obj.Amount);
        }

        /// <summary>
        /// 测试方法 EmitGet<ObjectT, ReturnT>
        /// </summary>
        [Fact]
        public void TestEmitGet2()
        {
            var obj = new TestInfo()
            {
                Name = "李四",
                Amount = 123.456M
            };

            var name = CommHelper.EmitGet<TestInfo, string>(obj, "Name");
            var amount = CommHelper.EmitGet<TestInfo, decimal>(obj, "Amount");

            Assert.Equal(name, obj.Name);
            Assert.Equal(amount, obj.Amount);
        }

        /// <summary>
        /// 测试方法 IsExistPropertyOrField
        /// </summary>
        [Fact]
        public void TestIsExistPropertyOrField()
        {
            var obj = new TestInfo()
            {
                Name = "李四",
                Amount = 123.456M
            };

            var hadname = CommHelper.IsExistPropertyOrField(obj, "name");
            var hadName = CommHelper.IsExistPropertyOrField(obj, "Name");
            var hadAmount = CommHelper.IsExistPropertyOrField(obj, "Amount");
            var hadAge = CommHelper.IsExistPropertyOrField(obj, "Age");
            var hadpAge = CommHelper.IsExistPropertyOrField(obj, "pAge");
            var hadpName = CommHelper.IsExistPropertyOrField(obj, "pName");

            Assert.False(hadname);
            Assert.True(hadName);
            Assert.True(hadAmount);
            Assert.True(hadAge);
            Assert.False(hadpAge);
            Assert.False(hadpName);
        }

        /// <summary>
        /// 
        /// </summary>
        public class TestInfo
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
            public decimal Amount { get; set; }

            ///// <summary>
            ///// 
            ///// </summary>
            //private int pAge;

            /// <summary>
            /// 
            /// </summary>
            private string pName { get; set; }
        }
    }
}
