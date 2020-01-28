# Examples

This guide contains code examples that show different ways that you can use the Shopify SDK for Unity.

The Shopify SDK for Unity queries Shopify's [Storefront API](https://help.shopify.com/api/storefront-api), which is a [GraphQL](http://graphql.org) API. GraphQL APIs accept queries that define the data that you want to retrieve. The Shopify SDK for Unity lets you query various data from Shopify, including store information, checkout URLs, products, and collections.

## Table of contents

- [Before you begin](#before-you-begin)
- [Supported build targets](#supported-build-targets)
- [Initialize the SDK](#initialize-the-sdk)
- [Query Products](#query-products)
- [Query Collections](#query-collections)
- [Build a cart](#build-a-cart)
- [Build a cart based on selected options](#build-a-cart-based-on-selected-options)
- [Checkout with a checkout link](#checkout-with-a-checkout-link)
- [Checkout with a Web View](#checkout-with-a-web-view)
- [Checkout with Native Pay (Apple Pay)](#checkout-with-native-pay-apple-pay)
- [Custom queries](#custom-queries)

## Before you begin

Before you can start using the Shopify SDK for Unity, you need:

- a Shopify store with at least one product. If you do not have a Shopify store you have two options:
    + [Sign up as a Shopify Partner to create a test store](https://www.shopify.ca/partners)
    + Start testing the SDK using our test store. (domain and access token listed below)
- [a storefront access token for your app](https://help.shopify.com/api/storefront-api/getting-started#obtaining-a-storefront-access-token)
- [to install the Shopify SDK for Unity into your Unity project](https://github.com/Shopify/unity-buy-sdk#using-the-shopify-sdk-for-unity)

## Supported build targets

See [Build Target Requirements](README.md#supported-build-targets) for information on setting up the SDK on iOS and Android

## Initialize the SDK

This code example initializes the SDK. The `ShopifyBuy.Init` method takes two arguments. The first is a storefront access token to communicate with the Storefront API. The second is the domain name of your shop.

```cs
string accessToken = "b8d417759a62f7b342f3735dbe86b322";
string shopDomain = "unity-buy-sdk.myshopify.com";

ShopifyBuy.Init(accessToken, shopDomain);
```

After you initialize the SDK, you can use `ShopifyBuy.Client()` to query Shopify. You need to initialize the SDK only once.

## Supporting multiple languages

If your store supports multiple languages, then you can use the Storefront API to return translated content for supported resource types and fields.
Learn more about [translating content](https://help.shopify.com/en/api/guides/multi-language/translating-content-api).

To return translated content, include the `locale` parameter in `ShopifyBuy.Init`:

```cs
string accessToken = "b8d417759a62f7b342f3735dbe86b322";
string shopDomain = "unity-buy-sdk.myshopify.com";
string locale = "es";

ShopifyBuy.Init(accessToken, shopDomain, locale);
```

## Query Products

The following example shows how to query two pages of products in your Shopify store:

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    // Init only needs to be called once
    ShopifyBuy.Init(accessToken, shopDomain);

    // Queries one page of products
    ShopifyBuy.Client().products((products, error, after) => {
        if (error != null) {
            Debug.Log(error.Description);

            switch(error.Type) {
            // An HTTP error is actually Unity's WWW.error
            case ShopifyError.ErrorType.HTTP:
                break;
                // Although it's unlikely, an invalid GraphQL query might be sent.
                // Report an issue to https://github.com/shopify/unity-buy-sdk/issues
            case ShopifyError.ErrorType.GraphQL:
                break;
            };
        } else {
            Debug.Log("Here is the first page of products:");

            // products is a List<Product>
            foreach(Product product in products) {
                Debug.Log("Product Title: " + product.title());
                Debug.Log("Product Description: " + product.descriptionHtml());
                Debug.Log("--------");
            }

            if (after != null) {
                Debug.Log("Here is the second page of products:");

                // Queries second page of products, as after is passed
                ShopifyBuy.Client().products((products2, error2, after2) => {
                    foreach(Product product in products2) {
                        Debug.Log("Product Title: " + product.title());
                        Debug.Log("Product Description: " + product.descriptionHtml());
                        Debug.Log("--------");
                    }
                }, after: after);
            } else {
                Debug.Log("There was only one page of products.");
            }
        }
    });
}
```

## Query Collections

The following example shows how to query one page of collections in your Shopify store:

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    // Queries one page of collections
    ShopifyBuy.Client().collections((collections, error, after) => {
        if (error != null) {
            Debug.Log(error.Description);

            switch(error.Type) {
            // An HTTP error is actually Unity's WWW.error
            case ShopifyError.ErrorType.HTTP:
                break;
            // It's unlikely but it may be that an invalid GraphQL query was sent.
            // Report an issue to https://github.com/shopify/unity-buy-sdk/issues
            case ShopifyError.ErrorType.GraphQL:
                break;
            };

        } else {
            Debug.Log("Loaded the first page of collections:");

            // collections is a List<Collection>
            foreach(Collection collection in collections) {
                Debug.Log("Collection title: " + collection.title());
                Debug.Log("Collection updated at: " + collection.updatedAt());

                List<Product> products = (List<Product>) collection.products();

                foreach(Product product in products) {
                    Debug.Log("Collection contains a product with the following id: " + product.id());
                }
            }

            if (after != null) {
                Debug.Log("We also have a second page of products");
            } else {
                Debug.Log("We don't have a second page of products");
            }
        }
    });
}
```

In this example, if you called `product.title()` then an exception would be thrown. Since the Shopify SDK for Unity is built on GraphQL queries, when `collections` are queried using this method only `id` is queried on `Product`. To learn which fields are queried using this method, see `DefaultQueries.cs`.

You can also make more complex requests using [custom queries](#custom-queries).

## Build a cart

The following example shows how to create a cart and add line items to the cart using product variants:

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    ShopifyBuy.Client().products((products, error, after) => {
        Cart cart = ShopifyBuy.Client().Cart();

        List<ProductVariant> firstProductVariants = (List<ProductVariant>) products[0].variants();
        ProductVariant firstProductFirstVariant = firstProductVariants[0];

        // The following example adds a line item using the first products first variant.
        // In this case, the cart will have 3 copies of the variant.
        cart.LineItems.AddOrUpdate(firstProductFirstVariant, 3);

        // The following will output the variant id which was setup using the first product variant
        Debug.Log("First line item's variant id is: " + cart.LineItems.All()[0].VariantId);
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

### Build a cart based on selected options

In Shopify, a product can have many options. These options map to **variants** of a product. The following example shows how to create line items in a cart based on selected product variants:

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    ShopifyBuy.Client().products((products, error, after) => {
        Cart cart = ShopifyBuy.Client().Cart();

        Product firstProduct = products[0];

        List<ProductOption> options = firstProduct.options();

        // Output all options for this product
        Debug.Log("Options:\n-------");

        foreach(ProductOption option in options) {
            // Options have possible values
            foreach(string value in option.values()) {
                // Each option has a name such as Color and multiple values for Color
                Debug.Log(option.name() + ": " + value);
            }
        }

        Debug.Log("-------");

        // The following creates a dictionary where keys are option names and values are option values.
        // You might, for instance, have drop downs where users can select relevant options.
        Dictionary<string,string> selectedOptions = new Dictionary<string,string>() {
            {"Size", "M"},
            {"Color", "Ash"}
        };

        // Create a line item based on the selected options for a product
        cart.LineItems.AddOrUpdate(firstProduct, selectedOptions, 1);

        // The following will output the variant id which was selected by the above options
        Debug.Log("First line item's variant id is: " + cart.LineItems.All()[0].VariantId);
    });
}
```

If you want to `Delete` or `Get` a line item, then use the following:

```cs
cart.LineItems.Get(firstProduct, selectedOptions);
cart.LineItems.Delete(firstProduct, selectedOptions);
```

## Checkout with a checkout link

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    ShopifyBuy.Client().products((products, error, after) => {
        Cart cart = ShopifyBuy.Client().Cart();

        List<ProductVariant> firstProductVariants = (List<ProductVariant>) products[0].variants();
        ProductVariant firstProductFirstVariant = firstProductVariants[0];

        // The following example adds a line item using the first products first variant.
        // In this case, the cart will have 3 copies of the variant.
        cart.LineItems.AddOrUpdate(firstProductFirstVariant, 3);

        // Checkout with the url in the Device Browser
        cart.GetWebCheckoutLink(
            success: (link) => {
                Application.OpenURL(link);
            },
            failure: (checkoutError) => {
                Debug.Log(checkoutError.Description);
            }
        );
    });
}
```

## Checkout with a Web View

After creating an instance of `Cart` and adding items to it, you can use the `CheckoutWithWebView` method to
start a web view that contains the checkout for the cart.

`CheckoutWithWebView` takes in 3 callback parameters:

* `CheckoutSuccessCallback` is called when the user has completed a checkout successfully.
* `CheckoutCancelCallback` is called when the user cancels out of the checkout.
* `CheckoutFailureCallback` is called when an error was encountered during the web checkout. The callback will pass an instance of `ShopifyError` describing the issue.

```cs
// Sample code for adding some product variants to your cart.

...

ShopifyBuy.Client().products((products, error, after) => {
    var cart = ShopifyBuy.Client().Cart();
    var firstProduct = products[0];
    var firstProductVariants = (List<ProductVariant>) firstProduct.variants();
    ProductVariant productVariantToCheckout = firstProductVariants[0];

    cart.LineItems.AddOrUpdate(productVariantToCheckout, 1);

    // Launches the web view checkout experience overlaid on top of your game.
    cart.CheckoutWithWebView(
        success: () => {
            Debug.Log("User finished purchase/checkout!");
        },
        cancelled: () => {
            Debug.Log("User cancelled out of the web checkout.");
        },
        failure: (e) => {
            Debug.Log("Something bad happened - Error: " + e);
        }
    );
});
```

## Checkout with Native Pay (Apple Pay)

You can allow users to pay with Apple Pay, depending on the device OS, providing a seamless checkout experience.

To determine whether the user is able to make a payment with Native Pay, you can use the `CanCheckoutWithNativePay` method. If the user has this capability, then you can use `CheckoutWithNativePay` to present the native pay authentication interface to the user. If they do not, then you can accept payment using [Web View Checkout](#checkout-with-a-web-view).

`CheckoutWithNativePay` takes in 4 parameters:

* `key`
  * **On iOS** is the Merchant ID of your application found on your [Apple Developer Portal](https://developer.apple.com/library/content/ApplePay_Guide/Configuration.html).
* `CheckoutSuccessCallback` is called when the user has completed a checkout successfully.
* `CheckoutCancelCallback` is called when the user cancels out of the checkout.
* `CheckoutFailureCallback` is called when an error was encountered during the checkout. The callback will be passed an instance of `ShopifyError` describing the issue.


```csharp
// Sample code for adding some product variants to your cart.

...

ShopifyBuy.Client().products((products, error) => {
    var cart = ShopifyBuy.Client().Cart();
    var firstProduct = products[0];
    var firstProductVariants = (List<ProductVariant>) firstProduct.variants();
    ProductVariant productVariantToCheckout = firstProductVariants[0];

    cart.LineItems.AddOrUpdate(productVariantToCheckout, 1);

    var key = null;
    #if UNITY_IOS
    key = "com.merchant.id";
    #elif UNITY_ANDROID
    key = "BL9QiRljozDhgfyfVHoK+l1l98fBY0x/in0rCYJxmTfnzJDWsX1+8l4HEa4LO0WeKQlYtuk8zcJtzimTMhr1UL8=";
    #endif

    // Check to see if the user can make a payment through Apple Pay
    cart.CanCheckoutWithNativePay((isNativePayAvailable) => {
        if (isNativePayAvailable) {
            cart.CheckoutWithNativePay(
                key,
                success: () => {
                    Debug.Log("User finished purchase/checkout!");
                },
                cancelled: () => {
                    Debug.Log("User cancelled out of the native checkout.");
                },
                failure: (e) => {
                    Debug.Log("Something bad happened - Error: " + e);
                }
            );
        }
    });
});

```

### Additional build settings (Apple Pay only)

To use Apple Pay, you must also enable **Background fetch** in the Player Settings:

1. In Unity, open **Player Settings**, and then click *Edit* > *Project Settings* > *Player*.
2. Select **Settings for iOS**.
3. Under **Other Settings**, set **Behavior in Background** to **Custom**.
4. Enable **Background fetch**.

### Enabling native pay on your Store

To enable native pay for your store through an app:

1. Follow [these instructions](https://help.shopify.com/manual/apps/private-apps#generate-credentials-from-the-shopify-admin) to create a private app in your Shopify admin page.
1. Enable _Allow this app to access your storefront data using the Storefront API_ flag in your private app configuration.
### Notes

`CheckoutWithNativePay` will throw an exception if the device is unable to make a payment through Apple Pay. So it is essential that `CanCheckoutWithNativePay` is used.

### Errors

On failure, you will receive a `ShopifyError`. There are 3 types of `ShopifyError` that you will have to handle:

* `HTTP`
* `NativePaymentProcessingError`
* `GraphQL`

`HTTP` errors will be thrown when there was an issue connecting or downloading to a required web server.

`NativePaymentProcessingError`

This error is thrown whenever something goes wrong on the native pay side of the checkout. It is unrecoverable and you should fall back to a different payment method, or allow the user to try going through the process again. The reasons for this error to be thrown depend on the platform:

* **iOS:** will be thrown when Apple Pay fails to generate a token while trying to authenticate the user's card.

`GraphQL` error will be thrown when there is something wrong with the SDK. This error is unrecoverable and you should fall back to a different payment method.

```csharp
cart.CheckoutWithNativePay(
    key,
    success: () => {
       Debug.Log("User finished purchase/checkout!");
    },
    cancelled: () => {
        Debug.Log("User cancelled out of the native checkout.");
    },
    failure: (e) => {
        switch(e.Type) {
        case ShopifyError.ErrorType.HTTP:
            // Let the user know that there is no internet connection
            break;
        default:
            // Let the user know that checkout could not be completed
            // Fallback to Web Checkout
            break;
        }
    }
);
```

### Extras

You might want to optionally drive users to setup their payment cards with Apple Pay. You can do so by using `CanShowNativePaySetup` and `ShowNativePaySetup`. `CanShowNativePaySetup` lets you know whether the device supports this, and `ShowNativePaySetup` launches the native `Wallet` app prompting the user to set up his or her card.

## Custom queries

The Shopify SDK for Unity is built on top of Shopify's [Storefront API](https://help.shopify.com/api/storefront-api), which is a GraphQL Web API. In GraphQL, you send queries to the endpoint and receive back a JSON responses.

The following example shows a GraphQL query that retrieves a store's name and some details about its domain address:

```graphql
query {
  shop {
    name
    primaryDomain {
      url
      host
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
      "primaryDomain": {
        "url": "https://unity-buy-sdk.myshopify.com",
        "host": "unity-buy-sdk.myshopify.com"
      }
    }
  }
}
```

As you can see, the data output takes the same form as the query sent to the GraphQL endpoint. To learn more about how GraphQL handles queries and responses, see [graphql.org](http://graphql.org).

In the previous example, the queries for `products` and `collections` were made using client-side utility functions that create generic queries for the most common types of information. But you can also send custom queries to the Storefront API to access additional data. To learn more about how the Storefront API works, see [the Storefront API documentation](https://help.shopify.com/api/storefront-api).

The following example shows how to build a custom query in C# that matches the GraphQL query mentioned above. It retrieves the same information that was queried in the previous example:

- the shop's `name`
- the shop's `primaryDomain`
    + `url`
    + `host`

```cs
using Shopify.Unity;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    // The following builds a custom query.
    // This example uses named parameters but these could be omitted.
    ShopifyBuy.Client().Query(
        // Pass a lambda expression to 'buildQuery'
        // The lambda receives a QueryRootQuery instance.
        buildQuery: (query) => query
            .shop(shopQuery => shopQuery
                .name()
                .primaryDomain(primaryDomainQuery => primaryDomainQuery
                    .url()
                    .host()
                )
            ),
        callback: (result, error) => {
            // Results in a QueryRoot instance
            Debug.Log("Shop name: " + result.shop().name());
            Debug.Log("Shop url: " + result.shop().primaryDomain().url());
        }
    );
}
```

To create mutations, you can use `ShopifyBuy.Client().Mutation` in a similar way.
