using System.Collections.Generic;
using System.Linq;
using Shopify.Examples.LineItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Shopify.Examples.Helpers;
using Shopify.Unity;

namespace Shopify.Examples.Panels {
    public class ShowProductEvent : UnityEvent<Shopify.Unity.Product> {
    }

    public class ProductsPanel : MonoBehaviour {

        public ShowProductEvent OnShowProduct = new ShowProductEvent();
        public UnityEvent OnViewCart;
        public UnityEvent OnClosePanel;
        public UnityEvent OnNetworkError;

        public Button ViewCartButton;
        public Button ClosePanelButton;

        public ProductsPanelCell ProductsPanelCellTemplate;

        private readonly List<ProductsPanelCell> _lineItems = new List<ProductsPanelCell>();
        private List<string> _addedProductIds = new List<string> ();

        public ScrollRect ScrollView;
        public RectTransform Content;

        private RectTransform _rectTransform;
        private bool _hitEndCursor;
        private string _after;
        private bool _wasScrolledToBottom;
        private bool _fetchEnabled = true;

        private void Start() {
            _rectTransform = GetComponent<RectTransform>();
            ViewCartButton.onClick.AddListener(() => OnViewCart.Invoke());
            ClosePanelButton.onClick.AddListener(() => OnClosePanel.Invoke());
            ScrollView.onValueChanged.AddListener(OnScrollRectPositionChanged);
            gameObject.SetActive(false);
        }

        public void Init() {
            FetchProducts ();
        }

        public void PauseFetching() {
            _fetchEnabled = false;
        }

        public void ResumeFetching() {
            _fetchEnabled = true;
        }

        private void FetchProducts() {
            if (!_fetchEnabled) {
                return;
            }

            ShopifyHelper.FetchProducts(
                delegate (List<Product> products, string cursor) {
                    foreach (var product in products) {
                        // For each of the products received, add them to the products panel
                        AddProduct(product);
                    }
                    _after = cursor;
                    _hitEndCursor = _after == null;
                },
                delegate {
                    OnNetworkError.Invoke();
                },
                _after
            );
        }

        private void OnScrollRectPositionChanged(Vector2 scrollOffset) {
            var visibleProductViews = _lineItems.Where((x) => {
                if (x.IsLoaded) return false;

                var productViewLocalPosition = _rectTransform.InverseTransformPoint(x.transform.position);
                return productViewLocalPosition.y > _rectTransform.rect.yMin
                    && productViewLocalPosition.y < _rectTransform.rect.yMax;
            });

            foreach (var productView in visibleProductViews) {
                productView.Load();
            }
				
            bool isScrolledToBottom = scrollOffset.y < 0;

            if (!_wasScrolledToBottom && isScrolledToBottom) {
                if (!_hitEndCursor) {
                    FetchProducts ();
                }
            }

            _wasScrolledToBottom = isScrolledToBottom;
        }

        public void AddProduct(Product product) {
            if (_addedProductIds.Contains (product.id ())) {
                return;
            }

            _addedProductIds.Add (product.id ());

            // Create instance of the template
            var instance = Instantiate(ProductsPanelCellTemplate);
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
