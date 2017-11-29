#if UNITY_EDITOR
namespace Shopify.Unity.Tests
{
    using UnityEngine;
    using UnityEngine.TestTools;
    using NUnit.Framework;
    using Shopify.Unity;
    using System.Collections;

    public class TestGenericQuery : MonoBehaviour {
        [UnityTest]
        public IEnumerator LoadShopName() {
            StoppableWaitForTime waiter = Utils.GetWaitQuery();

            Clients.GraphQL.Query(
                (q) => q.shop(s => s
                    .name()
                ), 
                (data, error) => {
                    waiter.Stop();
                    Assert.IsNull(error, "No errors");
                    Assert.AreEqual("graphql", data.shop().name(), "Shop name was \"graphql\"");
                }
            );

            yield return waiter;

            Assert.IsTrue (waiter.IsStopped, Utils.MaxQueryMessage);
        }
    }   
}
#endif
