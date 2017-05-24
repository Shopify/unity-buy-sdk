namespace Shopify.Tests {
    using System.Collections.Generic;
    using Shopify.Unity.SDK;

    public interface IMockLoader {
        bool DoesHandleQueryResponse(string query);
        void HandleResponse(string query, LoaderResponseHandler callback);
        List<string> GetQueries();
    }
}