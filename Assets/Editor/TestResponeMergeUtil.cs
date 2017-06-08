namespace Shopify.Tests
{
    using System;
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

            Assert.IsFalse(Object.ReferenceEquals(responseA, merged));
            Assert.IsFalse(Object.ReferenceEquals(responseB, merged));
            Assert.AreNotEqual(responseA, merged, "The merged dictionary is not responseA");
            Assert.AreNotEqual(responseB, merged, "The merged dictionary is not responseB");
            Assert.AreEqual(3, merged["fieldOnlyInA"], "Brought in field from A");
            Assert.AreEqual("i am b", merged["fieldInBoth"], "Overwrote field in A from B");
            Assert.AreEqual(1, ((List<int>) merged["fieldHasList"])[0], "Overwrote field in A from B that is a list");
            Assert.AreEqual("monkey", ((Dictionary<string, object>) merged["fieldHasDictionary"])["keyA"], "Overwrote field in A from B that is a list");
            Assert.IsTrue(Object.ReferenceEquals(responseB["fieldHasDictionary"], merged["fieldHasDictionary"]));
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
            Assert.IsFalse(Object.ReferenceEquals(responseA["fieldHasDictionary"], merged["fieldHasDictionary"]), "Created a new nested object that was not responseA");
            Assert.IsFalse(Object.ReferenceEquals(responseB["fieldHasDictionary"], merged["fieldHasDictionary"]), "Created a new nested object that was not responseB");
        }

        [Test]
        public void CustomFieldMerger() {
            Dictionary<string, object> responseA = GetResponseA();
            Dictionary<string, object> responseB = GetResponseB();

            ResponseMergeUtil merger = new ResponseMergeUtil();

            merger.AddFieldMerger("fieldInBoth", (string field, Dictionary<string, object> into, Dictionary<string, object> dataA, Dictionary<string, object> dataB) => {
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

        [Test]
        public void DoNotMerge() {
            Dictionary<string, object> responseA = GetResponseA();
            Dictionary<string, object> responseB = GetResponseB();

            ResponseMergeUtil merger = new ResponseMergeUtil();

            merger.AddFieldMerger("fieldInBoth", ResponseMergeUtil.DoNotMergeIfExists);

            Dictionary<string, object> merged = merger.Merge(responseA, responseB);

            Assert.AreEqual("i am a", merged["fieldInBoth"], "Did not merge when existed");
            Assert.AreEqual(33, merged["fieldOnlyInB"], "Merged when did not exist");
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
            Assert.IsTrue(!Object.ReferenceEquals(responseA["fieldHasDictionary"], merged["fieldHasDictionary"]), "Creted a new nested object that was not responseA");
            Assert.IsTrue(!Object.ReferenceEquals(responseB["fieldHasDictionary"], merged["fieldHasDictionary"]), "Creted a new nested object that was not responseB");
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

        [Test]
        public void DoNotMerge() {
            Dictionary<string, object> responseA = GetResponseA();
            Dictionary<string, object> responseB = GetResponseB();

            ResponseMergeUtil merger = new ResponseMergeUtil();

            merger.AddFieldMerger("fieldInBoth", ResponseMergeUtil.DoNotMerge);

            Dictionary<string, object> merged = merger.Merge(responseA, responseB);

            Assert.AreEqual("i am a", merged["fieldInBoth"], "Custom merger worked");
        }

        private Dictionary<string,object> GetResponseA() {
            return new Dictionary<string, object>() {
                {"fieldOnlyInA", 3},
                {"fieldInBoth", "i am a"},
                {"fieldHasList", new List<int>() { 0 }},
                {"fieldHasDictionary", new Dictionary<string, object>(){
                    {"keyA", "valueA"},
                    {"keyB", "valueB"}
                }}
            };
        }

        private Dictionary<string,object> GetResponseB() {
            return new Dictionary<string, object>() {
                {"fieldOnlyInB", 33},
                {"fieldInBoth", "i am b"},
                {"fieldHasList", new List<int>() { 1 }},
                {"fieldHasDictionary", new Dictionary<string, object>(){
                    {"keyA", "monkey"},
                    {"keyB", "zebra"}
                }}
            };
        }
    }
}
