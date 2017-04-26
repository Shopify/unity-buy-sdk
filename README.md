# Unity Buy SDK

The Unity Buy SDK allows Unity developers to query and sell products from Shopify directly in Unity.

## Table of contents

- [Features](#features)
- [Documentation](#documentation)
- [Examples](#examples)
- [Using the Unity Buy SDK in Unity](#using-the-unity-buy-sdk-in-unity)
- [Getting set up for contributing](#getting-set-up-for-contributing)
- [Build the GraphQL client](#build-the-unity-buy-sdk)
- [Testing](#testing)
    + [Testing in Unity](#testing-in-unity)
    + [Testing in Terminal](#testing-in-terminal-using-headless-unity)

## Features

- Query products
- Query collections
- Create a cart
- Check out via weblink
- Make custom GraphQL queries using the [Storefront API](https://help.shopify.com/api/storefront-api)

Coming Soon:

- Checkouts via Apple Pay and Android Pay

## Documentation

- [Information about the Unity SDK](https://help.shopify.com/api/sdks/custom-storefront/unity-buy-sdk)
- [How to obtain a storefront access token](https://help.shopify.com/api/storefront-api/getting-started#obtaining-a-storefront-access-token)
- [SDK API Documentation](https://shopify.github.io/unity-buy-sdk/)
- [Example code snippets](EXAMPLES.md)

## Using the Unity Buy SDK in Unity

By following the steps below you'll install the Unity Buy SDK into your Unity project using the [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage):

1. Download the [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage).
2. Make sure that your project is open in Unity.
3. Open the downloaded [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage).

Alternatively, you can clone this repo and build the Unity Buy SDK by following the documentation below. After building the SDK, you can copy `Assets/Shopify/` to your project. `Assets/Shopify/` contains all the source files that you need to run the Unity Buy SDK.

## Getting set up for contributing

Before contributing to the Unity Buy SDK, make sure that you have the required dependencies installed on your computer. Run the following command to check that you have all required applications:
```bash
$ scripts/check_setup.sh
```

## Build the Unity Buy SDK

Under the hood, the Unity Buy SDK works on top of GraphQL. In order to query Shopify, we generate a GraphQL client based on the [Storefront API](https://help.shopify.com/api-storefront-api) introspection schema.

To build the GraphQL client, run the following command:
```bash
$ scripts/build.sh
```

## Testing

### Testing in Unity

If you want to run tests in Unity, use Unity >5.3.0.

1. Open Unity.
2. Create a Unity project within this cloned folder.
3. Click on _Window > Editor Tests Runner_.
4. In the opened Editor Tests Runner click _"Run All"_.

### Testing in Terminal using Headless Unity

Headless Unity is basically Unity running Editor tests without the GUI. When running these tests, ensure that Unity is not open.

To run tests in Headless Unity run the following command:
```bash
$ scripts/test_unity.sh
```

### Testing in Terminal using Mono
There might be cases where you want to run tests fully outside of Unity.
That's where using Mono to run tests is handy. We're using Mono as it's the development platform on which Unity is built.

To run tests, run the following command:
```bash
$ scripts/test.sh
```

Alternatively, you might find it more efficient to run the following command:
```bash
$ scripts/test_watch.sh
```

`test_watch.sh` watches all development files for changes. When a change is detected, tests will be recompiled and run.


<img src="https://cdn.shopify.com/shopify-marketing_assets/builds/19.0.0/shopify-full-color-black.svg" width="200" />
