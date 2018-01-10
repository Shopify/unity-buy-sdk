namespace Shopify.UIToolkit.Test.Unit {
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

    [TestFixture]
    public class TestSingleProductShopController {
        private SingleProductShopController _controller;
        private ISingleProductShop _shop;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<SingleProductShopController>();
            _shop = Substitute.For<ISingleProductShop>();

            _controller.Shop = _shop;

            _controller.ShopDomain = Utils.TestShopDomain;
            _controller.AccessToken = Utils.TestAccessToken;
        }

        [Test]
        public void TestControllerHasProductGID() {
            _controller.ProductGID = "hello";
            Assert.AreEqual("hello", _controller.ProductGID);
        }

        [UnityTest]
        public IEnumerator TestShowLoadsProductAndTellsShop() {
            _controller.ProductGID = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODE0NzU=";
            _controller.Show();

            var waiter = new EditorTimeoutWaiter();

            _shop.When((x) => {
                x.OnShouldShowProduct(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
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
            _controller.Show();

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

            _shop.DidNotReceive().OnShouldShowProduct(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
        }

        [UnityTest]
        public IEnumerator TestShowPropogatesErrorsToShopOnError() {
            _controller.ProductGID = "notevenclose";
            _controller.Show();

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

            _shop.DidNotReceive().OnShouldShowProduct(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
        }
    }
}