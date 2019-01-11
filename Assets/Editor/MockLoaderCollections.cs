namespace Shopify.Tests {
    using System;
    using System.Text;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;

    public class MockLoaderCollections : BaseMockLoader {
        public static int CountPages = 4;

        public MockLoaderCollections() {
            SetupQueriesOnShopCollections();
            SetupQueriesOnNode();
        }

        private void SetupQueriesOnShopCollections() {
            for(int i = 0; i < CountPages; i++) {
                QueryRootQuery query = new QueryRootQuery();

                DefaultQueries.collections.ShopCollections(
                    query: query,
                    first: DefaultQueries.MaxCollectionsPageSize,
                    after: i > 0 ? (i * DefaultQueries.MaxCollectionsPageSize - 1).ToString() : null,
                    imageResolutions: ShopifyClient.DefaultImageResolutions
                );

                string response = String.Format(@"{{
                    ""data"": {{
                        ""collections"": {{
                            ""edges"": [
                                {0}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {1}
                            }}
                        }}
                    }}
                }}", GetCollectionEdges(i), UtilsMockLoader.GetJSONBool(i < CountPages - 1));

                AddResponse(query, response);
            }
        }

        private string GetCollectionEdges(int page) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < DefaultQueries.MaxCollectionsPageSize; i++) {
                int collection = page * DefaultQueries.MaxCollectionsPageSize + i;
                bool productsHasNextPage = collection == 0;

                StringBuilder resolutionImageResponses = new StringBuilder();
                int numAliasIterated = 0;
                foreach(string alias in ShopifyClient.DefaultImageResolutions.Keys) {
                    string aliasedImages = String.Format(@"
                        ""image___{0}"": {{
                            ""altText"": ""I am an image {0}"",
                            ""src"": ""http://cdn.com/images/collection{0}""
                        }}
                    ",
                    alias);

                    resolutionImageResponses.Append(aliasedImages);

                    if (numAliasIterated < ShopifyClient.DefaultImageResolutions.Keys.Count - 1) {
                        resolutionImageResponses.Append(",");
                    }

                    numAliasIterated++;
                }

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""id"": ""{0}"",
                        ""image"": {{
                            ""altText"": ""I am an image {0}"",
                            ""src"": ""http://cdn.com/images/collection{0}""
                        }},
                        ""title"": ""I am collection {0}"",
                        ""updatedAt"": ""2016-09-11T21:32:43Z"",
                        ""products"": {{
                            ""edges"": [
                                {1}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {2}
                            }}
                        }},
                        {3}
                    }},
                    ""cursor"": ""{0}""
                }}{4}",
                collection,
                GetProductOnCollections(0, collection, productsHasNextPage ? DefaultQueries.MaxPageSize : 1),
                UtilsMockLoader.GetJSONBool(productsHasNextPage),
                resolutionImageResponses.ToString(),
                i < DefaultQueries.MaxCollectionsPageSize - 1 ? "," : ""));
            }

            return edges.ToString();
        }

        private string GetProductOnCollections(int page, int collection, int countProducts = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countProducts; i++) {
                int product = page * DefaultQueries.MaxPageSize + i;

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""id"": ""product{0}""
                    }},
                    ""cursor"": ""product{0}""
                }}{1}", product, i < countProducts - 1 ? "," : ""));
            }

            return edges.ToString();
        }

        private void SetupQueriesOnNode() {
            QueryRootQuery query = new QueryRootQuery();

            query.node(n => n
                .onCollection(c => c
                    .id()
                    .products(pc => DefaultQueries.collections.ProductConnection(pc),
                        first: DefaultQueries.MaxPageSize, after: "product249"
                    )
                ),
                id: "0", alias: "a0"
            );

            string response = String.Format(@"{{
                ""data"": {{
                    ""node___a0"": {{
                        ""__typename"": ""Collection"",
                        ""id"": ""0"",
                        ""products"": {{
                            ""edges"": [
                                {0}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {1}
                            }}
                        }}
                    }}
                }}
            }}", GetProductOnCollections(1, 1, DefaultQueries.MaxPageSize), UtilsMockLoader.GetJSONBool(false));

            AddResponse(query, response);
        }
    }
}
