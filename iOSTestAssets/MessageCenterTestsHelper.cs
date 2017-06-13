#if UNITY_IPHONE
namespace Shopify.iOS.Tests {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Runtime.InteropServices;
    using Shopify.Unity.SDK;

    public class MessageCenterTestsHelper : NativeMessageResponder {
        void RepeatMessage(string serializedMessage) {
            var message = new NativeMessage(serializedMessage);
            _RespondToNativeMessage(message.Identifier, message.Content);
        }
    }
}
#endif
