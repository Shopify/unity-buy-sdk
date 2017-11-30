namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.SDK.Editor;

    /// <summary>
    /// A helper for the editor that allows verifying shop credentials
    /// and drawing a UI to support the feature.
    /// </summary>
    public class ShopCredentialsVerifier {
        private IShopCredentials _Credentials;
        private bool _IsRequestInProgress;

        /// <summary>
        /// Creates a new verifier
        /// </summary>
        /// <param name="context">The object with credentials</param>
        public ShopCredentialsVerifier(IShopCredentials credentials) {
            _Credentials = credentials;
        }

        /// <summary>
        /// Draws the GUI for the verifier, should be called from an editor
        /// in OnInspectorGUI
        /// </summary>
        /// <param name="serializedObject">The serialized version of the object provided as the context</param>
        public void DrawInspectorGUI(SerializedObject serializedObject) {
            EditorGUILayout.Separator();

            var disableCredentialsForm = _Credentials.CredentialsVerificationState == ShopCredentialsVerificationState.Verified;

            EditorGUI.BeginDisabledGroup(disableCredentialsForm);
            DrawCredentialsForm(serializedObject);
            EditorGUI.EndDisabledGroup();

            DrawMessageBox();
            DrawActionButton();
            EditorGUILayout.Separator();
        }

        /// <summary>
        /// Perform an async request to see if the credentials are valid.
        /// This request will also mutate the context object's CredentialsVerificationState.
        /// </summary>
        /// <param name="onSuccess">Callback called when the credentials are found to be valid</param>
        /// <param name="onFailure">Callback called when the credentials are found to be invalid</param>
        public void VerifyCredentials(Action onSuccess, Action onFailure) {
            if (_IsRequestInProgress) 
                throw new InvalidOperationException("Can't verify credentials when a verification request is already in progress.");

            _IsRequestInProgress = true;

            var client = MakeClient();
            if (client == null) {
                _IsRequestInProgress = false;
                onFailure();
                return;
            }

            client.Query((root) => {
                root.shop((shop) => {
                    shop.name();
                });
            }, (QueryRoot result, ShopifyError error) => {
                _IsRequestInProgress = false;

                OnVerificationRequestComplete(result, error);

                if (error != null) {
                    onFailure();
                } else {
                    onSuccess();
                }
            });
        }

        /// <summary>
        /// Perform an async request to see if the credentials are valid.
        /// This request will also mutate the context object's CredentialsVerificationState.
        /// </summary>
        public void VerifyCredentials() {
            VerifyCredentials(() => { }, () => { });
        }

        /// <summary>
        /// Have the context's credentials been verified already?
        /// </summary>
        /// <returns>True if the context's credentials are verified</returns>
        public bool HasVerifiedCredentials() {
            return _Credentials.CredentialsVerificationState == ShopCredentialsVerificationState.Verified;
        }

        /// <summary>
        /// Resets the verification state of the context object
        /// </summary>
        public void ResetVerificationState() {
            _Credentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
        }

        private void DrawCredentialsForm(SerializedObject serializedObject) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShopDomain"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AccessToken"));
        }

        private void DrawActionButton() {
            switch (_Credentials.CredentialsVerificationState) {
                case ShopCredentialsVerificationState.Invalid:
                    ActionButton("Try Again", VerifyCredentials);
                    break;
                case ShopCredentialsVerificationState.Verified:
                    ActionButton("Use Different Credentials", ResetVerificationState);
                    break;
                case ShopCredentialsVerificationState.Unverified:
                    ActionButton("Verify Credentials", VerifyCredentials);
                    break;
            }
        }

        private void ActionButton(string label, Action onClick) {
            if (GUILayout.Button(label)) {
                onClick.Invoke();
            }
        }

        private void DrawMessageBox() {
            if (_Credentials.CredentialsVerificationState == ShopCredentialsVerificationState.Invalid) {
                EditorGUILayout.HelpBox("The credentials provided could not be used to connect to your shop.", MessageType.Error);
            }
        }

        private ShopifyClient MakeClient() {
            if (_Credentials.GetShopDomain().Trim().Length == 0) {
                return null;
            }

            return new ShopifyClient(new UnityEditorLoader(_Credentials.GetShopDomain(), _Credentials.GetAccessToken()));
        }

        private void OnVerificationRequestComplete(QueryRoot result, ShopifyError errors) {
            _IsRequestInProgress = false;

            if (errors != null) {
                _Credentials.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
            } else {
                _Credentials.CredentialsVerificationState = ShopCredentialsVerificationState.Verified;
            }
        }
    }
}
