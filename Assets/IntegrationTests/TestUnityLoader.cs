#if UNITY_EDITOR
namespace Shopify.Unity.Tests
{
    using System;
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;
    using System.Collections;
    using System.Collections.Generic;

    public class TestQueries {
        public static QueryRootQuery Query = new QueryRootQuery().shop(shop => shop.name());
    }
        
    public class TestUnityLoader : MonoBehaviour {
        [UnityTest]
        public IEnumerator RanQueryUsingUnityLoader() {
            string domain = "graphql.myshopify.com";
            string authorization = "351c122017d0f2a957d32ae728ad749c";
            StoppableWaitForTime waiter = Utils.GetWaitQuery ();

            QueryLoader queryLoader = new QueryLoader(new UnityLoader(domain, authorization));

            queryLoader.Query(TestQueries.Query, (QueryResponse response) => {
                waiter.Stop();

                Assert.IsNull(response.HTTPError, "http errors were not null: " + response.HTTPError);
                Assert.IsNull(response.errors, "GraphQL errors were null");
                Assert.IsNotNull(response.data, "Received a response");
                Assert.AreEqual("graphql", response.data.shop().name());
            });

            yield return waiter;

            Assert.IsTrue (waiter.IsStopped, Utils.MaxQueryMessage);
        }

        [UnityTest]
        public IEnumerator CanQueryTwoShops() {
            string domain1 = "graphql.myshopify.com";
            string authorization1 = "351c122017d0f2a957d32ae728ad749c";
            string domain2 = "graphql-many-products.myshopify.com";
            string authorization2 = "43b7fef8bd2f27f1d645586b72c9b825";

            StoppableWaitForTime waiter = Utils.GetWaitQuery ();
            List<string> names = new List<string>();

            QueryLoader queryLoader1 = new QueryLoader(new UnityLoader(domain1, authorization1));
            QueryLoader queryLoader2 = new QueryLoader(new UnityLoader(domain2, authorization2));

            QueryResponseHandler callback = (QueryResponse response) => {
                names.Add(response.data.shop().name());

                if (names.Count == 2) {
                    waiter.Stop();
                }
            };

            queryLoader1.Query(TestQueries.Query, callback);
            queryLoader2.Query(TestQueries.Query, callback);

            yield return waiter;

            Assert.IsTrue (waiter.IsStopped, Utils.MaxQueryMessage);
            Assert.AreNotEqual(names[0], names[1]);
        }
    }   
}
#endif
