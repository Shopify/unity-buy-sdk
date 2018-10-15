namespace Shopify.UIToolkit.Test.Unit {
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.UIToolkit.Editor;
    using Shopify.Unity;
    using NSubstitute;
    using UnityEngine;
    using Shopify.Unity.SDK.Editor;
    using UnityEditor;
    using UnityEngine.TestTools;
    using Shopify.Unity.SDK;

    [TestFixture]
    public class TestProductPicker  {
        private const string AccessToken = "351c122017d0f2a957d32ae728ad749c";
        private const string ShopDomain = "graphql.myshopify.com";
        private IProductPickerView _view;
        private ProductPicker _picker;
        private ShopifyClient _client;

        [SetUp]
        public void Setup() {
            _client = new ShopifyClient(new UnityEditorLoader(ShopDomain, AccessToken));
            _view = Substitute.For<IProductPickerView>();
            _picker = new ProductPicker(_client, _view);

        }

        [Test]
        public void TestProductPickerRendersLoadingWhenNoProductsAvailable() {
            _picker.OnShouldRefreshProductList();
            _picker.DrawInspectorGUI(null);

            _view.DidNotReceive().DrawProductLoadingError(null);
            _view.DidNotReceive().DrawProductPicker(null, Arg.Any<Dictionary<string, string>>());
            _view.DidNotReceive().DrawShopHasNoProductsState(null);
            _view.Received().DrawProductLoadingState(null);
        }
    }
}
