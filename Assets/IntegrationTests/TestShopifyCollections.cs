#if UNITY_EDITOR
namespace Shopify.Unity.Tests
{
    using System;
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Collections;
    using System.Collections.Generic;

    public class TestShopifyCollections : MonoBehaviour {
        [UnityTest]
        public IEnumerator LoadACollection() {
            StoppableWaitForTime waiter = Utils.GetWaitQuery ();

            Clients.GraphQLMany.collections(
                first: 1,
                callback: (collections, error, after) => {
                waiter.Stop();

                    Assert.IsNull(error, "No errors");
                    Assert.AreEqual(1, collections.Count, "Loaded 1 collection");
                    Assert.AreEqual("Home page", collections[0].title(), "First collection is: Home page");

                    List<string> aliases = Utils.GetImageAliases();

                    foreach(string alias in aliases) {
                        // this will throw an exception if image was not queried with alias
                        // no collections in our test store have values
                        collections[0].image(alias);
                    }
                }
            );

            yield return waiter;

            Assert.IsTrue (waiter.IsStopped, Utils.MaxQueryMessage);
        }

        [UnityTest]
        public IEnumerator LoadAllCollections() {
            float maxDuration = 6f;
            int maxCollections = 13;
            StoppableWaitForTime waiter = new StoppableWaitForTime (maxDuration);
            ShopifyError errorsFromQueries = null;
            List<Collection> AllCollections = new List<Collection>();

            Action<string> loadPage = null;

            loadPage = (string after) => {
                Clients.GraphQLMany.collections((collections, errors, afterCollectionsLoaded) => {
                    if (collections != null) {
                        AllCollections.AddRange(collections);
                    }

                    if (errors != null) {
                        errorsFromQueries = errors;
                        waiter.Stop();
                    } else if (afterCollectionsLoaded == null || AllCollections.Count >= maxCollections) {
                        waiter.Stop();
                    } else {
                        loadPage(afterCollectionsLoaded);
                    }
                }, after: after);
            };

            loadPage(null);

            yield return waiter;

            Assert.IsTrue(waiter.IsStopped, "Query did not complete in " + maxDuration + " seconds");
            Assert.IsNull(errorsFromQueries);
            Assert.GreaterOrEqual(AllCollections.Count, maxCollections);
        }
    }
}
#endif
