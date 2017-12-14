namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.TestTools;
    using NSubstitute;
    using NUnit.Framework;
    using Shopify.UIToolkit.Editor;
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
        }

        [TearDown]
        public void TearDown() {
            GlobalGameObject.Destroy();
        }

        [Test]
        public void TestNotVerifiedCredentialsDoesNotShowProductPicker() {
            _controller.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _editor.OnEnable();
            _editor.OnInspectorGUI();
            _editor.View.DidNotReceive().DrawProductPicker();
        }

        [Test]
        public void TestVerifiedCredentialsDoesShowProductPicker() {
            _controller.CredentialsVerificationState = ShopCredentialsVerificationState.Verified;
            _editor.OnEnable();
            _editor.OnInspectorGUI();
            _editor.View.Received().DrawProductPicker();
        }

        [Test]
        public void TestInvalidCredentialsDoesNotShowProductPicker() {
            _controller.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
            _editor.OnEnable();
            _editor.OnInspectorGUI();
            _editor.View.DidNotReceive().DrawProductPicker();
        }
    }
}