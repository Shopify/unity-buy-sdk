namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System;
    using Shopify.Unity.MiniJSON;
    [Serializable]
    public class ShippingMethod : SummaryItem {
        private const string IDENTIFIER = "Identifier";
        private const string DETAIL = "Detail";

#pragma warning disable 0414
        public string Identifier;
        public string Detail;
#pragma warning restore 0414

        public ShippingMethod(string label, string amount, string identifier, string detail = null) : base(label, amount) {
            Identifier = identifier;
            Detail = detail;
        }

        public static ShippingMethod CreateFromJson(Dictionary<string, object> dataJSON) {
            string label = null;
            if (dataJSON.ContainsKey(LABEL)) {
                label = dataJSON[LABEL].ToString();
            }
            string amount = null;
            if (dataJSON.ContainsKey(AMOUNT)) {
                amount = dataJSON[AMOUNT].ToString();
            }
            string identifier = null;
            if (dataJSON.ContainsKey(IDENTIFIER)) {
                identifier = dataJSON[IDENTIFIER].ToString();
            }
            string detail = null;
            if (dataJSON.ContainsKey(DETAIL)) {
                detail = dataJSON[DETAIL].ToString();
            }
            return new ShippingMethod(label, amount, identifier, detail);
        }

        public override object ToJson() {
            var dict = (Dictionary<string, object>) base.ToJson();
            dict[IDENTIFIER] = Identifier;
            dict[DETAIL] = Detail;
            return dict;
        }

        public static List<object> ListToJson(List<ShippingMethod> shippingMethods) {
            List<object> list = new List<object>(shippingMethods.Count);
            foreach (ShippingMethod shippingMethod in shippingMethods) {
                list.Add(shippingMethod.ToJson());
            }
            return list;
        }
    }
}