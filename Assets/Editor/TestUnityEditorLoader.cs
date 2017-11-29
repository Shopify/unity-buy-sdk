#if !SHOPIFY_MONO_UNIT_TEST
namespace Shopify.Tests {
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using UnityEngine;
    using Shopify.Unity.SDK.Editor;
    using Shopify.Unity.SDK;
    using System.Collections;

    [TestFixture]
    public class TestUnityEditorLoader {
        [UnityTest]
        public IEnumerator TestWithValidQuery() {
            var requestIsFinished = false;

            string domain = "graphql.myshopify.com";
            string authorization = "351c122017d0f2a957d32ae728ad749c";

            var queryLoader = new QueryLoader(new UnityEditorLoader(domain, authorization));

            queryLoader.Query(TestQueries.Query, (QueryResponse response) => {
                Assert.True(response.HTTPError == null, "http errors were not null: " + response.HTTPError);
                Assert.True(response.errors == null, "GraphQL errors were null");
                Assert.True(response.data != null, "Received a response");
                Assert.True(response.data.shop().name() != null, "graphql");
                requestIsFinished = true;
            });

            while (!requestIsFinished) {
                yield return null;
            }
        }
    }
}
#endif