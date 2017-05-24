namespace Shopify.Tests {
    using Shopify.Unity.SDK;
    using System.Collections.Generic;

    public class MockLoaderErrors : IMockLoader {
        public bool DoesHandleQueryResponse(string query) {
            return Is404Query(query) || IsGraphQLErroQuery(query);
        }
        public void HandleResponse(string query, LoaderResponseHandler callback) {
            if (Is404Query(query)) {
                callback(null, "404 from mock loader");
            } else if (IsGraphQLErroQuery(query)) {
                callback(@"{""errors"": [{""message"": ""GraphQL error from mock loader""}]}", null);
            }
        }

        public List<string> GetQueries() {
            return new List<string>();
        }

        private bool Is404Query(string query) {
            return query.Contains(@"after:""404""");
        }

        private bool IsGraphQLErroQuery(string query) {
            return query.Contains(@"after:""666""");
        }
    }
}
