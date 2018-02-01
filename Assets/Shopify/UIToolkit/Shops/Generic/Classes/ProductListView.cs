namespace Shopify.UIToolkit.Shops.Generic {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Shopify.Unity;
    using UnityEngine;
    using UnityEngine.UI;

    public class ProductListView : GenericMultiProductShopView {
        public MultiProductListItem ListItemTemplate;

        public ListLoadingView LoadingView;

        public GameObject content;

        private const int potentiallyVisibleElements = 5;
        private float elementWidth;
        private int dataOffset = 0;

        private bool loadingMore;
        private bool firstLoad = true;

        private List<MultiProductListItem> _scrollPoolElements = new List<MultiProductListItem> ();

        public void Awake() {
            RectTransform rtt = (RectTransform)ListItemTemplate.transform;
            elementWidth = rtt.rect.width;

            for (int i = 0; i < potentiallyVisibleElements; i++) {
                var element = Instantiate (ListItemTemplate, content.transform);
                var rect = (RectTransform)element.transform;
                element.gameObject.SetActive(true);
                rect.anchoredPosition = new Vector2 (elementWidth * i, 0);
                _scrollPoolElements.Add (element);
            }
        }

        public void OnProductsLoaded(Product[] products) {
            if (firstLoad) {
                for (int i = 0; i < potentiallyVisibleElements; i++) {
                    var element = _scrollPoolElements[i];
                    var product = products[i];
                    var variants = product.variants().edges().Select((x) => x.node()).ToArray();
                    element.SetProduct(product);
                    element.OnClick.AddListener(() => {
                        Shop.ViewProductDetails(product, variants);
                    });
                }

                firstLoad = false;
                LoadingView.gameObject.SetActive(false);
            }

            loadingMore = false;
        }

        public void OnScroll() {
            var rectTransform = (RectTransform)content.transform;
            if ((elementWidth * (dataOffset + 1)) + rectTransform.anchoredPosition.x <= 0.001) {
                if (potentiallyVisibleElements + dataOffset >= Shop.ProductCache.Count - 1) {
                    if (!loadingMore && !Shop.ProductCache.Complete) {
                        Shop.LoadMoreProducts();
                        loadingMore = true;
                    }
                    return;
                } else {
                    dataOffset += 1;
                    SwapElements(0, _scrollPoolElements.Count - 1, potentiallyVisibleElements + dataOffset);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + elementWidth, rectTransform.sizeDelta.y);
                }
            }

            if ((rectTransform.anchoredPosition.x / elementWidth) + dataOffset >= 0.001) {
                if (dataOffset <= 0) {
                    return;
                } else {
                    dataOffset -= 1;
                    SwapElements(_scrollPoolElements.Count - 1, 0, dataOffset);
                    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - elementWidth, rectTransform.sizeDelta.y);
                }
            }
        }

        private void SwapElements(int fromIndex, int toIndex, int cacheOffset) {
            var element = _scrollPoolElements [fromIndex];
            _scrollPoolElements.Remove(element);
            _scrollPoolElements.Insert(toIndex, element);

            var rect = (RectTransform)element.transform;
            rect.anchoredPosition = new Vector3 ((toIndex + dataOffset) * elementWidth, 0, 0);

            var product = Shop.ProductCache.Get(cacheOffset) ;
            var variants = product.variants().edges().Select((x) => x.node()).ToArray();
            element.SetProduct (Shop.ProductCache.Get(cacheOffset));
            element.OnClick.AddListener(() => {
                Shop.ViewProductDetails(product, variants);
            });
        }
    }
}
