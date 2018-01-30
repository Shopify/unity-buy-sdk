#if !SHOPIFY_MONO_UNIT_TEST
namespace Shopify.Unity.UI {
    using System.Runtime.InteropServices;
    using System.Text;
    using System;
    using UnityEngine.UI;
    using UnityEngine;

    /// <summary>
    /// The Apple Pay payment button style C# enum. See https://developer.apple.com/documentation/passkit/pkpaymentbuttonstyle
    /// for all available options.
    /// </summary>
    public enum ApplePayButtonStyle {
        WHITE,
        WHITE_OUTLINE,
        BLACK
    }

    /// <summary>
    /// The Apple Pay payment button type C# enum. See https://developer.apple.com/documentation/passkit/pkpaymentbuttontype
    /// for all available options.
    /// </summary>
    public enum ApplePayButtonType {
        PLAIN,
        BUY,
        SETUP,
        IN_STORE,
        DONATE
    }

    public class NativePayButtonUI : MonoBehaviour {
        /// <summary>
        /// The Apple Pay payment button style. See https://developer.apple.com/documentation/passkit/pkpaymentbuttonstyle
        /// for all available options.
        /// </summary>
        public ApplePayButtonStyle applePayButtonStyle;

        /// <summary>
        /// The Apple Pay payment button type. See https://developer.apple.com/documentation/passkit/pkpaymentbuttontype
        /// for all available options.
        /// </summary>
        public ApplePayButtonType applePayButtonType;

        private RectTransform rectTransform;
        private Image buttonImage;
        private Texture2D imageTexture;

        public void Start() {
            buttonImage = this.gameObject.GetComponent<Image>();
            rectTransform = this.gameObject.GetComponent<RectTransform>();
            GenerateNativePayImage();
        }

        public void OnRectTransformDimensionsChange() {
            GenerateNativePayImage();
        }

        private void GenerateNativePayImage() {
        #if UNITY_IOS
            GenerateApplePayImage();
        #endif
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern IntPtr _GenerateApplePayButtonImage(string type, string style,
            float width, float height);

        private void GenerateApplePayImage() {
            // Ask iOS to generate us an image of the native Apple Pay button.
            var imageStringPtr = _GenerateApplePayButtonImage(
                applePayButtonType.ToString(),
                applePayButtonStyle.ToString(),
                rectTransform.rect.width,
                rectTransform.rect.height
            );

            string managedBase64ImageString = Marshal.PtrToStringAnsi(imageStringPtr);
            if (managedBase64ImageString == null) {
                return;
            }

            byte[] imageBytes = System.Convert.FromBase64String(managedBase64ImageString);

            // Create a Texture2D object from the raw byte data from iOS
            imageTexture = new Texture2D(1, 1);
            imageTexture.LoadImage(imageBytes);
            imageTexture.Apply();

            // Free malloc'ed base64 string from iOS
            Marshal.FreeHGlobal(imageStringPtr);

            // Create Sprite and assign to the image component.
            buttonImage.sprite = Sprite.Create(
                imageTexture,
                new Rect(0, 0, imageTexture.width, imageTexture.height),
                new Vector2(0.5f, 0.5f),
                100.0f
            );
        }
#endif
    }
}
#endif
