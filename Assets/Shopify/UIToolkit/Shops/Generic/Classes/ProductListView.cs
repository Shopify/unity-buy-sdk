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

        public void OnProductsLoaded(Product[] products) {
            foreach (var product in products) {
                var listItem = Instantiate(ListItemTemplate);
                listItem.gameObject.SetActive(true);
                listItem.transform.SetParent(_scrollContentRect.transform, false);
                listItem.SetProduct(product);
                listItem.OnClick.AddListener(() => {
                    Shop.ViewProductDetails(product, product.variants().edges().Select((x) => x.node()).ToArray());
                });
            }
        }

        private RectTransform _scrollContentRect {
            get { return ListView.content; }
        }

        private RectTransform _viewportRect {
            get { return ListView.viewport; }
        }
    }
}
