namespace Shopify.UIToolkit {
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    using Shopify.Unity.SDK;
    using Shopify.Unity;
    using UnityEngine;

    [AddComponentMenu("Shopify/Shop Controllers/Single Product Shop Controller")]
    public class SingleProductShopController : ShopControllerBase {
        public new ISingleProductShop Shop { 
            get { 
                return base.Shop as ISingleProductShop; 
            }

            set {
                base.Shop = value;
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
                Shop.OnError(error);
                return;
            }

            if (products.Count == 0) {
                Shop.OnError(new ShopifyError(ShopifyError.ErrorType.UserError, "Product not found"));
                return;
            }

            var product = products[0];
            var variants = product.variants().edges().Select((x) => x.node()).ToArray();
            Shop.OnShouldShowProduct(product, variants);
        }
    }
}
