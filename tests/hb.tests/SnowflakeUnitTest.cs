using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    [TestClass]
    public class SnowflakeUnitTest
    {
        [TestMethod]
        public void NextSid()
        {
            long sid1 = Snowflake.Instance.Next();
            long sid2 = Snowflake.Instance.Next();
            Assert.IsTrue(sid2 > sid1);
        }

        [TestMethod]
        public void TakeDateTime()
        {
            long sid1 = Snowflake.Instance.Next();
            long sid2 = Snowflake.Instance.Next();
            DateTime dt1 = Snowflake.Instance.TakeDateTime(sid1);
            DateTime dt2 = Snowflake.Instance.TakeDateTime(sid2);
            Assert.IsTrue(dt2 >= dt1);
        }


    }
}
