namespace Shopify.Tests {
    using Shopify.Unity.SDK;

    public interface IMockLoader {
        bool DoesHandleQueryResponse(string query);
        void HandleResponse(string query, LoaderResponseHandler callback);
    }
}