namespace Shopify.UIToolkit.Editor {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.SDK.Editor;

    public interface IShopControllerBaseEditorView {
        void DrawShopCredentialsVerifier();
        void DrawShopHelp();
    }

    public abstract class ShopControllerBaseEditor : Editor, IShopControllerBaseEditorView {
        public ShopControllerBase Target {
            get { return target as ShopControllerBase; }
        }

        private ShopCredentialsVerifier _verifier;
        private IShopCredentialsView _credentialsVerifierView;
        public IShopControllerBaseEditorView View;

        public virtual void OnEnable() {
            if (Target == null) return;

            View = View ?? this;
            _verifier = new ShopCredentialsVerifier((IShopCredentials) Target);
            _verifier.OnCredentialsStateChanged += OnCredentialsChanged;
            _credentialsVerifierView = new ShopCredentialsView(_verifier);
        }

        public override void OnInspectorGUI() {
            if (Target.Shop == null) {
                View.DrawShopHelp();
            }

            View.DrawShopCredentialsVerifier();
            if (_verifier.HasVerifiedCredentials()) {
                OnShowConfiguration();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_appleMerchantID"));

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Overriden by an implementor to add configurations under the credentials verifier.
        /// </summary>
        public abstract void OnShowConfiguration();

        private void OnCredentialsChanged() {
            _cachedClient = null;
            OnClientChanged();
            Repaint();
        }

        protected virtual void OnClientChanged() { }

        public ShopifyClient Client {
            get {
                _cachedClient = _cachedClient ?? new ShopifyClient(new UnityEditorLoader(Target.ShopDomain, Target.AccessToken));
                return _cachedClient;
            }
        }
        private ShopifyClient _cachedClient;

        void IShopControllerBaseEditorView.DrawShopCredentialsVerifier() {
            _credentialsVerifierView.DrawInspectorGUI(serializedObject);
        }

        void IShopControllerBaseEditorView.DrawShopHelp() {
            var message = @"
Shop Controllers require a shop to function.
Implement ISingleProductShop with your own custom script and add it to this gameObject to continue.
            ";

            EditorGUILayout.HelpBox(message, MessageType.Warning);
        }
    }
}
