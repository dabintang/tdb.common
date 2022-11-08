using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using static tdb.test.xUnit.common.TestCheckHelper;

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

        /// <summary>
        /// 测试 IsSubclassOf 方法
        /// </summary>
        [Fact]
        public void TestIsSubclassOf()
        {
            Assert.True(CheckHelper.IsSubclassOf(typeof(ClassB), typeof(ClassA)));
            Assert.False(CheckHelper.IsSubclassOf(typeof(ClassA), typeof(ClassB)));
            Assert.True(CheckHelper.IsSubclassOf(typeof(ClassD<string>), typeof(ClassC<string>)));
            Assert.False(CheckHelper.IsSubclassOf(typeof(ClassC<string>), typeof(ClassD<string>)));
            Assert.False(CheckHelper.IsSubclassOf(typeof(ClassD<int>), typeof(ClassA)));
            Assert.True(CheckHelper.IsSubclassOf(typeof(ClassE), typeof(ClassC<int>)));
            Assert.False(CheckHelper.IsSubclassOf(typeof(ClassC<int>), typeof(ClassE)));
            Assert.True(CheckHelper.IsSubclassOf(typeof(ClassE), typeof(ClassC<>)));
            Assert.False(CheckHelper.IsSubclassOf(typeof(ClassC<>), typeof(ClassE)));
            Assert.False(CheckHelper.IsSubclassOf(typeof(ClassE), typeof(int)));
        }

        #region 定义

        public class ClassA
        {
        }

        public class ClassB : ClassA
        {
        }

        public class ClassC<T>
        {
        }

        public class ClassD<T> : ClassC<T>
        {
        }

        public class ClassE : ClassC<int>
        {
        }

        #endregion
    }
}
