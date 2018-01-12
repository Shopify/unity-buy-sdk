namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;

    public interface ISingleProductShopControllerEditorView : IShopControllerBaseEditorView {
        void DrawProductPicker();
    }

    [CustomEditor(typeof(SingleProductShopController))]
    public class SingleProductShopControllerEditor : ShopControllerBaseEditor, ISingleProductShopControllerEditorView {
        public new SingleProductShopController Target {
            get {
                return (SingleProductShopController) target;
            }
        }

        public new ISingleProductShopControllerEditorView View {
            get {
                return (ISingleProductShopControllerEditorView) base.View;
            }

            set {
                base.View = value;
            }
        }

        public override void OnEnable() {
            base.OnEnable();
            View = View ?? this;
        }

        private ProductPicker _productPicker {
            get {
                if (_cachedPicker == null) {
                    _cachedPicker = new ProductPicker(Client);
                    _cachedPicker.OnProductListUpdated += () => {
                        Repaint();
                    };
                }

                return _cachedPicker;
            }
        }
        private ProductPicker _cachedPicker;

        public override void OnShowConfiguration() {
            View.DrawProductPicker();
        }

        protected override void OnClientChanged() {
            _productPicker.Client = Client;
        }

        void ISingleProductShopControllerEditorView.DrawProductPicker() {
            _productPicker.DrawInspectorGUI(serializedObject);
        }
    }
}
