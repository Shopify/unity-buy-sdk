namespace Shopify.Unity.SDK {
        using System.Collections.Specialized;
        using System.Collections;
        using System.Text;
        using System;

        /// <summary>
        /// Base class for all GraphQL query generator input objects.
        /// </summary>
        public class InputBase {
            private OrderedDictionary keysValues;

            public InputBase() {
                keysValues = new OrderedDictionary();
            }

            /// <summary>
            /// Returns a string representing a GraphQL input object.
            /// </summary>
            public override string ToString() {
                    StringBuilder stringValue = new StringBuilder();
                    bool isNotFirstKeyValue = false;

                    stringValue.Append("{");

            foreach (DictionaryEntry keyValue in keysValues) {
                if (isNotFirstKeyValue) {
                    stringValue.Append (",");
                }

                stringValue.Append (keyValue.Key);
                stringValue.Append (":");
                stringValue.Append (InputValueToString.Get (keyValue.Value));

                isNotFirstKeyValue = true;
            }

            stringValue.Append("}");

            return stringValue.ToString ();
        }

        protected void Set (string key, object value) {
            if (keysValues.Contains (key)) {
                keysValues[key] = value;
            } else {
                keysValues.Add (key, value);
            }
        }

        protected object Get (string key) {
            return keysValues[key];
        }
    }
    }