namespace Shopify.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using System.Text.RegularExpressions;

    [TestFixture]
    public class TestShopifyPurchaseSession {
        [Test]
        public void TestGenericQuery() {
            ShopifyBuy.Init(new MockLoader());

            Shopify.Unity.PurchaseSession purchaseSession = ShopifyBuy.Client().PurchaseSession();

            Assert.IsNotNull(purchaseSession, "created a purchase session");
        }

        [Test]
        public void TestAddRemoveLineItems() {
            ShopifyBuy.Init(new MockLoader());

            PurchaseSession purchaseSession = ShopifyBuy.Client().PurchaseSession();

            Dictionary<string,object> data = new Dictionary<string,object>();
            data.Add("id", "gid://shopify/ProductVariant/20756129347");

            Shopify.Unity.GraphQL.ProductVariant variant = new Shopify.Unity.GraphQL.ProductVariant(data);

            purchaseSession.LineItems.Set("gid://shopify/ProductVariant/20756129155", 33);
            purchaseSession.LineItems.Set(variant, 2);

            Assert.AreEqual(2, purchaseSession.LineItems.GetAll().Count);
            Assert.AreEqual(33, purchaseSession.LineItems.Get("gid://shopify/ProductVariant/20756129155").quantity);
            Assert.AreEqual(2, purchaseSession.LineItems.Get("gid://shopify/ProductVariant/20756129347").quantity);

            bool didDelete = purchaseSession.LineItems.Delete("gid://shopify/ProductVariant/20756129155");
            Assert.AreEqual(1, purchaseSession.LineItems.GetAll().Count);
            Assert.IsTrue(didDelete);

            didDelete = purchaseSession.LineItems.Delete("gid://shopify/ProductVariant/iamnotreal");
            Assert.IsFalse(didDelete);
        }

        [Test]
        public void TestPermalink() {
            ShopifyBuy.Init(new MockLoader());

            PurchaseSession purchaseSession = ShopifyBuy.Client().PurchaseSession();

            purchaseSession.LineItems.Set("gid://shopify/ProductVariant/20756129155", 33);
            purchaseSession.LineItems.Set("gid://shopify/ProductVariant/20756129347", 2);

            Assert.AreEqual("http://graphql.myshopify.com/cart/20756129155:33,20756129347:2", purchaseSession.WebCheckoutLink());
            Assert.AreEqual("http://graphql.myshopify.com/cart/20756129155:33,20756129347:2?note=i-am-a-note", purchaseSession.WebCheckoutLink("i-am-a-note"));
        }
    }
}
