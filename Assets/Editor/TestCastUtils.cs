namespace Shopify.Tests
{
    using NUnit.Framework;
    using Shopify.Unity;
    using System.Collections.Generic;

    public enum TestEnum {
        UNKNOWN, MIKKO, MATTI, TEPPO
    }

    public class TestClass {
        public int Value;
        public TestClass(int value) {
            Value = value;
        }
    }

    [TestFixture]
    public class TestCastUtils {
        [Test]
        public void TestGetEnumValue() {
            Assert.AreEqual(TestEnum.MIKKO, CastUtils.GetEnumValue<TestEnum>("MIKKO"));
            Assert.AreEqual(TestEnum.UNKNOWN, CastUtils.GetEnumValue<TestEnum>("IAMNOTHERE"));
        }

        [Test]   
        public void TestCastStringList() {
            List<object> list = new List<object> { "Mikko", "Matti", "Teppo" };

            List<string> castList = CastUtils.CastList<List<string>>(list);

            CollectionAssert.AreEqual(list, castList);
        }

        [Test]   
        public void TestCastStringListWithNull() {
            List<object> list = new List<object> { "Mikko", null, "Teppo" };

            List<string> castList = CastUtils.CastList<List<string>>(list);

            CollectionAssert.AreEqual(list, castList);
        }

        [Test]   
        public void TestCastStringList2D() {
            List<List<object>> list = new List<List<object>> { 
                new List<object>() { "Toronto", "Seinäjoki", "Vancouver" }
            };

            List<List<string>> castList = (List<List<string>>) CastUtils.CastList(list, typeof(List<List<string>>));

            CollectionAssert.AreEqual(list, castList);
        }

        [Test]
        public void TestCastEnumList() {
            List<object> list = new List<object> { "MIKKO", "IDONTEXIST", "TEPPO" };

            List<TestEnum> castList = CastUtils.CastList<List<TestEnum>>(list);

            CollectionAssert.AreEqual(new List<TestEnum>() { 
                TestEnum.MIKKO,
                TestEnum.UNKNOWN,
                TestEnum.TEPPO
            }, castList);
        }

        [Test]
        public void TestCastEnumList2D() {
            List<List<object>> list = new List<List<object>> { 
                new List<object>() { "MIKKO", "IDONTEXIST", "TEPPO" }
            };

            List<List<TestEnum>> castList = CastUtils.CastList<List<List<TestEnum>>>(list);

            CollectionAssert.AreEqual(new List<List<TestEnum>>() { 
                new List<TestEnum> {
                    TestEnum.MIKKO,
                    TestEnum.UNKNOWN,
                    TestEnum.TEPPO
                }
            }, castList);
        }

        [Test]
        public void TestCastClassList() {
            List<object> list = new List<object> { 1, 2, 3 };

            List<TestClass> castList = CastUtils.CastList<List<TestClass>>(list);

            Assert.AreEqual(1, castList[0].Value);
            Assert.AreEqual(2, castList[1].Value);
            Assert.AreEqual(3, castList[2].Value);
            Assert.AreEqual(3, castList.Count);
        }

        [Test]
        public void TestCastClassList2D() {
            List<List<object>> list = new List<List<object>> { 
                new List<object>() { 1, 2, 3 } 
            };

            List<List<TestClass>> castList = CastUtils.CastList<List<List<TestClass>>>(list);

            Assert.AreEqual(1, castList[0][0].Value);
            Assert.AreEqual(2, castList[0][1].Value);
            Assert.AreEqual(3, castList[0][2].Value);
            Assert.AreEqual(1, castList.Count);
            Assert.AreEqual(3, castList[0].Count);
        }
    }
}