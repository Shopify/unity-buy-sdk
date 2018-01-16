namespace Shopify.UIToolkit.Shops {
    using System.Linq;
    using System.Collections.Generic;
    using System.Text;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using UnityEngine;

    [AddComponentMenu("Shopify/Debug/Debug Shop Controller")]
    public class DebugSingleProductShop : MonoBehaviour, ISingleProductShop {

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

        public void OnPurchaseFailed(ShopifyError error) {
            LogEvent("OnPurchaseFailed", error);
        }

        public void OnProductLoaded(Product product, ProductVariant[] variants) {
            LogEvent("OnProductLoaded", product, variants);
        }

        public void OnCartQuantityChanged(int newQuantity) {
            LogEvent("OnCartQuantityChanged", newQuantity);
        }

        private void LogEvent(string eventName, params object[] args) {
            var eventString = new StringBuilder();
            eventString.AppendFormat("[DebugShop] {0} called", eventName);

            if (args.Length > 0) {
                var stringArgs = args.Aggregate((x, next) => string.Format("{0}, {1}", x.ToString(), next.ToString()));
                eventString.AppendFormat(" with arguments: {0}", stringArgs);
            }

            Debug.Log(eventString.ToString());
        }
    }
}
