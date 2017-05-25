namespace Shopify.Tests {
    using System;
    using System.Text;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;
    
    public class MockLoader : ILoader {
        private static bool Initialized;
        
        public static int PageSize = DefaultQueries.MaxPageSize;
        private static List<IMockLoader> _Loaders;
        
        private static void Initialize() {
            _Loaders = new List<IMockLoader>() {
                new MockLoaderErrors(),
                new MockLoaderProducts(),
                new MockLoaderGeneric(),
                new MockLoaderCollections(),
                new MockLoaderCheckouts()
            };
            
            Initialized = true;
        }
        
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
        
        public List<IMockLoader> Loaders {
            get {
                return _Loaders;
            }
        }
        
        public MockLoader() {
            if (!Initialized) {
                Initialize();
            }
        }
        
        public void Load(string query, LoaderResponseHandler callback) {
            bool handledResponse = false;
            
            foreach(IMockLoader loader in _Loaders) {
                if (loader.DoesHandleQueryResponse(query)) {
                    loader.HandleResponse(query, callback);
                    handledResponse = true;
                    
                    break;
                }
            }
            
            if (!handledResponse) {
                throw new Exception("NO QUERY RESPONSE: \n\n" + query + "\n\n");
            }
        }
    }
}
