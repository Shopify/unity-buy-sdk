namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestLoadProductsById : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            ShopifyBuy.Client().products(
                (products, errors, httpErrors) => {
                    IntegrationTest.Assert(null == errors, "No graphql errors");
                    IntegrationTest.Assert(null == httpErrors, "No http errors");
                    IntegrationTest.Assert(products.Count == 2, "Loaded products");
                    IntegrationTest.Assert("Cheery Vinyl Phone" == products[0].title(), "Title product 0: Cheery Vinyl Phone");
                    IntegrationTest.Assert("Mozartean Spruce Bag" == products[1].title(), "Title product 1: Mozartean Spruce Bag");

                    IntegrationTest.Pass();
                },
                "gid://shopify/Product/7336571139", "gid://shopify/Product/7336572035"
            );
        }
    }   
}
