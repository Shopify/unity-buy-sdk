﻿namespace Shopify.UIToolkit.Shops {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;

    [RequireComponent(typeof(SingleProductShopController))]
    public class GenericSingleProductShop : MonoBehaviour, ISingleProductShop {

        public Text Title;
        public Text Price;
        public Text Description;

        public Dropdown VariantDropdownMenu;

        private SingleProductShopController _controller;

        private const string SingleVariantTitle = "Default Title";

        void Awake() {
            _controller = GetComponent<SingleProductShopController>();
            _controller.Shop = this;
            _controller.OnShow();
        }

        void IShop.OnLoadingStarted() {

        }

        void IShop.OnLoadingFinished() {

        }

        void IShop.OnError(ShopifyError error) {

        }

        void IShop.OnPurchaseStarted() {
        }

        void IShop.OnPurchaseCancelled() {

        }

        void IShop.OnPurchaseCompleted() {

        }

        void IShop.OnPurchaseFailed(ShopifyError error) {

        }

        void IShop.OnCartQuantityChanged(int newQuantity) {

        }

        void ISingleProductShop.OnProductLoaded(Product product, ProductVariant[] variants) {
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
                VariantDropdownMenu.RefreshShownValue();
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
