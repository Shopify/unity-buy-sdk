#if UNITY_IPHONE
namespace Shopify.iOS.Tests {
    using UnityEngine;
    using System.Runtime.InteropServices;
    using Shopify.Unity.SDK;

    public class MessageCenterTestsHelper : MonoBehaviour {
        void RepeatMessage(string serializedMessage) {
            var message = NativeMessage.CreateFromJSON(serializedMessage);
            message.Respond(message.Content);
        }
    }
}
#endif
