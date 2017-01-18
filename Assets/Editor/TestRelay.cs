namespace Shopify.Tests {
    using NUnit.Framework;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;

    [TestFixture]
    public class TestRelay {
        [Test]
        public void TestAddingToConnection() {
            string responseString1 = @"{
                ""edges"": [
                    {
                        ""node"": {
                            ""title"": ""product1""
                        },
                        ""cursor"": ""0""
                    }
                ],
                ""pageInfo"": {
                    ""hasNextPage"": true
                }
            }";

            string responseString2 = @"{
                ""edges"": [
                    {
                        ""node"": {
                            ""title"": ""product2""
                        },
                        ""cursor"": ""1""
                    }
                ],
                ""pageInfo"": {
                    ""hasNextPage"": false
                }
            }";

            Dictionary<string, object> response1 = (Dictionary<string, object>) Json.Deserialize(responseString1);
            Dictionary<string, object> response2 = (Dictionary<string, object>) Json.Deserialize(responseString2);

            ProductConnection queryResponse1 = new ProductConnection(response1);
            ProductConnection queryResponse2 = new ProductConnection(response2);
            queryResponse1.AddFromConnection(queryResponse2);

            Assert.AreEqual(2, queryResponse1.edges().Count);
            Assert.AreEqual("product1", queryResponse1.edges()[0].node().title());
            Assert.AreEqual("product2", queryResponse1.edges()[1].node().title());
            Assert.AreEqual(false, queryResponse1.pageInfo().hasNextPage());

            // the following are to test whether original response was not mutated
            Assert.AreEqual(1, ((List<object>) response1["edges"]).Count);
            Assert.AreEqual("product1", ((Dictionary<string,object>) ((Dictionary<string,object>) ((List<object>) response1["edges"])[0])["node"])["title"]);
        }

        [Test]
        public void TestAddingToConnectionUnitialized() {
            string responseString2 = @"{
                ""edges"": [
                    {
                        ""node"": {
                            ""title"": ""product1""
                        },
                        ""cursor"": ""0""
                    }
                ],
                ""pageInfo"": {
                    ""hasNextPage"": true
                }
            }";
            Dictionary<string, object> response2 = (Dictionary<string, object>) Json.Deserialize(responseString2);

            Dictionary<string,object> response1 = new Dictionary<string,object>();
            ProductConnection queryResponse1 = new ProductConnection(response1);
            ProductConnection queryResponse2 = new ProductConnection(response2);

            queryResponse1.AddFromConnection(queryResponse2);

            Assert.AreEqual(1, queryResponse1.edges().Count);
            Assert.AreEqual("product1", queryResponse1.edges()[0].node().title());
            Assert.AreEqual(true, queryResponse1.pageInfo().hasNextPage());
        }
    }
}