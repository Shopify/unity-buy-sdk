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
        public Text VariantPickerTitle;

        public UnityEngine.UI.Image ActiveImage;

        public GameObject VariantDropdownMenu;
        public GameObject VariantPicker;
        public GameObject ImageContainer;
        public GameObject ImageHolderTemplate;

        private SelectionPicker _selectionPicker;
        private SingleProductThemeController _controller;

        private const string SingleVariantTitle = "Default Title";

        void Awake() {
            _selectionPicker = VariantDropdownMenu.GetComponent<SelectionPicker>();
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
            Title.text = product.title();
            Description.text = product.description();

            SetupVariantOptions(variants);
            UpdateDetailsUsingVariant(variants[0]);
            SetupPreviewImages(product);
        }

        public void ShowProductVariantSelection() {
            VariantDropdownMenu.SetActive(true);
        }

        public void HideProductVariantSelection() {
            VariantDropdownMenu.SetActive(false);
            var wrappedVariant = (ProductVariantOption)_selectionPicker.SelectedItem;
            UpdateDetailsUsingVariant(wrappedVariant.ProductVariant);
        }

        public void SelectImage(Button button) {
            ActiveImage.sprite = button.image.sprite;
        }

        private void SetupPreviewImages(Product product) {
            var images = (List<Shopify.Unity.Image>)product.images();
            if (images.Count == 0) {
                return;
            }

            ActiveImage.GetComponent<RemoteImageLoader>().LoadImage(images[0].src(), () => {
                ActiveImage.enabled = true;
            }, null);

            foreach (var image in images.Take(3)) {
                var imageHolder = Instantiate(ImageHolderTemplate, ImageContainer.transform.position, Quaternion.identity);
                imageHolder.SetActive(true);
                imageHolder.GetComponent<ProductImageHolder>().LoadImage(image.src()); 
                imageHolder.transform.SetParent(ImageContainer.transform, false);
            }
        }

        /// <summary>
        /// Constructs the selection picker's item list from this product's variants.
        /// </summary>
        /// <param name="variants">Current product's variants.</param>
        private void SetupVariantOptions(ProductVariant[] variants) {
            if (HasNoProductVariants(variants)) {
                VariantPicker.SetActive(false);
            } else {
                var wrappedVariants = variants.Select(variant => new ProductVariantOption(variant));

                _selectionPicker.Title = String.Join(" / ", AvailableOptionNamesFromVariants(variants));
                _selectionPicker.Items = wrappedVariants.ToArray(); 
                _selectionPicker.onSelectOption.AddListener(HideProductVariantSelection);
                _selectionPicker.Build();

                VariantPicker.SetActive(true);
            }
        }

        private void UpdateDetailsUsingVariant(ProductVariant variant) {
            VariantPickerTitle.text = StringFromVariant(variant);

            // TODO: Probably needs to be properly localized using CultureInfo.
            Price.text = "$" + variant.price().ToString();
        }

        /// <summary>
        /// Pulls out only the names of the selectable options from the first variant's SelectableOptions.
        /// </summary>
        /// <param name="variants">ProductVariants</param>
        /// <returns>String array of names of options.</returns>
        private string[] AvailableOptionNamesFromVariants(ProductVariant[] variants) {
            if (variants.Length == 0) {
                return new string[0];
            }

            return variants[0].selectedOptions().Aggregate(new List<string>(), (accum, option) => {
                accum.Add(option.name());
                return accum;
            }).ToArray();
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

    /// <summary>
    /// A wrapper object that contains a ProductVariant that is used for the selection picker.
    /// </summary>
    public class ProductVariantOption : ISelectionPickerItem {
        public readonly ProductVariant ProductVariant;
        public string Title {
            get {
                return ProductVariant.title();
            }
        }

        public ProductVariantOption(ProductVariant variant) {
            ProductVariant = variant;
        }
    }
}
