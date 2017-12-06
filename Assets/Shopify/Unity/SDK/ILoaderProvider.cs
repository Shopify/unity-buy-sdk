namespace Shopify.Unity.SDK {
    public interface ILoaderProvider {
        BaseLoader GetLoader(string accessToken, string domain);
    }
}