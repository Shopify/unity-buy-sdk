namespace Shopify.UIToolkit.Shops.Generic {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;

    public interface IGenericMultiProductShop {

    }

    [RequireComponent(typeof(MultiProductShopController))]
    public class GenericMultiProductShop : MonoBehaviour, IMultiProductShop, IGenericMultiProductShop {
        private MultiProductShopController _controller;

        #region MonoBehaviour

        private void Awake() {
            _controller = GetComponent<MultiProductShopController>();
        }

        private void Start() {
            InitializeViews();
        }

        #endregion

        #region Shop Controller Events

        void IShop.OnCartQuantityChanged(int newQuantity) {
            throw new System.NotImplementedException();
        }

        void IShop.OnError(ShopifyError error) {
            Debug.Log(error.Description);
        }

        void IShop.OnLoadingFinished() {
            throw new System.NotImplementedException();
        }

        void IShop.OnLoadingStarted() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseCancelled() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseCompleted() {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseFailed(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IShop.OnPurchaseStarted() {
            throw new System.NotImplementedException();
        }

        void IMultiProductShop.OnProductsLoaded(Product[] products, string after) {
            _productListView.OnProductsLoaded(products);
        }

        void IMultiProductShop.OnCartItemsChanged(CheckoutLineItem[] lineItems) { }

        #endregion

        [Header("Dependencies")]
        public Animator Animator;

        [Header("Templates")]
        public CartView CartViewPrefab;
        public ProductListView ProductListViewPrefab;
        public ProductDetailsView ProductDetailsViewPrefab;

        [Header("Views")]
        public ViewSwitcher ViewSwitcher;
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
            _waitForHiddenAndDeactivateRoutine = StartCoroutine(WaitForHiddenAndDeactivate());
        }

        private Coroutine _waitForHiddenAndDeactivateRoutine;
        private IEnumerator WaitForHiddenAndDeactivate() {
            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).IsName("Hidden"));
            gameObject.SetActive(false);
        }

        public void ViewProductDetails(Product product) {
            if (_activeView == _productDetailsView) return;
            ViewSwitcher.PushView(_productDetailsView.RectTransform);
            _activeView = _productDetailsView;
            OnViewChanged();
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

        private void OnViewChanged() {
            var canNavigateBack = ViewSwitcher.CanNavigateBack();
            CloseButton.gameObject.SetActive(!canNavigateBack);
            BackButton.gameObject.SetActive(canNavigateBack);
        }
    }
}
