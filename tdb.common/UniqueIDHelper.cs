using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tdb.common
{
    /// <summary>
    /// 生产唯一编码帮助类
    ///
    /// 唯一ID位数为64bit 每个bit的含义如下
    /// |1bit| 8bit  | 7bit  | 41bit |  7bit |
    /// | -  | ----- | ----- | ----- | ----- |
    /// | 2  |  256  |  128  |  2兆  |   128  |
    /// | -  | ----- | ----- | ----- | ----- |
    /// | ① |   ②  |   ③  |   ④  |   ⑤  |
    /// ①：符号位不用
    /// ②：服务ID，可以表示256个服务
    /// ③：服务序号，可以表示128个服务序号（多开负载均衡的情况）
    /// ④：时间戳，从2023-01-01（自定义的时间基准）到现在的毫秒数，可以表示69年左右
    /// ⑤：序列号，每毫秒内序列号有序，每毫秒可以产生128个序列号
    /// 最大数量  128000个/秒   128个/毫秒
    /// </summary>
    public class UniqueIDHelper
    {
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object objLock = new object();

        /// <summary>
        /// 序号
        /// </summary>
        private static long seq = 0;

        /// <summary>
        /// 时间基准（单位：100纳秒）
        /// </summary>
        private static long basicTicks = new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// 最后一次生成唯一编码使用的时间部分（毫秒）
        /// </summary>
        private static long lastTimePart;

        /// <summary>
        /// 生成唯一编码
        /// </summary>
        /// <param name="serverID">服务ID（0-255）</param>
        /// <param name="serverSeq">服务序号（0-127）</param>
        public static long CreateID(int serverID, int serverSeq = 0)
        {
            if (serverID < 0 || serverID > 255)
            {
                throw new Exception("服务ID值超出范围（0-255）");
            }

            if (serverSeq < 0 || serverSeq > 127)
            {
                throw new Exception("服务序号值超出范围（0-127）");
            }

            lock (objLock)
            {
                while (true)
                {
                    //100纳秒转毫秒
                    long time = (DateTime.UtcNow.Ticks - basicTicks) / 10000;
                    if (time != lastTimePart)
                    {
                        lastTimePart = time;
                        seq = 0;
                    }

                    //序号在127以内
                    if (seq < 128)
                    {
                        break;
                    }
                    //如果毫秒内生成唯一编码超过128个，等待1毫秒后再次尝试
                    else
                    {
                        Thread.Sleep(1);
                    }
                }

                //生成唯一编码
                long uid = (serverID << 55) +
                           (serverSeq << 48) +
                           (lastTimePart << 7) +
                           seq++;
                return uid;
            }
        }

        /// <summary>
        /// 生成多个唯一编码
        /// </summary>
        /// <param name="count">生产个数</param>
        /// <param name="serverID">服务ID（0-255）</param>
        /// <param name="serverSeq">服务序号（0-127）</param>
        public static long[] CreateIDs(ushort count, int serverID, int serverSeq = 0)
        {
            if (count <= 0)
            {
                throw new Exception($"生产个数应在此范围内（1-{ushort.MaxValue}）");
            }

            if (serverID < 0 || serverID > 255)
            {
                throw new Exception("服务ID值超出范围（0-255）");
            }

            if (serverSeq < 0 || serverSeq > 127)
            {
                throw new Exception("服务序号值超出范围（0-127）");
            }

            var array = new long[count];
            lock (objLock)
            {
                //100纳秒转毫秒
                long time = (DateTime.UtcNow.Ticks - basicTicks) / 10000;
                if (time != lastTimePart)
                {
                    lastTimePart = time;
                    seq = 0;
                }
                else if (seq > 127)
                {
                    lastTimePart++;
                    seq = 0;
                }

                //生成第一个唯一编码
                long first = (serverID << 55) +
                           (serverSeq << 48) +
                           (lastTimePart << 7) +
                           seq++;

                //生成最后一个唯一编码
                long last = first + count - 1;
                //填充到array
                int index = 0;
                for (long id = first; id <= last; id++)
                {
                    array[index++] = id;
                }

                //生成唯一ID需要消耗的毫秒数
                var needMilliseconds = (long)Math.Ceiling((count + seq) / 128d);

                //最后一次生成唯一编码使用的时间部分（毫秒）
                lastTimePart = lastTimePart + needMilliseconds;

                //当前时间
                time = (DateTime.UtcNow.Ticks - basicTicks) / 10000;
                //需要等待的毫秒数（为了反正生成ID数过多，lastTimePart超过了当前时间）
                var waitMilliseconds = lastTimePart - time;

                Thread.Sleep((int)waitMilliseconds);
            }

            return array;
        }
    }
}
