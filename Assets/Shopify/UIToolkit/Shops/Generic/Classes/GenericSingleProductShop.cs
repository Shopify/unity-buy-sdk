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

    [RequireComponent(typeof(SingleProductShopController))]
    public class GenericSingleProductShop : MonoBehaviour, ISingleProductShop {

        [HeaderAttribute("Text Fields")]
        public Text Title;
        public Text Price;
        public Text Description;

        [HeaderAttribute("Variant Selection")]
        public Dropdown VariantDropdownMenu;

        [HeaderAttribute("Images")]
        public GameObject ProductImageViewArea;
        public ProductImageHolder ProductImageHolderTemplate;
        public GameObject ProductImageContainer;
        public RemoteImageLoader ActiveImage;

        public ProductVariant CurrentSelectedVariant {
            get {
                if (_productVariants == null) {
                    return null;
                }

                return _productVariants[VariantDropdownMenu.value];
            }
        }

        private ProductVariant[] _productVariants;
        private SingleProductShopController _controller;

        private const string SingleVariantTitle = "Default Title";

        #region Mono Behaviour
        
        void Awake() {
            _controller = GetComponent<SingleProductShopController>();
            _controller.Shop = this;
            _controller.OnShow();
        }

        #endregion

        #region IShop Interface

        void IShop.OnLoadingStarted() {}

        void IShop.OnLoadingFinished() {}

        void IShop.OnError(ShopifyError error) {}

        void IShop.OnPurchaseStarted() {}

        void IShop.OnPurchaseCancelled() {}

        void IShop.OnPurchaseCompleted() {}

        void IShop.OnPurchaseFailed(ShopifyError error) {}

        void IShop.OnCartQuantityChanged(int newQuantity) {}

        void ISingleProductShop.OnProductLoaded(Product product, ProductVariant[] variants) {
            _productVariants = variants;

            Title.text = product.title();
            Description.text = product.description();
            SetupVariantOptions(variants);

            UpdateDetailsUsingVariant(variants[0]);

            var images = product.images().edges().Select((x) => x.node()).ToArray();
            UpdateProductImages(images);
        }

        #endregion

        #region Events

        public void OnSelectProductImage(Sprite sprite) {
            ActiveImage.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        }

        public void OnSelectedVariant(ProductVariant variant) {
            UpdateDetailsUsingVariant(variant);
        }

        public void OnAddToCart() {
            _controller.Cart.AddVariant(CurrentSelectedVariant);
        }

        #endregion

        #region Helpers

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
                VariantDropdownMenu.onValueChanged.AddListener((value) => { 
                    OnSelectedVariant(variants[VariantDropdownMenu.value]);
                });
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

        private void UpdateProductImages(Shopify.Unity.Image[] images) {
            if (images.Length == 0) {
                ProductImageViewArea.SetActive(false);
                return;
            } else {
                ProductImageViewArea.SetActive(true);
            }

            foreach (var image in images) {
                var productImage = Instantiate(ProductImageHolderTemplate);
                productImage.transform.SetParent(ProductImageContainer.transform, false);
                productImage.gameObject.SetActive(true);
                productImage.OnSelectedImage = OnSelectProductImage;
                productImage.LoadImage(image.src());
            }

            ActiveImage.LoadImage(
                imageURL: images[0].src(),
                success:() => { ActiveImage.GetComponent<UnityEngine.UI.Image>().enabled = true; },
                failure: null
            );
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

        #endregion
    }
}
