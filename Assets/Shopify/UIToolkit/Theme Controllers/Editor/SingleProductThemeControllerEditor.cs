namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;

    [CustomEditor(typeof(SingleProductThemeController))]
    public class SingleProductThemeControllerEditor : Editor {
        public ISingleProductThemeControllerEditorView View;
        public IShopCredentialsView CredentialsView;

        private ShopCredentialsVerifier _verifier {
            get {
                if (_cachedVerifier == null) {
                    _cachedVerifier = new ShopCredentialsVerifier(_credentials);
                }
                return _cachedVerifier;
            }
        }

        private ShopCredentialsVerifier _cachedVerifier;

        private IShopCredentials _credentials {
            get {
                return target as IShopCredentials;
            }
        }

        public SingleProductThemeController Target {
            get {
                return target as SingleProductThemeController;
            }
        }

        public void OnEnable() {
            if (Target == null) return;

            View = new SingleProductThemeControllerEditorView();
            CredentialsView = new ShopCredentialsView(_verifier);
            BindThemeIfPresent();
        }

        public override void OnInspectorGUI() {
            CredentialsView.DrawInspectorGUI(serializedObject);

            if (Target.Theme == null) {
                View.ShowThemeHelp();
                return;
            }
        }

        private void BindThemeIfPresent() {
            if (Target.Theme != null) return;
            Target.Theme = Target.GetComponent<ISingleProductTheme>();
        }
    }
}
