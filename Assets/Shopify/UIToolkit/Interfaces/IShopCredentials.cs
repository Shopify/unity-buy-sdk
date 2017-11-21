namespace Shopify.UIToolkit {
    public interface IShopCredentials {
        ShopCredentialsVerificationState CredentialsVerificationState { get; set; }
        string ShopDomain { get; }
        string AccessToken { get; }
    }
}