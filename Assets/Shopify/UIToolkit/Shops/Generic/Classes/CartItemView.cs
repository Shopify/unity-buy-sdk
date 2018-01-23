namespace Shopify.UIToolkit {
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.UIToolkit.Shops.Generic;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    /// <summary>
    /// Behaviour for linking all the pieces of the cart list item together.
    /// </summary>
    public class CartItemView : MonoBehaviour {
        [HideInInspector]
        public GenericMultiProductShop Shop;

        [Header("View Properties")]
        public RemoteImageLoader ImageLoader;
        public Text TitleLabel;
        public Text PriceLabel;
        public Text VariantLabel;
        public Text QuantityLabel;
        public Button DecreaseQuantityButton;
        public Button IncreaseQuantityButton;

        private CartItem _item;
        private ProductVariant _variant {
            get {
                return _item.Variant;
            }
        }

        public void SetCartItem(CartItem item) {
            _item = item;

            var product = _variant.product();

            TitleLabel.text = product.title();
            VariantLabel.gameObject.SetActive(_variant.title() == null);
            VariantLabel.text = _variant.title();

            PriceLabel.text = string.Format("${0}", _variant.price());
            QuantityLabel.text = item.Quantity.ToString();

            string imageURL;
            try {
                imageURL = _variant.image().transformedSrc();
            } catch (NullReferenceException) {
                var images = (List<Shopify.Unity.Image>)product.images();
                imageURL = images[0].transformedSrc();
            }

            if (imageURL != null) {
                ImageLoader.LoadImage(imageURL);
                ImageLoader.gameObject.SetActive(true);
            } else {
                ImageLoader.gameObject.SetActive(false);
            }
        }

        public void DecreaseQuantity() {
            Shop.UpdateCartQuantityForVariant(_variant, _item.Quantity - 1);
        }

        public void IncreaseQuantity() {
            Shop.UpdateCartQuantityForVariant(_variant, _item.Quantity + 1);
        }
    }
}
