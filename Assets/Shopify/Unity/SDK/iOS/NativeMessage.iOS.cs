#if UNITY_IOS
namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System;

    public partial class NativeMessage {
        [DllImport("__Internal")]
        private static extern void _RespondToNativeMessage(string identifier, string message);
    }
}
#endif