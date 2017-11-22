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
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");
			bool hadResult = false;

            ShopifyBuy.Client().Query(
                (q) => q.shop(s => s
                    .name()
                ), 
                (data, error) => {
					hadResult = true;
					Assert.IsNull(error, "No errors");
					Assert.AreEqual("graphql", data.shop().name(), "Shop name was \"graphql\"");
                }
            );

			yield return Utils.GetWaitMaxQueryDuration ();

			Assert.IsTrue (hadResult, Utils.MaxQueryMessage);
        }
    }   
}
