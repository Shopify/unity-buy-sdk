namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    [Serializable]
    public class SummaryItem : Serializable {
        protected const string LABEL = "Label";
        protected const string AMOUNT = "Amount";

#pragma warning disable 0414
        public string Label;
        public string Amount;
#pragma warning restore 0414

        public SummaryItem(string label, string amount) {
            Label = label;
            Amount = amount;
        }

        public override object ToJson() {
            var dict = new Dictionary<string, object>();
            dict[LABEL] = Label;
            dict[AMOUNT] = Amount;
            return dict;
        }
    }
}