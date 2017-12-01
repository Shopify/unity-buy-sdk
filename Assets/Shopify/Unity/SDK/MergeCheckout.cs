namespace Shopify.Unity.SDK {
    using Shopify.Unity;

    public class MergeCheckout : ResponseMergeUtil {
        public Checkout Merge(Checkout checkoutA, Checkout checkoutB) {
            return new Checkout(Merge(checkoutA.DataJSON, checkoutB.DataJSON));
        }
    }
}