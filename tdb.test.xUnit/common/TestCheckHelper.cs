using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;

namespace tdb.test.xUnit.common
{
    /// <summary>
    /// 测试CheckHelper帮助类
    /// </summary>
    public class TestCheckHelper
    {
        /// <summary>
        /// 测试 IsNumeric 方法
        /// </summary>
        [Fact]
        public void TestIsNumeric()
        {
            Assert.True(CheckHelper.IsNumeric(typeof(decimal)));
            Assert.True(CheckHelper.IsNumeric(typeof(short?)));
            Assert.True(CheckHelper.IsNumeric(typeof(int)));
            Assert.True(CheckHelper.IsNumeric(typeof(double)));
            Assert.False(CheckHelper.IsNumeric(typeof(string)));
            Assert.False(CheckHelper.IsNumeric(typeof(TestCheckHelper)));
        }

        /// <summary>
        /// 测试 IsNullableType 方法
        /// </summary>
        [Fact]
        public void TestIsNullableType()
        {
            Assert.False(CheckHelper.IsNullableType(typeof(decimal)));
            Assert.True(CheckHelper.IsNullableType(typeof(short?)));
            Assert.False(CheckHelper.IsNullableType(typeof(int)));
            Assert.False(CheckHelper.IsNullableType(typeof(double)));
            Assert.True(CheckHelper.IsNullableType(typeof(string)));
            Assert.True(CheckHelper.IsNullableType(typeof(TestCheckHelper)));
        }
    }
}
