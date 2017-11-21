﻿namespace Shopify.UIToolkit.Test.Unit {
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    [TestFixture]
    public class TestThemeControllerBase {
        public class MockThemeController : ThemeControllerBase { }

        private const string AccessToken = "351c122017d0f2a957d32ae728ad749c";
        private const string ShopDomain = "graphql.myshopify.com";

        private MockThemeController _themeController;

        [SetUp]
        public void Setup() {
            _themeController = new GameObject("mock").AddComponent<MockThemeController>();
        }

        [TearDown]
        public void Teardown() {
            GameObject.DestroyImmediate(_themeController.gameObject);
        }

        [Test]
        public void TestCreatesClientWithProperCredentials() {
            _themeController.AccessToken = AccessToken;
            _themeController.ShopDomain = ShopDomain;

            var client = _themeController.Client;

            Assert.AreEqual(AccessToken, client.AccessToken);
            Assert.AreEqual(ShopDomain, client.Domain);
        }

        [Test]
        public void TestCachesClientInstance() {
            _themeController.AccessToken = AccessToken;
            _themeController.ShopDomain = ShopDomain;

            var client = _themeController.Client;
            var clientOnNextCall = _themeController.Client;

            Assert.AreSame(client, clientOnNextCall);
        }

        [Test]
        public void TestClearsCachedClientIfAccessTokenChanges() {
            _themeController.AccessToken = AccessToken;
            _themeController.ShopDomain = ShopDomain;

            var client = _themeController.Client;

            _themeController.AccessToken = "1234";

            var clientOnNextCall = _themeController.Client;

            Assert.AreNotSame(client, clientOnNextCall);
            Assert.AreEqual("1234", clientOnNextCall.AccessToken);
        }

        [Test]
        public void TestClearsCachedClientIfShopDomainChanges() {
            _themeController.AccessToken = AccessToken;
            _themeController.ShopDomain = ShopDomain;

            var client = _themeController.Client;

            _themeController.ShopDomain = "other.myshopify.com";

            var clientOnNextCall = _themeController.Client;

            Assert.AreNotSame(client, clientOnNextCall);
            Assert.AreEqual("other.myshopify.com", clientOnNextCall.Domain);
        }
    }
}