namespace Shopify.UIToolkit {
    public interface IShopController {
        ShopCredentialsVerificationState CredentialsVerificationState { get; set; }

        string GetShopDomain();
        string GetAccessToken();

        void Load();
        void Unload();
    }
}
