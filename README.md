![image](https://user-images.githubusercontent.com/12721181/29127322-c0acb984-7cee-11e7-97bd-e55f72af29aa.png) 

# Shopify SDK for Unity

The Shopify SDK for Unity allows Unity developers to query and sell products from Shopify directly in Unity.

## Table of contents

- [Features](#features)
- [Documentation](#documentation)
- [Using the Shopify SDK for Unity](#using-the-shopify-sdk-for-unity)
- [Supported build targets](#supported-build-targets)
- [Examples](#examples)

## Features

- [Query products](EXAMPLES.md#query-products)
- [Query collections](EXAMPLES.md#query-collections)
- [Build a cart](EXAMPLES.md#build-a-cart)
- [Check out via weblink](EXAMPLES.md#checkout-with-a-checkout-link)
- [Checkouts via Safari Web View](EXAMPLES.md#checkout-with-a-web-view)
- [Checkouts via Apple Pay](EXAMPLES.md#checkout-with-native-pay-apple-pay--android-pay)
- [Checkouts via Android Chrome Custom Tab](EXAMPLES.md#checkout-with-a-web-view)
- [Make custom GraphQL queries using the Storefront API](EXAMPLES.md#custom-queries)

Coming Soon:

- [Checkouts via Android Pay](EXAMPLES.md#checkout-with-native-pay-apple-pay--android-pay)

## Documentation

- [Example code snippets](EXAMPLES.md)
- [Information about the Unity SDK](https://help.shopify.com/api/sdks/custom-storefront/unity-buy-sdk)
- [How to obtain a storefront access token](https://help.shopify.com/api/storefront-api/getting-started#obtaining-a-storefront-access-token)
- [SDK API Documentation](https://shopify.github.io/unity-buy-sdk/)
- [How to contribute](CONTRIBUTING.md)
- [How to Test](TESTING.md)

## Using the Shopify SDK for Unity

By following the steps below you'll install the Shopify SDK for Unity into your Unity project using the [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage):

1. Download the latest release of the [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/releases/latest)
2. Make sure that your project is open in Unity
3. Open the downloaded `shopify-buy.unitypackage`

When you are ready to build for a specific platform, please read the [Supported build targets](#supported-build-targets) section to know how to configure your `Player Settings` and `Build Settings` in Unity

## Supported build targets

The Shopify SDK for Unity should work on all platforms which support Unity's `Application.OpenURL`. However we've made it much easier for users to checkout on iOS and Android. Below we list out more details about each of these platforms

### iOS

The Shopify SDK for Unity requires iOS applications to be built using the Xcode 9 and above, have a minimum target SDK of iOS 9 and support Swift 3.

To target iOS see the [iOS Build Details](BUILDDETAILS.md#ios)

### Android

The Shopify SDK for Unity requires Android applications to have a minimum API level of Android 4.4 'Kit Kat' (API Level 19).

To target Android see the [Android Build Details](BUILDDETAILS.md#android)

## Examples
Checkout our [Example Guide](EXAMPLES.md).

<img src="https://cdn.shopify.com/shopify-marketing_assets/builds/19.0.0/shopify-full-color-black.svg" width="200" />
