namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.MiniJSON;
    using Shopify.Unity;

    /// <summary>
    /// Abstracts creating and sending queries and mutations via a BaseLoader.
    /// </summary>
    public class QueryLoader {
        BaseLoader Loader;

        /// <summary>
        /// Accepts the <see ref="BaseLoader">BaseLoader </see>, which will do network communcation to send GraphQL queries.
        /// </summary>
        /// <param name="loader">will perform network communication to send GraphQL queries to a GraphQL endpoint</param>
        public QueryLoader(BaseLoader loader) {
            Loader = loader;
        }

        /// <summary>
        /// Generates GraphQL queries.
        /// </summary>
        /// <param name="buildQuery">Delegate to build query</param>
        /// <param name="callback">callback that receives the query response</param>
        public void Query(QueryRootDelegate buildQuery, QueryResponseHandler callback) {
            QueryRootQuery query = new QueryRootQuery();

            buildQuery(query);

            Query(query, callback);
        }

        /// <summary>
        /// Sends GraphQL queries to the GraphQL endpoint.
        /// </summary>
        /// <param name="query">query to be sent to the GraphQL endpoint</param>
        /// <param name="callback">callback that receives the query response</param>
        public void Query(QueryRootQuery query, QueryResponseHandler callback) {
#if SHOPIFY_VERBOSE_DEBUG
            Console.WriteLine("Sending Query: " + query.ToString() + "\n");
#endif

            Loader.Load(query.ToString(), (string response, string error) => {
#if SHOPIFY_VERBOSE_DEBUG
                Console.WriteLine("Response: " + response + "\n");
                Console.WriteLine("Error: " + error + "\n");
#endif
                if (error != null) {
                    callback(new QueryResponse(error));
                } else {
                    callback(new QueryResponse(GetJSON(response)));
                }
            });
        }

        /// <summary>
        /// Generates GraphQL mutations.
        /// </summary>
        /// <param name="buildQuery">Delegate to build a mutation query</param>
        /// <param name="callback">callback that receives the mutation response</param>
        public void Mutation(MutationDelegate buildQuery, MutationResponseHandler callback) {
            MutationQuery query = new MutationQuery();

            buildQuery(query);

            Mutation(query, callback);
        }

        /// <summary>
        /// Sends GraphQL mutations to a GraphQL endpoint.
        /// </summary>
        /// <param name="query">mutation to be sent to GraphQL endpoint</param>
        /// <param name="callback">callback that receives the mutation response</param>
        public void Mutation(MutationQuery query, MutationResponseHandler callback) {
#if SHOPIFY_VERBOSE_DEBUG
            Console.WriteLine("Sending Mutation: " + query.ToString() + "\n");
#endif

            Loader.Load(query.ToString(), (string response, string error) => {
#if SHOPIFY_VERBOSE_DEBUG
                Console.WriteLine("Response: " + response + "\n");
                Console.WriteLine("Error: " + error + "\n");
#endif

                if (error != null) {
                    callback(new MutationResponse(error));
                } else {
                    callback(new MutationResponse(GetJSON(response)));
                }
            });
        }

        private Dictionary<string, object> GetJSON(string response) {
            Dictionary<string, object> json = (Dictionary<string, object>) Json.Deserialize(response);

            if (json == null) {
                throw new InvalidServerResponseException("Server did not return valid JSON data");
            }

            return json;
        }
    }
}