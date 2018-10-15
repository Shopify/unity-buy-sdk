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
    public class TestMultiProductShopController {
        private MultiProductShopController _controller;
        private IMultiProductShop _shop;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<MultiProductShopController>();
            _shop = Substitute.For<IMultiProductShop>();

            _controller.Shop = _shop;

            _controller.Credentials = new ShopCredentials(Utils.TestShopDomain, Utils.TestAccessToken);
        }
    }
}