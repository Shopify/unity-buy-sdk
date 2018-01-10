namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.TestTools;
    using NSubstitute;
    using NUnit.Framework;
    using Shopify.UIToolkit.Editor;
    using Shopify.Unity.SDK;

    [TestFixture]
    public class TestShopThemeControllerEditor {
        private ShopThemeControllerEditor _editor;
        private ShopThemeController _controller;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<ShopThemeController>();
            _editor = Editor.CreateEditor(_controller) as ShopThemeControllerEditor;
            _editor.View = Substitute.For<IShopThemeControllerEditorView>();
        }

        [TearDown]
        public void TearDown() {
            GlobalGameObject.Destroy();
        }
    }
}