namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using System.Collections.Generic;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestProductsById : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            ShopifyBuy.Client().products(
                (products, error) => {
                    IntegrationTest.Assert(null == error, "No errors");
                    IntegrationTest.Assert(products.Count == 2, "Loaded products");
                    IntegrationTest.Assert("Arena Zip Boot" == products[0].title(), "Title product 0: Arena Zip Boot");
                    IntegrationTest.Assert("Pin Boot" == products[1].title(), "Title product 1: Pin Boot");

                    List<string> aliases = new List<string>() {
                        "pico",
                        "pico",
                        "icon",
                        "thumb",
                        "small",
                        "compact",
                        "medium",
                        "large",
                        "grande",
                        "resolution_1024",
                        "resolution_2048"
                    };
                    
                    foreach(string imageAlias in aliases) {
                        IntegrationTest.Assert(null != products[0].images(imageAlias), string.Format("images alias {0} was queried", imageAlias));
                    }

                    List<ProductVariant> variants = (List<ProductVariant>) products[0].variants();
                    
                    foreach(string imageAlias in aliases) {
                        // this will throw an exception if not queried
                        variants[0].image(imageAlias);
                    }

                    // this will throw an exception if not queried
                    variants[0].image();

                    IntegrationTest.Assert(null != products[0].images(), "images with no alias queried");

                    IntegrationTest.Pass();
                },
                "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODE0NzU=", "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODQyOTE="
            );
        }
    }   
}
