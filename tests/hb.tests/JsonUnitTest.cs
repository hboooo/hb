using hb.Dynamic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace hb.tests
{
    [TestClass]
    public class JsonUnitTest
    {
        [TestMethod]
        public void JsonSerializeObject()
        {
            var jsonObj = new
            {
                name = "Bill",
                age = 18,
                createDate = "2021-07-21 22:00:00",
                test = default(dynamic)
            };
            string json = DynamicJson.SerializeObject(jsonObj);
            Assert.IsTrue(json.StartsWith("{"));
        }

        [TestMethod]
        public void SerializeObjectNullIgnore()
        {
            var jsonObj = new
            {
                name = "Bill",
                age = 18,
                createDate = "2021-07-21 22:00:00",
                test = default(dynamic)
            };
            string json = DynamicJson.SerializeObjectNullIgnore(jsonObj);
            Assert.IsTrue(json.StartsWith("{"));
        }

        [TestMethod]
        public void JsonDeserializeObject()
        {
            string json = "{'name':'bill','age':18,'createDate':1626880865}";
            object jsonObj = DynamicJson.DeserializeObject(json);
            Assert.IsNotNull(jsonObj);
        }

    }
}
