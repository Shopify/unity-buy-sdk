using System.Collections.Generic;
using Components;
using Shopify.Examples.Helpers;
using Shopify.Examples.Panels;
using Shopify.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopPopup : MonoBehaviour {
    public ProductsPanel ProductsPanel;
    public ProductPanel ProductPanel;
    public CartPanel CartPanel;
    public EmptyCartPanel EmptyCartPanel;
    public ErrorPanel ErrorPanel;

    public ViewCartButton ViewCartButton;
    public Button EmptyViewCartButton;

    public AnimationCanvas AnimationCanvas;

    public Animator AnimationCanvasAnimator;
    public Animator ErrorPanelAnimator;

    public UnityEvent OnPanelOpen;
    public UnityEvent OnPanelClose;

    // Credentials for access to Shopify store
    // For more information about getting an access token visit
    // https://help.shopify.com/api/storefront-api/getting-started#authentication

    public string AccessToken = "8f40b2ede3e02be97a81ac29cfabc6e0";
    public string ShopDomain = "unity-buy-sdk.myshopify.com";
    public string Locale;

    public void ShowPopup() {
        AnimationCanvasAnimator.SetBool("Visible", true);
        ShowPanel(ProductsPanel.gameObject);
        OnPanelOpen.Invoke();
    }

    public void HidePopup() {
        AnimationCanvasAnimator.SetBool("Visible", false);
        OnPanelClose.Invoke();
    }

    private void Start() {
        // Begin by initializing ShopifyHelper, to make connection to shop
        ShopifyHelper.Init(AccessToken, ShopDomain, Locale);
        // With initialization complete, initial products panel
        ProductsPanel.Init();

        // Setup event listeners for panels

        // Handle transition to specific product
        ProductsPanel.OnShowProduct.AddListener(product => {
            ShowPanel(ProductPanel.gameObject);
            ProductPanel.SetCurrentProduct(product);
        });

        // Handle event from click of the close pop up button
        ProductsPanel.OnClosePanel.AddListener(HidePopup);

        ProductsPanel.OnNetworkError.AddListener(() => RenderError("Could not find products."));

        // Handle transition back to products list from product view
        ProductPanel.OnReturnToProducts.AddListener(() => ShowPanel(ProductsPanel.gameObject));
        // Handle transition to the cart view
        ProductPanel.OnViewCart.AddListener(() => ShowPanel(CartPanel.gameObject));
        // Handles click of the add to cart button, which transitions to the cart view
        ProductPanel.OnAddProductToCart.AddListener(CartPanel.AddToCart);

        // Handle transition back to products list from cart view
        CartPanel.OnReturnToProducts.AddListener(() => ShowPanel(ProductsPanel.gameObject));
        // Handles a change in cart quanity, in order to update the cart quantity indicators
        CartPanel.OnCartQuantityChanged.AddListener(UpdateCartQuantity);
        // Handles a successful checkout, by hiding the shop pop up
        CartPanel.OnCheckoutSuccess.AddListener(HidePopup);
        // Handles a failed checkout by displaying the error
        CartPanel.OnCheckoutFailure.AddListener(RenderError);

        // Handle transition back to products list from empty cart view
        EmptyCartPanel.OnReturnToProducts.AddListener(() => ShowPanel(ProductsPanel.gameObject));

        // Handle transition to cart view, when view cart button is clicked
        ViewCartButton.OnClick.AddListener(() => ShowPanel(CartPanel.gameObject));
        // Handle click on empty view cart button, by transitioning to empty cart view
        EmptyViewCartButton.onClick.AddListener(() => ShowPanel(EmptyCartPanel.gameObject));

        AnimationCanvas.OnAnimationStarted.AddListener (() => ProductsPanel.PauseFetching ());
        AnimationCanvas.OnAnimationStopped.AddListener (() => ProductsPanel.ResumeFetching ());
    }

    private void RenderError(string errorMessage) {
        ErrorPanel.ErrorMessage.text = errorMessage;
        ErrorPanelAnimator.SetBool("Visible", true);
    }

    private void UpdateCartQuantity(int quantity) {
        ViewCartButton.UpdateCartQuantity(quantity);

        EmptyViewCartButton.gameObject.SetActive(quantity < 1);
        ViewCartButton.Quantity.gameObject.SetActive(true);
        ViewCartButton.CartIcon.gameObject.SetActive(true);

        if (quantity < 1) {
            ShowPanel(EmptyCartPanel.gameObject);
        }
    }

    private void ShowPanel(GameObject panelGameObject) {
        ProductPanel.gameObject.SetActive(false);
        ProductsPanel.gameObject.SetActive(false);
        CartPanel.gameObject.SetActive(false);
        EmptyCartPanel.gameObject.SetActive(false);

        panelGameObject.SetActive(true);
    }
}
