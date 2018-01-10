namespace Shopify.UIToolkit.Editor {
    using UnityEditor;
    using UnityEngine;
    using Shopify.UIToolkit.Themes;

    public interface IShopThemeControllerEditorView : IBaseThemeControllerEditorView {
    }

    [CustomEditor(typeof(ShopThemeController))]
    public class ShopThemeControllerEditor : BaseThemeControllerEditor, IBaseThemeControllerEditorView {
        public new ShopThemeController Target {
            get { return target as ShopThemeController; }
        }

       public override void OnShowConfiguration() {

       }
   }
}