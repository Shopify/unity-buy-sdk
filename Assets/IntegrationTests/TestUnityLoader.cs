#if !SHOPIFY_MONO_UNIT_TEST
namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;
    using System.Collections;

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
    }   
}
#endif
