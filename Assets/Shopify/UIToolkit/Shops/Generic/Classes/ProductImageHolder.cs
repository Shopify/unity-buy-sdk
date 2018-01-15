namespace Shopify.UIToolkit {
    using UnityEngine;
    using UnityEngine.UI;
    using Image = UnityEngine.UI.Image;

    public class ProductImageHolder : MonoBehaviour {
        public delegate void ProductImageOnSelect(Sprite sprite);

        public Button ProductImageButton;
        public Image BrokenImageIcon;

        public ProductImageOnSelect OnSelectedImage;

        void Start() {
            ProductImageButton.onClick.AddListener(() => {
                OnSelectedImage(ProductImageButton.GetComponent<Image>().sprite);
            });
        }

        public void LoadImage(string src) {
            var remoteImageLoader = ProductImageButton.gameObject.GetComponent<RemoteImageLoader>();
            remoteImageLoader.LoadImage(src, 
                success: () => {
                    ProductImageButton.GetComponent<Image>().enabled = true;
                },
                failure: null
            );
        }
    }
}
