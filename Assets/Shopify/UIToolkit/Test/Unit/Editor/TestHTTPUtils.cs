namespace Shopify.UIToolkit.Test.Unit {
    using System.Collections;
    using NUnit.Framework;
    using Shopify.UIToolkit;

    [TestFixture]
    public class TestHTTPUtils {

        [Test]
        public void TestParseStatusCode() {
            var input = "HTTP/1.1 200";
            Assert.AreEqual(200, HTTPUtils.ParseResponseCode(input));

            var input2 = "HTTP/1.1 304";
            Assert.AreEqual(304, HTTPUtils.ParseResponseCode(input2));
        }

        [Test]
        public void TestParseGarbageStatusCode() {
            var input = "HTTP/1.1 asdf";
            Assert.AreEqual(0, HTTPUtils.ParseResponseCode(input));
        }
    }
}
