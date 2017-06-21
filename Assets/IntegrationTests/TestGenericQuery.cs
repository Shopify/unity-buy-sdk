namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestGenericQuery : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            ShopifyBuy.Client().Query(
                (q) => q.shop(s => s
                    .name()
                ), 
                (data, error) => {
                    IntegrationTest.Assert(null == error, "No errors");
                    IntegrationTest.Assert("graphql" == data.shop().name(), "Shop name was \"graphql\"");

                    IntegrationTest.Pass();
                }
            );
        }
    }   
}
