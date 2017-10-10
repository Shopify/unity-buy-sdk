using System;
using System.Collections.Generic;
using Shopify.Examples.Helpers;
using Shopify.Unity;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Shopify.Examples.LineItems {
    public class VariantLineItemQuantityAdjustmentEvent : UnityEvent<ProductVariant, int> {
    }

    public class CartPanelLineItem : MonoBehaviour {

        public VariantLineItemQuantityAdjustmentEvent OnVariantLineItemQuantityAdjustment = new VariantLineItemQuantityAdjustmentEvent();

        private string _imageSrc;
        private ProductVariant _currentVariant;

        public Text ProductTitle;
        public Text VariantTitle;
        public Text Quantity;
        public Text Price;
        public Image ProductImage;
        public Button IncreaseQuantity;
        public Button DecreaseQuantity;
        public Image SeparatorLine;

        private void Start() {
            IncreaseQuantity.onClick.AddListener(() => OnVariantLineItemQuantityAdjustment.Invoke(_currentVariant, 1));
            DecreaseQuantity.onClick.AddListener(() => OnVariantLineItemQuantityAdjustment.Invoke(_currentVariant, -1));

            StartCoroutine(
                ImageHelper.AssignImage(
                    _imageSrc,
                    ProductImage
                )
            );
        }

        public void SetCurrentProduct(Product product, ProductVariant variant, int quantity) {
            gameObject.SetActive(true);

            ProductTitle.text = product.title();
            VariantTitle.gameObject.SetActive(variant.title() != "Default Title");
            VariantTitle.text = variant.title();
            Quantity.text = quantity.ToString();
            Price.text = variant.price().ToString("C");

            try {
                _imageSrc = variant.image().src();
            } catch (NullReferenceException) {
                var images = (List<Shopify.Unity.Image>)product.images();
                _imageSrc = images[0].src();
            }

            _currentVariant = variant;
        }
    }
}