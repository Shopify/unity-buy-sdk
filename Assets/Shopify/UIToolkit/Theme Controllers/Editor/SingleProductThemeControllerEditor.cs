namespace Shopify.UIToolkit.Editor {
    using UnityEngine;
    using UnityEditor;
    using System;

    [CustomEditor(typeof(SingleProductThemeController))]
    public class SingleProductThemeControllerEditor : Editor {
        public ISingleProductThemeControllerEditorView View;
        public SingleProductThemeController Target {
            get {
                return target as SingleProductThemeController;
            }
        }

        public void OnEnable() {
            if (Target == null) return;

            View = new SingleProductThemeControllerEditorView();
            BindThemeIfPresent();
        }

        public override void OnInspectorGUI() {
            if (Target.Theme == null) {
                View.ShowThemeHelp();
                return;
            }
        }

        private void BindThemeIfPresent() {
            if (Target.Theme != null) return;
            Target.Theme = Target.GetComponent<ISingleProductTheme>();
        }
    }
}