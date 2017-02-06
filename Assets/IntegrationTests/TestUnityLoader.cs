namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity.SDK;

    public class TestQueries {
        public static QueryRootQuery Query = new QueryRootQuery().shop(shop => shop.name());
    }

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestUnityLoader : MonoBehaviour {
        void Start() {
            string domain = "graphql.myshopify.com";
            string authorization = "MzUxYzEyMjAxN2QwZjJhOTU3ZDMyYWU3MjhhZDc0OWM=";

            QueryLoader queryLoader = new QueryLoader(new UnityLoader(domain, authorization));

            queryLoader.Query(TestQueries.Query, (QueryResponse response) => {
                IntegrationTest.Assert(response.HTTPError == null, "http errors were not null: " + response.HTTPError);
                IntegrationTest.Assert(response.errors == null, "GraphQL errors were null");
                IntegrationTest.Assert(response.data != null, "Received a response");
                IntegrationTest.Assert(response.data.shop().name() != null, "graphql");

                IntegrationTest.Pass();
            });
        }
    }   
}
