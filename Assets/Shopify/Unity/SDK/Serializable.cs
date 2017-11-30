namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using Shopify.Unity.MiniJSON;

    public abstract class Serializable {
        public string ToJsonString() {
            return Json.Serialize(ToJson());
        }

        public abstract object ToJson();

        public override string ToString() {
            return ToJsonString();
        }
    }
}