#if !SHOPIFY_MONO_UNIT_TEST
namespace Shopify.UIToolkit.Test.Unit {
    using System;
    using Shopify.UIToolkit;
	using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using Shopify.Unity;
    using System.Collections.Generic;

    [TestFixture]
    public class TestProductCache {
        private ProductCache _productCache;

        [SetUp]
        public void Setup() {
            _productCache = new ProductCache();
        }

        [Test]
        public void TestInit() {
            Assert.Null(_productCache.Cursor);
            Assert.AreEqual(_productCache.Complete, false);
            Assert.AreEqual(_productCache.Count, 0);
        }

        [Test]
        public void TestAdd() {
            Dictionary<string, object> dataJSON = new Dictionary<string, object>();

            Product[] products = new Product[] {
                new Product(dataJSON),
                new Product(dataJSON),
                new Product(dataJSON)
            };

            _productCache.Add(products, "afterafterafter");

            Assert.AreEqual(_productCache.Cursor, "afterafterafter");
            Assert.AreEqual(_productCache.Complete, false);
            Assert.AreEqual(_productCache.Count, 3);
        }

        [Test]
        public void TestAddFinal() {
            Dictionary<string, object> dataJSON = new Dictionary<string, object>();

            Product[] products = new Product[] {
                new Product(dataJSON)
            };

            _productCache.Add(products, null);

            Assert.Null(_productCache.Cursor);
            Assert.AreEqual(_productCache.Complete, true);
            Assert.AreEqual(_productCache.Count, 1);
        }

        [Test]
        public void TestGet() {
            Dictionary<string, object> dataJSON = new Dictionary<string, object>() {
                {"title", "yoloswag"}
            };

            Product[] products = new Product[] {
                new Product(dataJSON)
            };

            _productCache.Add(products, null);

            Assert.AreEqual(_productCache.Get(0).title(), "yoloswag");
        }
    }
}
#endif
