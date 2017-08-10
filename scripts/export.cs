using System;
using Shopify.Unity;
using Shopify.Unity.GraphQL;
using Shopify.Unity.SDK;
 
public class ExportDefaultQueries
{
    static public void Main ()
    {
        QueryRootQuery query = new QueryRootQuery();
        DefaultQueries.products.ShopProducts(query, ShopifyClient.DefaultImageResolutions);

        Console.WriteLine(query.ToString());
    }
}
