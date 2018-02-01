namespace Shopify.UIToolkit {
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Linq;

    public abstract class ShopControllerBase : MonoBehaviour {
        [SerializeField] private ShopCredentials _Credentials = new ShopCredentials();
        public ShopCredentials Credentials {
            get {
                return _Credentials;
            }

            set {
                _Credentials = value;
                InvalidateClient();
            }
        }

        public IShop Shop {
            get {
                _cachedShop = _cachedShop ?? GetComponent<IShop>();
                return _cachedShop;
            }

            set {
                _cachedShop = value;
            }
        }

        private IShop _cachedShop;

        [SerializeField] private string _appleMerchantID;

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
                    SetupClientAndCart();
                }
                return _cachedClient;
            }
        }
        private ShopifyClient _cachedClient;

        public CartController Cart {
            get {
                if (_cachedClient == null) {
                    SetupClientAndCart();
                }

                return _cachedCart;
            }
        }

        private void SetupClientAndCart() {
            _cachedClient = new ShopifyClient(LoaderProvider.GetLoader(Credentials.AccessToken, Credentials.Domain));
            _cachedCart = new CartController(_cachedClient.Cart(), _appleMerchantID);
            
            _cachedCart.OnPurchaseStarted.AddListener(Shop.OnPurchaseStarted);
            _cachedCart.OnPurchaseCancelled.AddListener(Shop.OnPurchaseCancelled);
            _cachedCart.OnPurchaseComplete.AddListener(Shop.OnPurchaseCompleted);
            _cachedCart.OnPurchaseFailed.AddListener(Shop.OnPurchaseFailed);
            _cachedCart.OnCartItemsChange.AddListener(Shop.OnCartItemsChanged);
            _cachedCart.OnPurchaseFailed.AddListener(Shop.OnPurchaseFailed);
            _cachedCart.OnQuantityChange.AddListener(Shop.OnCartQuantityChanged);
        }


        private CartController _cachedCart;

        /// <summary>
        /// Loads the shop
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Unloads the shop
        /// </summary>
        public abstract void Unload();

        public void InvalidateClient() {
            _cachedClient = null;
        }

    }
}
