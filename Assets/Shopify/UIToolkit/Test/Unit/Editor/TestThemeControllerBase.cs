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
    public class TestThemeControllerBase {
        public class MockThemeController : ThemeControllerBase {
            protected override void OnHide() {}
            protected override void OnShow() {}
        }

        private MockThemeController _themeController;
        private IThemeBase _theme;

        [SetUp]
        public void Setup() {
            _themeController = GlobalGameObject.AddComponent<MockThemeController>();
            _themeController.AccessToken = Utils.TestAccessToken;
            _themeController.ShopDomain = Utils.TestShopDomain;

            _theme = Substitute.For<IThemeBase>();
            _themeController.Theme = _theme;
        }

        [Test]
        public void TestCreatesClientWithProperCredentials() {
            var client = _themeController.Client;

            Assert.AreEqual(Utils.TestAccessToken, client.AccessToken);
            Assert.AreEqual(Utils.TestShopDomain, client.Domain);
        }

        [Test]
        public void TestCachesClientInstance() {
            var client = _themeController.Client;
            var clientOnNextCall = _themeController.Client;

            Assert.AreSame(client, clientOnNextCall);
        }

        [Test]
        public void TestClearsCachedClientIfAccessTokenChanges() {
            var client = _themeController.Client;

            _themeController.AccessToken = "1234";

            var clientOnNextCall = _themeController.Client;

            Assert.AreNotSame(client, clientOnNextCall);
            Assert.AreEqual("1234", clientOnNextCall.AccessToken);
        }

        [Test]
        public void TestClearsCachedClientIfShopDomainChanges() {
            var client = _themeController.Client;

            _themeController.ShopDomain = "other.myshopify.com";

            var clientOnNextCall = _themeController.Client;

            Assert.AreNotSame(client, clientOnNextCall);
            Assert.AreEqual("other.myshopify.com", clientOnNextCall.Domain);
        }

        [Test]
        public void TestClearsCachedClientIfLoaderProviderChanges() {
            _themeController.AccessToken = Utils.TestAccessToken;
            _themeController.ShopDomain = Utils.TestShopDomain;

            var client = _themeController.Client;

            var loaderProvider = new UnityLoaderProvider();
            _themeController.LoaderProvider = loaderProvider;

            var clientOnNextCall = _themeController.Client;

            Assert.AreNotSame(client, clientOnNextCall);
        }

        [UnityTest]
        public IEnumerator TestAddVariantToCartAddsNewItemToCart() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.AddVariantToCart(_variants.First());
            Assert.AreEqual(1, _themeController.Cart.LineItems.Get(_variants.First()).Quantity);
            _theme.Received().OnCartQuantityChanged(1);
        }

        [UnityTest]
        public IEnumerator TestAddVariantToCartIncrementsExistingItem() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.Cart.LineItems.AddOrUpdate(_variants.First(), 5);

            _themeController.AddVariantToCart(_variants.First());
            Assert.AreEqual(6, _themeController.Cart.LineItems.Get(_variants.First()).Quantity);
            _theme.Received().OnCartQuantityChanged(6);
        }

        [UnityTest]
        public IEnumerator TestRemoveVariantFromCartWhenItemIsNotInCart() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.RemoveVariantFromCart(_variants.First());
            Assert.IsNull(_themeController.Cart.LineItems.Get(_variants.First()));
        }

        [UnityTest]
        public IEnumerator TestRemoveVariantFromCartDecrementsItemCount() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.Cart.LineItems.AddOrUpdate(_variants.First(), 5);
            _themeController.RemoveVariantFromCart(_variants.First());
            Assert.AreEqual(4, _themeController.Cart.LineItems.Get(_variants.First()).Quantity);
            _theme.Received().OnCartQuantityChanged(4);
        }

        [UnityTest]
        public IEnumerator TestRemoveVariantFromCartDeletesItemIfQuantityBecomesZero() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.Cart.LineItems.AddOrUpdate(_variants.First(), 1);
            _themeController.RemoveVariantFromCart(_variants.First());
            Assert.IsNull(_themeController.Cart.LineItems.Get(_variants.First()));
            _theme.Received().OnCartQuantityChanged(0);
        }

        [UnityTest]
        public IEnumerator TestUpdateItemsInCartSetsQuantity() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.Cart.LineItems.AddOrUpdate(_variants.First(), 3);
            _themeController.UpdateVariantInCart(_variants.First(), 1);
            Assert.AreEqual(1, _themeController.Cart.LineItems.Get(_variants.First()).Quantity);
            _theme.Received().OnCartQuantityChanged(1);
        }

        [UnityTest]
        public IEnumerator TestUpdateItemsInCartToZeroDeletesItem() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.Cart.LineItems.AddOrUpdate(_variants.First(), 1);
            _themeController.UpdateVariantInCart(_variants.First(), 0);
            Assert.IsNull(_themeController.Cart.LineItems.Get(_variants.First()));
            _theme.Received().OnCartQuantityChanged(0);
        }

        [UnityTest]
        public IEnumerator TestUpdateVariantInCartWithNoExistingItemAddsNewItem() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.UpdateVariantInCart(_variants.First(), 1);
            Assert.AreEqual(1, _themeController.Cart.LineItems.Get(_variants.First()).Quantity);
            _theme.Received().OnCartQuantityChanged(1);
        }

        [UnityTest]
        public IEnumerator TestClearCartResetsCart() {
            var waiter = LoadProductVariantsFromShop();
            while (waiter.Await()) {
                yield return null;
            }

            _themeController.UpdateVariantInCart(_variants.First(), 1);
            _themeController.UpdateVariantInCart(_variants[1], 2);

            _themeController.ClearCart();
            Assert.AreEqual(0, _themeController.Cart.LineItems.All().Count);
            _theme.Received().OnCartQuantityChanged(0);
        }

        private List<ProductVariant> _variants;
        private EditorTimeoutWaiter LoadProductVariantsFromShop() {
            var waiter = new EditorTimeoutWaiter(5f);
            _themeController.Client.products((products, error, after) => {
                _variants = new List<ProductVariant>();
                foreach (var product in products) {
                    _variants.AddRange(product.variants().edges().Select((x) => x.node()));
                }

                waiter.Complete();
            });

            return waiter;
        }
    }
}