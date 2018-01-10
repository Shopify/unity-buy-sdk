namespace Shopify.UIToolkit {
    public interface IShopCredentials {
        ShopCredentialsVerificationState CredentialsVerificationState { get; set; }

        string GetShopDomain();
        string GetAccessToken();
    }
}
