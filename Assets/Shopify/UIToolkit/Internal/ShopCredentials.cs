namespace Shopify.UIToolkit {
    using UnityEngine;

    [System.Serializable]
    public class ShopCredentials {
        public enum VerificationState {
            Unverified,
            Invalid,
            Verified,
        }

        public ShopCredentials() {}

        public ShopCredentials(string domain, string accessToken) {
            _AccessToken = accessToken;
            _Domain = domain;
        }

        public VerificationState State;

        [SerializeField] private string _AccessToken;
        public string AccessToken { get { return _AccessToken; } }

        [SerializeField] private string _Domain;
        public string Domain { get { return _Domain; } }
    }
}
