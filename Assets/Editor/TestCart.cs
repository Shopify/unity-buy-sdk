namespace Shopify.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using System.Text.RegularExpressions;

    [TestFixture]
    public class TestCart {
        [SetUp]
        public void Setup() {
            #if (SHOPIFY_TEST)
            ShopifyBuy.Reset();
            #endif
        }

        [Test]
        public void TestGenericQuery() {
            ShopifyBuy.Init(new MockLoader());
            string cartWithId = "cartId";

            Assert.IsNotNull(ShopifyBuy.Client().Cart(), "created a cart");
            Assert.IsNotNull(ShopifyBuy.Client().Cart(cartWithId), "created a cart with id");
            Assert.AreNotEqual(ShopifyBuy.Client().Cart(), ShopifyBuy.Client().Cart(cartWithId));
        }

        [Test]
        public void TestAddRemoveLineItems() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            Dictionary<string,object> data = new Dictionary<string,object>();
            data.Add("id", "gid://shopify/ProductVariant/20756129347");

            Shopify.Unity.GraphQL.ProductVariant variant = new Shopify.Unity.GraphQL.ProductVariant(data);

            cart.LineItems.Set("gid://shopify/ProductVariant/20756129155", 33);
            cart.LineItems.Set(variant, 2);
            cart.LineItems.Set("gid://shopify/Product/7336568131");

            Assert.AreEqual(3, cart.LineItems.GetAll().Count, "has 3 items in cart");
            Assert.AreEqual(33, cart.LineItems.Get("gid://shopify/ProductVariant/20756129155").quantity, "variant 20756129155 quantity is 33");
            Assert.AreEqual(2, cart.LineItems.Get("gid://shopify/ProductVariant/20756129347").quantity, "variant 20756129347 quantity is 2");
            Assert.AreEqual(1, cart.LineItems.Get("gid://shopify/Product/7336568131").quantity, "variant 7336568131 quantity is 1");

            bool didDelete = cart.LineItems.Delete("gid://shopify/ProductVariant/20756129155");
            Assert.AreEqual(2, cart.LineItems.GetAll().Count, "After remove had 2 items in cart");
            Assert.IsTrue(didDelete, "returned true when deleting");

            didDelete = cart.LineItems.Delete("gid://shopify/ProductVariant/iamnotreal");
            Assert.IsFalse(didDelete, "returned false when did not delete");
        }

        [Test]
        public void TestPermalink() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.Set("gid://shopify/ProductVariant/20756129155", 33);
            cart.LineItems.Set("gid://shopify/ProductVariant/20756129347", 2);

            Assert.AreEqual("http://graphql.myshopify.com/cart/20756129155:33,20756129347:2", cart.GetWebCheckoutLink());
            Assert.AreEqual("http://graphql.myshopify.com/cart/20756129155:33,20756129347:2?note=i-am-a-note", cart.GetWebCheckoutLink("i-am-a-note"));
        }

        [Test]
        public void ModifyLineItems() {
            ShopifyBuy.Init(new MockLoader());

            string productId1 = "gid://shopify/ProductVariant/20756129155";
            string productId2 = "gid://shopify/ProductVariant/20756129347";
            List<AttributeInput> attributes1 = new List<AttributeInput>() {
                new AttributeInput("fancy", "i am fancy"),
                new AttributeInput("boring", "i am boring")
            };
            List<AttributeInput> attributes2 = new List<AttributeInput>() {
                new AttributeInput("animal", "lion"),
                new AttributeInput("spotted", "no")
            };

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.Set(productId1, 33);
            cart.LineItems.Set(productId2, 33);
            
            cart.LineItems.Set(productId1, 100);
            cart.LineItems.Set(productId1, null, attributes1);
            cart.LineItems.Set(productId2, 6, attributes2);

            Assert.AreEqual(100, cart.LineItems.Get(productId1).quantity, "variant 20756129155 quantity is 100 after change");
            Assert.AreEqual(6, cart.LineItems.Get(productId2).quantity, "variant 20756129155 quantity is 100 after change");
            Assert.AreEqual("i am fancy", cart.LineItems.Get(productId1).customAttributes[0].value);
            Assert.AreEqual("i am boring", cart.LineItems.Get(productId1).customAttributes[1].value);
            Assert.AreEqual("lion", cart.LineItems.Get(productId2).customAttributes[0].value);
            Assert.AreEqual("no", cart.LineItems.Get(productId2).customAttributes[1].value);
        }
    }
}
