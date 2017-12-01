#if UNITY_EDITOR
namespace Shopify.Unity.SDK {
    using UnityEngine;

    public class ShopifyEditorStyleHelper {
        public Color BackgroundColor {
            get { return ColorFromHex("#F9FAFBFF"); }
        }

        public Color AccentColor {
            get { return ColorFromHex("#5C6AC4FF"); }
        }

        public Color AltBackgroundColor {
            get { return ColorFromHex("#F4F6F8FF"); }
        }

        public Color SeparatorColor {
            get { return ColorFromHex("#DFE3E8FF"); }
        }

        public Texture LogoImage {
            get { return Resources.Load("shopify_logo_black") as Texture; }
        }

        public Texture Illustration {
            get { return Resources.Load("shopify_onboarding_illustration") as Texture; }
        }

        public Font BoldFont {
            get { return Resources.Load("shopify_onboarding_inter_font_bold") as Font; }
        }

        public Font MediumFont {
            get { return Resources.Load("shopify_onboarding_inter_font_medium") as Font; }
        }

        public Font RegularFont {
            get { return Resources.Load("shopify_onboarding_inter_font_regular") as Font; }
        }

        public GUIStyle HeadingStyle {
            get {
                var style = new GUIStyle();
                style.font = BoldFont;
                style.fontSize = 22;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = ColorFromHex("#212B35FF");
                style.margin = new RectOffset(120, 120, 10, 10);

                style.wordWrap = true;

                return style;
            }
        }

        public GUIStyle SubheadingStyle {
            get {
                var style = new GUIStyle();
                style.font = MediumFont;
                style.fontSize = 18;
                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = ColorFromHex("#212B35FF");
                style.margin = new RectOffset(70, 70, 30, 10);
                style.wordWrap = true;

                return style;
            }
        }

        public GUIStyle ButtonStyle {
            get {
                var style = new GUIStyle();
                style.normal.background = Resources.Load("shopify_indigo_button_normal") as Texture2D;
                style.onNormal.background = Resources.Load("shopify_indigo_button_normal") as Texture2D;
                style.hover.background = Resources.Load("shopify_indigo_button_hover") as Texture2D;
                style.onHover.background = Resources.Load("shopify_indigo_button_hover") as Texture2D;

                style.normal.textColor = ColorFromHex("#5C6AC4FF");
                style.onNormal.textColor = ColorFromHex("#5C6AC4FF");
                style.hover.textColor = ColorFromHex("#5C6AC4FF");
                style.onHover.textColor = ColorFromHex("#5C6AC4FF");

                style.border = new RectOffset(20, 20, 20, 20);
                style.padding = new RectOffset(40, 40, 15, 15);
                style.margin = new RectOffset(0, 0, 20, 20);

                style.font = MediumFont;
                style.fontSize = 14;
                style.stretchWidth = false;
                style.alignment = TextAnchor.MiddleCenter;

                return style;
            }
        }

        public GUIStyle BodyStyle {
            get {
                var style = new GUIStyle();
                style.font = RegularFont;
                style.fontSize = 15;
                style.normal.textColor = ColorFromHex("#454F5BFF");
                style.wordWrap = true;
                style.alignment = TextAnchor.MiddleCenter;
                style.margin = new RectOffset(60, 60, 10, 20);

                return style;
            }
        }

        public GUIStyle AccentBody {
            get {
                var style = new GUIStyle();
                style.font = RegularFont;
                style.fontSize = 16;
                style.normal.textColor = ColorFromHex("#212B35FF");
                style.wordWrap = true;
                style.alignment = TextAnchor.MiddleCenter;
                style.margin = new RectOffset(80, 80, 10, 20);

                return style;
            }
        }

        private Color ColorFromHex(string hex) {
            Color color;
            ColorUtility.TryParseHtmlString(hex, out color);

            return color;
        }
    }
}
#endif
