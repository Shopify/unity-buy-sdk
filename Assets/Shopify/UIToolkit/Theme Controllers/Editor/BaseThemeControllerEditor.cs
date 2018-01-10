namespace Shopify.UIToolkit.Editor {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.SDK.Editor;

    public interface IBaseThemeControllerEditorView {
        void DrawShopCredentialsVerifier();
        void DrawThemeHelp();
    }

    public abstract class BaseThemeControllerEditor : Editor, IBaseThemeControllerEditorView {
        public ThemeControllerBase Target {
            get { return target as ThemeControllerBase; }
        }

        private ShopCredentialsVerifier _verifier;
        private IShopCredentialsView _credentialsVerifierView;
        public IBaseThemeControllerEditorView View;

        public virtual void OnEnable() {
            if (Target == null) return;

            View = View ?? this;
            _verifier = new ShopCredentialsVerifier((IShopCredentials) Target);
            _verifier.OnCredentialsStateChanged += OnCredentialsChanged;
            _credentialsVerifierView = new ShopCredentialsView(_verifier);
        }

        public override void OnInspectorGUI() {
            if (Target.Theme == null) {
                View.DrawThemeHelp();
            }

            View.DrawShopCredentialsVerifier();
            if (_verifier.HasVerifiedCredentials()) {
                OnShowConfiguration();
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Overriden by an implementor to add configurations under the credentials verifier.
        /// </summary>
        public abstract void OnShowConfiguration();

        private void OnCredentialsChanged() {
            _cachedClient = null;
            OnClientChanged();
        }

        protected virtual void OnClientChanged() { }

        public ShopifyClient Client {
            get {
                _cachedClient = _cachedClient ?? new ShopifyClient(new UnityEditorLoader(Target.ShopDomain, Target.AccessToken));
                return _cachedClient;
            }
        }
        private ShopifyClient _cachedClient;

        void IBaseThemeControllerEditorView.DrawShopCredentialsVerifier() {
            _credentialsVerifierView.DrawInspectorGUI(serializedObject);
        }

        void IBaseThemeControllerEditorView.DrawThemeHelp() {
            var message = @"
Theme Controllers require a theme to function.
Implement ISingleProductTheme with your own custom script and add it to this gameObject to continue.
            ";

            EditorGUILayout.HelpBox(message, MessageType.Warning);
        }
    }
}
