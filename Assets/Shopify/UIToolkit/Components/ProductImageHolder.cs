namespace Shopify.UIToolkit {
    using UnityEngine;
    using UnityEngine.UI;
    using Image = UnityEngine.UI.Image;

    public class ProductImageHolder : MonoBehaviour {
        public Image BackgroundImage;
        public Button ForegroundButton;
        public Image BrokenImageIcon;

        public void LoadImage(string src) {
            ForegroundButton.gameObject.GetComponent<RemoteImageLoader>().LoadImage(src);
        }
    }
}
