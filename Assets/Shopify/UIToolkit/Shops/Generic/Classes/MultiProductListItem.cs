namespace Shopify.UIToolkit.Shops {
    using Shopify.Unity;
    using Shopify.UIToolkit;
    using Shopify.Examples.Helpers;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;

    public class MultiProductListItem : MonoBehaviour {
        public RemoteImageLoader ImageLoader;
        public Text TitleLabel;
        public Text PriceLabel;
        public Text DescriptionLabel;

        private const int MaxDescriptionCharacters = 80;

        public void SetProduct(Product product) {
            TitleLabel.text = product.title();
            DescriptionLabel.text = StringHelper.Ellipsisize(product.description(), MaxDescriptionCharacters);

            var variants = (List<Shopify.Unity.ProductVariant>)product.variants();
            PriceLabel.text = variants.First().price().ToString("C");

            var images = (List<Shopify.Unity.Image>)product.images();
            if (images.Count > 0) {
                ImageLoader.LoadImage(images.First().src());
            }
        }
    }
}
