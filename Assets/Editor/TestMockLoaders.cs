namespace Shopify.Tests {
    using System;
    using NUnit.Framework;
    using System.Collections.Generic;

    [TestFixture]
    public class TestMockLoaders {
        [Test]
        public void EnsureThatTwoLoadersAreNotHandlingTheSameQuery() {
            MockLoader mockLoader = new MockLoader();
            List<IMockLoader> loaders = mockLoader.Loaders;

            foreach(IMockLoader loaderA in loaders) {
                Type typeLoaderA = loaderA.GetType();

                List<string> queries = loaderA.GetQueries();

                foreach(IMockLoader loaderB in loaders) {
                    if (loaderA == loaderB) {
                        break;
                    }

                    Type typeLoaderB = loaderB.GetType();

                    foreach(string query in queries) {
                        Assert.IsFalse(
                            loaderB.DoesHandleQueryResponse(query), 
                            String.Format(
                                "{0} and {1} both handle the same query: \n\n{2}\n\n",
                                typeLoaderA.Name,
                                typeLoaderB.Name,
                                query
                            )
                        );
                    }
                }
            }
        }
    }
}
