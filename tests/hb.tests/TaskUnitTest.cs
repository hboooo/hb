using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace hb.tests
{
    [TestClass]
    public class TaskUnitTest
    {
        [TestMethod]
        public void TaskRun1()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            XTask.RunTask(list, (i) =>
            {
                Console.WriteLine($"run {i}");
            });
        }

        [TestMethod]
        public void TaskRun2()
        {
            List<int> list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
            var result = XTask.RunTask(list, (i) =>
            {
                Console.WriteLine($"run {i * 100}");
                return i * 100;
            });
            Assert.IsTrue(result.Count == list.Count);
        }

        [TestMethod]
        public void TakeRangeTest1()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 78; i++)
            {
                list.Add(i);
            }
            int count = 0;
            foreach (var item in list.TakeRange(10))
            {
                Console.WriteLine(string.Join(",", item));
                count++;
            }
            Assert.IsTrue(count == 8);
        }

        [TestMethod]
        public void TakeRangeTest2()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 70; i++)
            {
                list.Add(i);
            }
            int count = 0;
            foreach (var item in list.TakeRange(10))
            {
                Console.WriteLine(string.Join(",", item));
                count++;
            }
            Assert.IsTrue(count == 7);
        }

        [TestMethod]
        public void TakeRangeTest3()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                list.Add(i);
            }
            int count = 0;
            foreach (var item in list.TakeRange(10))
            {
                Console.WriteLine(string.Join(",", item));
                count++;
            }
            Assert.IsTrue(count == 1);
        }
    }
}
