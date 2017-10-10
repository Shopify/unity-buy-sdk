using System.Collections.Generic;
using System.Linq;
using Shopify.Examples.Helpers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shopify.Examples.LineItems {
    public class ProductsPanelLineItem : MonoBehaviour {
        public UnityEvent OnClick;

        public int MaxDescriptionCharacters = 150;

        private string _imageSrc;

        public Text TitleText;
        public Text DescriptionText;
        public Image ProductImage;
        public Text PriceText;
        public Button ClickRegionButton;
        public Image SeparatorLine;
        public Image BrokenImageIcon;

        [HideInInspector]
        public RectTransform RectTransform;

        public bool IsLoaded { get; private set; }

        private void Start() {
            RectTransform = GetComponent<RectTransform>();
        }

        public void Load() {
            IsLoaded = true;

            if (!string.IsNullOrEmpty(_imageSrc)) {
                StartCoroutine(
                    ImageHelper.AssignImage(
                        _imageSrc,
                        ProductImage,
                        BrokenImageIcon
                    )
                );
            }
        }

        public void SetCurrentProduct(Shopify.Unity.Product product, int index) {
            gameObject.SetActive(true);

            TitleText.text = product.title();
            DescriptionText.text = StringHelper.Ellipsisize(product.description(), MaxDescriptionCharacters);

            var variants = (List<Shopify.Unity.ProductVariant>)product.variants();
            PriceText.text = variants.First().price().ToString("C");

            var images = (List<Shopify.Unity.Image>)product.images();

            ClickRegionButton.onClick.AddListener(() => OnClick.Invoke());

            if (images.Count > 0) {
                _imageSrc = images.First().src();
            }
        }
    }
}
