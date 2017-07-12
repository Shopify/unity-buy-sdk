namespace Shopify.Tests.iOS {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Shopify.Unity.SDK.iOS;

    public class ApplePayEventTester : MonoBehaviour {
        public void SetupApplePayEventTest(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            GetComponent<ApplePayEventReceiverBridge>().Receiver = new ApplePayEventTesterReceiver();
            message.Respond("");
        }
    }
}
