namespace Shopify.UIToolkit.Themes {
    using System.Linq;
    using System.Text;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using UnityEngine;

    [AddComponentMenu("Shopify/Debug/Debug Theme Controller")]
    public class DebugSingleProductTheme : SingleProductTheme {
        public override void OnError(ShopifyError error) {
            LogEvent("OnError", error);
        }

        public override void OnLoadingFinished() {
            LogEvent("OnLoadingFinished");
        }

        public override void OnLoadingStarted() {
            LogEvent("OnLoadingStarted");
        }

        public override void OnPurchaseCancelled() {
            LogEvent("OnPurchaseCancelled");
        }

        public override void OnPurchaseCompleted() {
            LogEvent("OnPurchaseCompleted");
        }

        public override void OnPurchaseStarted() {
            LogEvent("OnPurchaseStarted");
        }

        public override void OnShouldShowProduct(Product product, ProductVariant[] variants) {
            LogEvent("OnShouldShowProduct", product, variants);
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
