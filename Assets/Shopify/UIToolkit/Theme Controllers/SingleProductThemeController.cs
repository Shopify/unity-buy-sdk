namespace Shopify.UIToolkit {
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using UnityEngine;

    [AddComponentMenu("Shopify/Theme Controllers/Single Product Theme Controller")]
    public class SingleProductThemeController : ThemeControllerBase {
        public ISingleProductTheme Theme;
        public string ProductGID;

        public override void OnHide() {
            throw new System.NotImplementedException();
        }

        public override void OnShow() {
            Client.products(OnProductsLoaded, ProductGID);
        }

        private void OnProductsLoaded(List<Product> products, ShopifyError error) {
            if (error != null) {
                Theme.OnError(error);
                return;
            }

            var product = products[0];
            var variants = product.variants().edges().Select((x) => x.node()).ToArray();
            Theme.OnShouldShowProduct(product, variants);
        }
    }
}