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

Install the project's dependencies:
```
$ scripts/setup.sh
```

Write some features.

Add some tests and make your change. Re-run the tests with:
```bash
$ scripts/build.sh
$ scripts/test_unity.sh
```

## Documentation
If your change affects how people use the project (i.e. adding or changing arguments to a function, adding a new function, 
changing the return value, etc), please ensure the documentation is also updated to reflect this. Documentation is in the `docs/` folder
if further documentation is needed please communicate via Github Issues.
