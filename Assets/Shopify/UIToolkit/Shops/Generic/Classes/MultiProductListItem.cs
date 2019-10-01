namespace Shopify.UIToolkit.Shops {
    using Shopify.Unity;
    using Shopify.UIToolkit;
    using Shopify.Examples.Helpers;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Events;

    public class MultiProductListItem : MonoBehaviour {
        public RemoteImageLoader ImageLoader;
        public UnityEngine.UI.Image Image;
        public Sprite ErrorImage;
        public Text TitleLabel;
        public Text PriceLabel;
        public Text DescriptionLabel;
        public AspectRatioFitter AspectRatioFitter;
        public UnityAction OnClick;
        private const int MaxDescriptionCharacters = 80;

        public void SetProduct(Product product) {
            Image.CrossFadeAlpha(0f, 0.01f, true);
            TitleLabel.text = product.title();
            DescriptionLabel.text = StringHelper.Ellipsisize(product.description(), MaxDescriptionCharacters);

            var variants = (List<Shopify.Unity.ProductVariant>)product.variants();
            PriceLabel.text = variants.First().priceV2().amount().ToString("C");

            var images = (List<Shopify.Unity.Image>)product.images();
            if (images.Count > 0) {
                if (ImageLoader.LoadingInProgress) {
                    ImageLoader.CancelPreviousLoad();
                }

                ImageLoader.LoadImage(images.First().transformedSrc(), () => {
                    //success
                    AspectRatioFitter.aspectRatio = (float) Image.sprite.texture.width / Image.sprite.texture.height;
                    Image.CrossFadeAlpha(1f, 0.3f, true);
                }, (error) => {
                    //failure
                    Image.sprite = ErrorImage;
                    Image.CrossFadeAlpha(1f, 0.3f, true);
                });
            }
        }

        public void DispatchOnClick() {
            OnClick();
        }
    }
}
