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

        private const int potentiallyVisibleElements = 6;
        private float elementWidth;
        private int dataOffset = 0;

		private bool loadingMore;
		private bool firstLoad = true;

        private List<MultiProductListItem> _scrollPoolElements = new List<MultiProductListItem> ();

        public ContentRegion contentRegion;

		public void Awake() {
			contentRegion.OnScroll.AddListener (ScrollDat);

			RectTransform rtt = (RectTransform)ListItemTemplate.transform;
			elementWidth = rtt.rect.width;

			for (int i = 0; i < potentiallyVisibleElements; i++) {
				var element = Instantiate (ListItemTemplate, content.transform);
				element.gameObject.SetActive(true);
				RectTransform rt = (RectTransform)element.transform;

				element.transform.localPosition = new Vector2 (rt.rect.width * i, 0);
				_scrollPoolElements.Add (element);
			}
		}

        public void OnProductsLoaded(Product[] products) {
			if (firstLoad) {
				for (int i = 0; i < potentiallyVisibleElements; i++) {
					_scrollPoolElements[i].SetProduct (products [i]);
				}

				firstLoad = false;
			}

			contentRegion.endOffset = -elementWidth * (shop.ProductCache.Count - 5);

			loadingMore = false;
        }

		private void ScrollDat(bool clipStart, bool clipEnd) {
			if (clipEnd) {

			}

            if ((elementWidth*(dataOffset+1)) + content.transform.localPosition.x <= 0.001) {
                if (potentiallyVisibleElements + dataOffset >= shop.ProductCache.Count - 1) {
					LoadMoreProducts();

                    return;
                } else {
                    dataOffset += 1;

                    var firstElement = _scrollPoolElements [0];
                    var lastElementX = _scrollPoolElements [_scrollPoolElements.Count - 1].transform.localPosition.x;
                    _scrollPoolElements.Remove (firstElement);
                    _scrollPoolElements.Add (firstElement);
                    firstElement.transform.localPosition = new Vector3 (lastElementX + elementWidth, 0, 0);
                    firstElement.SetProduct (shop.ProductCache.Get(potentiallyVisibleElements + dataOffset));
                }
            }

            if ((content.transform.localPosition.x / elementWidth) + dataOffset >= 0.001) {
                if (dataOffset <= 0) {
                    return;
                } else {
                    dataOffset -= 1;

                    var firstElementX = _scrollPoolElements [0].transform.localPosition.x;
                    var lastElement = _scrollPoolElements [_scrollPoolElements.Count - 1];
                    _scrollPoolElements.Remove (lastElement);
                    _scrollPoolElements.Insert (0, lastElement);
                    lastElement.transform.localPosition = new Vector3 (firstElementX - elementWidth, 0, 0);
                    lastElement.SetProduct (shop.ProductCache.Get(dataOffset));
                }
            }
        }

        private void LoadMoreProducts() {
            if (loadingMore) {
                return;
            }

            shop.LoadMoreProducts();

            loadingMore = true;
        }

        private GenericMultiProductShop shop {
            get { return (GenericMultiProductShop)this.Shop; }
        }

        private RectTransform _scrollContentRect {
            get { return ListView.content; }
        }

        private RectTransform _viewportRect {
            get { return ListView.viewport; }
        }
    }
}
