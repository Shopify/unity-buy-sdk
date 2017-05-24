namespace Shopify.Tests {
    using System.Collections.Generic;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;

    public class BaseMockLoader : IMockLoader {
        private Dictionary<string, string> Responses;

        public BaseMockLoader() {
            Responses = new Dictionary<string, string>();
        }

        public bool DoesHandleQueryResponse(string query) {
            return Responses.ContainsKey(query);
        }

        public void HandleResponse(string query, LoaderResponseHandler callback) {
            callback(Responses[query], null);
        }

        public List<string> GetQueries() {
            List<string> queries = new List<string>();

            foreach(string query in Responses.Keys) {
                queries.Add(query);
            }

            return queries;
        }

        protected void AddResponse(string query, string response) {
            Responses[query] = response;
        }

        protected void AddResponse(MutationQuery query, string response) {
            AddResponse(query.ToString(), response);
        }

        protected void AddResponse(QueryRootQuery query, string response) {
            AddResponse(query.ToString(), response);
        }
    }
}
