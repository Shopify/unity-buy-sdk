namespace Shopify.UIToolkit {
    using UnityEngine;
    using UnityEngine.UI;
    using Image = UnityEngine.UI.Image;

    public class ProductImageHolder : MonoBehaviour {
        public delegate void ProductImageOnSelect(Sprite sprite);

        public Button ProductImageButton;
        public Image ProductImage;
        public RemoteImageLoader ProductImageLoader;
        public Image BrokenImageIcon;

        public ProductImageOnSelect OnSelectedImage;

        void Start() {
            ProductImageButton.onClick.AddListener(() => {
                OnSelectedImage(ProductImage.sprite);
            });
        }

        public void LoadImage(string src) {
            ProductImageLoader.LoadImage(
                imageURL: src, 
                success: () => {
                    ProductImageButton.GetComponent<Image>().enabled = true;
                },
                failure: null
            );
        }
    }
}
