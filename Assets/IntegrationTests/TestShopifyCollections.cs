namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity.SDK;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestLoad3Collections : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            ShopifyBuy.Client().collections(
                first: 3,
                callback: (collections, errors, httpErrors) => {
                    IntegrationTest.Equals(null, errors);
                    IntegrationTest.Equals(null, httpErrors);
                    IntegrationTest.Assert(collections.Count > 0, "Loaded collections");

                    IntegrationTest.Pass();
                }
            );
        }
    }   
}
