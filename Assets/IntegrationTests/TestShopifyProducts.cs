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
                    IntegrationTest.Assert(null == errors, "No graphql errors");
                    IntegrationTest.Assert(null == httpErrors, "No http errors");
                    IntegrationTest.Assert(products.Count == 3, "Loaded products");
                    IntegrationTest.Assert("beans" == products[0].title(), "Title product 0: beans");
                    IntegrationTest.Assert("Orphean Steel Bag" == products[1].title(), "Title product 1: Orphean Steel Bag");

                    IntegrationTest.Pass();
                }
            );
        }
    }   
}
