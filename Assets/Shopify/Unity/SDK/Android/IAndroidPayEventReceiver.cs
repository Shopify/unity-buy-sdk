#if UNITY_ANDROID
namespace Shopify.Unity.SDK.Android {
    using System.Collections.Generic;
    using System.Collections;

    public interface IAndroidPayEventReceiver {
        void OnCanCheckoutWithAndroidPayResult(string serializedMessage);
        void OnUpdateShippingAddress(string serializedMessage);
        void OnUpdateShippingLine(string serializedMessage);
        void OnConfirmCheckout(string serializedMessage);
        void OnError(string serializedMessage);
        void OnCancel(string serializedMessage);
    }
}
#endif