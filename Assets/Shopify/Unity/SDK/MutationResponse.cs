namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;

    /// <summary>
    /// Top-level response for all GraphQL mutation queries.
    /// </summary>
    public class MutationResponse : TopLevelResponse {
        public Mutation data {
            get {
                return _data;
            }
        }

        private Mutation _data;

        public MutationResponse(Dictionary<string, object> dataJSON) : base(dataJSON) {
            if (DataJSON != null) {
                _data = new Mutation(this.DataJSON);
            }
        }

        public MutationResponse(string httpError) : base(httpError) { }
    }
}