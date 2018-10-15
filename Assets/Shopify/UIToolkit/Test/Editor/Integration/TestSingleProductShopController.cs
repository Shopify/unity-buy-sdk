namespace Shopify.UIToolkit.Test.Integration {
    using UnityEngine;
    using System.Linq;
    using NUnit.Framework;
    using NSubstitute;
    using UnityEngine.TestTools;
    using Shopify.Unity;
    using Shopify.Tests;
    using Shopify.Unity.SDK;
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity.SDK.Editor;

    public class TestSingleProductShopController {
        private SingleProductShopController _controller;
        private ISingleProductShop _shop;
        private GameObject _gameObject;

        [SetUp]
        public void Setup() {
            _gameObject = new GameObject("TestSingleProductShopController");
            _controller = _gameObject.AddComponent<SingleProductShopController>();
            _controller.LoaderProvider = new UnityEditorLoaderProvider();
            _shop = Substitute.For<ISingleProductShop>();
            _controller.Shop = _shop;
            _controller.Credentials = new ShopCredentials(Utils.TestShopDomain, Utils.TestAccessToken);
        }

        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(_gameObject);
        }

        [Test]
        public void TestControllerHasProductGID() {
            _controller.ProductGID = "hello";
            Assert.AreEqual("hello", _controller.ProductGID);
        }

        [UnityTest]
        public IEnumerator TestLoadLoadsProductAndTellsShop() {
            _controller.ProductGID = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODE0NzU=";
            _controller.Load();

            var waiter = new EditorTimeoutWaiter();

            _shop.When((x) => {
                x.OnProductLoaded(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
            }).Do((x) => {
                var product = x.Args()[0] as Product;
                var productVariants = x.Args()[1] as ProductVariant[];

                Assert.AreEqual("Arena Zip Boot", product.title());
                Assert.AreEqual(10, productVariants.Length);

                waiter.Complete();
            });

            while (waiter.Await()) {
               yield return null;
            }
        }

        [UnityTest]
        public IEnumerator TestShowCallsOnErrorWhenProductDoesntExist() {
            _controller.ProductGID = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0LzEzNTI1NDIzOQ==";
            _controller.Load();

            var callbackCalled = false;

            _shop.When((x) => {
                x.OnError(Arg.Any<ShopifyError>());
            }).Do((x) => {
                var error = x.Args()[0] as ShopifyError;

                Assert.AreEqual(ShopifyError.ErrorType.UserError, error.Type);
                Assert.AreEqual("Product not found", error.Description);

                callbackCalled = true;
            });

            while (!callbackCalled) {
               yield return null;
            }

            _shop.DidNotReceive().OnProductLoaded(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
        }

        [UnityTest]
        public IEnumerator TestShowPropogatesErrorsToShopOnError() {
            _controller.ProductGID = "notevenclose";
            _controller.Load();

            var waiter = new EditorTimeoutWaiter();

            _shop.When((x) => {
                x.OnError(Arg.Any<ShopifyError>());
            }).Do((x) => {
                var error = x.Args()[0] as ShopifyError;
                Assert.AreEqual(ShopifyError.ErrorType.GraphQL, error.Type);
                waiter.Complete();
            });

            while (!waiter.Await()) {
               yield return null;
            }

            _shop.DidNotReceive().OnProductLoaded(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
        }
    }
}
