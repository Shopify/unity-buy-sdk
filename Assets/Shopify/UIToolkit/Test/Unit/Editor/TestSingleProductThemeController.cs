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
    public class TestSingleProductThemeController {
        private SingleProductThemeController _controller;
        private ISingleProductTheme _theme;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<SingleProductThemeController>();
            _theme = Substitute.For<ISingleProductTheme>();

            _controller.Theme = _theme;

            _controller.ShopDomain = Utils.TestShopDomain;
            _controller.AccessToken = Utils.TestAccessToken;
        }

        [Test]
        public void TestControllerHasProductGID() {
            _controller.ProductGID = "hello";
            Assert.AreEqual("hello", _controller.ProductGID);
        }

        [UnityTest]
        public IEnumerator TestShowLoadsProductAndTellsTheme() {
            _controller.ProductGID = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODE0NzU=";
            _controller.Show();

            var callbackCalled = false;

            _theme.When((x) => {
                x.OnShouldShowProduct(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
            }).Do((x) => {
                var product = x.Args()[0] as Product;
                var productVariants = x.Args()[1] as ProductVariant[];

                Assert.AreEqual("Arena Zip Boot", product.title());
                Assert.AreEqual(10, productVariants.Length);

                callbackCalled = true;
            });

            while (!callbackCalled) {
               yield return null;
            }
        }

        /* TODO: Fix bug where exception is thrown when productGID is valid but not found on shop
        [UnityTest]
        public IEnumerator TestShowCallsOnErrorWhenProductDoesntExist() {
            _controller.ProductGID = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0LzEzNTI1NDIzOQ==";
            _controller.Show();

            var callbackCalled = false;

            _theme.When((x) => {
                x.OnError(Arg.Any<ShopifyError>());
            }).Do((x) => {
                var error = x.Args()[0] as ShopifyError;

                Assert.AreEqual(ShopifyError.ErrorType.GraphQL, error.Type);
                Assert.AreEqual("", error.Description);

                callbackCalled = true;
            });

            while (!callbackCalled) {
               yield return null;
            }

            _theme.DidNotReceive().OnShouldShowProduct(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
        }
        */

        [UnityTest]
        public IEnumerator TestShowPropogatesErrorsToThemeOnError() {
            _controller.ProductGID = "notevenclose";
            _controller.Show();

            var callbackCalled = false;

            _theme.When((x) => {
                x.OnError(Arg.Any<ShopifyError>());
            }).Do((x) => {
                var error = x.Args()[0] as ShopifyError;
                Assert.AreEqual(ShopifyError.ErrorType.GraphQL, error.Type);
                callbackCalled = true;
            });

            while (!callbackCalled) {
               yield return null;
            }

            _theme.DidNotReceive().OnShouldShowProduct(Arg.Any<Product>(), Arg.Any<ProductVariant[]>());
        }
    }
}