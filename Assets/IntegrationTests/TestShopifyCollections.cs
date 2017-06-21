namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using Shopify.Unity;
    using System.Collections.Generic;

    [IntegrationTest.DynamicTest ("TestScene")]
    [IntegrationTest.SucceedWithAssertions]
    public class TestLoad3Collections : MonoBehaviour {
        void Start() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");

            ShopifyBuy.Client().collections(
                first: 3,
                callback: (collections, error) => {
                    IntegrationTest.Assert(null == error, "No errors");
                    IntegrationTest.Assert(collections.Count > 0, "Loaded collections");
                    IntegrationTest.Assert("Home page" == collections[0].title(), "First collection is: Home page");
                    
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

                    foreach(string alias in aliases) {
                        // this will throw an exception if image was not queried with alias
                        // no collections in our test store have values
                        collections[0].image(alias);
                    }

                    IntegrationTest.Pass();
                }
            );
        }
    }   
}
