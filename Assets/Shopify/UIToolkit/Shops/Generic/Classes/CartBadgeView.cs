namespace Shopify.UIToolkit.Shops.Generic {
    using UnityEngine;
    using UnityEngine.UI;

    public class CartBadgeView : MonoBehaviour {
        public Text BadgeText;

        public void SetCount(int count) {
            BadgeText.text = count.ToString();
        }
    }
}
