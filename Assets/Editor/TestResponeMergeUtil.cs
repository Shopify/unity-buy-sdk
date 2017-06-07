namespace Shopify.Tests
{
    using NUnit.Framework;
    using Shopify.Unity.SDK;
    using System.Collections.Generic;

    [TestFixture]
    public class TestResponseMergeUtil {
        [Test]
        public void TestDefaultMerge() {
            Dictionary<string, object> responseA = GetResponseA();
            Dictionary<string, object> responseB = GetResponseB();

            ResponseMergeUtil merger = new ResponseMergeUtil();

            Dictionary<string, object> merged = merger.Merge(responseA, responseB);

            Assert.AreNotEqual(responseA, merged, "the merged dictionary is a dictionary. Is not responseA");
            Assert.AreNotEqual(responseB, merged, "the merged dictionary is a dictionary. Is not responseB");
            Assert.AreEqual(3, merged["fieldOnlyInA"], "Brought in field from A");
            Assert.AreEqual("i am b", merged["fieldInBoth"], "Overwrote field in A from B");
            Assert.AreEqual(1, ((List<int>) merged["fieldHasList"])[0], "Overwrote field in A from B that is a list");
            Assert.AreEqual("monkey", ((Dictionary<string, string>) merged["fieldHasDictionary"])["keyA"], "Overwrote field in A from B that is a list");
            Assert.AreEqual(responseB["fieldHasDictionary"], merged["fieldHasDictionary"]);
        }

        private Dictionary<string,object> GetResponseA() {
            return new Dictionary<string, object>() {
                {"fieldOnlyInA", 3},
                {"fieldInBoth", "i am a"},
                {"fieldHasList", new List<int>() { 0 }},
                {"fieldHasDictionary", new Dictionary<string, string>(){
                    {"keyA", "valueA"},
                    {"keyB", "valueB"}
                }}
            };
        }

        private Dictionary<string,object> GetResponseB() {
            return new Dictionary<string, object>() {
                {"fieldInBoth", "i am b"},
                {"fieldHasList", new List<int>() { 1 }},
                {"fieldHasDictionary", new Dictionary<string, string>(){
                    {"keyA", "monkey"},
                    {"keyB", "zebra"}
                }}
            };
        }
    }
}
