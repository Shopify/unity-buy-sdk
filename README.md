# Unity Buy SDK

The Unity Buy SDK allows Unity developers to query and sell products within Unity.

## Table Of Contents

- [Build the GraphQL Client](#build-the-graphql-client)
- [Testing](#testing)
    + [Testing in Unity](#testing-in-unity)
    + [Testing in Terminal](#testing-in-terminal)

## Build the GraphQL Client

Under the hood the Unity Buy SDK works on top of GraphQL. In order to query Shopify we generate a GraphQL client
based on the Store Front API introspection schema.

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