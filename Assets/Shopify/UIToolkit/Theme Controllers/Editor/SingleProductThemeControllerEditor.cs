namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;

    [CustomEditor(typeof(SingleProductThemeController))]
    public class SingleProductThemeControllerEditor : Editor {
        public SingleProductThemeController Target {
            get {
                return target as SingleProductThemeController;
            }
        }

        public void OnEnable() {
            BindThemeIfPresent();
        }

        public override void OnInspectorGUI() {
            if (Target.Theme == null) {
                ShowThemeHelp();
                return;
            }
        }

        private void ShowThemeHelp() {
            var message = @"
Theme Controllers require a theme to function.

Override SingleProductTheme with your own custom script and add it to this class to continue.
            ";

            EditorGUILayout.HelpBox(message, MessageType.Warning);
        }

        private void BindThemeIfPresent() {
            if (Target.Theme != null) return;
            Target.Theme = Target.GetComponent<ISingleProductTheme>();
        }
    }
}