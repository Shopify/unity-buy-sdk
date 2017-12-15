namespace Shopify.UIToolkit.Themes {
    using System.Linq;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(SingleProductThemeController))]
    public class GenericSingleProductTheme : MonoBehaviour, ISingleProductTheme {
        [Header("Dependencies")]
        public SingleProductThemeController Controller;

        [Header("UI Bindings")]
        public Text ProductTitleLabel;
        public Text PriceLabel;
        public Text ProductDescriptionLabel;

        private ProductVariant _selectedVariant;
        private Product _product;

        public void Start() {
            if (isActiveAndEnabled) {
                Controller.Show();
            } else {
                Controller.Hide();
            }
        }

        void IThemeBase.OnError(ShopifyError error) {
        }

        void IThemeBase.OnLoadingFinished() {
        }

        void IThemeBase.OnLoadingStarted() {
        }

        void IThemeBase.OnPurchaseCancelled() {
        }

        void IThemeBase.OnPurchaseCompleted() {
        }

        void IThemeBase.OnPurchaseStarted() {
        }

        void IThemeBase.OnCartQuantityChanged(int newQuantity) {
        }

        void ISingleProductTheme.OnShouldShowProduct(Product product, ProductVariant[] variants) {
            SetProduct(product);
            SetSelectedVariant(variants.First());
        }

        private void SetSelectedVariant(ProductVariant variant) {
            _selectedVariant = variant;
            PriceLabel.text = FormatCurrency(_selectedVariant.price());
        }

        private void SetProduct(Product product) {
            _product = product;
            ProductTitleLabel.text = product.title();
            ProductDescriptionLabel.text = product.description();
        }

        private string FormatCurrency(decimal amount) {
            return string.Format("{0:0.00}$", amount);
        }
    }
}