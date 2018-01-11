namespace Shopify.UIToolkit.Editor {
    using UnityEditor;
    using UnityEngine;
    using Shopify.UIToolkit.Shops;

    public interface IMultiProductShopControllerEditorView : IShopControllerBaseEditorView {
    }

    [CustomEditor(typeof(MultiProductShopController))]
    public class MultiProductShopControllerEditor : ShopControllerBaseEditor, IMultiProductShopControllerEditorView {
        public new MultiProductShopController Target {
            get { return target as MultiProductShopController; }
        }

       public override void OnShowConfiguration() {

       }
   }
}