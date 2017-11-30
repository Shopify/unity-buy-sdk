namespace Shopify.UIToolkit {
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.SDK;

    public abstract class ThemeControllerBase : MonoBehaviour {
        /// <summary>
        /// The Shop Domain to connect to, in the form of "myshop.myshopify.com"
        /// </summary>
        public string ShopDomain {
            get { return _shopDomain; }
            set {
                _shopDomain = value;
                InvalidateClient();
            }
        }
        [SerializeField] private string _shopDomain;

        /// <summary>
        /// The Storefront Access Token used to authenticate with the connected shop
        /// </summary>
        public string AccessToken {
            get { return _accessToken; }
            set {
                _accessToken = value;
                InvalidateClient();
            }
        }
        [SerializeField] private string _accessToken;

        /// <summary>
        /// What loader provider this controller uses to create the loader.
        /// You can change this if you require a special kind of request loader
        /// to be used instead of the default UnityLoader.
        /// </summary>
        public ILoaderProvider LoaderProvider {
            get { return _loaderProvider; }
            set {
                _loaderProvider = value;
                InvalidateClient();
            }
        }
        private ILoaderProvider _loaderProvider = new UnityLoaderProvider();

        /// <summary>
        /// The client that the controller is using to make requests against the SDK.
        /// </summary>
        /// <returns></returns>
        public ShopifyClient Client {
            get {
                if (_cachedClient == null) {
                    _cachedClient = new ShopifyClient(LoaderProvider.GetLoader(AccessToken, ShopDomain));
                }
                return _cachedClient;
            }
        }
        private ShopifyClient _cachedClient;

        private void InvalidateClient() {
            _cachedClient = null;
        }

        /// <summary>
        /// Shows the shop
        /// </summary>
        public void Show() {
            OnShow();
        }

        /// <summary>
        /// Hides the shop
        /// </summary>
        public void Hide() {
            OnHide();
        }

        public abstract void OnShow();
        public abstract void OnHide();
    }
}