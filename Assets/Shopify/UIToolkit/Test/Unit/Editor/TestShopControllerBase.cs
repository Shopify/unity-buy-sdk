namespace Shopify.UIToolkit.Test.Unit {
    using NUnit.Framework;
    using Shopify.Tests;
    using Shopify.Unity.SDK;
    using Shopify.Unity;
    using System.Linq;
    using UnityEngine;
    using NSubstitute;
    using UnityEngine.TestTools;
    using System.Collections.Generic;
    using System.Collections;

    [TestFixture]
    public class TestShopControllerBase {
        public class MockShopController : ShopControllerBase {
            public override void Unload() {}
            public override void Load() {}
        }

        private MockShopController _shopController;
        private IShop _shop;

        [SetUp]
        public void Setup() {
            _shopController = GlobalGameObject.AddComponent<MockShopController>();
            _shopController.Credentials = new ShopCredentials(Utils.TestShopDomain, Utils.TestAccessToken);
            _shopController.LoaderProvider = new MockLoaderProvider();

            _shop = Substitute.For<IShop>();
            _shopController.Shop = _shop;
        }

        [Test]
        public void TestCreatesClientWithProperCredentials() {
            var client = _shopController.Client;

            Assert.AreEqual(Utils.TestAccessToken, client.AccessToken);
            Assert.AreEqual(Utils.TestShopDomain, client.Domain);
        }

        [Test]
        public void TestCachesClientInstance() {
            var client = _shopController.Client;
            var clientOnNextCall = _shopController.Client;

            Assert.AreSame(client, clientOnNextCall);
        }

        [Test]
        public void TestClearsCachedClientIfLoaderProviderChanges() {
            _shopController.Credentials = new ShopCredentials(Utils.TestShopDomain, Utils.TestAccessToken);

            var client = _shopController.Client;

            var loaderProvider = new UnityLoaderProvider();
            _shopController.LoaderProvider = loaderProvider;

            var clientOnNextCall = _shopController.Client;
            Assert.AreNotSame(client, clientOnNextCall);
        }

        public void TestClearsCachedClientIfCredentialsChange() {
            _shopController.Credentials = new ShopCredentials(Utils.TestShopDomain, Utils.TestAccessToken);
            var client = _shopController.Client;
            _shopController.Credentials = new ShopCredentials("", "");
            var clientOnNextCall = _shopController.Client;
            Assert.AreNotSame(client, clientOnNextCall);
        }

        [UnityTest]
        public IEnumerator TestAddVariantToCartAddsNewItemToCart() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.AddVariant(_variants.First(), _product);
            Assert.AreEqual(1, _shopController.Cart.LineItems.Get(_variants.First()).Quantity);
            _shop.Received().OnCartQuantityChanged(1);
        }

        [UnityTest]
        public IEnumerator TestAddVariantToCartIncrementsExistingItem() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.LineItems.AddOrUpdate(_variants.First(), 5);

            _shopController.Cart.AddVariant(_variants.First(), _product);
            Assert.AreEqual(6, _shopController.Cart.LineItems.Get(_variants.First()).Quantity);
            _shop.Received().OnCartQuantityChanged(6);
        }

        [UnityTest]
        public IEnumerator TestRemoveVariantFromCartWhenItemIsNotInCart() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.RemoveVariant(_variants.First(), _product);
            Assert.IsNull(_shopController.Cart.LineItems.Get(_variants.First()));
        }

        [UnityTest]
        public IEnumerator TestRemoveVariantFromCartDecrementsItemCount() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.LineItems.AddOrUpdate(_variants.First(), 5);
            _shopController.Cart.RemoveVariant(_variants.First(), _product);
            Assert.AreEqual(4, _shopController.Cart.LineItems.Get(_variants.First()).Quantity);
            _shop.Received().OnCartQuantityChanged(4);
        }

        [UnityTest]
        public IEnumerator TestRemoveVariantFromCartDeletesItemIfQuantityBecomesZero() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.LineItems.AddOrUpdate(_variants.First(), 1);
            _shopController.Cart.RemoveVariant(_variants.First(), _product);
            Assert.IsNull(_shopController.Cart.LineItems.Get(_variants.First()));
            _shop.Received().OnCartQuantityChanged(0);
        }

        [UnityTest]
        public IEnumerator TestUpdateItemsInCartSetsQuantity() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.LineItems.AddOrUpdate(_variants.First(), 3);
            _shopController.Cart.UpdateVariant(_variants.First(), _product, 1);
            Assert.AreEqual(1, _shopController.Cart.LineItems.Get(_variants.First()).Quantity);
            _shop.Received().OnCartQuantityChanged(1);
        }

        [UnityTest]
        public IEnumerator TestUpdateItemsInCartToZeroDeletesItem() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.LineItems.AddOrUpdate(_variants.First(), 1);
            _shopController.Cart.UpdateVariant(_variants.First(), _product, 0);
            Assert.IsNull(_shopController.Cart.LineItems.Get(_variants.First()));
            _shop.Received().OnCartQuantityChanged(0);
        }

        [UnityTest]
        public IEnumerator TestUpdateVariantInCartWithNoExistingItemAddsNewItem() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.UpdateVariant(_variants.First(), _product, 1);
            Assert.AreEqual(1, _shopController.Cart.LineItems.Get(_variants.First()).Quantity);
            _shop.Received().OnCartQuantityChanged(1);
        }

        [UnityTest]
        public IEnumerator TestClearCartResetsCart() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _shopController.Cart.UpdateVariant(_variants.First(), _product, 1);
            _shopController.Cart.UpdateVariant(_variants[1], _product, 2);

            _shopController.Cart.ClearCart();
            Assert.AreEqual(0, _shopController.Cart.LineItems.All().Count);
            _shop.Received().OnCartQuantityChanged(0);
        }

        private List<ProductVariant> _variants;
        private Product _product;
        private EditorTimeoutWaiter LoadProductVariantsFromShop() {
            var waiter = new EditorTimeoutWaiter(5f);
            _shopController.Client.products((products, error, after) => {
                _variants = new List<ProductVariant>();
                _product = products[0];
                foreach (var product in products) {
                    _variants.AddRange(product.variants().edges().Select((x) => x.node()));
                }

                waiter.Complete();
            });

            return waiter;
        }
    }
}
