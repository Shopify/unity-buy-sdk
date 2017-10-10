using System.Collections.Generic;
using System.Linq;
using Shopify.Examples.LineItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Shopify.Examples.Panels {
    public class ShowProductEvent : UnityEvent<Shopify.Unity.Product> {
    }

    public class ProductsPanel : MonoBehaviour {

        public ShowProductEvent OnShowProduct = new ShowProductEvent();
        public UnityEvent OnViewCart;
        public UnityEvent OnClosePanel;

        public Button ViewCartButton;
        public Button ClosePanelButton;

        public ProductsPanelLineItem ProductsPanelLineItemTemplate;

        private readonly List<ProductsPanelLineItem> _lineItems = new List<ProductsPanelLineItem>();

        public ScrollRect ScrollView;
        public RectTransform Content;

        private RectTransform RectTransform;

        private void Start() {
            RectTransform = GetComponent<RectTransform>();
            ViewCartButton.onClick.AddListener(() => OnViewCart.Invoke());
            ClosePanelButton.onClick.AddListener(() => OnClosePanel.Invoke());
            ScrollView.onValueChanged.AddListener(OnScrollRectPositionChanged);
            gameObject.SetActive(false);
        }

        private void OnScrollRectPositionChanged(Vector2 scrollOffset) {
            var visibleProductViews = _lineItems.Where((x) => {
                if (x.IsLoaded) return false;

                var productViewLocalPosition = RectTransform.InverseTransformPoint(x.transform.position);
                return productViewLocalPosition.y > RectTransform.rect.yMin
                    && productViewLocalPosition.y < RectTransform.rect.yMax;
            });

            foreach (var productView in visibleProductViews) {
                productView.Load();
            }
        }

        public void AddProduct(Shopify.Unity.Product product) {
            // Create instance of the template
            var instance = Instantiate(ProductsPanelLineItemTemplate);
            // Need to set transform so that scrolling works properly
            instance.transform.SetParent(Content, false);
            // Pass in the product to set the line item attributes
            instance.SetCurrentProduct(product, _lineItems.Count);

            // When the instance is clicked, dispatch up the event to change the view to the product
            instance.OnClick.AddListener(() => OnShowProduct.Invoke(product));

            // Add to our list of line items, as we need to iterate them later to adjust properties 
            // such as the separator element visibility
            _lineItems.Add(instance);

            UpdateSeparatorVisibility();
        }

        private void UpdateSeparatorVisibility() {
            // Reset the separator visibility by showing all the separators
            foreach (var lineItem in _lineItems) {
                lineItem.SeparatorLine.gameObject.SetActive(true);
            }

            // Then hide the separator on the last item, so we don't have a trailing separator
            _lineItems.Last().SeparatorLine.gameObject.SetActive(false);
        }
    }
}