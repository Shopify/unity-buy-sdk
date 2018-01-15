namespace Shopify.UIToolkit.Shops.Generic {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GenericMultiProductShopView : MonoBehaviour {
        private RectTransform _cachedRectTransform;
        public RectTransform RectTransform {
            get {
                _cachedRectTransform = _cachedRectTransform ?? GetComponent<RectTransform>();
                return _cachedRectTransform;
            }
        }

        public IGenericMultiProductShop Shop;
    }
}
