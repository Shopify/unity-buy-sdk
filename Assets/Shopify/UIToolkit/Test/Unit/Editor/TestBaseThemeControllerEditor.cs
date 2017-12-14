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
    public class TestBaseThemeControllerEditor {
        public class MockThemeController : ThemeControllerBase {
            public override void OnHide() {
            }

            public override void OnShow() {
            }
        }

        [CustomEditor(typeof(MockThemeController))]
        public class MockThemeControllerEditor : BaseThemeControllerEditor {
            public override void OnShowConfiguration() {
            }
        }

        private BaseThemeControllerEditor _editor;
        private MockThemeController _controller;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<MockThemeController>();
            _editor = Editor.CreateEditor(_controller) as BaseThemeControllerEditor;
            _editor.View = Substitute.For<IBaseThemeControllerEditorView>();
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
            _editor.View.Received().DrawThemeHelp();
        }

        [Test]
        public void TestBoundThemeDoesNotDrawHelpBox() {
            _controller.Theme = GlobalGameObject.AddComponent<DebugSingleProductTheme>();
            _editor.OnInspectorGUI();
            _editor.View.DidNotReceive().DrawThemeHelp();
        }

        [Test]
        public void TestCredentialsViewDrawn() {
            _editor.OnInspectorGUI();
            _editor.View.Received().DrawShopCredentialsVerifier();
        }
    }
}
