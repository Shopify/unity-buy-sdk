namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;

    /// <summary>
    /// Top level response for all GraphQL queries.
    /// </summary>
    public class QueryResponse : TopLevelResponse {
        public QueryRoot data {
            get {
                return _data;
            }
        }

        private QueryRoot _data;

        public QueryResponse(Dictionary<string, object> dataJSON) : base(dataJSON) {
            if (DataJSON != null) {
                _data = new QueryRoot(this.DataJSON);
            }
        }

        public QueryResponse(string httpError) : base(httpError) { }
    }
}