namespace Shopify.Unity.Tests
{
    using UnityEngine;
	using UnityEngine.TestTools;
	using NUnit.Framework;
    using Shopify.Unity;
	using System.Collections;
    using System.Collections.Generic;

    public class TestShopifyCollections : MonoBehaviour {
		[UnityTest]
		public IEnumerator LoadACollection() {
            ShopifyBuy.Init("351c122017d0f2a957d32ae728ad749c", "graphql.myshopify.com");
			StoppableWaitForTime waiter = Utils.GetWaitQuery ();

            ShopifyBuy.Client().collections(
                first: 1,
                callback: (collections, error) => {
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
    }   
}
