namespace Shopify.UIToolkit.Shops.Generic {
    using UnityEngine;
    using UnityEngine.UI;

    public class CartBadgeView : MonoBehaviour {
        public Text BadgeText;
        public int Count { private set; get; }

        public void SetCount(int count) {
            Count = count;
            BadgeText.text = count.ToString();
        }
    }
}
