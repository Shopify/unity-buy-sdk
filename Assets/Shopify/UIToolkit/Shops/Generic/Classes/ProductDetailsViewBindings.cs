namespace Shopify.UIToolkit.Shops.Generic {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Shopify.UIToolkit;
    using Shopify.Unity;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// Helper class for binding a product details UI to the data stored within the Product
    /// </summary>
    public class ProductDetailsViewBindings : MonoBehaviour {
        private const string SingleVariantTitle = "Default Title";

        [Header("Labels")]
        public Text Title;
        public Text Price;
        public Text Description;

        [Header("Variant Selection")]
        public Dropdown VariantDropdownMenu;

        [Header("Images")]
        public GameObject ProductImageViewArea;
        public ProductImageHolder ProductImageHolderTemplate;
        public GameObject ProductImageContainer;
        public RemoteImageLoader ActiveImage;

        [Header("Animator")]
        public Animator LoadingOverlayAnimator;


        [HideInInspector]
        public UnityEvent OnAddToCartClicked = new UnityEvent();

        /// <summary>
        /// The currently selected product variant shown by the UI.
        /// </summary>
        public ProductVariant CurrentSelectedVariant {
            get {
                if (_productVariants == null) {
                    return null;
                }

                return _productVariants[VariantDropdownMenu.value];
            }
        }

        public Product Product { private set; get; }

        private ProductVariant[] _productVariants;
        private List<ProductImageHolder> _displayedProductImages = new List<ProductImageHolder>();

        /// <summary>
        /// Fills all of the UI with information from the models that are passed in
        /// </summary>
        /// <param name="product">The Product to fill the UI from</param>
        /// <param name="variants">The ProductVariants of that product</param>
        public void FillWithProductWithVariants(Product product, ProductVariant[] variants) {
            _productVariants = variants;
            Product = product;

            Title.text = product.title();
            Description.text = product.description();
            SetupVariantOptions();

            UpdateDetailsUsingVariant(variants[0]);

            var images = product.images().edges().Select((x) => x.node()).ToArray();
            UpdateProductImages(images);
        }

        public void OnSelectedVariant(ProductVariant variant) {
            UpdateDetailsUsingVariant(variant);
        }

        public void OnSelectProductImage(Sprite sprite) {
            ActiveImage.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
        }

        public void NotifyOnAddToCartClicked() {
            OnAddToCartClicked.Invoke();
        }

        public void SetLoadingOverlayVisible(bool visible) {
            LoadingOverlayAnimator.SetBool("Visible", visible);
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
            Price.text = "$" + variant.priceV2().amount().ToString();
        }

        private void UpdateProductImages(IList<Shopify.Unity.Image> images) {
            foreach(var image in _displayedProductImages) {
                Destroy(image.gameObject);
            }

            _displayedProductImages.Clear();

            if (images.Count == 0) {
                ProductImageViewArea.SetActive(false);
                return;
            } else {
                ProductImageViewArea.SetActive(true);
            }

            foreach (var image in images) {
                var productImage = Instantiate<ProductImageHolder>(ProductImageHolderTemplate);
                productImage.transform.SetParent(ProductImageContainer.transform, false);
                productImage.gameObject.SetActive(true);
                productImage.OnSelectedImage = OnSelectProductImage;
                productImage.LoadImage(image.transformedSrc());
                _displayedProductImages.Add(productImage);
            }

            ActiveImage.LoadImage(
                imageURL: images[0].transformedSrc(),
                success: () => { ActiveImage.GetComponent<UnityEngine.UI.Image>().enabled = true; },
                failure: null
            );
        }

        private bool HasNoProductVariants(ProductVariant[] variants) {
            if (variants.Length == 0) {
                return true;
            }

            // This is what the Storefront API returns back to us when there are no variants...
            if (variants.Length == 1 && variants[0].title() == SingleVariantTitle) {
                return true;
            }

            return false;
        }

        private string StringFromVariant(ProductVariant variant) {
            var strings = variant.selectedOptions().Select(option => {
                return option.name() + ": " + option.value();
            });
            return System.String.Join("  ", strings.ToArray());
        }


        private void SetupVariantOptions() {
            if (HasNoProductVariants(_productVariants)) {
                VariantDropdownMenu.gameObject.SetActive(false);
            } else {
                VariantDropdownMenu.gameObject.SetActive(true);
                VariantDropdownMenu.options.Clear();

                var options = DropdownOptionsFromVariants(_productVariants);
                VariantDropdownMenu.AddOptions(options);
                VariantDropdownMenu.RefreshShownValue();
                VariantDropdownMenu.onValueChanged.AddListener((value) => {
                    OnSelectedVariant(_productVariants[VariantDropdownMenu.value]);
                });
            }
        }
    }
}
