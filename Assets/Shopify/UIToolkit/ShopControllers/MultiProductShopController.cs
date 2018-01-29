namespace Shopify.UIToolkit {
    using System.Collections;
    using System.Collections.Generic;
    using Shopify.Unity;
    using UnityEngine;

    public class MultiProductShopController : ShopControllerBase {
        public new IMultiProductShop Shop {
            get {
                return base.Shop as IMultiProductShop;
            }

            set {
                base.Shop = value;
            }
        }

        public override void Load() {
            LoadMoreProducts();
        }

        public override void Unload() {
            Client.products(OnProductsLoaded);
        }

        public void LoadMoreProducts(string cursor = null) {
            Client.products(OnProductsLoaded, after: cursor);
        }

        private void OnProductsLoaded(List<Product> products, ShopifyError error, string after) {
            if (error != null) {
                Shop.OnError(error);
                return;
            }

            if (products.Count == 0) {
                Shop.OnError(new ShopifyError(ShopifyError.ErrorType.UserError, "Product not found"));
                return;
            }

            Shop.OnProductsLoaded(products.ToArray(), after);
        }
    }
}
