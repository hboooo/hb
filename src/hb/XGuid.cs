using System;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/24 18:39:15
    /// description:Guid生成器
    /// </summary>
    public class XGuid
    {
        /// <summary>
        /// Guid生成，格式 9af7f46a-ea52-4aa3-b8c3-9fd484c2af12
        /// </summary>
        /// <returns></returns>
        public static string Next()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Guid生成，格式 e0a953c3ee6040eaa9fae2b667060e09
        /// </summary>
        /// <returns></returns>
        public static string NextN()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// Guid生成，格式 {734fd453-a4f8-4c5d-9c98-3fe2d7079760}
        /// </summary>
        /// <returns></returns>
        public static string NextB()
        {
            return Guid.NewGuid().ToString("B");
        }

        /// <summary>
        /// Guid生成，格式 (ade24d16-db0f-40af-8794-1e08e2040df3)
        /// </summary>
        /// <returns></returns>
        public static string NextP()
        {
            return Guid.NewGuid().ToString("P");
        }

        /// <summary>
        /// Guid生成，格式 {0x3fa412e3,0x8356,0x428f,{0xaa,0x34,0xb7,0x40,0xda,0xaf,0x45,0x6f}}
        /// </summary>
        /// <returns></returns>
        public static string NextX()
        {
            return Guid.NewGuid().ToString("X");
        }
    }
}
