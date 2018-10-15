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
        private GameObject _gameObject;

        [SetUp]
        public void Setup() {
            _gameObject = new GameObject("TestShopControllerBase");
            _shopController = _gameObject.AddComponent<MockShopController>();
            _shopController.Credentials = new ShopCredentials(Utils.TestShopDomain, Utils.TestAccessToken);

            _shop = Substitute.For<IShop>();
            _shopController.Shop = _shop;
        }

        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(_gameObject);
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

        [Test]
        public void TestClearsCachedClientIfCredentialsChange() {
            _shopController.Credentials = new ShopCredentials(Utils.TestShopDomain, Utils.TestAccessToken);
            var client = _shopController.Client;
            _shopController.Credentials = new ShopCredentials("", "");
            var clientOnNextCall = _shopController.Client;
            Assert.AreNotSame(client, clientOnNextCall);
        }
    }
}
