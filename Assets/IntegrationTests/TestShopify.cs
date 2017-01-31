namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity.SDK;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestLoad3Products : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            ShopifyBuy.Client().products(
                first: 3,
                callback: (products, errors, httpErrors) => {
                    IntegrationTest.Equals(null, errors);
                    IntegrationTest.Equals(null, httpErrors);
                    IntegrationTest.Assert(products.Count == 3, "Loaded products");

                    IntegrationTest.Pass();
                }
            );
        }
    }   
}
