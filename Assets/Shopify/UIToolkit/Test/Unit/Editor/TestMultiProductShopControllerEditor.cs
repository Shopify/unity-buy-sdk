namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.TestTools;
    using NSubstitute;
    using NUnit.Framework;
    using Shopify.Unity.SDK;

    [TestFixture]
    public class TestMultiProductShopEditor {
        private MultiProductShopControllerEditor _editor;
        private MultiProductShopController _controller;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<MultiProductShopController>();
            _editor = Editor.CreateEditor(_controller) as MultiProductShopControllerEditor;
            _editor.View = Substitute.For<IMultiProductShopControllerEditorView>();
        }

        [TearDown]
        public void TearDown() {
            GlobalGameObject.Destroy();
        }
    }
}