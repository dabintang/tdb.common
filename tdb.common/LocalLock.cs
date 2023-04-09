using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.common
{
    /// <summary>
    /// 本地锁，按字符串锁
    /// </summary>
    public class LocalLock : IDisposable
    {
        /// <summary>
        /// 内存缓存
        /// </summary>
        private static readonly MemoryCache memoryCache = new(new MemoryCacheOptions());

        #region 静态方法

        /// <summary>
        /// 对key上锁
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="maxWaitSeconds">最多等待时间（秒）</param>
        /// <param name="maxLockSeconds">最大上锁时间（秒）【预防宕机导致一直锁着】</param>
        /// <returns></returns>
        public static LocalLock Lock(string key, int maxWaitSeconds = 60, int maxLockSeconds = 60)
        {
            DateTime startTime = DateTime.Now;
            var localLock = new LocalLock(key);
            Guid? lockValue = null; //锁内容

            //保证lock时间比较短
            lock (memoryCache)
            {
                lockValue = memoryCache.Get<Guid?>(key);
                if (lockValue is null)
                {
                    localLock.Lock(maxLockSeconds);
                    return localLock;
                }
            }

            //如果被别人锁着，等待其释放锁或等待超时
            while (lockValue is not null)
            {
                //超过等待时间，返回并告知锁被别人占着
                if ((DateTime.Now - startTime).TotalSeconds > maxWaitSeconds)
                {
                    return localLock;
                }

                Thread.Sleep(10);

                //保证lock时间比较短
                lock (memoryCache)
                {
                    lockValue = memoryCache.Get<Guid?>(key);
                    if (lockValue is null)
                    {
                        localLock.Lock(maxLockSeconds);
                        return localLock;
                    }
                }
            }

            //代码应该不会进来到这里
            localLock.Lock(maxLockSeconds);
            return localLock;
        }

        #endregion

        #region 变量/属性

        /// <summary>
        /// key
        /// </summary>
        private string Key { get; set; }

        /// <summary>
        /// 上锁的内容
        /// </summary>
        private Guid LockValue { get; set; } = Guid.NewGuid();

        /// <summary>
        /// 是否被他人锁着
        /// </summary>
        public bool IsLockedByOther { get; private set; } = true;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="key">key</param>
        private LocalLock(string key)
        {
            this.Key = key;
        }

        #endregion

        #region 方法

        /// <summary>
        /// 上锁
        /// </summary>
        /// <param name="maxLockSeconds">最大上锁时间（秒）【预防宕机导致一直锁着】</param>
        private void Lock(int maxLockSeconds)
        {
            memoryCache.Set<Guid?>(this.Key, this.LockValue, TimeSpan.FromSeconds(maxLockSeconds));
            this.IsLockedByOther = false;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            //如果锁不在此实例，不做处理
            if (this.IsLockedByOther)
            {
                return;
            }

            //如果不是自己上的锁，不处理
            var lockValue = memoryCache.Get<Guid?>(this.Key);
            if (lockValue is null || lockValue.Value != this.LockValue)
            {
                return;
            }

            //释放锁
            memoryCache.Remove(this.Key);
        }

        #endregion
    }
}
