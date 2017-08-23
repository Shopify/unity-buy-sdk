# Examples

This guide contains code examples that show different ways that you can use the Unity Buy SDK.

The Unity Buy SDK queries Shopify's [Storefront API](https://help.shopify.com/api/storefront-api), which is a [GraphQL](http://graphql.org) API. GraphQL APIs accept queries that define the data that you want to retrieve. The Unity Buy SDK lets you query various data from Shopify, including store information, checkout URLs, products, and collections.

## Table of contents

- [Before you begin](#before-you-begin)
- [Using the SDK on iOS](#using-the-sdk-on-ios)
- [Initialize the SDK](#initialize-the-sdk)
- [Query all products](#query-all-products)
- [Query all collections](#query-all-collections)
- [Build a cart](#build-a-cart)
- [Native web view checkout](#native-web-view-checkout)
- [Apple Pay checkout](#apple-pay-checkout)
- [Custom queries](#custom-queries)

## Before you begin

Before you can start using the Unity Buy SDK, you need:

- a Shopify store with at least one product
- [a storefront access token for your app](https://help.shopify.com/api/storefront-api/getting-started#obtaining-a-storefront-access-token)
- [to install the Unity Buy SDK into your Unity project](README.md#using-the-unity-buy-sdk-in-unity)

## Supported build targets

See [Build Target Requirements](BUILDTARGETS.md) for information on setting up the SDK on iOS and Android

## Initialize the SDK

This code example initializes the SDK. The `ShopifyBuy.Init` method takes two arguments. The first is a storefront access token to communicate with the Storefront API. The second is the domain name of your shop.

```cs
string accessToken = "b8d417759a62f7b342f3735dbe86b322";
string shopDomain = "unity-buy-sdk.myshopify.com";

ShopifyBuy.Init(accessToken, shopDomain);
```

After you initialize the SDK, you can use `ShopifyBuy.Client()` to query Shopify. You need to initialize the SDK only once.

## Query all products

The following example shows how to query all products in your Shopify store:

```cs
using Shopify.Unity;
using Shopify.Unity.SDK;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    // Init only needs to be called once
    ShopifyBuy.Init(accessToken, shopDomain);

    // The following queries the shop for all products
    ShopifyBuy.Client().products((products, error) => {
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
            // products is a List<Product>
            Debug.Log("Your shop has " + products.Count + " products");
            Debug.Log("==================================================");

            foreach(Product product in products) {
                Debug.Log("Product Title: " + product.title());
                Debug.Log("Product Description: " + product.descriptionHtml());
                Debug.Log("--------");
            }
        }
    });
}
```

## Query all collections

The following example shows how to query all collections in your Shopify store:

```cs
using Shopify.Unity;
using Shopify.Unity.SDK;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    // Queries all collections on shop
    ShopifyBuy.Client().collections((collections, error) => {
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
        }
    });
}
```

In this example, if you called `product.title()` then an exception would be thrown. Since the Unity Buy SDK is built on GraphQL queries, when `collections` are queried using this method only `id` is queried on `Product`. To learn which fields are queried using this method, see `DefaultQueries.cs`.

You can also make more complex requests using custom queries (see below).

## Build a cart

The following example shows how to create a cart and add line items to the cart using product variants:

```cs
using Shopify.Unity;
using Shopify.Unity.SDK;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    ShopifyBuy.Client().products((products, error) => {
        Cart cart = ShopifyBuy.Client().Cart();

        List<ProductVariant> firstProductVariants = (List<ProductVariant>) products[0].variants();
        ProductVariant firstProductFirstVariant = firstProductVariants[0];

        // The following example adds a line item using the first products first variant.
        // In this case, the cart will have 3 copies of the variant.
        cart.LineItems.AddOrUpdate(firstProductFirstVariant, 3);

        // AddOrUpdate is overloaded to accept either ProductVariants or strings, or to select a variant based on selected options.
        // Alternately, you can use a product variant id string to create line items.
        // This example updates the first product in the cart to have a quantity of 1.
        cart.LineItems.AddOrUpdate(firstProductFirstVariant.id(), 1);
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
using Shopify.Unity.SDK;

void Start () {
    string accessToken = "b8d417759a62f7b342f3735dbe86b322";
    string shopDomain = "unity-buy-sdk.myshopify.com";

    ShopifyBuy.Init(accessToken, shopDomain);

    ShopifyBuy.Client().products((products, error) => {
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
            {"Color", "White"}
        };

        // Create a line item based on the selected options for a product
        cart.LineItems.AddOrUpdate(firstProduct, selectedOptions, 1);

        // Checkout the selected product
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

If you want to `Delete` or `Get` a line item, then use the following:

```cs
cart.LineItems.Get(firstProduct, selectedOptions);
cart.LineItems.Delete(firstProduct, selectedOptions);
```

## Native web view checkout

After creating an instance of `Cart` and adding items to it, you can use the `CheckoutWithWebView` method to
start a native modal overlay on top of your game with a web view that contains the checkout for the cart.

`CheckoutWithWebView` takes in 3 callback parameters:

* `CheckoutSuccessCallback` is called when the user has completed a checkout successfully.
* `CheckoutCancelCallback` is called when the user cancels out of the checkout.
* `CheckoutFailureCallback` is called when an error was encountered during the web checkout. The callback will pass an instance of `ShopifyError` describing the issue.

```cs
// Sample code for adding some product variants to your cart.

...

ShopifyBuy.Client().products((products, error) => {
    var cart = ShopifyBuy.Client().Cart();
    var firstProduct = products[0];
    var firstProductVariants = (List<ProductVariant>) firstProduct.variants();
    ProductVariant productVariantToCheckout = firstProductVariants[0];

    cart.LineItems.AddOrUpdate(productVariantToCheckout, 1);

    // Launches the native web checkout experience overlaid on top of your game.
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

## Apple Pay checkout

You can allow users to pay with Apple Pay, providing a seamless checkout experience.

To determine whether the user is able to make a payment with Apple Pay, you can use the `CanCheckoutWithNativePay` method. If the user has this capability, then you can use `CheckoutWithNativePay` to present the Apple Pay authentication interface to the user. If they do not, then you can accept payment using [Native Web Checkout](#native-web-view-checkout).

`CheckoutWithNativePay` takes in 4 parameters:

* `key` is the Merchant ID of your application found on your [Apple Developer Portal](https://developer.apple.com/library/content/ApplePay_Guide/Configuration.html)
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

    // Check to see if the user can make a payment through Apple Pay
    cart.CanCheckoutWithNativePay((isNativePayAvailable) => {
        if (isNativePayAvailable) {
            cart.CheckoutWithNativePay(
                "com.merchant.id",
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

### Additional Build Settings

To use Apple Pay, you must also enable **Background fetch** in the Player Settings:

1. In Unity, open **Player Settings**, and then click *Edit* > *Project Settings* > *Player*.
2. Select **Settings for iOS**.
3. Under **Other Settings**, set **Behavior in Background** to **Custom**.
4. Enable **Background fetch**.

### Enabling Apple Pay on your Store

To enable Apple Pay for your store through an app:

1. Add the [Mobile App sales channel](https://help.shopify.com/api/sdks/custom-storefront/mobile-buy-sdk/add-mobile-app-sales-channel) in your Shopify admin.
2. Enable Apple Pay in the Mobile App settings page.

### Notes

`CheckoutWithNativePay` will throw an exception if the device is unable to make a payment through Apple Pay. So it is essential that `CanCheckoutWithNativePay` is used.

### Errors

On failure, you will receive a `ShopifyError`. There are 3 types of `ShopifyError` that you will have to handle:

* `HTTP`
* `NativePaymentProcessingError`
* `GraphQL`

`HTTP` errors will be thrown when there was an issue connecting or downloading to a required web server.

`NativePaymentProcessingError` will be thrown when Apple Pay fails to generate a token while trying to authenticate the user's card. This error is unrecoverable and you should fall back to a different payment method, or allow the user to try going through the process again.

`GraphQL` error will be thrown when there is something wrong with the SDK. This error is unrecoverable and you should fall back to a different payment method.

```csharp
cart.CheckoutWithNativePay(
    "com.merchant.id",
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

The Unity Buy SDK is built on top of Shopify's [Storefront API](https://help.shopify.com/api/storefront-api), which is a GraphQL Web API. In GraphQL, you send queries to the endpoint and receive back a JSON responses.

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
using Shopify.Unity.SDK;

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
