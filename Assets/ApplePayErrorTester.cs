namespace Shopify.Tests {
    using System; 
    using System.Collections.Generic;
    using UnityEngine; 
    using Shopify.Unity.MiniJSON;

    #if UNITY_IOS
    using Shopify.Unity.SDK.iOS;
    #endif

    public partial class ApplePayErrorTester : MonoBehaviour {}

    #if UNITY_IOS
    public partial class ApplePayErrorTester : MonoBehaviour {

        private const string FIELD_KEY = "Field";
        private const string DESCRIPTION_KEY = "Description";

        private struct ErrorMeta {
            public string Field;
            public string Description;

            public ErrorMeta(string jsonString) {
                var dictionary = (Dictionary<string, object>) Json.Deserialize(jsonString);

                if (dictionary.ContainsKey(FIELD_KEY)) {
                    Field = (string) dictionary[FIELD_KEY];
                } else {
                    Field = null;
                }

                Description = (string) dictionary[DESCRIPTION_KEY];
            }
        }

        void CreateApplePayShippingAddressInvalidError(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            var meta = new ErrorMeta(message.Content);

            var field = (ApplePayAddressInvalidError.AddressField) Enum.Parse(typeof(ApplePayAddressInvalidError.AddressField), meta.Field);
            var error = new ApplePayShippingAddressInvalidError(meta.Description, field);

            message.Respond(error.ToJsonString());
        }

        void CreateApplePayBillingAddressInvalidError(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            var meta = new ErrorMeta(message.Content);

            var field = (ApplePayAddressInvalidError.AddressField) Enum.Parse(typeof(ApplePayAddressInvalidError.AddressField), meta.Field);
            var error = new ApplePayBillingAddressInvalidError(meta.Description, field);

            message.Respond(error.ToJsonString());
        }

        void CreateApplePayContactInvalidError(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            var meta = new ErrorMeta(message.Content);

            var field = (ApplePayContactInvalidError.ContactField) Enum.Parse(typeof(ApplePayContactInvalidError.ContactField), meta.Field);
            var error = new ApplePayContactInvalidError(meta.Description, field);

            message.Respond(error.ToJsonString());
        }

        void CreateApplePayShippingAddressUnservicableError(string serializedMessage) {
            var message = NativeMessageTester.CreateFromJSON(serializedMessage);
            var meta = new ErrorMeta(message.Content);

            var error = new ApplePayShippingAddressUnservicableError(meta.Description);
            message.Respond(error.ToJsonString());
        }
    }
    #endif
}
