namespace Shopify.Tests {
    using UnityEngine;
    using Shopify.Unity.SDK;

    public class NativeMessageTester: NativeMessage {

        public static NativeMessage LastMessage;

        new public static NativeMessage CreateFromJSON(string jsonString)
        {
            LastMessage = JsonUtility.FromJson<NativeMessage>(jsonString);
            return LastMessage;
        }
    }
}
