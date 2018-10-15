#if UNITY_ANDROID
namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.MiniJSON;
    using UnityEngine;

    public partial class NativeMessage {
        private static AndroidJavaObject MessageCenter;

        private static void _RespondToNativeMessage(string identifier, string message) {
            if (MessageCenter == null) {
                MessageCenter = new AndroidJavaObject("com.shopify.buy.UnityMessageCenter");
            }
            MessageCenter.CallStatic("onUnityResponse", identifier, message);
        }
    }
}
#endif
