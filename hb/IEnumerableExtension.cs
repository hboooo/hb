using System.Collections.Generic;
using System.Linq;

namespace hb
{
    /// <summary>
    /// author     :habo
    /// date       :2021/7/23 22:03:18
    /// description:
    /// </summary>
    public static class IEnumerableExtension
    {
        /// <summary>
        /// 分批循环遍历
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> TakeRange<T>(this IEnumerable<T> source, int size)
        {
            T[] bucket = null;
            int count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new T[size];

                bucket[count++] = item;

                if (count != size)
                    continue;

                yield return bucket.Select(x => x);

                bucket = null;
                count = 0;
            }
            if (bucket != null && count > 0)
                yield return bucket.Take(count);
        }
    }
}
