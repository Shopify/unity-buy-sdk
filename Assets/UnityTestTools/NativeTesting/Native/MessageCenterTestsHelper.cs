namespace Shopify.Tests {
    using UnityEngine;
    using System.Runtime.InteropServices;
    using Shopify.Unity.SDK;

    public class MessageCenterTestsHelper : MonoBehaviour {
        void RepeatMessage(string serializedMessage) {
            var message =  NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(message.Content);
        }

        void GetLastMessage(string serializedMessage) {
            var lastMessage = NativeMessageTester.LastMessage;
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            message.Respond(lastMessage.Content);
        }
    }
}
