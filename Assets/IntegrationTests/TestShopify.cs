namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity.SDK;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestLoad250Products : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            System.Console.WriteLine("START LOAD");

            ShopifyBuy.Client().products(
                first: 250,
                callback: (products) => {
                    System.Console.WriteLine("---> LOADED: " + products.Count);

                    IntegrationTest.Assert(products.Count > 0, "Loaded products");
                }
            );
        }
    }   
}
