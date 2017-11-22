namespace Shopify.UIToolkit {
    using UnityEngine;
    using Shopify.Unity;

    public abstract class ThemeControllerBase : MonoBehaviour {
        /// <summary>
        /// The Shop Domain to connect to, in the form of "myshop.myshopify.com"
        /// </summary>
        public string ShopDomain;

        /// <summary>
        /// The Storefront Access Token used to authenticate with the connected shop
        /// </summary>
        public string AccessToken;

        /// <summary>
        /// The client that the controller is using to make requests against the SDK.
        /// </summary>
        /// <returns></returns>
        public ShopifyClient Client {
            get {
                if (_cachedClient != null &&
                    _cachedClient.AccessToken == AccessToken && 
                    _cachedClient.Domain == ShopDomain) {
                        return _cachedClient;
                }

                _cachedClient = new ShopifyClient(AccessToken, ShopDomain);
                return _cachedClient;
            }
        }
        private ShopifyClient _cachedClient;
    }
}
