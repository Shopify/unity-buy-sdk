namespace Shopify.UIToolkit.Shops.Generic {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Shopify.Unity;
    using Shopify.UIToolkit;
    using UnityEngine;
    using UnityEngine.UI;

    public class ProductDetailsView : GenericMultiProductShopView {
        public ProductDetailsViewBindings ViewBindings;

        public void Start() {
            ViewBindings.OnAddToCartClicked.AddListener(() => {
                Shop.AddItemToCart(ViewBindings.CurrentSelectedVariant, ViewBindings.Product);
            });
        }

        public void FillWithProductAndVariants(Product product, ProductVariant[] variants) {
            ViewBindings.FillWithProductWithVariants(product, variants);
        }
    }
}
