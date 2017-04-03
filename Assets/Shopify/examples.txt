# Examples

This file contains small code snippets which document different use cases for the Unity Buy SDK. The Unity Buy SDK
queries Shopify's Storefront API. The Storefront API is a GraphQL API. GraphQL API's accept GraphQL queries
which define what data the developer would like to access.

### Initialize the SDK

This code example initializes the SDK. The `ShopifyBuy.Init` method takes two `strings`. The first
`string` being an access token to communicate with the Storefront API. The second string is the 
domain name of your shop.
```cs
string accessToken = "351c122017d0f2a957d32ae728ad749c";
string shopDomain = "graphql.myshopify.com";

ShopifyBuy.Init(accessToken, shopDomain);
```
Once initialized you'll be using `ShopifyBuy.Client()` to query Shopify. Initialization only needs to happen
once.


### Query All Products

The following example shows how to query all products in your Shopify store.
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

### Query All Collections

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

It should be noted that in the above example if you were to call `product.title()` an Exception would be thrown. As stated 
in the begining of this document the Unity SDK is built on top of GraphQL queries. When `collections` are queried using
this helper/utility method only `id` is queried on `Product`. To get an idea of what fields are queried checkout `DefaultQueries.cs`.
We will discuss creating custom queries later.

### Cart + Web Checkout

The following example will create a cart, add line items to the cart using product variants, and create
a webcheckout link which will be opened in the Browser.

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

To adjust or change quantity on a line item you could simply just call `AddOrUpdate` again with the new quantity. For instance
from the above example if we wanted to adjust the quantity for `firstProductFirstVariant` to 1 we could call:
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

### Cart Line items based on selected options

In Shopify a product may have many options. These options map to variants of a product. The following
example shows how to create Line items in a Cart based on selected options.

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

If you'd like to `Delete` or `Get` a line item using do the following:
```cs
cart.LineItems.Get(firstProduct, selectedOptions);
cart.LineItems.Delete(firstProduct, selectedOptions);
```

### Custom Queries

The Unity Buy SDK is built ontop of the Storefront API which is a GraphQL Web API. In GraphQL you send queries
to the end point and receive back a JSON responses. Here's an example GraphQL query to get the name of a shop, and
details about the billing address of the store:

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

When this query is sent to the Storefront API the JSON response from the server would look like this:
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
As you can see the data takes the same form as the query sent to the GraphQL endpoint. For more info on GraphQL 
checkout: [http://graphql.org/](http://graphql.org/)

In the previous examples when querying `products` and `collections` we were actually using utility functions on
the Shopify Buy client that generate generic queries that query data most people would expect to see on 
products and collections. It is possible to however build custom queries that can be sent to the Storefront API
to access additional data.

In the following example we will build a query in C# which matches the GraphQL query mentioned above. Again we'll be querying:
- The shops `name`
- The shops `billingAddress`
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

To create mutations you'd use `ShopifyBuy.Client().Mutation` in a similar fashion.