# How to contribute
We ❤️ pull requests. If you'd like to fix a bug, contribute a feature or
just correct a typo, please feel free to do so, as long as you follow
our [Code of Conduct](https://github.com/Shopify/unity-buy-sdk/blob/master/CODE_OF_CONDUCT.md)
and the [Contributing Developer Certificate of Origin](https://github.com/Shopify/unity-buy-sdk/blob/master/CONTRIBUTING_DEVELOPER_CERTIFICATE_OF_ORIGIN.txt).

If you're thinking of adding a big new feature, consider opening an
issue first to discuss it to ensure it aligns to the direction of the
project (and potentially save yourself some time!).

## Getting Started
To start working on the codebase, first fork the repo, then clone it:
```
git clone git@github.com:your-username/unity-buy-sdk.git
```
*Note: replace "your-username" with your Github handle*

To check your the project's dependencies run:
```
$ scripts/check_setup.sh
```

## Build the Unity Buy SDK

Under the hood, the Unity Buy SDK works on top of GraphQL. In order to query Shopify, we generate a GraphQL client based on the [Storefront API](https://help.shopify.com/api-storefront-api) introspection schema.

To build the GraphQL client, run the following command:
```bash
$ scripts/build.sh
```

Now you are free to write some features.

## Testing

Read our [Testing Guide](TESTING.md)

Add some tests and make your change. Re-run the tests with:

```bash
$ scripts/build.sh
$ scripts/test_unity.sh

# If you made iOS plugin changes
$ scripts/test_ios.sh
```
## Creating your Pull Requests
We use the [beta](https://github.com/shopify/unity-buy-sdk/tree/beta) branch to hold new features and bug fixes before they are seen on [master](https://github.com/Shopify/unity-buy-sdk/tree/master). Please make your pull request to the [beta](https://github.com/shopify/unity-buy-sdk/tree/beta) branch.

## Documentation
If your change affects how people use the project (i.e. adding or changing arguments to a function, adding a new function, 
changing the return value, etc), please ensure the documentation is also updated to reflect this. Documentation is in the `docs/` folder
if further documentation is needed please communicate via Github Issues.
