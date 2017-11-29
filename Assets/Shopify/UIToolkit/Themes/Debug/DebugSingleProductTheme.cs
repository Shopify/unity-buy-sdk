namespace Shopify.UIToolkit.Themes {
    using System.Linq;
    using System.Text;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using UnityEngine;

    [AddComponentMenu("Shopify/Debug/Debug Theme Controller")]
    public class DebugSingleProductTheme : MonoBehaviour, ISingleProductTheme {

        public void OnError(ShopifyError error) {
            LogEvent("OnError", error);
        }

        public void OnLoadingFinished() {
            LogEvent("OnLoadingFinished");
        }

        public void OnLoadingStarted() {
            LogEvent("OnLoadingStarted");
        }

        public void OnPurchaseCancelled() {
            LogEvent("OnPurchaseCancelled");
        }

        public void OnPurchaseCompleted() {
            LogEvent("OnPurchaseCompleted");
        }

        public void OnPurchaseStarted() {
            LogEvent("OnPurchaseStarted");
        }

        public void OnShouldShowProduct(Product product, ProductVariant[] variants) {
            LogEvent("OnShouldShowProduct", product, variants);
        }

        public void OnCartQuantityChanged(int newQuantity) {
            LogEvent("OnCartQuantityChanged", newQuantity);
        }

        private void LogEvent(string eventName, params object[] args) {
            var eventString = new StringBuilder();
            eventString.AppendFormat("[DebugTheme] {0} called", eventName);

            if (args.Length > 0) {
                var stringArgs = args.Aggregate((x, next) => string.Format("{0}, {1}", x.ToString(), next.ToString()));
                eventString.AppendFormat(" with arguments: {0}", stringArgs);
            }

            Debug.Log(eventString.ToString());
        }
    }
}
