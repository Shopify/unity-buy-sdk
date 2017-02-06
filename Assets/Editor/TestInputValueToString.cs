namespace Shopify.Tests
{
    using NUnit.Framework;
    using Shopify.Unity.SDK;
    using System.Collections.Generic;

    [TestFixture]
    public class TestInputValueToString {
        [Test]
        public void TestPrimitives() {
            Assert.AreEqual("10", InputValueToString.Get(10));
            Assert.AreEqual("0.321", InputValueToString.Get(0.321));
            Assert.AreEqual("true", InputValueToString.Get(true));
            Assert.AreEqual("false", InputValueToString.Get(false));
            Assert.AreEqual("\"something\"", InputValueToString.Get("something"));
            Assert.AreEqual("\"I have a\\nNEW LINE\"", InputValueToString.Get("I have a\nNEW LINE"));
        }
        
        [Test]
        public void TestList() {
            List<IHaveToString> complexList = new List<IHaveToString>();
            complexList.Add(new IHaveToString("A"));
            complexList.Add(new IHaveToString("B"));
            complexList.Add(new IHaveToString("C"));

            Assert.AreEqual("[\"Mikko\",\"Matti\",\"Teppo\"]", InputValueToString.Get(new List<string>() {"Mikko", "Matti", "Teppo"}));
            Assert.AreEqual("[1,2,3]", InputValueToString.Get(new List<int>() {1, 2, 3}));
            Assert.AreEqual("[A,B,C]", InputValueToString.Get(complexList));
        }

        private class IHaveToString {
            string MyValue;
            public IHaveToString(string myValue) {
                MyValue = myValue;
            }

            public override string ToString() {
                return MyValue;
            }
        }
    }
}