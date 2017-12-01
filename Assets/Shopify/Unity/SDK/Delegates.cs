namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using Shopify.Unity.GraphQL;

    public delegate QueryRootQuery BuildQueryOnConnectionLoopDelegate(QueryResponse response = null);
    public delegate object GetConnectionFromResponseDelegate(object response);
    public delegate void BuildQueryOnNodeDelegate(QueryRootQuery query, List<ConnectionQueryInfo> connectionInfosToBuildQuery, string nodeId, string alias);
    public delegate void BuildQueryOnEdgesNodeDelegate(object node, string after);
    public delegate void ResponseNodeHandler(List<Node> nodes, ShopifyError error);

    public delegate void LoaderResponseHandler(string response, string error);

    public delegate void DictionaryChangeHandler();

    public delegate void QueryResponseHandler(QueryResponse response);
    public delegate void MutationResponseHandler(MutationResponse response);

    public delegate void MergeFieldDelegate(string field, Dictionary<string, object> into, Dictionary<string, object> responseA, Dictionary<string, object> responseB);

    public delegate void ProductsHandler(List<Product> products, ShopifyError error);
    public delegate void ProductsPaginatedHandler(List<Product> products, ShopifyError error, string after);
    public delegate void CollectionsPaginatedHandler(List<Collection> collections, ShopifyError error, string after);
    public delegate void QueryRootHandler(QueryRoot queryRoot, ShopifyError error);
    public delegate void MutationRootHandler(Mutation mutationRoot, ShopifyError error);

    public delegate void ShopMetadataHandler(ShopMetadata? metadata, ShopifyError error);

    public delegate void OnCartLineItemChange();

    public delegate bool PollUpdatedHandler(QueryRoot updatedQueryRoot);

    public delegate void CanCheckoutWithNativePayCallback(bool canCheckout);
    public delegate void CanShowNativePaySetupCallback(bool canShowPaymentSetup);
    public delegate void CheckoutSuccessCallback();
    public delegate void CheckoutFailureCallback(ShopifyError error);
    public delegate void CheckoutCancelCallback();

    public delegate void GetWebCheckoutLinkSuccessCallback(string url);
    public delegate void GetWebCheckoutLinkFailureCallback(ShopifyError error);

    public delegate void CompletionCallback(ShopifyError error);
}