namespace Shopify.UIToolkit.Test.Unit {
    using UnityEngine.TestTools;
    using UnityEditor;
    using UnityEngine;
    using NUnit.Framework;
    using Shopify.UIToolkit.Editor;
    using Shopify.UIToolkit.Themes;
    using NSubstitute;
    using Shopify.Unity.SDK;

    [TestFixture]
    public class TestSingleProductThemeControllerEditor {
        private SingleProductThemeControllerEditor _editor;
        private SingleProductThemeController _controller;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<SingleProductThemeController>();
            _editor = Editor.CreateEditor(_controller) as SingleProductThemeControllerEditor;

            _editor.View = Substitute.For<ISingleProductThemeControllerEditorView>();
            _editor.CredentialsView = Substitute.For<IShopCredentialsView>();
        }

        [TearDown]
        public void TearDown() {
            GlobalGameObject.Destroy();
        }

        [Test]
        public void TestBindsThemeOnEnable() {
            var theme = GlobalGameObject.AddComponent<DebugSingleProductTheme>();
            Assert.IsNull(_controller.Theme);
            _editor.OnEnable();
            Assert.AreEqual(_controller.Theme, theme);
        }

        [Test]
        public void TestNullThemeDrawsHelpBox() {
            Assert.IsNull(_controller.Theme);
            _editor.OnInspectorGUI();
            _editor.View.Received().ShowThemeHelp();
        }

        [Test]
        public void TestBoundThemeDoesNotDrawHelpBox() {
            _controller.Theme = GlobalGameObject.AddComponent<DebugSingleProductTheme>();
            _editor.OnInspectorGUI();
            _editor.View.DidNotReceive().ShowThemeHelp();
        }
    }
}
