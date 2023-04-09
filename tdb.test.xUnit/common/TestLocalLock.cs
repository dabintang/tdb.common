using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tdb.common;
using Xunit.Abstractions;

namespace tdb.test.xUnit.common
{
    /// <summary>
    /// 测试本地锁
    /// </summary>
    public class TestLocalLock
    {
        /// <summary>
        /// 输出
        /// </summary>
        private readonly ITestOutputHelper output;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_output"></param>
        public TestLocalLock(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// 测试对同一个key上锁,完成后再次对同一key上锁
        /// </summary>
        [Fact]
        public void SameKeyTwoTimes()
        {
            var key = "SameKeyTwoTimes";

            //最多等待时间（秒）
            var maxWaitSeconds = 60;
            //模拟任务耗时（秒）
            var taskSeconds = 1;

            var watch = new Stopwatch();
            watch.Start();

            this.DoSomeThing(key, maxWaitSeconds, taskSeconds);
            this.DoSomeThing(key, maxWaitSeconds, taskSeconds);

            watch.Stop();
            Assert.True(watch.Elapsed.TotalSeconds < 4);
        }

        /// <summary>
        /// 测试对同一个key上锁且未等待超时的情况
        /// </summary>
        [Fact]
        public void SameKeyWithoutTimeout()
        {
            var key = "SameKeyWithoutTimeout";
            //最多等待时间（秒）
            var maxWaitSeconds = 60;
            //模拟任务耗时（秒）
            var taskSeconds = 1;

            //并行3任务
            Parallel.Invoke(
                () => { this.DoSomeThing(key, maxWaitSeconds, taskSeconds); },
                () => { this.DoSomeThing(key, maxWaitSeconds, taskSeconds); },
                () => { this.DoSomeThing(key, maxWaitSeconds, taskSeconds); }
            );
        }

        /// <summary>
        /// 测试对同一个key上锁且等待超时的情况
        /// </summary>
        [Fact]
        public void SameKeyWithTimeout()
        {
            var key = "SameKeyWithTimeout";
            //最多等待时间（秒）
            var maxWaitSeconds = 1;
            //模拟任务耗时（秒）
            var taskSeconds = 2;

            //并行3任务
            Parallel.Invoke(
                () => { this.DoSomeThing(key, maxWaitSeconds, taskSeconds); },
                //() => { this.DoSomeThing(key, maxWaitSeconds, taskSeconds); },
                () => { this.DoSomeThing(key, maxWaitSeconds, taskSeconds); }
            );
        }

        /// <summary>
        /// 测试对不同key上锁的情况
        /// </summary>
        [Fact]
        public void DifferentKey()
        {
            var key1 = "key1";
            var key2 = "key2";
            var key3 = "key3";
            //最多等待时间（秒）
            var maxWaitSeconds = 1;
            //模拟任务耗时（秒）
            var taskSeconds = 2;

            //并行3任务
            Parallel.Invoke(
                () => { this.DoSomeThing(key1, maxWaitSeconds, taskSeconds); },
                () => { this.DoSomeThing(key2, maxWaitSeconds, taskSeconds); },
                () => { this.DoSomeThing(key3, maxWaitSeconds, taskSeconds); }
            );
        }

        /// <summary>
        /// 模拟做任务
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="maxWaitSeconds">最多等待时间（秒）</param>
        /// <param name="taskSeconds">模拟任务耗时（秒）</param>
        private void DoSomeThing(string key, int maxWaitSeconds, int taskSeconds)
        {
            this.output.WriteLine($"[{Environment.CurrentManagedThreadId}]尝试对[{key}]上锁，{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}");
            using (var localLock = LocalLock.Lock(key, maxWaitSeconds))
            {
                if (localLock.IsLockedByOther)
                {
                    this.output.WriteLine($"[{Environment.CurrentManagedThreadId}]对[{key}]上锁超时，{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}");
                    return;
                }

                this.output.WriteLine($"[{Environment.CurrentManagedThreadId}]对[{key}]上锁成功，{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}");
                this.output.WriteLine($"[{Environment.CurrentManagedThreadId}]开始干活[{key}]，{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}");
                Thread.Sleep(taskSeconds * 1000);
                this.output.WriteLine($"[{Environment.CurrentManagedThreadId}]完成干活[{key}]，，{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}");
                this.output.WriteLine($"[{Environment.CurrentManagedThreadId}]对[{key}]解锁，{DateTime.Now:yyyy-MM-dd HH:mm:ss.ffff}");
            }
        }
    }
}
