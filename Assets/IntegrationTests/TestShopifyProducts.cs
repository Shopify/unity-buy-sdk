namespace Shopify.Unity.Tests
{
    using System;
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;
    using System.Collections;
    using System.Collections.Generic;

    public class TestShopifyProducts : MonoBehaviour {
        [UnityTest]
        public IEnumerator Load3Products() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");
            StoppableWaitForTime waiter = Utils.GetWaitQuery ();

            ShopifyBuy.Client().products(
                first: 3,
                callback: (products, error, after) => {
                    waiter.Stop();

                    Assert.IsNull(error, "No errors");
                    Assert.AreEqual(3, products.Count, "Loaded 3 products");
                    Assert.AreEqual("Snare Boot", products[0].title(), "Title product 0: Snare Boot");
                    Assert.AreEqual("Neptune Boot", products[1].title(), "Title product 1: Neptune Boot");

                    List<string> aliases = Utils.GetImageAliases();
                    
                    foreach(string imageAlias in aliases) {
                        Assert.IsNotNull(products[0].images(imageAlias), string.Format("images alias {0} was queried", imageAlias));
                    }

                    List<ProductVariant> variants = (List<ProductVariant>) products[0].variants();
                    
                    foreach(string imageAlias in aliases) {
                        // this will throw an exception if not queried
                        variants[0].image(imageAlias);
                    }

                    // this will throw an exception if not queried
                    variants[0].image();

                    Assert.IsNotNull(products[0].images(), "images with no alias queried");
                }
            );

            yield return waiter;

            Assert.IsTrue(waiter.IsStopped, Utils.MaxQueryMessage);
        }

        [UnityTest]
        public IEnumerator LoadProductsById() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");
            StoppableWaitForTime waiter = Utils.GetWaitQuery ();

            ShopifyBuy.Client().products(
                (products, error) => {
                    waiter.Stop();

                    Assert.IsNull(error, "No errors");
                    Assert.AreEqual(2, products.Count, "Loaded 2 products");
                    Assert.AreEqual("Arena Zip Boot", products[0].title(), "Title product 0: Snare Boot");
                    Assert.AreEqual("Pin Boot", products[1].title(), "Title product 1: Neptune Boot");

                    List<string> aliases = Utils.GetImageAliases();

                    foreach(string imageAlias in aliases) {
                        Assert.IsNotNull(products[0].images(imageAlias), string.Format("images alias {0} was queried", imageAlias));
                    }

                    List<ProductVariant> variants = (List<ProductVariant>) products[0].variants();

                    foreach(string imageAlias in aliases) {
                        // this will throw an exception if not queried
                        variants[0].image(imageAlias);
                    }

                    // this will throw an exception if not queried
                    variants[0].image();

                    Assert.IsNotNull(products[0].images(), "images with no alias queried");
                },
                "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODE0NzU=", "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODQyOTE="
            );

            yield return waiter;

            Assert.IsTrue(waiter.IsStopped, Utils.MaxQueryMessage);
        }

        [UnityTest]
        public IEnumerator Loads20ProductsInLessThan6Seconds() {
            ShopifyBuy.Init("43b7fef8bd2f27f1d645586b72c9b825", "graphql-many-products.myshopify.com");
            float maxDuration = 6f;
            int maxProducts = 20;
            StoppableWaitForTime waiter = new StoppableWaitForTime (maxDuration);
            ShopifyError errorsFromQueries = null;
            List<Product> AllProducts = new List<Product>();

            Action<string> loadPage = null;

            loadPage = (string after) => {
                ShopifyBuy.Client().products((products, errors, afterProductsLoaded) => {
                    if (products != null) {
                        AllProducts.AddRange(products);
                    }
                    
                    if (errors != null) {
                        errorsFromQueries = errors;
                        waiter.Stop();
                    } else if (afterProductsLoaded == null || AllProducts.Count >= maxProducts) {
                        waiter.Stop();
                    } else {
                        loadPage(afterProductsLoaded);
                    }
                }, after: after);
            };

            loadPage(null);

            yield return waiter;

            Assert.IsTrue(waiter.IsStopped, "Query did not complete in " + maxDuration + " seconds");
            Assert.IsNull(errorsFromQueries);
            Assert.GreaterOrEqual(AllProducts.Count, maxProducts);
        }
    }
}
