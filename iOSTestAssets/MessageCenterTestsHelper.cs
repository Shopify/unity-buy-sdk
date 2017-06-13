#if UNITY_IPHONE
namespace Shopify.Unity.SDK {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Runtime.InteropServices;

    public class MessageCenterTestsHelper : NativeMessageResponder {
        void RepeatMessage(string serializedMessage) {
            var message = new NativeMessage(serializedMessage);
            _RespondToNativeMessage(message.Identifier, message.Content);
        }
    }
}
#endif
