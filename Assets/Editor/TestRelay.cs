namespace Shopify.Tests {
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.MiniJSON;

    class CollectionTestQueries {
        public static QueryRootQuery query1 = (new QueryRootQuery()).shop(s => s.
            products(p => p.
                edges(e => e.
                    node(n => n.
                        title()
                    ).
                    cursor()
                ).
                pageInfo(pi => pi.
                    hasNextPage()
                ),
                first: 1
            )
        );
        public static QueryRootQuery query2 = (new QueryRootQuery()).shop(s => s.
            products(p => p.
                edges(e => e.
                    node(n => n.
                        title()
                    ).
                    cursor()
                ).
                pageInfo(pi => pi.
                    hasNextPage()
                ),
                first: 1, after: "first page"
            )
        );
        public static List<QueryRootQuery> queries = new List<QueryRootQuery> {query1, query2};
    }

    class CollectionTestLoader : ILoader {
        public string Domain {
            get {
                return "someshop.myshopify.com";
            }
        }

        public string AccessToken {
            get {
                return "1234";
            }
        }

        public void Load(string query, LoaderResponseHandler callback) {
            if (query == CollectionTestQueries.query1.ToString()) {
                callback(@"{""data"":{
                    ""shop"": {
                        ""products"": {
                            ""edges"": [
                                {
                                    ""node"": {
                                        ""title"": ""Product1""
                                    },
                                    ""cursor"": ""first page""
                                }
                            ],
                            ""pageInfo"": {
                                ""hasNextPage"": true
                            }
                        }
                    }
                }}", null);
            } else if (query == CollectionTestQueries.query2.ToString()) {
                callback(@"{""data"":{
                    ""shop"": {
                        ""products"": {
                            ""edges"": [
                                {
                                    ""node"": {
                                        ""title"": ""Product2""
                                    },
                                    ""cursor"": ""second page""
                                }
                            ],
                            ""pageInfo"": {
                                ""hasNextPage"": false
                            }
                        }
                    }
                }}", null);
            }
        }
    }

    [TestFixture]
    public class TestRelay {
        

        [Test]
        public void TestCastConnectionToList() {
            string stringJSON = @"{
                ""edges"": [ 
                    {
                        ""node"": {
                            ""title"": ""Product 1""
                        }
                    },
                    {
                        ""node"": {
                            ""title"": ""Product 2""
                        }
                    }
                ]
            }";

            ProductConnection connection = new ProductConnection((Dictionary<string, object>) Json.Deserialize(stringJSON));
            List<Product> products = (List<Product>) connection;
            bool couldIterateOverConnection = false;

            foreach (Product product in products) {
                couldIterateOverConnection = true;
            }

            Assert.IsNotNull(products, "products is not null");
            Assert.AreEqual(2, products.Count, "products list has two products");
            Assert.AreEqual("Product 2", products[1].title(), "List had data");
            Assert.IsTrue(couldIterateOverConnection, "could iterate over connection");
        }

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

        [Test]
        public void TestConnectionLoader() {
            int countRequests = 0;
            ConnectionLoader ConnectionLoader = new ConnectionLoader(new QueryLoader(new CollectionTestLoader()));
            List<QueryResponse> responsesToRequestQuery = new List<QueryResponse>();
            QueryResponse mergedResponse = null;

            ConnectionLoader.QueryConnection(
                (response) => {
                    responsesToRequestQuery.Add(response);

                    if (countRequests < CollectionTestQueries.queries.Count) {
                        countRequests++;
                        return CollectionTestQueries.queries[countRequests - 1];
                    } else {
                        return null;
                    }
                },
                (response) => {
                    return ((QueryRoot) response).shop().products();
                },
                (response) => {
                    mergedResponse = response;
                }
            );

            Assert.AreEqual(3, responsesToRequestQuery.Count);
            Assert.AreEqual(null, responsesToRequestQuery[0]);
            Assert.AreEqual("Product1", responsesToRequestQuery[1].data.shop().products().edges()[0].node().title());
            Assert.AreEqual("Product2", responsesToRequestQuery[2].data.shop().products().edges()[0].node().title());
            Assert.AreEqual(2, countRequests);
            Assert.AreEqual(2, mergedResponse.data.shop().products().edges().Count);
        }
    }
}