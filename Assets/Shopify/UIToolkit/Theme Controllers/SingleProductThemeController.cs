namespace Shopify.UIToolkit {
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using Shopify.Unity.SDK;
    using Shopify.Unity;
    using UnityEngine;

    [AddComponentMenu("Shopify/Theme Controllers/Single Product Theme Controller")]
    public class SingleProductThemeController : ThemeControllerBase {
        public new ISingleProductTheme Theme { 
            get { 
                return base.Theme as ISingleProductTheme; 
            }

            set {
                base.Theme = value;
            } 
        }

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

            if (products.Count == 0) {
                Theme.OnError(new ShopifyError(ShopifyError.ErrorType.UserError, "Product not found"));
                return;
            }

            var product = products[0];
            var variants = product.variants().edges().Select((x) => x.node()).ToArray();
            Theme.OnShouldShowProduct(product, variants);
        }
    }
}