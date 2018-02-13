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
        public class QuantityChangeEvent : UnityEvent<ProductVariant, Product, long> {};
        public QuantityChangeEvent OnQuantityChange = new QuantityChangeEvent();

        [Header("View Properties")]
        public RemoteImageLoader ImageLoader;
        public Text TitleLabel;
        public Text PriceLabel;
        public Text VariantLabel;
        public Text QuantityLabel;
        
        private CartItem _item;
        private ProductVariant _variant {
            get {
                return _item.Variant;
            }
        }

        private Product _product {
            get {
                return _item.Product;
            }
        }

        private string _imageURL;

        public void SetCartItem(CartItem item) {
            _item = item;

            TitleLabel.text = _product.title();
            VariantLabel.gameObject.SetActive(_variant.title() != null);
            VariantLabel.text = _variant.title();

            PriceLabel.text = string.Format("${0}", _variant.price());
            QuantityLabel.text = item.Quantity.ToString();

            try {
                _imageURL = _variant.image().transformedSrc();
            } catch (NullReferenceException) {
                var images = (List<Shopify.Unity.Image>)_product.images();
                _imageURL = images[0].transformedSrc();
            }

            if (_imageURL != null) {
                ImageLoader.gameObject.SetActive(true);
                ImageLoader.LoadImage(_imageURL);
            } else {
                ImageLoader.gameObject.SetActive(false);
            }
        }

        public void DecreaseQuantity() {
            OnQuantityChange.Invoke(_variant, _product, _item.Quantity - 1);
        }

        public void IncreaseQuantity() {
            OnQuantityChange.Invoke(_variant, _product, _item.Quantity + 1);
        }
    }
}
