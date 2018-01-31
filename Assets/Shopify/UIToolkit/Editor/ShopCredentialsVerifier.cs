namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.SDK.Editor;

    /// <summary>
    /// A helper for the editor that allows verifying shop credentials.
    /// </summary>
    public class ShopCredentialsVerifier {
        public delegate void OnCredentialsStateChangedHandler(ShopCredentials.VerificationState newState);
        public event OnCredentialsStateChangedHandler OnCredentialsStateShouldChange;

        public ShopCredentials Credentials { get; set; }
        public bool RequestInProgress { get; private set; }

        /// <summary>
        /// Creates a new verifier
        /// </summary>
        /// <param name="context">The object with credentials</param>
        public ShopCredentialsVerifier(ShopCredentials credentials) {
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
                UpdateVerificationState(ShopCredentials.VerificationState.Invalid);
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
            return Credentials.State == ShopCredentials.VerificationState.Verified;
        }

        /// <summary>
        /// Resets the verification state of the context object
        /// </summary>
        public void ResetVerificationState() {
            UpdateVerificationState(ShopCredentials.VerificationState.Unverified);
        }

        private void UpdateVerificationState(ShopCredentials.VerificationState newState) {
            if (OnCredentialsStateShouldChange != null) OnCredentialsStateShouldChange(newState);
        }

        private ShopifyClient Client() {
            return new ShopifyClient(new UnityEditorLoader(Credentials.Domain, Credentials.AccessToken));
        }

        private void OnVerificationRequestComplete(QueryRoot result, ShopifyError errors) {
            RequestInProgress = false;

            if (errors != null) {
                UpdateVerificationState(ShopCredentials.VerificationState.Invalid);
            } else {
                UpdateVerificationState(ShopCredentials.VerificationState.Verified);
            }
        }
    }
}
