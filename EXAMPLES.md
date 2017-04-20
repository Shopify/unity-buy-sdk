# Examples

This guide contains code examples that show different ways that you can use the Unity Buy SDK.

The Unity Buy SDK queries Shopify's [Storefront API](https://help.shopify.com/api/storefront-api), which is a [GraphQL](http://graphql.org) API. GraphQL APIs accept queries that define the data that you want to retrieve. The Unity Buy SDK lets you query various data from Shopify, including store information, checkout URLs, products, and collections.

### Before you begin

Before you can start using the Unity Buy SDK, you need:

- a Shopify store with at least one product
- [a storefront access token for your app](https://help.shopify.com/api/storefront-api/getting-started#obtaining-a-storefront-access-token)
- [to install the Unity Buy SDK into your Unity project](https://github.com/shopify/unity-buy-sdk#using-the-unity-buy-sdk-in-unity)

### Initialize the SDK

This code example initializes the SDK. The `ShopifyBuy.Init` method takes two arguments. The first is a storefront access token to communicate with the Storefront API. The second is the domain name of your shop.

```cs
string accessToken = "351c122017d0f2a957d32ae728ad749c";
string shopDomain = "graphql.myshopify.com";

ShopifyBuy.Init(accessToken, shopDomain);
```

After you initialize the SDK, you can use `ShopifyBuy.Client()` to query Shopify. You need to initialize the SDK only once.

### Query all products

The following example shows how to query all products in your Shopify store:

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "351c122017d0f2a957d32ae728ad749c";
    string shopDomain = "graphql.myshopify.com";

    // Init only needs to be called once
    ShopifyBuy.Init(accessToken, shopDomain);

    // the following will query the shop for all products
    ShopifyBuy.Client().products((products, errors, httpError) => {
        // httpError is actually Unity's WWW.error
        if (httpError != null) {
            Debug.Log("There was an HTTP problem communicating with the API");
            return;
        // it's unlikely but if an invalid GraphQL query was sent a List of errors will be returned
        } else if(errors != null) {
            Debug.Log("There was an error with the graphql query sent");
            return;
        }

        // products is a List<Product>
        Debug.Log("Your shop has " + products.Count + " products");
        Debug.Log("==================================================");

        foreach(Product product in products) {
            Debug.Log("Product Title: " + product.title());
            Debug.Log("Product Description: " + product.descriptionHtml());
            Debug.Log("--------");
        }
    });
}
```

### Query all collections

The following example shows how to query all collections in your Shopify store:

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "351c122017d0f2a957d32ae728ad749c";
    string shopDomain = "graphql.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    // will query all collections on shop
    ShopifyBuy.Client().collections((collections, errors, httpError) => {
        if (httpError != null) {
            Debug.Log("There was an HTTP problem communicating with the API");
            return;
        } else if(errors != null) {
            Debug.Log("There was an error with the graphql query sent");
            return;
        }

        // collections is a List<Collection>
        Debug.Log("Your shop has " + collections.Count + " collections");
        Debug.Log("==================================================");

        foreach(Collection collection in collections) {
            Debug.Log("Collection title: " + collection.title());
            Debug.Log("Collection updated at: " + collection.updatedAt());

            List<Product> products = (List<Product>) collection.products();

            foreach(Product product in products) {
                Debug.Log("Collection contains a product with the following id: " + product.id());
            }
        }
    });
}
```

In this example, if you called `product.title()` then an exception would be thrown. Since the Unity Buy SDK is built on GraphQL queries, when `collections` are queried using this method only `id` is queried on `Product`. To learn which fields are queried using this method, see `DefaultQueries.cs`.

You can also make more complex requests using custom queries (see below).

### Cart and web checkout

The following example shows how to create a cart, add line items to the cart using product variants, and then create a web checkout link that will open in the player's browser.

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "351c122017d0f2a957d32ae728ad749c";
    string shopDomain = "graphql.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    ShopifyBuy.Client().products((products, errors, httpError) => {
        Cart cart = ShopifyBuy.Client().Cart();

        List<ProductVariant> firstProductVariants = (List<ProductVariant>) products[0].variants();
        List<ProductVariant> secondProductVariants = (List<ProductVariant>) products[1].variants();
        ProductVariant firstProductFirstVariant = firstProductVariants[0];
        ProductVariant secondProductFirstVariant = secondProductVariants[0];

        // the following example adds a line item using the first products first variant
        // in this case the cart will have 3 copies of the variant
        cart.LineItems.AddOrUpdate(firstProductFirstVariant, 3);

        // AddOrUpdate is overloaded to accept ProductVariant's or strings or select a variant based on selected options
        // alternately you can use a product variant id string to create line items
        // this example adds 1 item to the cart
        cart.LineItems.AddOrUpdate(secondProductFirstVariant.id(), 1);

        // the following line will get a checkout url using the above line items and open the url in browser
        Application.OpenURL(cart.GetWebCheckoutLink());
    });
}
```

If you want to adjust or change quantity on a line item, then you can call `AddOrUpdate` again with the new quantity. For example, if you want to adjust the quantity for `firstProductFirstVariant` to 1 in the code above, then you could call:

```cs
cart.LineItems.AddOrUpdate(firstProductFirstVariant, 1);
```

To delete a line item do the following:
```cs
cart.LineItems.Delete(firstProductFirstVariant);
```

To get a line item do the following:
```cs
LineItemInput lineItem = cart.LineItems.Get(firstProductFirstVariant);
```

### Cart line items based on selected options

In Shopify, a product can have many options. These options map to **variants** of a product. The following example shows how to create line items in a cart based on selected product variants:

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "351c122017d0f2a957d32ae728ad749c";
    string shopDomain = "graphql.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    ShopifyBuy.Client().products((products, errors, httpError) => {
        Cart cart = ShopifyBuy.Client().Cart();

        Product firstProduct = products[0];

        List<ProductOption> options = firstProduct.options();

        // we will output all options for this product
        Debug.Log("Options:\n-------");

        foreach(ProductOption option in options) {
            // Options have possible values
            foreach(string value in option.values()) {
                // Each option has a name such as Color and multiple values for color
                Debug.Log(option.name() + ": " + value);
            }
        }

        Debug.Log("-------");

        // the following will create a dictionary where keys are option names and values are option values
        // you may for instance have drop downs for the user to select options from
        Dictionary<string,string> selectedOptions = new Dictionary<string,string>() {
            {"Size", "Large"},
            {"Color", "Green"}
        };

        // create a line item based on the selected options for a product
        cart.LineItems.AddOrUpdate(firstProduct, selectedOptions, 1);

        // checkout the selected product
        Application.OpenURL(cart.GetWebCheckoutLink());
    });
}
```

