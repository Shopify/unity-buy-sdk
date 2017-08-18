namespace Shopify.Tests {
    using System.Collections.Generic;
    using UnityEngine;
    using Shopify.Unity.SDK;
    using Shopify.Unity.MiniJSON;

    public class NativeMessageTester: NativeMessage {

        public static NativeMessage LastMessage;

        new public static NativeMessage CreateFromJSON(string jsonString)
        {
            var dict = (Dictionary<string, object>) Json.Deserialize(jsonString);
            LastMessage = new NativeMessage((string) dict["Identifier"], (string) dict["Content"]);
            return LastMessage;
        }

        public NativeMessageTester(string identifier, string content) : base(identifier, content) {}
    }
}
