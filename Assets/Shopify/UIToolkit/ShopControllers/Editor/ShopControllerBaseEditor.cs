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
        void DrawPaymentProperties();
    }

    public abstract class ShopControllerBaseEditor : Editor, IShopControllerBaseEditorView {
        public ShopControllerBase Target {
            get { return target as ShopControllerBase; }
        }

        private ShopCredentialsVerifier _verifier;
        public IShopControllerBaseEditorView View;
        public ShopifyClient Client {
            get {
                _cachedClient = _cachedClient ?? new ShopifyClient(new UnityEditorLoader(Target.Credentials.Domain, Target.Credentials.AccessToken));
                return _cachedClient;
            }
        }
        private ShopifyClient _cachedClient;

        public virtual void OnEnable() {
            if (Target == null) return;

            View = View ?? this;
            _verifier = new ShopCredentialsVerifier(Target.Credentials);
            _verifier.OnCredentialsStateShouldChange += OnCredentialsStateShouldChange;
        }

        public override void OnInspectorGUI() {
            if (Target.Shop == null) {
                View.DrawShopHelp();
            }

            View.DrawShopCredentialsVerifier();

            if (_verifier.HasVerifiedCredentials()) {
                OnShowConfiguration();
                View.DrawPaymentProperties();
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Overriden by an implementor to add configurations under the credentials verifier.
        /// </summary>
        public abstract void OnShowConfiguration();

        private void OnCredentialsStateShouldChange(ShopCredentials.VerificationState newState) {
            Undo.RecordObject(Target, "Credentials Verification State Changed");

            Target.Credentials.State = newState;

            _cachedClient = null;
            OnClientChanged();
            Repaint();
        }

        protected virtual void OnClientChanged() { }

        void IShopControllerBaseEditorView.DrawShopHelp() {
            var message = @"
Shop Controllers require a shop to function.
Implement ISingleProductShop with your own custom script and add it to this gameObject to continue.
            ";

            EditorGUILayout.HelpBox(message, MessageType.Warning);
        }

        void IShopControllerBaseEditorView.DrawPaymentProperties() {
            #if UNITY_IOS
            EditorGUILayout.LabelField("Payment Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_appleMerchantID"));
            #endif
        }

        void IShopControllerBaseEditorView.DrawShopCredentialsVerifier() {
            EditorGUILayout.Separator();

            var disableCredentialsForm = _verifier.HasVerifiedCredentials() || _verifier.RequestInProgress;

            EditorGUI.BeginDisabledGroup(disableCredentialsForm);

            EditorGUI.BeginChangeCheck();
            var newDomain = EditorGUILayout.TextField("Shop Domain", Target.Credentials.Domain);
            var newAccessToken = EditorGUILayout.TextField("Access Token", Target.Credentials.AccessToken);

            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(Target, "Changed Shop Credentials");
                Target.Credentials = new ShopCredentials(newDomain, newAccessToken);
                _verifier.Credentials = Target.Credentials;
            }

            EditorGUI.EndDisabledGroup();

            if (_verifier.Credentials.State == ShopCredentials.VerificationState.Invalid) {
                EditorGUILayout.HelpBox("The credentials provided could not be used to connect to your shop.", MessageType.Error);
            }

            if (_verifier.RequestInProgress) {
                EditorGUI.BeginDisabledGroup(true);
                ActionButton("Verifying...", () => { });
                EditorGUI.EndDisabledGroup();
            } else {
                switch (Target.Credentials.State) {
                    case ShopCredentials.VerificationState.Invalid:
                        ActionButton("Try Again", _verifier.VerifyCredentials);
                        break;
                    case ShopCredentials.VerificationState.Verified:
                        ActionButton("Use Different Credentials", _verifier.ResetVerificationState);
                        break;
                    case ShopCredentials.VerificationState.Unverified:
                        ActionButton("Verify Credentials", _verifier.VerifyCredentials);
                        break;
                }
            }
        }

        private void ActionButton(string label, System.Action onClick) {
            if (GUILayout.Button(label)) {
                onClick.Invoke();
            }
        }
    }
}
