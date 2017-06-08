namespace Shopify.Tests
{
    using NUnit.Framework;
    using Shopify.Unity.SDK;
    using System.Collections;
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

        [Test]
        public void TestNestedMerge() {
            Dictionary<string, object> responseA = GetResponseA();
            Dictionary<string, object> responseB = GetResponseB();

            ResponseMergeUtil merger = new ResponseMergeUtil();

            merger.AddObjectMerger("fieldHasDictionary", new ResponseMergeUtil());

            Dictionary<string, object> merged = merger.Merge(responseA, responseB);

            Assert.AreEqual(3, merged["fieldOnlyInA"], "Brought in field from A");
            Assert.AreEqual("i am b", merged["fieldInBoth"], "Overwrote field in A from B");
            Assert.AreNotEqual(responseA["fieldHasDictionary"], merged["fieldHasDictionary"]);
            Assert.AreNotEqual(responseB["fieldHasDictionary"], merged["fieldHasDictionary"]);
        }

        [Test]
        public void CustomFieldMerger() {
            Dictionary<string, object> responseA = GetResponseA();
            Dictionary<string, object> responseB = GetResponseB();

            ResponseMergeUtil merger = new ResponseMergeUtil();

            merger.AddFieldMerger("fieldInBoth", (string field, IDictionary into, IDictionary dataA, IDictionary dataB) => {
                into[field] = (string) dataA[field] + (string) dataB[field];
            });

            Dictionary<string, object> merged = merger.Merge(responseA, responseB);

            Assert.AreEqual("i am ai am b", merged["fieldInBoth"], "Custom merger worked");
        }

        [Test]
        public void MergeListsConcat() {
            Dictionary<string, object> responseA = GetResponseA();
            Dictionary<string, object> responseB = GetResponseB();

            ResponseMergeUtil merger = new ResponseMergeUtil();

            merger.AddFieldMerger("fieldHasList", ResponseMergeUtil.MergeListsConcat);

            Dictionary<string, object> merged = merger.Merge(responseA, responseB);

            Assert.AreEqual(2, ((List<object>) merged["fieldHasList"]).Count, "Has two elements");
            Assert.AreEqual(0, ((List<object>) merged["fieldHasList"])[0], "First value was correct");
            Assert.AreEqual(1, ((List<object>) merged["fieldHasList"])[1], "Second value was correct");
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
