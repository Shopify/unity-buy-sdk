namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Events;
    using System;
    using Shopify.UIToolkit;

    /// <summary>
    /// Base/common functionality for all of our ThemeController editor scripts.
    /// </summary>
    public class ThemeControllerEditorBase : Editor {

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

        public override void OnInspectorGUI() {
            _verifier.DrawInspectorGUI(serializedObject);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
