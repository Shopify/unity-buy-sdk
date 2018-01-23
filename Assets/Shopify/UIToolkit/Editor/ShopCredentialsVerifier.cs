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

            var disableCredentialsForm = _verifier.HasVerifiedCredentials() || _verifier.RequestInProgress;

            EditorGUI.BeginDisabledGroup(disableCredentialsForm);
            DrawCredentialsForm(serializedObject);
            EditorGUI.EndDisabledGroup();

            DrawMessageBox();
            DrawActionButton();
            EditorGUILayout.Separator();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCredentialsForm(SerializedObject serializedObject) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_shopDomain"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_accessToken"));
        }

        private void DrawActionButton() {
            if (_verifier.RequestInProgress) {
                EditorGUI.BeginDisabledGroup(true);
                ActionButton("Verifying...", () => { });
                EditorGUI.EndDisabledGroup();
                return;
            }

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
        public delegate void OnCredentialsStateChangedHandler();
        public event OnCredentialsStateChangedHandler OnCredentialsStateChanged;

        public IShopCredentials Credentials { private set; get; }

        public bool RequestInProgress { get; private set; }

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
            RequestInProgress = true;

            ShopifyClient client;

            try {
                client = Client();

                client.Query((root) => {
                    root.shop((shop) => {
                        shop.name();
                    });
                }, (QueryRoot result, ShopifyError error) => {
                    RequestInProgress = false;

                    OnVerificationRequestComplete(result, error);

                    if (error != null) {
                        onFailure();
                    } else {
                        onSuccess();
                    }
                });
            } catch (Exception e) {
                Debug.LogError(e);
                RequestInProgress = false;
                UpdateVerificationState(ShopCredentialsVerificationState.Invalid);
                onFailure();
                return;
            }
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
            UpdateVerificationState(ShopCredentialsVerificationState.Unverified);
        }

        private void UpdateVerificationState(ShopCredentialsVerificationState newState) {
            Credentials.CredentialsVerificationState = newState;
            if (OnCredentialsStateChanged != null) OnCredentialsStateChanged();
        }

        private ShopifyClient Client() {
            return new ShopifyClient(new UnityEditorLoader(Credentials.GetShopDomain(), Credentials.GetAccessToken()));
        }

        private void OnVerificationRequestComplete(QueryRoot result, ShopifyError errors) {
            RequestInProgress = false;

            if (errors != null) {
                UpdateVerificationState(ShopCredentialsVerificationState.Invalid);
            } else {
                UpdateVerificationState(ShopCredentialsVerificationState.Verified);
            }
        }
    }
}
