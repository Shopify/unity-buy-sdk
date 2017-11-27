namespace Shopify.UIToolkit.Editor {
    using UnityEditor;

    public interface ISingleProductThemeControllerEditorView {
        void ShowThemeHelp();
    }

    public class SingleProductThemeControllerEditorView : ISingleProductThemeControllerEditorView {
        public void ShowThemeHelp() {
            var message = @"
Theme Controllers require a theme to function.
Implement ISingleProductTheme with your own custom script and add it to this gameObject to continue.
            ";

            EditorGUILayout.HelpBox(message, MessageType.Warning);
        }
    }
}