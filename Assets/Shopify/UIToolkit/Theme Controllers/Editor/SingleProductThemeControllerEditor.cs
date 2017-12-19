namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;

    public interface ISingleProductThemeControllerEditorView : IBaseThemeControllerEditorView {
        void DrawProductPicker();
    }

    [CustomEditor(typeof(SingleProductThemeController))]
    public class SingleProductThemeControllerEditor : BaseThemeControllerEditor, ISingleProductThemeControllerEditorView {
        public new SingleProductThemeController Target {
            get {
                return (SingleProductThemeController) target;
            }
        }

        public new ISingleProductThemeControllerEditorView View {
            get {
                return (ISingleProductThemeControllerEditorView) base.View;
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
                _cachedPicker = _cachedPicker ?? new ProductPicker(Client);
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

        void ISingleProductThemeControllerEditorView.DrawProductPicker() {
            _productPicker.DrawInspectorGUI(serializedObject);
        }
    }
}
