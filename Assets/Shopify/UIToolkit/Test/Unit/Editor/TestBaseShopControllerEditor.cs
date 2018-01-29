namespace Shopify.UIToolkit.Test.Unit {
    using UnityEngine.TestTools;
    using UnityEditor;
    using UnityEngine;
    using NUnit.Framework;
    using Shopify.UIToolkit.Editor;
    using Shopify.UIToolkit.Shops;
    using NSubstitute;
    using Shopify.Unity.SDK;

    [TestFixture]
    public class TestShopControllerBaseEditor {
        public class MockShopController : ShopControllerBase {
            public override void Load() {
            }
            
            public override void UnLoad() {
            }
        }

        [CustomEditor(typeof(MockShopController))]
        public class MockShopControllerEditor : ShopControllerBaseEditor {
            public override void OnShowConfiguration() {
            }
        }

        private ShopControllerBaseEditor _editor;
        private MockShopController _controller;

        [SetUp]
        public void Setup() {
            _controller = GlobalGameObject.AddComponent<MockShopController>();
            _editor = Editor.CreateEditor(_controller) as ShopControllerBaseEditor;
            _editor.View = Substitute.For<IShopControllerBaseEditorView>();
        }

        [TearDown]
        public void TearDown() {
            GlobalGameObject.Destroy();
        }

        [Test]
        public void TestNullShopDrawsHelpBox() {
            Assert.IsNull(_controller.Shop);
            _editor.OnInspectorGUI();
            _editor.View.Received().DrawShopHelp();
        }

        [Test]
        public void TestBoundShopDoesNotDrawHelpBox() {
            GlobalGameObject.AddComponent<DebugSingleProductShop>();
            _editor.OnInspectorGUI();
            _editor.View.DidNotReceive().DrawShopHelp();
        }

        [Test]
        public void TestCredentialsViewDrawn() {
            _editor.OnInspectorGUI();
            _editor.View.Received().DrawShopCredentialsVerifier();
        }

        [Test]
        public void TestPaymentPropertiesViewDraw() {
            _editor.OnInspectorGUI();
            _editor.View.Received().DrawPaymentProperties();
        }
    }
}
