namespace Shopify.UIToolkit {
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Linq;

    public abstract class ThemeControllerBase : MonoBehaviour, IShopCredentials {

        public IThemeBase Theme {
            get {
                _cachedTheme = _cachedTheme ?? GetComponent<IThemeBase>();
                return _cachedTheme;
            }

            set {
                _cachedTheme = value;
            }
        }

        private IThemeBase _cachedTheme;

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

        public string GetShopDomain() {
            return ShopDomain;
        }

        public string GetAccessToken() {
            return AccessToken;
        }

        [HideInInspector]
        public ShopCredentialsVerificationState CredentialsVerificationState { get; set; }

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

        /// <summary>
        /// The active cart that the controller is using.
        /// </summary>
        /// <returns>The Cart</returns>
        public Cart Cart { 
            get { 
                return Client.Cart(); 
            } 
        }

        private int TotalItemsInCart() {
            return Cart.LineItems.All().Sum((x) => (int) x.Quantity);
        }

        /// <summary>
        /// Adds a variant to the cart
        /// </summary>
        /// <param name="variant">The variant to add to the cart</param>
        public void AddVariantToCart(ProductVariant variant) {
            var existingItem = Cart.LineItems.Get(variant);
            var newQuantity = existingItem == null ? 1 : existingItem.Quantity + 1;
            Cart.LineItems.AddOrUpdate(variant, newQuantity);
            Theme.OnCartQuantityChanged(TotalItemsInCart());
        }

        /// <summary>
        /// Removes a variant from the cart.
        /// </summary>
        /// <param name="variant">The variant to remove from the cart</param>
        public void RemoveVariantFromCart(ProductVariant variant) {
            var existingItem = Cart.LineItems.Get(variant);
            if (existingItem == null) return;
            var newQuantity = existingItem.Quantity - 1;

            if (newQuantity == 0) {
                Cart.LineItems.Delete(variant);
            } else {
                Cart.LineItems.AddOrUpdate(variant, newQuantity);
            }
            Theme.OnCartQuantityChanged(TotalItemsInCart());
        }

        /// <summary>
        /// Sets the variant to the specified quantity in the cart.
        /// </summary>
        /// <param name="variant">The variant to modify</param>
        /// <param name="quantity">The desired quantity</param>
        public void UpdateVariantInCart(ProductVariant variant, int quantity) {
            if (quantity <= 0) {
                Cart.LineItems.Delete(variant);
            } else {
                Cart.LineItems.AddOrUpdate(variant, quantity);
            }
            Theme.OnCartQuantityChanged(TotalItemsInCart());
        }

        /// <summary>
        /// Clears all items from the cart
        /// </summary>
        public void ClearCart() {
            Cart.Reset();
            Theme.OnCartQuantityChanged(0);
        }

        /// <summary>
        /// Start a purchase with the current cart.
        /// </summary>
        /// <param name="mode">How do you want to make the purchase? Native, Web or Auto</param>
        /// <param name="nativePayKey">Vendor-specific key to be passed to Native purchase methods</param>
        public void StartPurchase(CheckoutMode mode, string nativePayKey = null) {
            Theme.OnPurchaseStarted();
            switch (mode) {
                case CheckoutMode.Native:
                    Cart.CheckoutWithNativePay(nativePayKey, Theme.OnPurchaseCompleted, Theme.OnPurchaseCancelled, Theme.OnPurchaseFailed);
                    break;

                case CheckoutMode.Web:
                    Cart.CheckoutWithWebView(Theme.OnPurchaseCompleted, Theme.OnPurchaseCancelled, Theme.OnPurchaseFailed);
                    break;

                case CheckoutMode.Auto:
                    Cart.CanCheckoutWithNativePay((canCheckoutWithNativePay) => {
                        if (canCheckoutWithNativePay) {
                            Cart.CheckoutWithNativePay(nativePayKey, Theme.OnPurchaseCompleted, Theme.OnPurchaseCancelled, Theme.OnPurchaseFailed);
                        } else {
                            Cart.CheckoutWithWebView(Theme.OnPurchaseCompleted, Theme.OnPurchaseCancelled, Theme.OnPurchaseFailed);
                        }
                    });
                    break;
            }
        }
    }
}