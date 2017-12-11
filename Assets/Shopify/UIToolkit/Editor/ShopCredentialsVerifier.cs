namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.SDK.Editor;

    public interface IShopCredentialsView {
        void DrawInspectorGUI(SerializedObject serializedObject);
    }

    public class ShopCredentialsView : IShopCredentialsView {
        private ShopCredentialsVerifier _verifier;

        public ShopCredentialsView(ShopCredentialsVerifier verifier) {
            _verifier = verifier;
        }

        /// <summary>
        /// Draws the GUI for the verifier, should be called from an editor
        /// in OnInspectorGUI
        /// </summary>
        /// <param name="serializedObject">The serialized version of the object provided as the context</param>
        public void DrawInspectorGUI(SerializedObject serializedObject) {
            EditorGUILayout.Separator();

            var disableCredentialsForm = _verifier.Credentials.CredentialsVerificationState == ShopCredentialsVerificationState.Verified;

            EditorGUI.BeginDisabledGroup(disableCredentialsForm);
            DrawCredentialsForm(serializedObject);
            EditorGUI.EndDisabledGroup();

            DrawMessageBox();
            DrawActionButton();
            EditorGUILayout.Separator();
        }

        private void DrawCredentialsForm(SerializedObject serializedObject) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShopDomain"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AccessToken"));
        }

        private void DrawActionButton() {
            switch (_verifier.Credentials.CredentialsVerificationState) {
                case ShopCredentialsVerificationState.Invalid:
                    ActionButton("Try Again", _verifier.VerifyCredentials);
                    break;
                case ShopCredentialsVerificationState.Verified:
                    ActionButton("Use Different Credentials", _verifier.ResetVerificationState);
                    break;
                case ShopCredentialsVerificationState.Unverified:
                    ActionButton("Verify Credentials", _verifier.VerifyCredentials);
                    break;
            }
        }

        private void ActionButton(string label, Action onClick) {
            if (GUILayout.Button(label)) {
                onClick.Invoke();
            }
        }

        private void DrawMessageBox() {
            if (_verifier.Credentials.CredentialsVerificationState == ShopCredentialsVerificationState.Invalid) {
                EditorGUILayout.HelpBox("The credentials provided could not be used to connect to your shop.", MessageType.Error);
            }
        }
    }

    /// <summary>
    /// A helper for the editor that allows verifying shop credentials
    /// and drawing a UI to support the feature.
    /// </summary>
    public class ShopCredentialsVerifier {
        public IShopCredentials Credentials { private set; get; }

        private bool _IsRequestInProgress;

        /// <summary>
        /// Creates a new verifier
        /// </summary>
        /// <param name="context">The object with credentials</param>
        public ShopCredentialsVerifier(IShopCredentials credentials) {
            Credentials = credentials;
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

            ShopifyClient client;
            try {
                client = Client();
            } catch (ArgumentException e) {
                Debug.LogWarning(e);
                _IsRequestInProgress = false;
                Credentials.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
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
            return Credentials.CredentialsVerificationState == ShopCredentialsVerificationState.Verified;
        }

        /// <summary>
        /// Resets the verification state of the context object
        /// </summary>
        public void ResetVerificationState() {
            Credentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
        }

        private ShopifyClient Client() {
            return new ShopifyClient(new UnityEditorLoader(Credentials.GetShopDomain(), Credentials.GetAccessToken()));
        }

        private void OnVerificationRequestComplete(QueryRoot result, ShopifyError errors) {
            _IsRequestInProgress = false;

            if (errors != null) {
                Credentials.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
            } else {
                Credentials.CredentialsVerificationState = ShopCredentialsVerificationState.Verified;
            }
        }
    }
}
