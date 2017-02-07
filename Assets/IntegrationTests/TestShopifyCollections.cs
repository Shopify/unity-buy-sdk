namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestLoad3Collections : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            ShopifyBuy.Client().collections(
                first: 3,
                callback: (collections, errors, httpErrors) => {
                    IntegrationTest.Assert(null == errors, "No graphql errors");
                    IntegrationTest.Assert(null == httpErrors, "No http errors");
                    IntegrationTest.Assert(collections.Count > 0, "Loaded collections");
                    IntegrationTest.Assert("Home page" == collections[0].title(), "First collection is: Home page");
                    
                    IntegrationTest.Pass();
                }
            );
        }
    }   
}
