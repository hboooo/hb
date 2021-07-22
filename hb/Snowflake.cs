using System;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/23 1:33:39
    /// description:[Twitter snowflake] sid生成器
    /// </summary>
    public class Snowflake
    {
        private static Snowflake _snowflake;

        private static readonly object _locker = new object();

        public static Snowflake Instance
        {
            get
            {
                if (_snowflake == null)
                {
                    lock (_locker)
                    {
                        if (_snowflake == null)
                        {
                            _snowflake = new Snowflake();
                        }
                    }
                }
                return _snowflake;
            }
        }

        //开始时间戳 [2020-01-01 00:00:00]
        public const long epoch = 1577808000000L;

        //机器id位数
        const int workerIdBits = 5;

        //数据标识位数
        const int dataCenterIdBits = 5;

        //序列位数
        const int sequenceBits = 12;

        //支持最大机器id
        const long maxWorkerId = -1L ^ (-1L << workerIdBits);

        //支持最大数据标识id
        const long maxDataCenterId = -1L ^ (-1L << dataCenterIdBits);

        //机器id左移
        private const int workIdShift = sequenceBits;

        //数据标识id左移
        private const int dataCenterIdShift = sequenceBits + workerIdBits;

        //时间戳左移
        private const int timestampLeftShift = sequenceBits + workerIdBits + dataCenterIdBits;

        //生成序列的掩码 4095 (0b111111111111=0xfff=4095)
        private const long sequenceMask = -1L ^ (-1L << sequenceBits);

        private static readonly object _lock = new object();

        //毫秒内序列
        private long _sequence = 0L;

        //上次生成id的时间戳
        private long _lastTimestamp = -1L;

        /// <summary>
        /// 机器标识
        /// </summary>
        public long WorkerId { get; protected set; }

        /// <summary>
        /// 数据中心标识
        /// </summary>
        public long DataCenterId { get; protected set; }

        /// <summary>
        /// 生成的序号
        /// </summary>
        public long Sequence
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }

        private Snowflake(long workerId = 1, long dataCenterId = 1, long sequence = 0)
        {
            if (workerId > maxWorkerId || workerId < 0)
                throw new ArgumentOutOfRangeException($"wroker id can't be greater than {maxWorkerId} or less than 0");

            if (dataCenterId > maxDataCenterId || dataCenterId < 0)
                throw new ArgumentOutOfRangeException($"dataCenter id can't be greater than {maxDataCenterId} or less than 0");

            WorkerId = workerId;
            DataCenterId = dataCenterId;
            _sequence = sequence;

        }

        private long NextId()
        {
            lock (_lock)
            {
                var timestamp = TimeGen();
                if (timestamp < _lastTimestamp)
                {
                    throw new InvalidSystemClockException($"Clock moved backwards. Refusing to generate id for {_lastTimestamp - timestamp} milliseconds");
                }

                if (_lastTimestamp == timestamp)
                {
                    _sequence = (_sequence + 1) & sequenceMask;
                    if (_sequence == 0)
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                }
                else
                {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = ((timestamp - epoch) << timestampLeftShift) | (DataCenterId << dataCenterIdShift) | (WorkerId << workIdShift) | _sequence;

                return id;
            }
        }

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        protected virtual long TimeGen()
        {
            return SnowflakeSystem.CurrentTimeMillis();
        }

        /// <summary>
        /// 生成sid
        /// </summary>
        /// <returns></returns>
        public long Next()
        {
            return NextId();
        }

        /// <summary>
        /// 从sid中获取时间
        /// </summary>
        /// <param name="sid">sid</param>
        /// <returns></returns>
        public DateTime TakeDateTime(long sid)
        {
            long temp = sid >> 22;
            long unixTime = epoch + temp;
            return ConvertToDateTime(unixTime);
        }

        /// 将Unix时间戳转换为DateTime类型时间
        /// </summary>
        /// <param name="d">double 型数字</param>
        /// <returns>DateTime</returns>
        private DateTime ConvertToDateTime(long unixTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return startTime.AddMilliseconds(unixTime);
        }
    }

    public static class SnowflakeSystem
    {
        public static Func<long> currentTimeFunc = InternalCurrentTimeMillis;

        public static long CurrentTimeMillis()
        {
            return currentTimeFunc();
        }

        public static IDisposable StubCurrentTime(Func<long> func)
        {
            currentTimeFunc = func;
            return new CallbackOnDispose(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        public static IDisposable StubCurrentTime(long millis)
        {
            currentTimeFunc = () => millis;
            return new CallbackOnDispose(() =>
            {
                currentTimeFunc = InternalCurrentTimeMillis;
            });
        }

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static long InternalCurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
    }

    public class InvalidSystemClockException : Exception
    {
        public InvalidSystemClockException(string message) : base(message) { }
    }
}
