# Unity Buy SDK

The Unity Buy SDK allows Unity developers to query and sell products within Unity.

## Table Of Contents

- [Build the GraphQL Client](#build-the-graphql-client)
- [Testing](#testing)

## Build the GraphQL Client

Under the hood the Unity Buy SDK works on top of GraphQL. In order to query Shopify we generate a GraphQL client
based on the Store Front API introspection schema.

To build the GraphQL client run the following command:
```bash
$ scripts/build.sh
```

## Testing

To run tests run the following command:
```bash
$ scripts/test.sh
```

<img src="https://cdn.shopify.com/shopify-marketing_assets/builds/19.0.0/shopify-full-color-black.svg" width="200" />