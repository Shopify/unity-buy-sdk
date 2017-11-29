namespace Shopify.UIToolkit.Test.Unit {
    using NUnit.Framework;
    using Shopify.Tests;
    using Shopify.Unity.SDK;
    using UnityEngine;
    using UnityEngine.TestTools;

    [TestFixture]
    public class TestThemeControllerBase {
        public class MockThemeController : ThemeControllerBase {
            public override void OnHide() {}
            public override void OnShow() {}
        }

        private MockThemeController _themeController;

        [SetUp]
        public void Setup() {
            _themeController = GlobalGameObject.AddComponent<MockThemeController>();
        }

        [TearDown]
        public void Teardown() {
            GameObject.DestroyImmediate(_themeController.gameObject);
        }

        [Test]
        public void TestCreatesClientWithProperCredentials() {
            _themeController.AccessToken = Utils.TestAccessToken;
            _themeController.ShopDomain = Utils.TestShopDomain;

            var client = _themeController.Client;

            Assert.AreEqual(Utils.TestAccessToken, client.AccessToken);
            Assert.AreEqual(Utils.TestShopDomain, client.Domain);
        }

        [Test]
        public void TestCachesClientInstance() {
            _themeController.AccessToken = Utils.TestAccessToken;
            _themeController.ShopDomain = Utils.TestShopDomain;

            var client = _themeController.Client;
            var clientOnNextCall = _themeController.Client;

            Assert.AreSame(client, clientOnNextCall);
        }

        [Test]
        public void TestClearsCachedClientIfAccessTokenChanges() {
            _themeController.AccessToken = Utils.TestAccessToken;
            _themeController.ShopDomain = Utils.TestShopDomain;

            var client = _themeController.Client;

            _themeController.AccessToken = "1234";

            var clientOnNextCall = _themeController.Client;

            Assert.AreNotSame(client, clientOnNextCall);
            Assert.AreEqual("1234", clientOnNextCall.AccessToken);
        }

        [Test]
        public void TestClearsCachedClientIfShopDomainChanges() {
            _themeController.AccessToken = Utils.TestAccessToken;
            _themeController.ShopDomain = Utils.TestShopDomain;

            var client = _themeController.Client;

            _themeController.ShopDomain = "other.myshopify.com";

            var clientOnNextCall = _themeController.Client;

            Assert.AreNotSame(client, clientOnNextCall);
            Assert.AreEqual("other.myshopify.com", clientOnNextCall.Domain);
        }
    }
}