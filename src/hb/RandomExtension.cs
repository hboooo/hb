﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/31 21:34:32
    /// description:
    /// </summary>
    public static class RandomExtension
    {
        /// <summary>
        /// 生成真正的随机数
        /// </summary>
        /// <param name="r"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        public static int StrictNext(this Random r, int seed = int.MaxValue)
        {
            return new Random((int)Stopwatch.GetTimestamp()).Next(seed);
        }

        /// <summary>
        /// 产生正态分布的随机数
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="mean">均值</param>
        /// <param name="stdDev">方差</param>
        /// <returns></returns>
        public static double NextGauss(this Random rand, double mean, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            return mean + stdDev * randStdNormal;
        }

        public static int Random()
        {
            int randomState = 1804289383;
            int num = randomState;
            num ^= num << 13;
            num ^= num >> 17;
            num ^= num << 5;
            randomState = num;

            return num;
        }
    }
}
