namespace Shopify.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;

    [TestFixture]
    public class TestShopify {
        /*
        * since ShopifyBuy uses static methods and the following tests setting up the generic client
        * this test needs to be run first which is why this test starts with _ since nunit runs tests alphabetically
        */
        [Test]
        public void _TestInit() {
            Assert.IsNull(ShopifyBuy.Client());
            Assert.IsNull(ShopifyBuy.Client("domain.com"));

            ShopifyBuy.Init("apiKey", "domain.com");

            Assert.IsNotNull(ShopifyBuy.Client());
            Assert.IsNotNull(ShopifyBuy.Client("domain.com"));
        }

        [Test]
        public void TestProductsAll() {
            List<Product> products = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().products(callback: (p, errors, httpError) => {
                products = p;
                Assert.IsNull(errors);
                Assert.IsNull(httpError);
            });

            Assert.AreEqual(MockLoader.CountProductsPages * MockLoader.PageSize, products.Count);
            Assert.AreEqual("Product0", products[0].title());
            Assert.AreEqual("Product1", products[1].title());

            Assert.AreEqual(1, products[0].images().edges().Count, "First product has one image");
            Assert.AreEqual(1, products[0].variants().edges().Count, "First product has one variant");

            Assert.AreEqual(2 * MockLoader.PageSize, products[1].images().edges().Count, "Second product has 2 pages of images");
            Assert.AreEqual(1, products[1].variants().edges().Count, "Second product has 1 variant");
            
            Assert.AreEqual(3 * MockLoader.PageSize, products[2].images().edges().Count, "Third page has 3 pages of images");
            Assert.AreEqual(2 * MockLoader.PageSize, products[2].variants().edges().Count, "Third page has 2 pages of variants");
        }

        [Test]
        public void TestProductsFirst() {
            List<Product> products = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().products(callback: (p, errors, httpError) => {
                products = p;
                Assert.IsNull(errors);
                Assert.IsNull(null, httpError);
            }, first: 250);

            Assert.AreEqual(250, products.Count);
        }

        [Test]
        public void TestProductsAfter() {
            List<Product> products = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().products(callback: (p, errors, httpError) => {
                products = p;
                Assert.IsNull(errors);
                Assert.IsNull(httpError);
            }, first: 250, after: "249");

            Assert.AreEqual(250, products.Count);
            Assert.AreEqual("250", products[0].id());
            Assert.AreEqual("499", products[products.Count - 1].id());
        }

        [Test]
        public void TestGraphQLError() {
            ShopifyBuy.Init(new MockLoader());

            // when after is set to 3 MockLoader will return a graphql error
            ShopifyBuy.Client().products(callback: (p, errors, httpError) => {
                Assert.IsNull(p);
                Assert.IsNotNull(errors);
                Assert.AreEqual("GraphQL error from mock loader", errors[0]);
                Assert.IsNull(httpError);
            }, first: 250, after: "666");
        }

        [Test]
        public void TestHttpError() {
            ShopifyBuy.Init(new MockLoader());

            // when after is set to 404 MockLoader loader will return an httpError
            ShopifyBuy.Client().products(callback: (p, errors, httpError) => {
                Assert.IsNull(p);
                Assert.IsNull(errors);
                Assert.AreEqual("404 from mock loader", httpError);
            }, first: 250, after: "404");
        }
    }
}
