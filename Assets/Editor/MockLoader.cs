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

        public static int PageSize = DefaultQueries.MaxPageSize;
        private static List<IMockLoader> Loaders;


        private static void Initialize() {
            Loaders = new List<IMockLoader>() {
                new MockLoaderProducts(),
                new MockLoaderGeneric(),
                new MockLoaderCollections()
            };

            Initialized = true;
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

            if (query.Contains(@"after:""404""")) {
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
