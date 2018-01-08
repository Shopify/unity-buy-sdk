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
    public class TestShopThemeController {
        private ShopThemeController _controller;
        private IShopTheme _theme;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<ShopThemeController>();
            _theme = Substitute.For<IShopTheme>();

            _controller.Theme = _theme;

            _controller.ShopDomain = Utils.TestShopDomain;
            _controller.AccessToken = Utils.TestAccessToken;
        }
    }
}