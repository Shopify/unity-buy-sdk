namespace Shopify.Tests {
    using System;
    using System.Text;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;

    public class MockLoader : ILoader {
        public string Domain {
            get {
                return "graphql.myshopify.com";
            }
        }

        public string AccessToken {
            get {
                return "1234";
            }
        }

        private static bool Initialized;

        public static int CountCollectionsPages = 4;
        public static int PageSize = DefaultQueries.MaxPageSize;
        public static Dictionary<string,string> ResponseNodes;
        public static Dictionary<string,string> ResponseCollections;
        private static List<IMockLoader> Loaders;


        private static void Initialize() {
            Loaders = new List<IMockLoader>() {
                new MockLoaderProducts(),
                new MockLoaderGeneric()
            };

            InitCollections();

            ResponseNodes = new Dictionary<string,string>();
            InitResponseOnNodeForCollection();

            Initialized = true;
        }

        private static void InitCollections() {
            ResponseCollections = new Dictionary<string, string>();

            for(int i = 0; i < CountCollectionsPages; i++) {
                QueryRootQuery query = new QueryRootQuery();

                DefaultQueries.collections.ShopCollections(
                    query: query, 
                    first: PageSize, 
                    after: i > 0 ? (i * PageSize - 1).ToString() : null,
                    imageResolutions: ShopifyClient.DefaultImageResolutions
                );

                ResponseCollections[query.ToString()] = String.Format(@"{{
                    ""data"": {{
                        ""shop"": {{
                            ""collections"": {{
                                ""edges"": [
                                    {0}
                                ],
                                ""pageInfo"": {{
                                    ""hasNextPage"": {1}
                                }}
                            }}
                        }}
                    }}
                }}", GetCollectionEdges(i), UtilsMockLoader.GetJSONBool(i < CountCollectionsPages - 1));
            }
        }

        private static void InitResponseOnNodeForCollection() {
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

            ResponseNodes[query.ToString()] = String.Format(@"{{
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
            }}", GetProductOnCollections(1, 1, PageSize), UtilsMockLoader.GetJSONBool(false));
        }

        private static string GetCollectionEdges(int page) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < PageSize; i++) {
                int collection = page * PageSize + i;
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
                GetProductOnCollections(0, collection, productsHasNextPage ? PageSize : 1), 
                UtilsMockLoader.GetJSONBool(productsHasNextPage),
                resolutionImageResponses.ToString(),
                i < PageSize - 1 ? "," : ""));
            }

            return edges.ToString();        
        }

        private static string GetProductOnCollections(int page, int collection, int countProducts = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countProducts; i++) {
                int product = page * PageSize + i;

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""id"": ""product{0}""
                    }},
                    ""cursor"": ""product{0}""
                }}{1}", product, i < countProducts - 1 ? "," : ""));
            }

            return edges.ToString(); 
        }

        public MockLoader() {
            if (!Initialized) {
                Initialize();
            }
        }
        
        public void Load(string query, LoaderResponseHandler callback) {
            bool handledResponse = false;

            foreach(IMockLoader loader in Loaders) {
                if (loader.DoesHandleQueryResponse(query)) {
                    loader.HandleResponse(query, callback);                    
                    handledResponse = true;

                    break;
                }
            }

            if (ResponseCollections.ContainsKey(query)) {
                handledResponse = true;
                callback(ResponseCollections[query], null);
            } else if (ResponseNodes.ContainsKey(query)) {
                handledResponse = true;
                callback(ResponseNodes[query], null);
            } else if (query.Contains(@"after:""404""")) {
                handledResponse = true;
                callback(null, "404 from mock loader");
            } else if (query.Contains(@"after:""666""")) {
                handledResponse = true;
                callback(@"{""errors"": [{""message"": ""GraphQL error from mock loader""}]}", null);
            }

            if (!handledResponse) {
                throw new Exception("NO QUERY RESPONSE: \n\n" + query + "\n\n");
            }
        }
    }
}
