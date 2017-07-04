namespace Shopify.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using System.Text.RegularExpressions;

    [TestFixture]
    public class TestShopify {
        [SetUp]
        public void Setup() {
            #if (SHOPIFY_TEST)
            ShopifyBuy.Reset();
            #endif
        }

        [Test]
        public void TestInit() {
            Assert.IsNull(ShopifyBuy.Client());
            Assert.IsNull(ShopifyBuy.Client("domain.com"));

            ShopifyBuy.Init("AccessToken", "domain.com");

            Assert.IsNotNull(ShopifyBuy.Client());
            Assert.IsNotNull(ShopifyBuy.Client("domain.com"));
            Assert.AreEqual(ShopifyBuy.Client(), ShopifyBuy.Client("domain.com"));
            Assert.AreEqual("domain.com", ShopifyBuy.Client().Domain);
            Assert.AreEqual("AccessToken", ShopifyBuy.Client().AccessToken);
        }

        [Test]
        public void CannotInitTwiceUsingDomain() {
            ShopifyBuy.Init("AccessToken", "domain2.com");
            ShopifyClient client1 = ShopifyBuy.Client("domain2.com");

            ShopifyBuy.Init("AccessToken", "domain2.com");
            ShopifyClient client2 = ShopifyBuy.Client("domain2.com");

            Assert.IsNotNull(client1);
            Assert.AreSame(client1, client2);
        }

        [Test]
        public void CannotInitTwiceUsingLoader() {
            ShopifyBuy.Init(new MockLoader());
            ShopifyClient client1 = ShopifyBuy.Client("graphql.myshopify.com");

            ShopifyBuy.Init(new MockLoader());
            ShopifyClient client2 = ShopifyBuy.Client("graphql.myshopify.com");

            Assert.IsNotNull(client1);
            Assert.AreSame(client1, client2);
        }

        [Test]
        public void TestGenericQuery() {
            ShopifyBuy.Init(new MockLoader());
            QueryRoot response = null;

            ShopifyBuy.Client().Query(
                (q) => q.shop(s => s
                    .name()
                ),
                (data, error) => {
                    response = data;
                    Assert.IsNull(error);
                }
            );

            Assert.AreEqual("this is the test shop yo", response.shop().name());
        }

        [Test]
        public void TestGenericMutation() {
            ShopifyBuy.Init(new MockLoader());
            Mutation response = null;

            ShopifyBuy.Client().Mutation(
                (q) => q.customerAccessTokenCreate((a) => a
                    .customerAccessToken(at => at
                        .accessToken()
                    ),
                    input: new CustomerAccessTokenCreateInput("some@email.com", "password")
                ),
                (data, error) => {
                    response = data;
                    Assert.IsNull(error);
                }
            );

            Assert.AreEqual("i am a token", response.customerAccessTokenCreate().customerAccessToken().accessToken());
        }

        [Test]
        public void TestProductsAll() {
            List<Product> products = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().products(callback: (p, error) => {
                products = p;
                Assert.IsNull(error);
            });

            Assert.AreEqual(MockLoaderProducts.CountProductsPages * MockLoader.PageSize, products.Count);
            Assert.AreEqual("Product0", products[0].title());
            Assert.AreEqual("Product1", products[1].title());

            Assert.AreEqual(1, products[0].images().edges().Count, "First product has one image");
            Assert.AreEqual(1, products[0].variants().edges().Count, "First product has one variant");
            Assert.AreEqual(1, products[0].collections().edges().Count, "First product has one collection");

            Assert.AreEqual(2 * MockLoader.PageSize, products[1].images().edges().Count, "Second product has 2 pages of images");
            Assert.AreEqual(1, products[1].variants().edges().Count, "Second product has 1 variant");
            Assert.AreEqual(1, products[1].collections().edges().Count, "Second product has one collection");

            Assert.AreEqual(3 * MockLoader.PageSize, products[2].images().edges().Count, "Third page has 3 pages of images");
            Assert.AreEqual(2 * MockLoader.PageSize, products[2].variants().edges().Count, "Third page has 2 pages of variants");
            Assert.AreEqual(1, products[1].collections().edges().Count, "Third product has one collection");
        }

        [Test]
        public void TestProductsFirst() {
            List<Product> products = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().products(callback: (p, error) => {
                products = p;
                Assert.IsNull(error);
            }, first: 250);

            Assert.AreEqual(250, products.Count);
        }

        [Test]
        public void TestCollectionsAll() {
            List<Collection> collections = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().collections(callback: (c, error) => {
                collections = c;
                Assert.IsNull(error);
            });

            Assert.AreEqual(MockLoaderCollections.CountPages * MockLoader.PageSize, collections.Count);
            Assert.AreEqual("I am collection 0", collections[0].title());
            Assert.AreEqual("I am collection 1", collections[1].title());

            Assert.AreEqual(2 * MockLoader.PageSize, collections[0].products().edges().Count, "First collection has one product");
            Assert.AreEqual(1, collections[1].products().edges().Count, "Second collection has one product");
        }

        [Test]
        public void TestCollectionsFirst() {
            List<Product> products = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().products(callback: (p, error) => {
                products = p;
                Assert.IsNull(error);
            }, first: 250);

            Assert.AreEqual(250, products.Count);
        }

        [Test]
        public void TestProductsAfter() {
            List<Product> products = null;

            ShopifyBuy.Init(new MockLoader());

            ShopifyBuy.Client().products(callback: (p, error) => {
                products = p;
                Assert.IsNull(error);
            }, first: 250, after: "249");

            Assert.AreEqual(250, products.Count);
            Assert.AreEqual("250", products[0].id());
            Assert.AreEqual("499", products[products.Count - 1].id());
        }

        [Test]
        public void TestGraphQLError() {
            ShopifyBuy.Init(new MockLoader());

            // when after is set to 3 MockLoader will return a graphql error
            ShopifyBuy.Client().products(callback: (p, error) => {
                Assert.IsNull(p);
                Assert.IsNotNull(error);
                Assert.AreEqual("[\"GraphQL error from mock loader\"]", error.Description);
            }, first: 250, after: "666");
        }

        [Test]
        public void TestHttpError() {
            ShopifyBuy.Init(new MockLoader());

            // when after is set to 404 MockLoader loader will return an httpError
            ShopifyBuy.Client().products(callback: (p, error) => {
                Assert.IsNull(p);
                Assert.IsNotNull(error);
                Assert.AreEqual("404 from mock loader", error.Description);
            }, first: 250, after: "404");
        }

        [Test]
        public void TestHasVersionNumber() {
            Regex version = new Regex(@"\d+\.\d+\.\d+");

            Assert.IsTrue(version.IsMatch(ShopifyBuy.VERSION));
        }

        [Test]
        public void TestProductsByIds() {
            ShopifyBuy.Init(new MockLoader());
            List<Product> products = null;
            List<string> ids = new List<string>();
            ids.Add("productId333");
            ids.Add("productId444");

            ShopifyBuy.Client().products((p, error) => {
                 products = p;
                 Assert.IsNull(error);
             }, ids);

            Assert.AreEqual(2, products.Count);
            Assert.AreEqual("productId333", products[0].id());
            Assert.AreEqual("productId444", products[1].id());
            Assert.AreEqual(1, products[0].images().edges().Count);
        }
    }
}
