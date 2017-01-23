namespace Shopify.Tests {
    using System;
    using System.Text;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.SDK;

    public class MockLoader : ILoader {
        private static bool Initialized;

        public static int CountProductsPages = 10;
        public static int PageSize = 250;
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
                                first: PageSize, after: i > 0 ? (i * PageSize - 1).ToString() : null
                            )
                        );

                StringBuilder edges = new StringBuilder();

                for(int j = 0; j < PageSize; j++) {
                    
                    edges.Append(String.Format(@"{{
                        ""cursor"": ""{0}"",
                        ""node"": {{
                            ""title"": ""Product{0}""
                        }}
                    }}{1}", i * PageSize + j, j < PageSize - 1 ? "," : ""));
                }

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
                }}", edges.ToString(), i < CountProductsPages - 1 ? "true" : "false");
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
                Console.WriteLine("NO RESPONSE: " + query);
                throw new Exception("NO QUERY RESPONSE");
            }
        }
    }
}
