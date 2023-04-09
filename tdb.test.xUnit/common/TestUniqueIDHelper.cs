using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using Xunit.Abstractions;

namespace tdb.test.xUnit.common
{
    /// <summary>
    /// 测试唯一编码生成器
    /// </summary>
    public class TestUniqueIDHelper
    {
        /// <summary>
        /// 输出
        /// </summary>
        private readonly ITestOutputHelper output;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_output"></param>
        public TestUniqueIDHelper(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// 测试并发连续生成多个ID无重复
        /// </summary>
        [Fact]
        public void TestCreateID()
        {
            var list = new ConcurrentBag<long>();

            Parallel.For(0, 10, (i) =>
            {
                for (int j = 0; j < 3000; j++)
                {
                    list.Add(UniqueIDHelper.CreateID(0, 0));
                }
            });

            Assert.True(list.Distinct().Count() == list.Count);
            Assert.True(list.Count == 30000);
        }

        /// <summary>
        /// 测试并发连续生成多个ID无重复
        /// </summary>
        [Fact]
        public void TestCreateIDs()
        {
            var list = new ConcurrentBag<long>();
            var random = new Random();

            Parallel.For(0, 10, (i) => {
                for (int j = 0; j < 10; j++)
                {
                    var ids = UniqueIDHelper.CreateIDs((ushort)random.Next(1, 1000), 0, 0);
                    foreach (var id in ids)
                    {
                        list.Add(id);
                    }
                }
            });

            var distinctCount = list.Distinct().Count();
            var count = list.Count;
            Assert.True(distinctCount == count);
            this.output.WriteLine($"TestCreateIDs生成了{list.Count}个ID");
        }

        /// <summary>
        /// 测试生成单个ID和生成多个ID效率
        /// </summary>
        [Fact]
        public void TestEfficiency()
        {
            var count = ushort.MaxValue;
            var sw = new Stopwatch();

            sw.Start();
            for (int j = 0; j < count; j++)
            {
                UniqueIDHelper.CreateID(0, 0);
            }
            sw.Stop();
            this.output.WriteLine($"CreateID生成{count}个ID耗时{sw.ElapsedMilliseconds}ms");

            sw.Restart();
            UniqueIDHelper.CreateIDs(count, 0, 0);
            sw.Stop();
            this.output.WriteLine($"CreateIDs生成{count}个ID耗时{sw.ElapsedMilliseconds}ms");
        }
    }
}
