using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Shopify.Examples.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shopify.Examples.Panels {
    public class AddProductToCartEvent : UnityEvent<Shopify.Unity.Product, Shopify.Unity.ProductVariant> {
    }

    public class ProductPanel : MonoBehaviour {

        public AddProductToCartEvent OnAddProductToCart = new AddProductToCartEvent();
        public UnityEvent OnReturnToProducts;
        public UnityEvent OnViewCart;

        public Button BackButton;
        public Text ProductTitle;
        public Image ProductImage;
        public Text ProductTitleDescription;
        public Text ProductTitleDescNoVariant;
        public Text ProductPrice;
        public Button AddToCartButton;

        public Dropdown VariantsDropdown;

        public Shopify.Unity.Product CurrentProduct;
        public Shopify.Unity.ProductVariant CurrentVariant;

        public ImageHolder ImageHolderTemplate;

        private readonly List<GameObject> _imageHolders = new List<GameObject>();

        private void Start() {
            BackButton.onClick.AddListener(() => OnReturnToProducts.Invoke());
            AddToCartButton.onClick.AddListener(() => {
                OnAddProductToCart.Invoke(CurrentProduct, CurrentVariant);
                OnViewCart.Invoke();
            });

            gameObject.SetActive(false);
        }

        public void SetCurrentProduct(Shopify.Unity.Product product) {
            ProductTitle.text = product.title();

            // Reset the variants dropdown
            VariantsDropdown.ClearOptions();

            // Parse variant titles into a list of strings and assign to the dropdown as the new options
            var options = new List<string>();
            var variants = (List<Shopify.Unity.ProductVariant>)product.variants();

            foreach (var variant in variants) {
                options.Add(variant.title());
            }

            VariantsDropdown.AddOptions(options);

            // Only need to show the variants dropdown if there are more than one variant to choose from
            VariantsDropdown.gameObject.SetActive(variants.Count > 1);

            // Show the appropriately positioned description text
            ProductTitleDescription.gameObject.SetActive(variants.Count > 1);
            ProductTitleDescNoVariant.gameObject.SetActive(variants.Count <= 1);

            VariantsDropdown.onValueChanged.AddListener(HandleDropdownChange);

            // Assign the first product image to the main product image display
            var images = (List<Shopify.Unity.Image>)product.images();
            StartCoroutine(ImageHelper.AssignImage(images[0].transformedSrc(), ProductImage));

            RenderVariantImages(images);

            ProductPrice.text = variants[0].price().ToString("C");
            ProductTitleDescription.text = product.description();
            ProductTitleDescNoVariant.text = product.description();

            CurrentProduct = product;
            CurrentVariant = variants[0];
        }

        private void RenderVariantImages(List<Shopify.Unity.Image> images) {
            // Clean up the existing thumbnail images
            foreach (var imageHolder in _imageHolders) {
                Destroy(imageHolder);
            }
            _imageHolders.Clear();

            // We only have space for a fixed number of thumbnails, so we render out the first five
            var offset = 0;
            var maxImages = 5;
            if (images.Count < maxImages) {
                maxImages = images.Count;
            }
            foreach (var image in images.GetRange(0, maxImages)) {
                // Generate instance of thumbail template
                var instance = Instantiate(ImageHolderTemplate);
                instance.gameObject.SetActive(true);
                _imageHolders.Add(instance.gameObject);
                var instanceImage = instance.ForegroundButton.GetComponent<Image>();
                StartCoroutine(
                    ImageHelper.AssignImage(
                        image.transformedSrc(),
                        instanceImage,
                        instance.BrokenImageIcon
                    )
                );
                instance.transform.SetParent(transform, false);
                // Shift the thumbail over to the right
                instance.transform.position += new Vector3(offset, 0, 0);
                var instanceButton = instance.ForegroundButton.GetComponent<Button>();
                instanceButton.onClick.AddListener(() =>
                   StartCoroutine(ImageHelper.AssignImage(image.transformedSrc(), ProductImage))
                );

                offset += 100;
            }
        }

        private void HandleDropdownChange(int option) {
            // Change the current variant to what has been selected
            var variants = (List<Shopify.Unity.ProductVariant>)CurrentProduct.variants();
            var variant = variants[VariantsDropdown.value];
            CurrentVariant = variant;

            string imageSrc;

            // If the variant has a particular image
            try {
                imageSrc = variant.image().transformedSrc();
            } catch (NullReferenceException) {
                var images = (List<Shopify.Unity.Image>)CurrentProduct.images();
                imageSrc = images.First().transformedSrc();
            }

            StartCoroutine(ImageHelper.AssignImage(imageSrc, ProductImage));

            ProductPrice.text = variant.price().ToString("C");
            ProductTitleDescription.text = CurrentProduct.description();
            ProductTitleDescNoVariant.text = CurrentProduct.description();
        }
    }
}
