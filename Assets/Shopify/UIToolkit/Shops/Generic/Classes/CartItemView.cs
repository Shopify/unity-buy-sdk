namespace Shopify.UIToolkit {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public struct CartItemViewModel {
        public readonly string Title;
        public readonly string VariantDescription;
        public readonly string PriceText;
        public readonly string QuantityText;
        public readonly string ImageURL;

        public CartItemViewModel(CartItem cartItem) {
            var variant = cartItem.Variant;
            var product = variant.product();

            Title = product.title();
            VariantDescription = variant.title();
            PriceText = string.Format("${0}", variant.price());
            QuantityText = cartItem.Quantity.ToString();

            string imageURL;
            try {
                imageURL = variant.image().src();
            } catch (NullReferenceException) {
                var images = (List<Shopify.Unity.Image>)product.images();
                imageURL = images[0].src();
            }

            ImageURL = imageURL;
        }
    }

    public class CartItemView : MonoBehaviour {
        public RemoteImageLoader ImageLoader;
        public Text TitleLabel;
        public Text PriceLabel;
        public Text VariantLabel;
        public Text QuantityLabel;

        public void SetCartLineItemViewModel(CartItemViewModel model) {
            TitleLabel.text = model.Title;
            PriceLabel.text = model.PriceText;

            VariantLabel.gameObject.SetActive(model.VariantDescription == null);
            VariantLabel.text = model.VariantDescription;
            QuantityLabel.text = model.QuantityText;

            if (model.ImageURL != null) {
                ImageLoader.LoadImage(model.ImageURL);
                ImageLoader.gameObject.SetActive(true);
            } else {
                ImageLoader.gameObject.SetActive(false);
            }
        }
    }
}
