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

        public string ApiKey {
            get {
                return "1234";
            }
        }

        private static bool Initialized;

        public static int CountProductsPages = 4;
        public static int CountCollectionsPages = 4;
        public static int PageSize = DefaultQueries.MaxPageSize;
        public static Dictionary<string,string> ResponseProducts;
        public static Dictionary<string,string> ResponseNodes;
        public static Dictionary<string,string> ResponseCollections;
        public static Dictionary<string,string> ResponseGeneric;


        private static void Initialize() {
            InitProducts();
            InitCollections();

            ResponseNodes = new Dictionary<string,string>();
            InitGeneric();
            InitResponseOnNodeForProduct();
            InitResponseOnNodeForCollection();

            Initialized = true;
        }

        private static void InitGeneric() {
            ResponseGeneric = new Dictionary<string,string>();

            QueryRootQuery query = new QueryRootQuery();
            query.shop(s => s.name());

            ResponseGeneric[query.ToString()] = @"{
                ""data"": {
                    ""shop"": {
                        ""name"": ""this is the test shop yo""
                    }
                }
            }";

            MutationQuery mutation = new MutationQuery();

            mutation.customerAccessTokenCreate((a) => a
                .customerAccessToken(at => at
                    .accessToken()
                ),
                input: new CustomerAccessTokenCreateInput("some@email.com", "password")
            );

            ResponseGeneric[mutation.ToString()] = @"{
                ""data"": {
                    ""customerAccessTokenCreate"": {
                        ""customerAccessToken"": {
                            ""accessToken"": ""i am a token""
                        }
                    }
                }
            }";
        }

        private static void InitProducts() {
            ResponseProducts = new Dictionary<string,string>();
        
            for(int i = 0; i < CountProductsPages; i++) {
                QueryRootQuery query = new QueryRootQuery();

                DefaultQueries.products.ShopProducts(
                    query: query, 
                    first: PageSize, 
                    after: i > 0 ? (i * PageSize - 1).ToString() : null
                );

                ResponseProducts[query.ToString()] = String.Format(@"{{
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
                }}", GetProductEdges(i), GetJSONBool(i < CountProductsPages - 1));
            }
        }

        private static void InitCollections() {
            ResponseCollections = new Dictionary<string, string>();

            for(int i = 0; i < CountCollectionsPages; i++) {
                QueryRootQuery query = new QueryRootQuery();

                DefaultQueries.collections.ShopCollections(
                    query: query, 
                    first: PageSize, 
                    after: i > 0 ? (i * PageSize - 1).ToString() : null
                );

                ResponseProducts[query.ToString()] = String.Format(@"{{
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
                }}", GetCollectionEdges(i), GetJSONBool(i < CountCollectionsPages - 1));
            }
        }

        private static void InitResponseOnNodeForProduct() {
            QueryRootQuery query = new QueryRootQuery();
            
            query.node(n => n
                .onProduct(p => p
                    .id()
                    .images(ic => DefaultQueries.products.ImageConnection(ic),
                        first: PageSize, after: "image249"
                    )
                ),
                id: "1", alias: "a1"
            );

            query.node(n => n
                .onProduct(p => p
                    .id()
                    .images(ic => DefaultQueries.products.ImageConnection(ic),
                        first: PageSize, after: "image249"
                    )
                    .variants(
                        vc => DefaultQueries.products.ProductVariantConnection(vc),
                        first: DefaultQueries.MaxPageSize, after: "variant249"
                    )
                ),
                id: "2", alias: "a2"
            );

            ResponseNodes[query.ToString()] = String.Format(@"{{
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
            }}", GetImages(1, 1, DefaultQueries.MaxPageSize), GetJSONBool(false), GetImages(1, 2, DefaultQueries.MaxPageSize), GetJSONBool(true), GetVariants(1, 2, DefaultQueries.MaxPageSize), GetJSONBool(false));

            query = new QueryRootQuery();
            query.node(n => n
                .onProduct(p => p
                    .id()
                    .images(ic => DefaultQueries.products.ImageConnection(ic),
                        first: PageSize, after: "image499"
                    )
                ),
                id: "2", alias: "a2"
            );

            ResponseNodes[query.ToString()] = String.Format(@"{{
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
            }}", GetImages(2, 2, DefaultQueries.MaxPageSize), GetJSONBool(false));
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
            }}", GetProductOnCollections(1, 1, PageSize), GetJSONBool(false));
        }

        private static string GetProductEdges(int page) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < PageSize; i++) {
                int product = page * PageSize + i;
                bool imagesHasNextPage = product == 1 || product == 2;
                bool variantsHasNextPage = product == 2;
                
                edges.Append(String.Format(@"{{
                    ""cursor"": ""{0}"",
                    ""node"": {{
                        ""id"": ""{0}"",
                        ""title"": ""Product{0}"",
                        ""descriptionHtml"": ""<div>This is product {0}</div>"",
                        ""images"": {{
                            ""edges"": [
                                {1}
                            ],
                            ""pageInfo"": {{
                                ""hasNextPage"": {2}
                            }}
                        }},
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
                        }}
                    }}
                }}{7}", 
                product, 
                GetImages(0, product, imagesHasNextPage ? PageSize : 1), 
                GetJSONBool(imagesHasNextPage),
                GetVariants(0, product, variantsHasNextPage ? PageSize : 1),
                GetJSONBool(variantsHasNextPage),
                GetCollectionsOnProduct(0, product, 1),
                GetJSONBool(false),
                i < PageSize - 1 ? "," : ""));
            }

            return edges.ToString();
        }

        private static string GetCollectionEdges(int page) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < PageSize; i++) {
                int collection = page * PageSize + i;
                bool productsHasNextPage = collection == 0;

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""id"": ""{0}"",
                        ""images"": {{
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
                        }}
                    }},
                    ""cursor"": ""{0}""
                }}{3}", 
                collection,
                GetProductOnCollections(0, collection, productsHasNextPage ? PageSize : 1), 
                GetJSONBool(productsHasNextPage),
                i < PageSize - 1 ? "," : ""));
            }

            return edges.ToString();        
        }

        private static string GetVariants(int page, int product, int countVariants = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countVariants; i++) {
                int variant = page * PageSize + i;

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""id"": ""{0}"",
                        ""available"": true,
                        ""images"": [],
                        ""title"": ""variant{0}"",
                        ""price"": 3.01,
                        ""weight"": 1.02,
                        ""weightUnit"": ""KILOGRAMS""
                    }},
                    ""cursor"": ""variant{0}""
                }}{1}", variant, i < countVariants - 1 ? "," : ""));
            }
            return edges.ToString();
        }

        private static string GetImages(int page, int product, int countImages = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countImages; i++) {
                int image = page * PageSize + i;

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

        private static string GetCollectionsOnProduct(int page, int product, int countCollections = 1) {
            StringBuilder edges = new StringBuilder();

            for(int i = 0; i < countCollections; i++) {
                int collection = page * PageSize + i;

                edges.Append(String.Format(@"{{
                    ""node"": {{
                        ""id"": ""collection{0}"",
                        ""images"": {{
                            ""altText"": ""I am an image {0}"",
                            ""src"": ""http://cdn.com/images/collection{0}-{1}""
                        }},
                        ""title"": ""I am collection {0}"",
                        ""updatedAt"": ""2016-09-11T21:32:43Z""
                    }},
                    ""cursor"": ""collection{0}""
                }}{1}", collection, i < countCollections - 1 ? "," : ""));
            }

            return edges.ToString();        
        }

        private static string GetJSONBool(bool value) {
            return value ? "true" : "false";
        }

        public MockLoader() {
            if (!Initialized) {
                Initialize();
            }
        }
        
        public void Load(string query, LoaderResponseHandler callback) {
            if (ResponseGeneric.ContainsKey(query)) {
                callback(ResponseGeneric[query], null);
            } else if (ResponseProducts.ContainsKey(query)) {
                callback(ResponseProducts[query], null);
            } else if (ResponseNodes.ContainsKey(query)) {
                callback(ResponseNodes[query], null);
            } else if (query.Contains(@"after:""404""")) {
                callback(null, "404 from mock loader");
            } else if (query.Contains(@"after:""666""")) {
                callback(@"{""errors"": [{""message"": ""GraphQL error from mock loader""}]}", null);
            } else {
                throw new Exception("NO QUERY RESPONSE: \n\n" + query + "\n\n");
            }
        }
    }
}
