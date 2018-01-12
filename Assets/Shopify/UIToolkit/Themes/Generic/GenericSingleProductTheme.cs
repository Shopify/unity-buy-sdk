namespace Shopify.UIToolkit.Themes {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;

    [RequireComponent(typeof(SingleProductThemeController))]
    public class GenericSingleProductTheme : MonoBehaviour, ISingleProductTheme {

        public Text Title;
        public Text Price;
        public Text Description;

        public Dropdown VariantDropdownMenu;

        private SingleProductThemeController _controller;

        private const string SingleVariantTitle = "Default Title";

        void Awake() {
            _controller = GetComponent<SingleProductThemeController>();
            _controller.Theme = this;
            _controller.OnShow();
        }

        void IThemeBase.OnError(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnLoadingFinished() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnLoadingStarted() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseCancelled() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseCompleted() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseStarted() {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnPurchaseFailed(ShopifyError error) {
            throw new System.NotImplementedException();
        }

        void IThemeBase.OnCartQuantityChanged(int newQuantity) {
            throw new System.NotImplementedException();
        }

        void ISingleProductTheme.OnShouldShowProduct(Product product, ProductVariant[] variants) {
            Title.GetComponent<Text>().text = product.title();
            Description.GetComponent<Text>().text = product.description();
            SetupVariantOptions(variants);

            UpdateDetailsUsingVariant(variants[0]);
        }

        /// <summary>
        /// Constructs the selection picker's item list from this product's variants.
        /// </summary>
        /// <param name="variants">Current product's variants.</param>
        private void SetupVariantOptions(ProductVariant[] variants) {
            if (HasNoProductVariants(variants)) {
                VariantDropdownMenu.gameObject.SetActive(false);
            } else {
                VariantDropdownMenu.gameObject.SetActive(true);
                VariantDropdownMenu.options.Clear();

                var options = DropdownOptionsFromVariants(variants);
                VariantDropdownMenu.AddOptions(options);
            }
        }

        private List<UnityEngine.UI.Dropdown.OptionData> DropdownOptionsFromVariants(ProductVariant[] variants) {
            var optionDatas = new List<UnityEngine.UI.Dropdown.OptionData>();
            foreach (var variant in variants) {
                var option = new Dropdown.OptionData();
                option.text = StringFromVariant(variant);
                optionDatas.Add(option);
            }
            return optionDatas;
        }

        private void UpdateDetailsUsingVariant(ProductVariant variant) {
            Price.text = "$" + variant.price().ToString();
        }

        /// <summary>
        /// Checks if variants is empty or contains the default/no-variant option.
        /// </summary>
        /// <param name="variants">ProductVariants</param>
        /// <returns>true if variants is empty or contains the default variant, false otherwise.</returns>
        private bool HasNoProductVariants(ProductVariant[] variants) {
            if (variants.Length == 0)  {
                return true;
            }

            // This is what the Storefront API returns back to us when there are no variants...
            if (variants.Length == 1 && variants[0].title() == SingleVariantTitle) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Formats a string to be used when a variant is selected.
        /// </summary>
        /// <param name="variant">ProductVariant</param>
        /// <returns>String representing the variant.</returns>
        private string StringFromVariant(ProductVariant variant) {
            var strings = variant.selectedOptions().Select(option => {
                return option.name() + ": " + option.value();
            });
            return String.Join("  ", strings.ToArray());
        }
    }
}
