namespace Shopify.UIToolkit.Shops {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using Shopify.UIToolkit.Shops.Generic;
    using System.Collections;

    [RequireComponent(typeof(SingleProductShopController))]
    public class GenericSingleProductShop : MonoBehaviour, ISingleProductShop {
        public Animator Animator;
        public ProductDetailsViewBindings ViewBindings;

        private SingleProductShopController _controller;

        #region Mono Behaviour
        
        void Awake() {
            _controller = GetComponent<SingleProductShopController>();
            _controller.Shop = this;

            gameObject.SetActive(false);
        }

        #endregion

        #region IShop Interface

        void IShop.OnLoadingStarted() {
            ViewBindings.SetLoadingOverlayVisible(true);
        }

        void IShop.OnLoadingFinished() {
            ViewBindings.SetLoadingOverlayVisible(false);
        }

        void IShop.OnError(ShopifyError error) {
            throw new System.NotImplementedException(); 
        }

        void IShop.OnPurchaseStarted() {}

        void IShop.OnPurchaseCancelled() {}

        void IShop.OnPurchaseCompleted() {}

        void IShop.OnPurchaseFailed(ShopifyError error) {}

        void IShop.OnCartQuantityChanged(int totalNumberOfCartItems) {}

        void IShop.OnCartItemsChanged(List<CartItem> cartItems) {}

        void ISingleProductShop.OnProductLoaded(Product product, ProductVariant[] variants) {
            ViewBindings.FillWithProductWithVariants(product, variants);
            ViewBindings.OnAddToCartClicked.AddListener(() => {
                OnBuyNowClicked(ViewBindings.CurrentSelectedVariant, product);
            });
        }

        #endregion

        #region events

        public void Show() {
            gameObject.SetActive(true);
            Animator.Play("Show", 0);
            _controller.Load();
        }

        public void Hide() {
            Animator.Play("Hide", 0);

            if (_waitForHiddenAndDeactivateRoutine != null) StopCoroutine(_waitForHiddenAndDeactivateRoutine);
            _waitForHiddenAndDeactivateRoutine = StartCoroutine(WaitForHiddenAndDeactivate());
        }

        public void OnBuyNowClicked(ProductVariant variant, Product product) {
            _controller.Cart.AddVariant(variant, product);
            _controller.Cart.StartPurchase(CheckoutMode.Auto);
        }

        #endregion

        private Coroutine _waitForHiddenAndDeactivateRoutine;
        private IEnumerator WaitForHiddenAndDeactivate() {
            yield return new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).IsName("Hidden"));
            gameObject.SetActive(false);
        }
    }
}
