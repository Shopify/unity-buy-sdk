namespace Shopify.Tests {
    using System;
    using System.Text;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;

    public class MockLoaderProducts : BaseMockLoader {
        public static int CountProductsPages = 4;

        public MockLoaderProducts() {
            SetupQueriesOnShopProducts();
            SetupQueriesOnNodeForImageConnections();
            SetupQueriesOnNodesForProducts();
        }

        private void SetupQueriesOnShopProducts() {
            for(int i = 0; i < CountProductsPages; i++) {
                QueryRootQuery query = new QueryRootQuery();

                DefaultQueries.products.ShopProducts(
                    query: query,
                    first: DefaultQueries.MaxProductPageSize,
                    imageResolutions: ShopifyClient.DefaultImageResolutions,
                    after: i > 0 ? (i * DefaultQueries.MaxProductPageSize - 1).ToString() : null
                );

                string response = String.Format(@"{{
                    ""data"": {{
                        ""shop"": {{
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
                }}", GetProductEdges(i), UtilsMockLoader.GetJSONBool(i < CountProductsPages - 1));

                AddResponse(query, response);
            }
        }

        private string GetProductEdges(int page) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < DefaultQueries.MaxProductPageSize; i++) {
                int product = page * DefaultQueries.MaxProductPageSize + i;
                bool imagesHasNextPage = product == 1 || product == 2;
                bool variantsHasNextPage = product == 2;

                StringBuilder resolutionImageResponses = new StringBuilder();

                int numAliasIterated = 0;
                foreach(string alias in ShopifyClient.DefaultImageResolutions.Keys) {
                    string aliasedImages = String.Format(@"
                        ""images___{0}"": {{
                            ""edges"": [
                                {1}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {2}
                            }}
                        }}
                    ",
                    alias,
                    GetImageNodes(0, 1),
                    UtilsMockLoader.GetJSONBool(false));

                    resolutionImageResponses.Append(aliasedImages);

                    if (numAliasIterated < ShopifyClient.DefaultImageResolutions.Keys.Count - 1) {
                        resolutionImageResponses.Append(",");
                    }

                    numAliasIterated++;
                }

                edges.Append(String.Format(@"{{
                    ""cursor"": ""{0}"",
                    ""node"": {{
                        ""id"": ""{0}"",
                        ""title"": ""Product{0}"",
                        ""descriptionHtml"": ""<div>This is product {0}</div>"",
                        ""options"": [
                            {{
                                ""name"": ""default option"",
                                ""values"": [""default-opt""]
                            }}
                        ],
                        ""variants"": {{
                            ""edges"": [
                                {3}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {4}
                            }}
                        }},
                        ""collections"": {{
                            ""edges"": [
                                {5}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {6}
                            }}
                        }},
                        ""images"": {{
                            ""edges"": [
                                {1}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {2}
                            }}
                        }},
                        {7}
                    }}
                }}{8}",
                product,
                GetImageNodes(0, imagesHasNextPage ? DefaultQueries.MaxPageSize : 1),
                UtilsMockLoader.GetJSONBool(imagesHasNextPage),
                GetVariants(0, product, variantsHasNextPage ? DefaultQueries.MaxPageSize : 1),
                UtilsMockLoader.GetJSONBool(variantsHasNextPage),
                GetCollectionsOnProduct(0, product, 1),
                UtilsMockLoader.GetJSONBool(false),
                resolutionImageResponses.ToString(),
                i < DefaultQueries.MaxProductPageSize - 1 ? "," : ""));
            }

            return edges.ToString();
        }

        private void SetupQueriesOnNodeForImageConnections() {
            QueryRootQuery query = new QueryRootQuery();

            query.node(n => n
                .onProduct(p => p
                    .id()
                    .images(ic => DefaultQueries.products.ImageConnection(ic, ShopifyClient.DefaultImageResolutions),
                        first: DefaultQueries.MaxPageSize, after: "image249"
                    )
                ),
                id: "1", alias: "a1"
            );

            query.node(n => n
                .onProduct(p => p
                    .id()
                    .images(ic => DefaultQueries.products.ImageConnection(ic, ShopifyClient.DefaultImageResolutions),
                        first: DefaultQueries.MaxPageSize, after: "image249"
                    )
                    .variants(
                        vc => DefaultQueries.products.ProductVariantConnection(vc, ShopifyClient.DefaultImageResolutions),
                        first: DefaultQueries.MaxPageSize, after: "variant249"
                    )
                ),
                id: "2", alias: "a2"
            );

            string response = String.Format(@"{{
                ""data"": {{
                    ""node___a1"": {{
                        ""__typename"": ""Product"",
                        ""id"": ""1"",
                        ""images"": {{
                            ""edges"": [
                                {0}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {1}
                            }}
                        }}
                    }},
                    ""node___a2"": {{
                        ""__typename"": ""Product"",
                        ""id"": ""2"",
                        ""images"": {{
                            ""edges"": [
                                {2}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {3}
                            }}
                        }},
                        ""variants"": {{
                            ""edges"": [
                                {4}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {5}
                            }}
                        }}
                    }}
                }}
            }}", GetImageNodes(1, DefaultQueries.MaxPageSize), UtilsMockLoader.GetJSONBool(false), GetImageNodes(1, DefaultQueries.MaxPageSize), UtilsMockLoader.GetJSONBool(true), GetVariants(1, 2, DefaultQueries.MaxPageSize), UtilsMockLoader.GetJSONBool(false));

            AddResponse(query, response);

            query = new QueryRootQuery();
            query.node(n => n
                .onProduct(p => p
                    .id()
                    .images(ic => DefaultQueries.products.ImageConnection(ic, ShopifyClient.DefaultImageResolutions),
                        first: DefaultQueries.MaxPageSize, after: "image499"
                    )
                ),
                id: "2", alias: "a2"
            );

            response = String.Format(@"{{
                ""data"": {{
                    ""node___a2"": {{
                        ""__typename"": ""Product"",
                        ""id"": ""2"",
                        ""images"": {{
                            ""edges"": [
                                {0}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {1}
                            }}
                        }}
                    }}
                }}
            }}", GetImageNodes(2, DefaultQueries.MaxPageSize), UtilsMockLoader.GetJSONBool(false));

            AddResponse(query, response);
        }

        public void SetupQueriesOnNodesForProducts() {
            QueryRootQuery query = new QueryRootQuery();

            List<string> productIds = new List<string>();
            productIds.Add("productId333");
            productIds.Add("productId444");

            query.nodes(n => n
                .onProduct(p => DefaultQueries.products.Product(p, ShopifyClient.DefaultImageResolutions)),
                ids: productIds
            );

            string response = String.Format(@"{{
                ""data"": {{
                    ""nodes"": [
                        {{
                            ""__typename"": ""Product"",
                            ""id"": ""productId333"",
                            {0},
                            ""variants"": {{
                                ""edges"": [
                                    {2}
                                ],
                                ""pageInfo"": {{
                                    ""hasNextPage"": {3}
                                }}
                            }},
                            ""collections"": {{
                                ""edges"": [
                                    {{
                                        ""node"": {{
                                            ""id"": ""0"",
                                            ""title"": ""I am collection 0"",
                                            ""updatedAt"": ""2016-09-11T21:32:43Z""
                                        }},
                                        ""cursor"": ""0""
                                    }}
                                ],
                                ""pageInfo"": {{
                                    ""hasNextPage"": false
                                }}
                            }}
                        }},
                        {{
                            ""__typename"": ""Product"",
                            ""id"": ""productId444"",
                            {0},
                            ""variants"": {{
                                ""edges"": [
                                    {2}
                                ],
                                ""pageInfo"": {{
                                    ""hasNextPage"": {3}
                                }}
                            }},
                            ""collections"": {{
                                ""edges"": [
                                    {{
                                        ""node"": {{
                                            ""id"": ""0"",
                                            ""title"": ""I am collection 0"",
                                            ""updatedAt"": ""2016-09-11T21:32:43Z""
                                        }},
                                        ""cursor"": ""0""
                                    }}
                                ],
                                ""pageInfo"": {{
                                    ""hasNextPage"": false
                                }}
                            }}
                        }}
                    ]
                }}
            }}", GetImagesConnection(0, 1, true), GetImagesConnection(0, 1, false), GetVariants(1, 2, 1), UtilsMockLoader.GetJSONBool(false));

            AddResponse(query, response);
        }

        private string GetVariants(int page, int product, int countVariants = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countVariants; i++) {
                int variant = page * DefaultQueries.MaxPageSize + i;

                StringBuilder resolutionImageResponses = new StringBuilder();
                int numAliasIterated = 0;
                foreach(string alias in ShopifyClient.DefaultImageResolutions.Keys) {
                    string aliasedImages = String.Format(@"
                        ""image___{0}"": null
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
                        ""available"": true,
                        ""image"": null,
                        ""title"": ""variant{0}"",
                        ""price"": 3.01,
                        ""weight"": 1.02,
                        ""weightUnit"": ""KILOGRAMS"",
                        {1}
                    }},
                    ""cursor"": ""variant{0}""
                }}{2}", variant, resolutionImageResponses.ToString(), i < countVariants - 1 ? "," : ""));
            }
            return edges.ToString();
        }

        private string GetCollectionsOnProduct(int page, int product, int countCollections = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countCollections; i++) {
                int collection = page * DefaultQueries.MaxPageSize + i;

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""id"": ""collection{0}"",
                        ""title"": ""I am collection {0}"",
                        ""updatedAt"": ""2016-09-11T21:32:43Z""
                    }},
                    ""cursor"": ""collection{0}""
                }}{1}", collection, i < countCollections - 1 ? "," : ""));
            }

            return edges.ToString();
        }

        private string GetImagesConnection(int page, int countImages, bool hasNextPage) {
            StringBuilder imagesResponse = new StringBuilder();

            imagesResponse.Append(String.Format(@"
                ""images"": {{
                    ""edges"": [
                        {0}
                    ],
                    ""pageInfo"": {{
                        ""hasNextPage"": {1}
                    }}
                }},
            ", GetImageNodes(page, countImages), UtilsMockLoader.GetJSONBool(hasNextPage)));

            int numAliasIterated = 0;

            foreach(string alias in ShopifyClient.DefaultImageResolutions.Keys) {
                string aliasedImages = String.Format(@"
                    ""images___{0}"": {{
                        ""edges"": [
                            {1}
                        ],
                        ""pageInfo"": {{
                            ""hasNextPage"": {2}
                        }}
                    }}
                ",
                alias,
                GetImageNodes(page, countImages),
                UtilsMockLoader.GetJSONBool(hasNextPage));

                imagesResponse.Append(aliasedImages);

                if (numAliasIterated < ShopifyClient.DefaultImageResolutions.Keys.Count - 1) {
                    imagesResponse.Append(",");
                }

                numAliasIterated++;
            }

            return imagesResponse.ToString();
        }

        private string GetImageNodes(int page, int countImages = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countImages; i++) {
                int image = page * DefaultQueries.MaxPageSize + i;

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""altText"": ""I am an image {0}"",
                        ""src"": ""http://cdn.com/images/{0}-{1}""
                    }},
                    ""cursor"": ""image{0}""
                }}{1}", image, i < countImages - 1 ? "," : ""));
            }

            return edges.ToString();
        }
    }
}
