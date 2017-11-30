namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public abstract class AbstractResponse {
        public Dictionary<string, object> DataJSON { get; protected set; }
        protected Dictionary<string, object> Data;

        protected T Get<T>(string field, string alias = null) {
            if (alias != null) {
                ValidationUtils.ValidateAlias(alias);
            }

            string key = alias != null ? String.Format("{0}___{1}", field, alias) : field;

            if (Data.ContainsKey(key)) {
                return (T) Data[key];
            } else {
                if (alias != null) {
                    throw new NoQueryException(String.Format("It looks like you did not query the field `{0}` with alias `{1}`", field, alias));
                } else {
                    throw new NoQueryException(String.Format("It looks like you did not query the field `{0}`", field));
                }
            }
        }
    }
}