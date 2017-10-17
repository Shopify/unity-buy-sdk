namespace Shopify.Tests {
    using System;
    using System.Text;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;

    public class MockLoader : BaseLoader {
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

        public List<IMockLoader> Loaders {
            get {
                return _Loaders;
            }
        }

        public MockLoader() : base("graphql.myshopify.com", "1234") {
            if (!Initialized) {
                Initialize();
            }
        }

        public override void Load(string query, LoaderResponseHandler callback) {
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

        public override void SetHeader(string key, string value) {
            // No-op
        }

        public override string SDKVariantName() {
            return "unity-test";
        }
    }
}
