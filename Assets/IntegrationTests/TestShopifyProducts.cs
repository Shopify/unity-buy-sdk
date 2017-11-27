namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using System.Collections;
    using System.Collections.Generic;

    public class TestShopifyProducts : MonoBehaviour {
        [UnityTest]
        public IEnumerator Load3Products() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");
            var waiter = UnityTestUtils.GetWaitQuery ();

            ShopifyBuy.Client().products(
                first: 3,
                callback: (products, error) => {
                    waiter.Stop();

                    Assert.IsNull(error, "No errors");
                    Assert.AreEqual(3, products.Count, "Loaded 3 products");
                    Assert.AreEqual("Snare Boot", products[0].title(), "Title product 0: Snare Boot");
                    Assert.AreEqual("Neptune Boot", products[1].title(), "Title product 1: Neptune Boot");

                    var aliases = UnityTestUtils.GetImageAliases();
                    
                    foreach(var imageAlias in aliases) {
                        Assert.IsNotNull(products[0].images(imageAlias), string.Format("images alias {0} was queried", imageAlias));
                    }

                    var variants = (List<ProductVariant>) products[0].variants();
                    
                    foreach(var imageAlias in aliases) {
                        // this will throw an exception if not queried
                        variants[0].image(imageAlias);
                    }

                    // this will throw an exception if not queried
                    variants[0].image();

                    Assert.IsNotNull(products[0].images(), "images with no alias queried");
                }
            );

            yield return waiter;

            Assert.IsTrue (waiter.IsStopped, UnityTestUtils.MaxQueryMessage);
        }

        [UnityTest]
        public IEnumerator LoadProductsById() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");
            bool hadResult = false;

            ShopifyBuy.Client().products(
                (products, error) => {
                    hadResult = true;

                    Assert.IsNull(error, "No errors");
                    Assert.AreEqual(2, products.Count, "Loaded 2 products");
                    Assert.AreEqual("Arena Zip Boot", products[0].title(), "Title product 0: Snare Boot");
                    Assert.AreEqual("Pin Boot", products[1].title(), "Title product 1: Neptune Boot");

                    var aliases = UnityTestUtils.GetImageAliases();

                    foreach(var imageAlias in aliases) {
                        Assert.IsNotNull(products[0].images(imageAlias), string.Format("images alias {0} was queried", imageAlias));
                    }

                    var variants = (List<ProductVariant>) products[0].variants();

                    foreach(var imageAlias in aliases) {
                        // this will throw an exception if not queried
                        variants[0].image(imageAlias);
                    }

                    // this will throw an exception if not queried
                    variants[0].image();

                    Assert.IsNotNull(products[0].images(), "images with no alias queried");
                },
                "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODE0NzU=", "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODQyOTE="
            );

            yield return UnityTestUtils.GetWaitQuery ();

            Assert.IsTrue (hadResult, UnityTestUtils.MaxQueryMessage);
        }
    }   
}
