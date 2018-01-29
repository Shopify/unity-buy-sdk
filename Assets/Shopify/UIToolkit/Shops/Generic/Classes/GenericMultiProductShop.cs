namespace Shopify.UIToolkit.Shops.Generic {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;
    using System.Linq;

    public interface IGenericMultiProductShop {
        void AddItemToCart(ProductVariant variant, Product product);
        void ViewProductDetails(Product product, ProductVariant[] variants);
        void UpdateCartQuantityForVariant(ProductVariant variant, Product product, long quantity);
        void PerformNativeCheckout();
        void PerformWebCheckout();
        void CanCheckoutWithNativePay(CanCheckoutWithNativePayCallback callback);
    }

    [RequireComponent(typeof(MultiProductShopController))]
    public class GenericMultiProductShop : MonoBehaviour, IMultiProductShop, IGenericMultiProductShop {
        private MultiProductShopController _controller;
        private ProductCache _productCache;

        public ProductCache ProductCache {
            get {
                return _productCache;
            }
        }

        #region MonoBehaviour

        private void Awake() {
            _controller = GetComponent<MultiProductShopController>();
            _productCache = new ProductCache();
        }

        private void Start() {
            InitializeViews();
        }

        #endregion

        #region Shop Controller Events

        void IShop.OnError(ShopifyError error) {
            Debug.Log(error.Description);
        }

        void IShop.OnLoadingFinished() {
            throw new System.NotImplementedException();
        }

        void IShop.OnLoadingStarted() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseCancelled() {}

        void IShop.OnPurchaseCompleted() {}

        void IShop.OnPurchaseFailed(ShopifyError error) {}

        void IShop.OnPurchaseStarted() {}

        void IShop.OnCartQuantityChanged(int totalNumberOfCartItems) {
            BadgeView.gameObject.SetActive(totalNumberOfCartItems > 0);
            BadgeView.SetCount(totalNumberOfCartItems);
        }

        void IShop.OnCartItemsChanged(List<CartItem> cartItems) {
            _cartView.OnCartItemsChanged(cartItems);
        }

        void IMultiProductShop.OnProductsLoaded(Product[] products, string after) {
            if (_productCache.Complete) {
                return;
            }

            _productCache.Add(products, after);
            _productListView.OnProductsLoaded(products);
        }

        #endregion

        [Header("Dependencies")]
        public Animator Animator;

        [Header("Templates")]
        public CartView CartViewPrefab;
        public ProductListView ProductListViewPrefab;
        public ProductDetailsView ProductDetailsViewPrefab;

        [Header("Views")]
        public ViewSwitcher ViewSwitcher;
        public CartBadgeView BadgeView;
        private CartView _cartView;
        private ProductListView _productListView;
        private ProductDetailsView _productDetailsView;
        private GenericMultiProductShopView _activeView;

        [Header("Buttons")]
        public Button CloseButton;
        public Button BackButton;

        public void InitializeViews() {
            _productListView = Instantiate<ProductListView>(ProductListViewPrefab);
            _productDetailsView = Instantiate<ProductDetailsView>(ProductDetailsViewPrefab);
            _cartView = Instantiate<CartView>(CartViewPrefab);

            _productListView.Shop = this;
            _productDetailsView.Shop = this;
            _cartView.Shop = this;

            ViewSwitcher.RegisterView(_productListView.RectTransform);
            ViewSwitcher.RegisterView(_productDetailsView.RectTransform);
            ViewSwitcher.RegisterView(_cartView.RectTransform);

            _activeView = _productListView;
            OnViewChanged();
        }

        public void Show() {
            if (_waitForHiddenAndDeactivateRoutine != null) {
                StopCoroutine(_waitForHiddenAndDeactivateRoutine);
            }

            gameObject.SetActive(true);
            Animator.Play("Show", 0);

            //Temporary, until we refactor
            _controller.Show();
        }

        public void Hide() {
            Animator.Play("Hide", 0);

            if (_waitForHiddenAndDeactivateRoutine != null) StopCoroutine(_waitForHiddenAndDeactivateRoutine);
            _waitForHiddenAndDeactivateRoutine = StartCoroutine(WaitForHiddenAndDeactivate());
        }

        private Coroutine _waitForHiddenAndDeactivateRoutine;
        private IEnumerator WaitForHiddenAndDeactivate() {
            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).IsName("Hidden"));
            gameObject.SetActive(false);
        }

        public void ViewProductDetails(Product product, ProductVariant[] variants) {
            if (_activeView == _productDetailsView) return;
            ViewSwitcher.PushView(_productDetailsView.RectTransform);
            _activeView = _productDetailsView;
            OnViewChanged();
            _productDetailsView.FillWithProductAndVariants(product, variants);
        }

        public void OpenCart() {
            if (_activeView == _cartView) return;
            ViewSwitcher.PushView(_cartView.RectTransform);
            _activeView = _cartView;
            OnViewChanged();
        }

        public void GoBack() {
            ViewSwitcher.GoBack();
            _activeView = ViewSwitcher.ActiveView().GetComponent<GenericMultiProductShopView>();
            OnViewChanged();
        }

        public void AddItemToCart(ProductVariant variant, Product product) {
            _controller.Cart.AddVariant(variant, product);
        }

        public void UpdateCartQuantityForVariant(ProductVariant variant, Product product, long quantity) {
            _controller.Cart.UpdateVariant(variant, product, quantity);
        }

        public void CanCheckoutWithNativePay(CanCheckoutWithNativePayCallback callback) {
            _controller.Cart.CanCheckoutWithNativePay(callback);
        }

        public void PerformWebCheckout() {
            _controller.Cart.StartPurchase(CheckoutMode.Web);
        }

        public void PerformNativeCheckout() {
            _controller.Cart.StartPurchase(CheckoutMode.Native);
        }

        private void OnViewChanged() {
            var canNavigateBack = ViewSwitcher.CanNavigateBack();
            CloseButton.gameObject.SetActive(!canNavigateBack);
            BackButton.gameObject.SetActive(canNavigateBack);
        }

        public void LoadMoreProducts() {
            if (_productCache.Complete) {
                return;
            }

            _controller.LoadMoreProducts (_productCache.Cursor);
        }
    }
}