If you want to `Delete` or `Get` a line item, then use the following:

```cs
cart.LineItems.Get(firstProduct, selectedOptions);
cart.LineItems.Delete(firstProduct, selectedOptions);
```

### Custom queries

The Unity Buy SDK is built on top of Shopify's [Storefront API](https://help.shopify.com/api/storefront-api), which is a GraphQL Web API. In GraphQL, you send queries to the endpoint and receive back a JSON responses.

The following example shows a GraphQL query that retrieves a store's name and some details about its billing address:

```graphql
query {
  shop {
      name
      billingAddress {
          city
          address1
      }
  }
}
```

When this query is sent to the Storefront API, the JSON response from the server looks like this:

```json
{
  "data": {
    "shop": {
      "name": "graphql",
      "billingAddress": {
        "city": "Toronto",
        "address1": "80 Spadina Ave"
      }
    }
  }
}
```

As you can see, the data output takes the same form as the query sent to the GraphQL endpoint. To learn more about how GraphQL handles queries and responses, see [graphql.org](http://graphql.org).

In the previous example, the queries for `products` and `collections` were made using client-side utility functions that create generic queries for the most common types of information. But you can also send custom queries to the Storefront API to access additional data. To learn more about how the Storefront API works, see [the Storefront API documentation](https://help.shopify.com/api/storefront-api).

The following example shows how to build a custom query in C# that matches the GraphQL query mentioned above. It retrieves the same information that was queried in the previous example:

- the shop's `name`
- the shop's `billingAddress`
    + `city`
    + `address1`

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "351c122017d0f2a957d32ae728ad749c";
    string shopDomain = "graphql.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    // the following will build a custom query
    // this example uses named parameters but these could be omitted
    ShopifyBuy.Client().Query(
        // pass a lambda expression to 'buildQuery'
        // the lambda will receive a QueryRootQuery instance
        buildQuery: (query) => query
            .shop(shopQuery => shopQuery
                .name()
                .billingAddress(mailingAddressQuery => mailingAddressQuery
                    .address1()
                    .city()
                )
            ),
        callback: (result, errors, httpError) => {
            // result is a QueryRoot instance
            Debug.Log("Shop name: " + result.shop().name());
            Debug.Log("Shop city: " + result.shop().billingAddress().city());
        }
    );
}
```

To create mutations, you can use `ShopifyBuy.Client().Mutation` in a similar way.
