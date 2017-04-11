# Unity Buy SDK

The Unity Buy SDK allows Unity developers to query and sell products within Unity.

## Table Of Contents

- [Using the Unity Buy SDK in Unity](#using-the-unity-buy-sdk-in-unity)
- [Getting Setup For Contributing](#getting-setup-for-contributing)
- [Build the GraphQL Client](#build-the-unity-buy-sdk)
- [Testing](#testing)
    + [Testing in Unity](#testing-in-unity)
    + [Testing in Terminal](#testing-in-terminal)

## Using the Unity Buy SDK in Unity

By following the steps below you'll install the Unity Buy SDK into your Unity project using the [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage):

1. Download the [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage)
2. Ensure your project is open in Unity
3. Open the downloaded [shopify-buy.unitypackage](https://github.com/Shopify/unity-buy-sdk/raw/master/shopify-buy.unitypackage)

Alternately you could clone this repo build the Unity Buy SDK by following the documentation below. After building the SDK
you can copy `Assets/Shopify/` to your project. `Assets/Shopify/` will contain all source needed to run the Unity Buy SDK.

## Getting Setup For Contributing

Before contributing to the Unity Buy SDK it might be handy to check that you have the required dependencies installed on your
computer. Run the following command to check you have all required applications:
```bash
$ scripts/check_setup.sh
```

## Build the Unity Buy SDK

Under the hood the Unity Buy SDK works on top of GraphQL. In order to query Shopify we generate a GraphQL client
based on the Storefront API introspection schema.

To build the GraphQL client run the following command:
```bash
$ scripts/build.sh
```

## Testing

### Testing in Unity
If you'd like to run tests in Unity use Unity >5.3.0. 

1. Open Unity
2. Create a Unity project within this cloned folder 
3. Click on _Window > Editor Tests Runner_
4. In the opened Editor Tests Runner click _"Run All"_

### Testing In Terminal Using Headless Unity
Headless Unity is basically Unity running Editor tests without the GUI.
When running these tests ensure that Unity is not open.

To run tests in Headless Unity run the following command:
```bash
$ scripts/test_unity.sh
```

### Testing In Terminal Using Mono
There might be cases where you want to run tests fully outside of Unity.
That's where using Mono to run tests is handy. We're using mono as it's the development platform on which Unity
is built on.

To run tests run the following command:
```bash
$ scripts/test.sh
```

<img src="https://cdn.shopify.com/shopify-marketing_assets/builds/19.0.0/shopify-full-color-black.svg" width="200" />