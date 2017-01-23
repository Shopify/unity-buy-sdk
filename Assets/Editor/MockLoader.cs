namespace Shopify.Tests {
    using System;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.SDK;

    public class MockLoader : ILoader {
        private static bool Initialized;

        public static int CountProductsPages = 10;
        public static List<QueryRootQuery> QueryProducts;
        public static Dictionary<string,string> ResponseProducts;

        private static void Initialize() {
            Initialized = true;

            QueryProducts = new List<QueryRootQuery>();
            ResponseProducts = new Dictionary<string,string>();

            for(int i = 0; i < CountProductsPages; i++) {
                QueryRootQuery query = new QueryRootQuery().
                        shop(s => s.
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
                                first: 250, after: i > 0 ? (i - 1).ToString() : null
                            )
                        );
           
                ResponseProducts[query.ToString()] = String.Format(@"{{
                    ""data"": {{
                        ""shop"": {{
                            ""products"": {{
                                ""edges"": [
                                    {{
                                        ""cursor"": ""{0}"",
                                        ""node"": {{
                                            ""title"": ""Product{1}""
                                        }}
                                    }}
                                ],
                                ""pageInfo"": {{
                                    ""hasNextPage"": {2}
                                }}
                            }}
                        }}
                    }}
                }}", i, i, i < CountProductsPages - 1 ? "true" : "false");
            }
        }

        public MockLoader() {
            if (!Initialized) {
                Initialize();
            }
        }
        
        public void Load(string query, LoaderResponse callback) {
            if (ResponseProducts.ContainsKey(query)) {
                callback(ResponseProducts[query], null);
            } else {
                throw new Exception("NO RESPONSE");
            }
        }
    }
}
