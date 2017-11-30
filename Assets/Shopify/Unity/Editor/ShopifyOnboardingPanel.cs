#if UNITY_EDITOR
namespace Shopify.Unity.SDK {
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ShopifyOnboardingPanel : EditorWindow {
        private const string HasSeenOnboardingPanelEditorPrefsKey = "shopify_buy_has_seen_onboarding";

        [InitializeOnLoadMethod]
        public static void OnLoad() {
            if (HasSeenPopup()) return;
            ShowWindow();
        }

        [MenuItem("Shopify/Help")]
        public static void ShowWindow() {
            var window = EditorWindow.GetWindowWithRect<ShopifyOnboardingPanel>(
                new Rect(0, 0, DesiredWindowWidth, DesiredWindowHeight)
            );

            window.ShowPopup();
            EditorPrefs.SetBool(HasSeenOnboardingPanelEditorPrefsKey, true);
        }

        private static bool HasSeenPopup() {
            return EditorPrefs.GetBool(HasSeenOnboardingPanelEditorPrefsKey, false);
        }

        private ShopifyEditorStyleHelper Styles;
        private const float DesiredWindowWidth = 550f;
        private const float DesiredWindowHeight = 745f;
        private const float AccentBarHeight = 5f;

        private void OnEnable() {
            minSize = new Vector2(DesiredWindowWidth, DesiredWindowHeight);
            titleContent = new GUIContent("Shopify SDK for Unity");
            position = new Rect(position.xMin, position.yMin, DesiredWindowWidth, DesiredWindowHeight);
            Styles = new ShopifyEditorStyleHelper();
        }

        private void OnGUI() {
            DrawRect(new Rect(0, 0, position.width, position.height), Styles.BackgroundColor);
            DrawRect(new Rect(0, 0, position.width, 5f), Styles.AccentColor);

            GUILayout.BeginArea(
                new Rect(
                    0,
                    AccentBarHeight,
                    Mathf.Min(position.width, DesiredWindowWidth),
                    position.height - AccentBarHeight
                )
            );

            GUILayout.BeginVertical();

            GUILayout.Space(20f);

            GUILayout.Label(Styles.Illustration, Styles.HeadingStyle, GUILayout.Height(Styles.Illustration.height / 3f));

            GUILayout.Label("Generate additional revenue with an in-game store", Styles.HeadingStyle);

            GUILayout.Label(
                "Start selling merchandise directly inside your game with the Shopify SDK for Unity.",
                Styles.AccentBody
            );

            GUILayout.EndVertical();

            var bottomArea = EditorGUILayout.BeginVertical();

            DrawRect(new Rect(0, bottomArea.yMin, position.width, bottomArea.height), Styles.AltBackgroundColor);
            DrawRect(new Rect(0, bottomArea.yMin, position.width, 1), Styles.SeparatorColor);

            GUILayout.Space(40f);

            GUILayout.Label("Start your 30-day free trial", Styles.SubheadingStyle);

            GUILayout.Label(
                "In order to use this plugin, you need to sign up for a Shopify account. Your Shopify account allows you to build products, manage orders, and accept payments.",
                Styles.BodyStyle
            );

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Start your free trial", Styles.ButtonStyle))
                Application.OpenURL("https://www.shopify.com/unity?ref=" + VersionInformation.PUBLISH_DESTINATION + "-unityeditor");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Label("Need help getting started?", Styles.SubheadingStyle);

            GUILayout.Label(
                "Getting started with the SDK is easy, with setup in less than an hour. Hit the button below to view a step-by-step guide to get up and running.",
                Styles.BodyStyle
            );

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("View the guide", Styles.ButtonStyle))
                Application.OpenURL("https://www.shopify.com/partners/blog/using-shopify-unity-buy-sdk");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(40f);

            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawRect(Rect rect, Color color) {
            var colorTex = new Texture2D(1, 1);
            colorTex.SetPixel(0, 0, color);
            colorTex.Apply();

            var style = new GUIStyle();
            style.normal.background = colorTex;

            GUI.Label(rect, "", style);
        }
    }
}
#endif
