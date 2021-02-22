using System.Collections.Generic;
using System.Linq;
using Shopify.Examples.Helpers;
using Shopify.Examples.LineItems;
using Shopify.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shopify.Examples.Panels {
    public class CartQuantityChangedEvent : UnityEvent<int> {
    }

    public class CheckoutFailureEvent : UnityEvent<string> {
    }

    public class CartPanel : MonoBehaviour {

        // Shopify cart object which manages the items which are added for purchase, and handles
        // initiating the checkout process
        private Cart _cart;

        public CartQuantityChangedEvent OnCartQuantityChanged = new CartQuantityChangedEvent();
        public CheckoutFailureEvent OnCheckoutFailure = new CheckoutFailureEvent();
        public UnityEvent OnCheckoutSuccess;
        public UnityEvent OnCheckoutCancelled;
        public UnityEvent OnReturnToProducts;

        public Button BackToProductsButton;
        public Button CheckoutButton;
        public Text CheckoutButtonText;
        public Text SubtotalText;

        public CartPanelLineItem CartPanelLineItemTemplate;

        public ScrollRect ScrollView;
        public RectTransform Content;

        private readonly Dictionary<string, ProductVariant> _idVariantMapping = new Dictionary<string, ProductVariant>();
        private readonly Dictionary<string, Product> _idProductMapping = new Dictionary<string, Product>();
        private readonly Dictionary<string, CartPanelLineItem> _idCartPanelLineItemMapping = new Dictionary<string, CartPanelLineItem>();
        private readonly List<CartPanelLineItem> _lineItems = new List<CartPanelLineItem>();

        private void Start() {
            BackToProductsButton.onClick.AddListener(() => OnReturnToProducts.Invoke());
            CheckoutButton.onClick.AddListener(() => {
				_cart.CheckoutWithWebView(
                    () => {
                        OnCheckoutSuccess.Invoke();
                        EmptyCart();
                    },
                    () => {
                        OnCheckoutCancelled.Invoke();
                    },
                    (checkoutError) => {
                        OnCheckoutFailure.Invoke(checkoutError.Description);
                    }
                );
            });

            gameObject.SetActive(false);
        }

        // When the quantity adjustment buttons are clicked they will trigger this event handler.
        private void HandleVariantLineItemQuantityAdjustment(ProductVariant variant, int quantityAdjustment) {
            // First we find the lineitem associated with the the variant
            var lineItem = _cart.LineItems.Get(variant);
            // We then update the current quantity with the adjustment
            _cart.LineItems.AddOrUpdate(variant, lineItem.Quantity + quantityAdjustment);

            // In addition to updating the cart itself, we also need to update the associated UI elements. We begin
            // this process by retrieving the cart panel line item in question
            var cartPanelLineItem = _idCartPanelLineItemMapping[variant.id()];


            if (lineItem.Quantity < 1) {
                // If the lineitem quantity has reached zero, then we need to remove its visual representation
                // which is accomplished by destroying the game object
                Destroy(cartPanelLineItem.gameObject);
                // We also take care of removing the associated mappings
                _cart.LineItems.Delete(variant);
                _lineItems.Remove(cartPanelLineItem);
                _idCartPanelLineItemMapping.Remove(variant.id());
            } else {
                // In the case where we have a new, non zero quantity, we take care of updating the UI to
                // reflect this changed quantity
                cartPanelLineItem.Quantity.text = lineItem.Quantity.ToString();
            }

            // Finally, we dispatch an event saying we've updated the cart quantity, so any secondary calculations
            // can be handled
            DispatchCartQuantityChanged();
        }

        public void AddToCart(Product product, ProductVariant variant) {
            if (_cart == null) {
                _cart = ShopifyHelper.CreateCart();
            }

            // Handle adding a particular variant to the cart
            // For more information on adding variants to the cart visit
            // https://help.shopify.com/api/sdks/custom-storefront/unity-buy-sdk/getting-started#create-cart-line-items-based-on-selected-options

            var existingLineItem = _cart.LineItems.Get(variant);
            if (existingLineItem == null) {
                _cart.LineItems.AddOrUpdate(variant);

                var instance = Instantiate(CartPanelLineItemTemplate);
                instance.transform.SetParent(Content, false);
                instance.SetCurrentProduct(product, variant, 1);

                instance.OnVariantLineItemQuantityAdjustment.AddListener(HandleVariantLineItemQuantityAdjustment);

                _idCartPanelLineItemMapping.Add(variant.id(), instance);
                _lineItems.Add(instance);
            } else {
                _cart.LineItems.AddOrUpdate(variant, existingLineItem.Quantity + 1);

                var cartPanelLineItem = _idCartPanelLineItemMapping[variant.id()];
                cartPanelLineItem.Quantity.text = existingLineItem.Quantity.ToString();
            }

            if (!_idVariantMapping.ContainsKey(variant.id())) {
                _idVariantMapping.Add(variant.id(), variant);
            }

            if (!_idProductMapping.ContainsKey(variant.id())) {
                _idProductMapping.Add(variant.id(), product);
            }

            DispatchCartQuantityChanged();
            UpdateSeparatorVisibility();
        }

        private void UpdateSeparatorVisibility() {
            foreach (var lineItem in _lineItems) {
                lineItem.SeparatorLine.gameObject.SetActive(true);
            }

            _lineItems.Last().SeparatorLine.gameObject.SetActive(false);
        }

        public void EmptyCart() {
            _cart.Reset();

            foreach (var lineItem in _lineItems) {
                Destroy(lineItem.gameObject);
            }

            _idCartPanelLineItemMapping.Clear();

            DispatchCartQuantityChanged();
        }

        private void DispatchCartQuantityChanged() {
            var totalLineItemQuantity = 0;
            foreach (var lineItem in _cart.LineItems.All()) {
                totalLineItemQuantity += (int)lineItem.Quantity;
            }

            CheckoutButtonText.text = "Checkout " + _cart.Subtotal().ToString("c");

            OnCartQuantityChanged.Invoke(totalLineItemQuantity);
        }
    }
}