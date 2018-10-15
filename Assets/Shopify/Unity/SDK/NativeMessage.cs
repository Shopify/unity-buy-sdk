namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System;
    using Shopify.Unity.MiniJSON;

    public partial class NativeMessage {
        public string Identifier;
        public string Content;

        public static NativeMessage CreateFromJSON(string jsonString) {
            var dict = (Dictionary<string, object>) Json.Deserialize(jsonString);
            return new NativeMessage((string) dict["Identifier"], (string) dict["Content"]);
        }

        public NativeMessage(string identifier, string content) {
            Identifier = identifier;
            Content = content;
        }

        public void Respond(string message) {
#if (UNITY_IOS || UNITY_ANDROID)
            _RespondToNativeMessage(Identifier, message);
#endif
        }
    }
}
