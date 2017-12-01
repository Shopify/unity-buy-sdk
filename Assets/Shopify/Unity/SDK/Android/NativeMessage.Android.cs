#if UNITY_ANDROID
namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.MiniJSON;

#if !SHOPIFY_MONO_UNIT_TEST
    using UnityEngine;
#endif

    public partial class NativeMessage {
#if !SHOPIFY_MONO_UNIT_TEST
        private static AndroidJavaObject MessageCenter;
#endif

        private static void _RespondToNativeMessage(string identifier, string message) {
#if !SHOPIFY_MONO_UNIT_TEST
            if (MessageCenter == null) {
                MessageCenter = new AndroidJavaObject("com.shopify.buy.UnityMessageCenter");
            }
            MessageCenter.CallStatic("onUnityResponse", identifier, message);
#endif
        }
    }
}
#endif