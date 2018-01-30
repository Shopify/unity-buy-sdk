namespace Shopify.UIToolkit.Shops.Generic {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Shopify.Unity;
    using UnityEngine;
    using UnityEngine.UI;

    public class ProductListView : GenericMultiProductShopView {
        public MultiProductListItem ListItemTemplate;
        public ScrollRect ListView;
        public GameObject content;

        private const int potentiallyVisibleElements = 5;
        private float elementWidth;
        private int dataOffset = 0;

        private bool loadingMore;
        private bool firstLoad = true;

        private List<MultiProductListItem> _scrollPoolElements = new List<MultiProductListItem> ();

        public ContentRegion contentRegion;

        public void Awake() {
            contentRegion.OnScroll.AddListener (OnScroll);

            RectTransform rtt = (RectTransform)ListItemTemplate.transform;
            elementWidth = rtt.rect.width;

            for (int i = 0; i < potentiallyVisibleElements; i++) {
                var element = Instantiate (ListItemTemplate, content.transform);
                element.gameObject.SetActive(true);
                element.transform.localPosition = new Vector2 (elementWidth * i, 0);
                _scrollPoolElements.Add (element);
            }
        }

        public void OnProductsLoaded(Product[] products) {
            if (firstLoad) {
                for (int i = 0; i < potentiallyVisibleElements; i++) {
                    var element = _scrollPoolElements[i];
                    var product = products[i];
                    element.SetProduct(product);
                    element.OnClick.AddListener(() => {
                        Shop.ViewProductDetails(product, product.variants().edges().Select((x) => x.node()).ToArray());
                    });
                }

                firstLoad = false;
            }

            contentRegion.endOffset = -elementWidth * (shop.ProductCache.Count - potentiallyVisibleElements);

            loadingMore = false;
        }

        private void OnScroll() {
            if ((elementWidth*(dataOffset+1)) + content.transform.localPosition.x <= 0.001) {
                if (potentiallyVisibleElements + dataOffset >= shop.ProductCache.Count - 1) {
                    LoadMoreProducts();

                    return;
                } else {
                    dataOffset += 1;

                    SwapElements(0, _scrollPoolElements.Count - 1, potentiallyVisibleElements + dataOffset);
                }
            }

            if ((content.transform.localPosition.x / elementWidth) + dataOffset >= 0.001) {
                if (dataOffset <= 0) {
                    return;
                } else {
                    dataOffset -= 1;

                    SwapElements(_scrollPoolElements.Count - 1, 0, dataOffset);
                }
            }
        }

        private void SwapElements(int fromIndex, int toIndex, int cacheOffset) {
            var element = _scrollPoolElements [fromIndex];
            _scrollPoolElements.Remove(element);
            _scrollPoolElements.Insert(toIndex, element);
            element.transform.localPosition = new Vector3 ((toIndex + dataOffset) * elementWidth, 0, 0);

            var product = shop.ProductCache.Get(cacheOffset);
            element.SetProduct (product);
            element.OnClick.AddListener(() => {
                Shop.ViewProductDetails(product, product.variants().edges().Select((x) => x.node()).ToArray());
            });
        }

        private void LoadMoreProducts() {
            if (loadingMore) {
                return;
            }

            shop.LoadMoreProducts();

            loadingMore = true;
        }

        private new GenericMultiProductShop shop {
            get { return Shop as GenericMultiProductShop; }
        }

        private RectTransform _scrollContentRect {
            get { return ListView.content; }
        }

        private RectTransform _viewportRect {
            get { return ListView.viewport; }
        }
    }
}
