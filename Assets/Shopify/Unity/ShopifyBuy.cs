namespace Shopify.Unity {
    using System.Collections.Generic;
    using System.Collections;
    using System.Text;
    using System;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;

    /// <summary>
    /// Is the main entry point for the Shopify SDK for Unity. Using <see ref="ShopifyBuy.Client" /> you'll
    /// create a client which can create queries on the Storefront API. <see ref="ShopifyClient">ShopifyClient </see>
    /// can also be used to create a <see ref="Cart">Cart </see>.
    /// </summary>
    public class ShopifyBuy {
#if (SHOPIFY_TEST)
        public static void Reset () {
            DefaultClient = null;
            ClientByDomain = new Dictionary<string, ShopifyClient> ();
        }
#endif

        /// <summary>
        /// Returns the default <see ref="ShopifyClient">ShopifyClient </see>. The default client is the first client created after
        /// calling  <see ref="ShopifyBuy.Init">ShopifyBuy.Init </see>. Note that <see ref="ShopifyBuy.Init">ShopifyBuy.Init </see>
        /// must be called before trying to call this function.
        /// </summary>
        /// \code
        /// // Example usage querying all products using `Client`
        /// ShopifyBuy.Client().products((products, error) => {
        /// 	Debug.Log(products[0].title());
        /// 	Debug.Log(products[1].title());
        /// 	Debug.Log(products.Count);
        /// });
        /// \endcode
        public static ShopifyClient Client () {
            return DefaultClient;
        }

        /// <summary>
        /// Returns a <see ref="ShopifyClient">ShopifyClient </see> for the <c>domain<c> passed in. This is useful
        /// when you have multiple Shops your application is querying. Note that
        /// <see ref="ShopifyBuy.Init">ShopifyBuy.Init </see> must be called before trying to call this function.
        /// </summary>
        /// <param name="domain">the domain associated to a client</param>
        /// \code
        /// // Example usage querying all products using `Client` and a domain
        /// ShopifyBuy.Client("unity-buy-sdk.myshopify.com").products((products, error) => {
        /// 	Debug.Log(products[0].title());
        /// 	Debug.Log(products[1].title());
        /// 	Debug.Log(products.Count);
        /// });
        /// \endcode
        public static ShopifyClient Client (string domain) {
            if (ClientByDomain.ContainsKey (domain)) {
                return ClientByDomain[domain];
            } else {
                return null;
            }
        }

        /// <summary>
        /// Initializes a <see ref="ShopifyClient">ShopifyClient </see> that can be used to query the Storefront API.
        /// The default client which can be accessed by calling <see ref="ShopifyBuy.Client">ShopifyBuy.Client() </see> which is
        /// initialized after <see ref="ShopifyBuy.Init">ShopifyBuy.Init </see> is called. To access the client for the specific
        /// domain passed, use <see ref="ShopifyBuy.Client">ShopifyBuy.Client(domain) </see>.
        /// </summary>
        /// <param name="accessToken">access token that was generated for your store</param>
        /// <param name="domain">domain of your Shopify store</param>
        /// \code
        /// // Example that initializes the Shopify SDK for Unity
        /// string accessToken = "b8d417759a62f7b342f3735dbe86b322";
        /// string shopDomain = "unity-buy-sdk.myshopify.com";
        ///
        /// // Init only needs to be called once
        /// ShopifyBuy.Init(accessToken, shopDomain);
        /// \endcode
        public static ShopifyClient Init (string accessToken, string domain) {
            if (!ClientByDomain.ContainsKey (domain)) {
                ClientByDomain[domain] = new ShopifyClient (accessToken, domain);

                if (DefaultClient == null) {
                    DefaultClient = ClientByDomain[domain];
                }
            }

            return ClientByDomain[domain];
        }

        /// <summary>
        /// Typically not used, but it is possible to create a <see ref="ShopifyClient">ShopifyClient </see> by passing an instance
        /// that implements <see ref="BaseLoader">BaseLoader </see>. <see ref="BaseLoader">BaseLoaders </see> handle network communicationication with
        /// the Storefront API. This functionality is useful if you'd like to use the Shopify SDK for Unity in a C# environment
        /// outside of Unity. The <c>domain</c> string used to access specific initialized clients is inferred from
        /// <see ref="BaseLoader">BaseLoader.Domain</see> which can be used to request a specific client.
        /// </summary>
        /// <param name="loader">a loader which will handle network communicationication with the Storefront API</param>
        /// \code
        /// // Mock example using a custom loader for another C# platform
        /// string accessToken = "b8d417759a62f7b342f3735dbe86b322";
        /// string shopDomain = "unity-buy-sdk.myshopify.com";
        ///
        /// CustomLoaderForNonUnityPlatform loader = new CustomLoaderForNonUnityPlatform(accessToken, shopDomain);
        ///
        /// // Init only needs to be called once
        /// ShopifyBuy.Init(loader);
        /// \endcode
        public static ShopifyClient Init (BaseLoader loader) {
            string domain = loader.Domain;

            if (!ClientByDomain.ContainsKey (domain)) {
                ClientByDomain[domain] = new ShopifyClient (loader);

                if (DefaultClient == null) {
                    DefaultClient = ClientByDomain[domain];
                }
            }

            return ClientByDomain[domain];
        }

        private static ShopifyClient DefaultClient;
        private static Dictionary<string, ShopifyClient> ClientByDomain = new Dictionary<string, ShopifyClient> ();
    }

    /// <summary>
    /// <see cref="ShopifyClient">ShopifyClient </see> is the entry point to communicate with the Shopify Storefront API.
    /// <see cref="ShopifyClient">ShopifyClient </see> also has functionality to easily generate and send queries to receive
    /// information about products, collections, and has the ability to create checkouts.
    /// </summary>
    public class ShopifyClient {
        /// <summary>
        /// This is a dictionary that defines aliases, <c>maxWidth</c>, and <c>maxHeight</c> for images loaded by
        /// <see ref="ShopifyClient.products">products </see> and <see ref="ShopifyClient.collections">collections </see>.
        /// All Products images, Product variant images, and Collection images will be queried using aliases
        /// defined by this dictionary's keys and the <c>maxWidth</c> and <c>maxHeight</c> will be this dictionary's values.
        ///
        /// \code{.cs}
        /// // Returns an image source url whose dimensions are never greater than 100px
        /// string srcSmallImage = productVariant.image("small").src();
        ///
        /// // Returns an image source url whose dimensions are never greater than 1024px
        /// string src1024Image = productVariant.image("resolution_1024").src();
        /// \endcode
        /// </summary>
        public static Dictionary<string, int> DefaultImageResolutions = new Dictionary<string, int> () { { "pico", 16 }, { "icon", 32 }, { "thumb", 50 }, { "small", 100 }, { "compact", 160 }, { "medium", 240 }, { "large", 480 }, { "grande", 600 }, { "resolution_1024", 1024 }, { "resolution_2048", 2048 }
        };

        /// <summary>
        /// <see ref="ShopifyClient.AccessToken">AccessToken </see> is the access token associated with this client to query Shopify.
        /// </summary>
        public string AccessToken {
            get {
                return _AccessToken;
            }
        }

        /// <summary>
        /// <see ref="ShopifyClient.Domain">Domain </see> is the Shopify store domain associated with this client.
        /// </summary>
        public string Domain {
            get {
                return _Domain;
            }
        }

        QueryLoader Loader;
        string _AccessToken;
        string _Domain;
        Cart DefaultCart;
        Dictionary<string, Cart> CartsById;

        /// <summary>
        /// <see ref="ShopifyClient">ShopifyClient </see> is the entry point to communicate with the Shopify Storefront API.
        /// <see ref="ShopifyClient">ShopifyClient </see> also has functionality to easily generate and send queries to receive
        /// information about products and collections. It also has the ability to create checkouts.
        /// </summary>
        /// <param name="accessToken">the access token used to query the Shopify Storefront API for a store</param>
        /// <param name="domain">domain for the Shopify store</param>
        /// \code
        /// // Example that initializes a new ShopifyClient which will query all products
        /// string accessToken = "b8d417759a62f7b342f3735dbe86b322";
        /// string shopDomain = "unity-buy-sdk.myshopify.com";
        ///
        /// ShopifyClient client = new ShopifyClient(accessToken, shopDomain);
        ///
        /// client.products((products, error) => {
        ///     Debug.Log(products[0].title());
        ///     Debug.Log(products[1].title());
        /// });
        /// \endcode
        public ShopifyClient (string accessToken, string domain) {
            _AccessToken = accessToken;
            _Domain = domain;

#if !SHOPIFY_MONO_UNIT_TEST
            Loader = new QueryLoader (new UnityLoader (domain, AccessToken));
#endif
        }

        /// <summary>
        /// It is possible to instantiate a <see ref="ShopifyClient">ShopifyClient </see> by passing an instance
        /// that implements <see ref="BaseLoader">BaseLoader </see>. <see ref="BaseLoader">BaseLoaders </see> handle network communication with
        /// the Storefront API. This functionality is useful if you'd like to use the Shopify SDK for Unity in a C# environment
        /// outside of Unity. The <c>domain</c> string is inferred from <see ref="BaseLoader">BaseLoaders </see> which can be used
        /// to request a specific client.
        /// </summary>
        /// <param name="loader">a loader which will handle network communication with the Storefront API</param>
        /// \code
        /// // Example that initializes a new ShopifyClient using a custom loader for another C# platform
        /// string accessToken = "b8d417759a62f7b342f3735dbe86b322";
        /// string shopDomain = "unity-buy-sdk.myshopify.com";
        ///
        /// CustomLoaderForNonUnityPlatform loader = new CustomLoaderForNonUnityPlatform(accessToken, shopDomain);
        ///
        /// ShopifyClient client = new ShopifyClient(loader);
        ///
        /// client.products((products, error) => {
        ///     Debug.Log(products[0].title());
        ///     Debug.Log(products[1].title());
        /// });
        /// \endcode
        public ShopifyClient (BaseLoader loader) {
            _AccessToken = loader.AccessToken;
            _Domain = loader.Domain;
            Loader = new QueryLoader (loader);
        }

        /// <summary>
        /// Generates a query to receive one page of <c>products</c> from a Shopify store. The generated query will query the following on products:
        ///     - id
        ///     - title
        ///     - descriptionHtml
        ///     - images (with aliases defined by ShopifyClient.DefaultImageResolutions)
        ///         - altText
        ///         - src
        ///     - options
        ///         - name
        ///         - values
        ///     - variants
        ///         - id
        ///         - available
        ///         - price
        ///         - title
        ///         - weight
        ///         - weightUnit
        ///         - selectedOptions
        ///             - name
        ///             - values
        ///         - image (with aliases defined by ShopifyClient.DefaultImageResolutions)
        ///             - altText
        ///             - src
        ///     - collections
        ///         - image (with aliases defined by ShopifyClient.DefaultImageResolutions)
        ///             - altText
        ///             - src
        ///         - title
        ///         - updatedAt
        ///
        /// Note that <c>shop.products</c> is a Connection (GraphQL paginated data structure).
        ///
        /// <c>first</c> will define the page size. <c>after</c> will be the cursor for the next page.
        /// </summary>
        /// <param name="callback">callback that will receive responses from server. This callback also can receive <c>ShopifyError</c> or <c>null</c> if no error happened. The callback also receives <c>after</c> will be the cursor for the next page.</param>
        /// <param name="first">can be used to limit how many products are returned. For instance 10 would return only 10 products</param>
        /// <param name="after">
        /// is used to load subsequent pages. Basically it's a cursor variable to define what page to load next. For example, when used with <c>first: 10</c> and <c>after: "abc"</c>, only the first 10
        /// products would be loaded after cursor <c>"abc"</c>. If no <c>after</c> is passed the first page of products will be loaded.
        /// </param>
        /// \code
        /// // Example usage querying one page of products after the cursor "abc":
        /// ShopifyBuy.Client().products((products, error, after) => {
        ///     Debug.Log(products[0].title());
        ///     Debug.Log(products[1].title());
        ///     Debug.Log(products.Count);
        ///
        ///     if (after != null) {
        ///         // more products could be loaded here
        ///     }
        /// }, after: "abc");
        /// \endcode
        public void products (ProductsPaginatedHandler callback, int? first = null, string after = null) {
            if (first == null) {
                first = DefaultQueries.MaxProductPageSize;
            }

            GetProductsList ((products, error, afterGetProductsList) => {
                if (error != null) {
                    callback (null, error, null);
                } else {
                    GetConnectionsForProducts (products, (List<Product> productsWithSubConnections, ShopifyError errorForSubConnections) => {
                        if (error != null) {
                            callback (null, error, null);
                        } else {
                            callback (productsWithSubConnections, errorForSubConnections, afterGetProductsList);
                        }
                    });
                }
            }, first : first, after : after);
        }

        /// <summary>
        /// Generates a query to receive selected <c>products</c> from a Shopify store. The generated query will query the following on products:
        ///     - id
        ///     - title
        ///     - descriptionHtml
        ///     - images (with aliases defined by ShopifyClient.defaultImageResolutions)
        ///         - altText
        ///         - src
        ///     - options
        ///         - name
        ///         - values
        ///     - variants
        ///         - id
        ///         - available
        ///         - price
        ///         - title
        ///         - weight
        ///         - weightUnit
        ///         - selectedOptions
        ///             - name
        ///             - values
        ///         - image (with aliases defined by ShopifyClient.defaultImageResolutions)
        ///             - altText
        ///             - src
        ///     - collections
        ///         - image (with aliases defined by ShopifyClient.defaultImageResolutions)
        ///             - altText
        ///             - src
        ///         - title
        ///         - updatedAt
        ///
        /// </summary>
        /// <param name="callback">callback that will receive responses or errors from server</param>
        /// <param name="firstProductId">you must pass in at least one product id to query</param>
        /// <param name="otherProductIds">
        /// after the first product id you can pass in as many product ids as you'd like.
        /// </param>
        /// \code
        /// // Example usage querying two product ids
        /// ShopifyBuy.Client().products((products, error) => {
        ///     Debug.Log(products[0].title());
        ///     Debug.Log(products[1].title());
        /// }, "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyNzYwOTk=", "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyNzkwNDM=");
        /// \endcode
        public void products (ProductsHandler callback, string firstProductId, params string[] otherProductIds) {
            List<string> productIds = new List<string> ();
            productIds.Add (firstProductId);
            productIds.AddRange (otherProductIds);

            products (callback, productIds);
        }

        /// <summary>
        /// Generates a query to receive selected <c>products</c> from a Shopify store. The generated query will query the following on products:
        ///     - id
        ///     - title
        ///     - descriptionHtml
        ///     - images (with aliases defined by ShopifyClient.defaultImageResolutions)
        ///         - altText
        ///         - src
        ///     - options
        ///         - name
        ///         - values
        ///     - variants
        ///         - id
        ///         - available
        ///         - price
        ///         - title
        ///         - weight
        ///         - weightUnit
        ///         - selectedOptions
        ///             - name
        ///             - values
        ///         - image (with aliases defined by ShopifyClient.defaultImageResolutions)
        ///             - altText
        ///             - src
        ///     - collections
        ///         - image (with aliases defined by ShopifyClient.defaultImageResolutions)
        ///             - altText
        ///             - src
        ///         - title
        ///         - updatedAt
        ///
        /// </summary>
        /// <param name="callback">callback that will receive responses from server</param>
        /// <param name="productIds">a list of product ids you'd like to query</param>
        /// \code
        /// // Example usage querying two product ids using a List<string>
        /// List<string> productIds = new List<string>() {
        ///     "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyNzYwOTk=",
        ///     "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyNzkwNDM="
        /// };
        ///
        /// ShopifyBuy.Client().products((products, error) => {
        ///     Debug.Log(products[0].title());
        ///     Debug.Log(products[1].title());
        /// }, productIds);
        /// \endcode
        public void products (ProductsHandler callback, List<string> productIds) {
            QueryRootQuery query = new QueryRootQuery ();

            query.nodes (n => n
                .onProduct ((p) => {
                    DefaultQueries.products.Product (p, DefaultImageResolutions);
                }),
                ids : productIds
            );

            Loader.Query (query, (response) => {
                var error = (ShopifyError) response;
                if (error != null) {
                    callback (null, error);
                } else {
                    List<Product> products = new List<Product> ();

                    foreach (Shopify.Unity.Product product in response.data.nodes ()) {
                        products.Add (product);
                    }

                    callback (products, error);
                }
            });
        }

        /// <summary>
        /// Generates a query to receive a page of <c>collections</c> from a Shopify store. The generated query will query the following on collections:
        ///     - id
        ///     - title
        ///     - description
        ///     - descriptionHtml
        ///     - updatedAt
        ///     - image
        ///         - altText
        ///         - src
        ///     - products
        ///         - id
        ///
        /// Note that <c>shop.collections</c> is a Connection (GraphQL paginated data structure). <see ref="ShopifyClient.collections">collections </see>
        /// </summary>
        /// <param name="callback">callback that will receive responses from server</param>
        /// <param name="first">can be used to limit how many products are returned. For instance 10 would return only 10 collections</param>
        /// <param name="after">
        /// is used to load subsequent pages. Basically it's a cursor variable to define what page to load next. For example, when used with <c>first: 10</c> and <c>after: "abc"</c>, only the first 10
        /// collections would be loaded after cursor <c>"abc"</c>. If no <c>after</c> is passed the first page of collections will be loaded.
        /// </param>
        /// \code
        /// // Example that queries all collections on a shop
        /// ShopifyBuy.Client().collections((collections, error) => {
        /// 	Debug.Log(collections[0].title());
        /// 	Debug.Log(collections.Count);
        /// });
        /// \endcode
        public void collections (CollectionsPaginatedHandler callback, int? first = null, string after = null) {
            if (first == null) {
                first = DefaultQueries.MaxCollectionsPageSize;
            }

            GetCollectionsList ((collections, error, afterGetProductsList) => {
                // Short circuit if we have an error.
                if (error != null) {
                    callback (null, error, null);
                    return;
                }

                List<ConnectionQueryInfo> connectionInfos = new List<ConnectionQueryInfo> () {
                    new ConnectionQueryInfo (
                        getConnection: (c) => ((Collection) c).products (),
                        query: (c, productsAfter) => {
                            ((CollectionQuery) c).products (pc => DefaultQueries.collections.ProductConnection (pc),
                                first : DefaultQueries.MaxPageSize, after : productsAfter
                            );
                        }
                    )
                };

                ConnectionLoader loader = new ConnectionLoader (Loader);
                List<Node> nodes = collections.ConvertAll (p => (Node) p);

                loader.QueryConnectionsOnNodes (nodes, connectionInfos, BuildCollectionQueryOnNode, (nodesResult, responseError) => {
                    callback (nodesResult.ConvertAll (n => (Collection) n), responseError, afterGetProductsList);
                });
            }, first : first, after : after);
        }

        /// <summary>
        /// Allows you to send custom GraphQL queries to the Storefront API. While having utility functions like <see ref="ShopifyClient.products">products </see>
        /// <see ref="ShopifyClient.collections">collections </see> is useful, the Storefront API has more functionality. This method
        /// allows you to access all the extra functionality that the Storefront API provides.
        /// </summary>
        /// <param name="query">a GraphQL query to be sent to the Storefront API</param>
        /// <param name="callback">callback which will receive a response from the query</param>
        /// \code
        /// // Example that queries a Shop's name
        /// QueryRoot query = new QueryRootQuery();
        ///
        /// query.shop(s => s
        ///     .name()
        /// );
        ///
        /// ShopifyBuy.Client().Query(
        ///     query: query,
        ///     callback: (data, error) => {
        ///         if (error != null) {
        ///             Debug.Log("There was an error: " + error.Reason);
        ///         } else {
        ///             Debug.Log(data.shop().name());
        ///         }
        ///     }
        /// );
        /// \endcode
        public void Query (QueryRootQuery query, QueryRootHandler callback) {
            Loader.Query (query, (response) => {
                callback (response.data, (ShopifyError) response);
            });
        }

        /// <summary>
        /// Allows you to build and send custom GraphQL queries to the Storefront API. While having utility functions like <see ref="ShopifyClient.products">products </see>
        /// <see ref="ShopifyClient.collections">collections </see> is useful, the Storefront API has more functionality. This method
        /// allows you to access all the extra functionality that the Storefront API provides.
        /// </summary>
        /// <param name="buildQuery">
        /// delegate that will build a query starting at <see ref="QueryRootQuery">QueryRootQuery </see>
        /// which will be sent to the Storefront API
        /// </param>
        /// <param name="callback">callback which will receive a response</param>
        /// \code
        /// // Example that builds a query that queries a Shop's name
        /// ShopifyBuy.Client().Query(
        ///     query: q => q
        ///         .shop(s => s
        ///             .name()
        ///         ),
        ///     callback: (data, error) => {
        ///         if (error != null) {
        ///             Debug.Log("There was an error: " + error.Reason);
        ///         } else {
        ///             Debug.Log(data.shop().name());
        ///         }
        ///     }
        /// );
        /// \endcode
        public void Query (QueryRootDelegate buildQuery, QueryRootHandler callback) {
            QueryRootQuery query = new QueryRootQuery ();

            buildQuery (query);

            Query (query, callback);
        }

        /// <summary>
        /// Allows you to continuously make a root query, till the response is deemed ready.
        /// </summary>
        /// <param name="isReady">A <see cref="Delegates.PollUpdatedHandler"/> that determines if polling should stop by returning true</param>
        /// <param name="query">The query to be queried continuously</param>
        /// \code
        /// // Example that uses polling
        /// QueryRootQuery query = new QueryRootQuery();
        ///
        /// query.node(
        ///     buildQuery: node => node
        ///         .onCheckout(checkout => checkout.ready()),
        ///     id: "someCheckoutID""
        /// );
        ///
        /// PollUpdatedHandler isReadyHandler = (updatedQueryRoot) => {
        ///     var expectedNode = (Checkout) updatedQueryRoot.node();
        ///     return expectedNode.ready();
        /// };
        ///
        /// PollQuery(isReadyHandler, query, (response, error) => {
        ///     if (error == null) {
        ///         var checkout = (Checkout) response.node();
        ///         // checkout.ready() is true
        ///     }
        ///})
        public void PollQuery (PollUpdatedHandler isReady, QueryRootQuery query, QueryRootHandler callback) {
            const float POLL_DELAY_SECONDS = 0.5f;

            Query (query, (QueryRoot response, ShopifyError error) => {
                if (error != null) {
                    callback (response, error);
                } else {
                    if (isReady (response)) {
                        callback (response, null);
                    } else {
#if !SHOPIFY_MONO_UNIT_TEST
                        UnityTimeout.Start (POLL_DELAY_SECONDS, () => {
                            PollQuery (isReady, query, callback);
                        });
#else
                        PollQuery (isReady, query, callback);
#endif
                    }
                }
            });
        }

        /// <summary>
        /// Allows you to send custom prebuilt GraphQL mutation queries to the Storefront API.
        /// </summary>
        /// <param name="query">a query to be sent to the Storefront API</param>
        /// <param name="callback">callback which will receive a response</param>
        /// \code
        /// // Example that creates a custom mutation query
        /// MutationQuery mutation = new MutationQuery();
        ///
        /// mutation.customerCreate(
        /// 	buildQuery: (cc) => { cc
        ///         .userErrors(
        ///         	buildQuery: (ue) => { ue
        ///                 .field()
        ///                 .message();
        ///         	}
        ///         );
        /// 	},
        /// 	input: new CustomerCreateInput(
        ///         email: "mikkoh@email.com",
        ///         password: "oh so secret"
        /// 	)
        /// );
        ///
        /// ShopifyBuy.Client().Mutation(
        /// 	query: mutation,
        /// 	callback: (data, error) => {
        ///         if (error != null) {
        ///             Debug.Log("There was an error: " + error.Reason);
        ///         } else {
        ///         	List<UserError> userErrors = data.customerCreate().userErrors();
        ///
        ///         	if (userErrors != null) {
        ///                 foreach(UserError error in userErrors) {
        ///                 	// field which may have a user error
        ///                 	Debug.Log(error.field());
        ///                 	// error message for the field which had an error
        ///                 	Debug.Log(error.message());
        ///                 }
        ///         	} else {
        ///                 Debug.Log("No user errors the customer was created");
        ///         	}
        ///         }
        /// 	}
        /// );
        /// \endcode
        public void Mutation (MutationQuery query, MutationRootHandler callback) {
            Loader.Mutation (query, (response) => {
                callback (response.data, (ShopifyError) response);
            });
        }

        /// <summary>
        /// Allows you to build and send custom GraphQL mutation queries to the Storefront API.
        /// </summary>
        /// <param name="buildQuery">delegate that will build a query starting at <see ref="QueryRootQuery">MutationQuery </see></param>
        /// <param name="callback">callback which will receive a response</param>
        /// \code
        /// // Example that creates a new customer on a store
        /// ShopifyBuy.Client().Mutation(
        ///     buildQuery: (mutation) => { mutation
        ///         .customerCreate(
        ///             buildQuery: (cc) => { cc
        ///                 .userErrors(
        ///                     buildQuery: (ue) => { ue
        ///                         .field()
        ///                         .message();
        ///                     }
        ///                 );
        ///             },
        ///             input: new CustomerCreateInput(
        ///                 email: "mikkoh@email.com",
        ///                 password: "oh so secret"
        ///             )
        ///         );
        ///     },
        ///     callback: (data, error) => {
        ///         if (error != null) {
        ///             Debug.Log("There was an error: " + error.Reason);
        ///         } else {
        ///             List<UserError> userErrors = data.customerCreate().userErrors();
        ///
        ///             if (userErrors != null) {
        ///                 foreach(UserError error in userErrors) {
        ///                     // field which may have a user error
        ///                     Debug.Log(error.field());
        ///                     // error message for the field which had an error
        ///                     Debug.Log(error.message());
        ///                 }
        ///             } else {
        ///                 Debug.Log("No user errors the customer was created");
        ///             }
        ///         }
        ///     }
        /// );
        /// \endcode
        public void Mutation (MutationDelegate buildQuery, MutationRootHandler callback) {
            MutationQuery query = new MutationQuery ();

            buildQuery (query);

            Mutation (query, callback);
        }

        /// <summary>
        /// Creates a <see ref="Cart">Cart </see>, which can be used to manage line items for an order and create a
        /// web checkout link. One <see ref="ShopifyClient">client </see> can have multiple carts, so it's
        /// possible to pass in a <c>cartId</c> to reference a specific cart. If no <c>cartId</c> is passed, then
        /// a default <see ref="Cart">Cart </see> is used.
        /// </summary>
        /// <param name="cartId">can be optionally passed in. This is useful if your application needs multiple carts</param>
        /// \code
        /// // Example that checks how many line items the cart contains
        /// Cart cart = ShopifyBuy.Client().Cart();
        ///
        /// Debug.Log("The cart has " + cart.LineItems.All().Count + " line items");
        /// \endcode
        public Cart Cart (string cartId = null) {
            if (cartId == null) {
                if (DefaultCart == null) {
                    DefaultCart = new Cart (this);
                }

                return DefaultCart;
            } else {
                if (CartsById == null) {
                    CartsById = new Dictionary<string, Cart> ();
                }

                if (!CartsById.ContainsKey (cartId)) {
                    CartsById[cartId] = new Cart (this);
                }

                return CartsById[cartId];
            }
        }

        private void GetConnectionsForProducts (List<Product> products, ProductsHandler callback) {
            List<ConnectionQueryInfo> connectionInfos = new List<ConnectionQueryInfo> () {
                new ConnectionQueryInfo (
                        getConnection: (p) => ((Product) p).images (),
                        query: (p, imagesAfter) => {
                            ((ProductQuery) p).images (ic => DefaultQueries.products.ImageConnection (ic),
                                first : DefaultQueries.MaxPageSize, after : imagesAfter
                            );
                        }
                    ),
                    new ConnectionQueryInfo (
                        getConnection: (p) => ((Product) p).variants (),
                        query: (p, variantsAfter) => {
                            ((ProductQuery) p).variants (vc => DefaultQueries.products.ProductVariantConnection (vc, DefaultImageResolutions),
                                first : DefaultQueries.MaxPageSize, after : variantsAfter
                            );
                        }
                    ),
                    new ConnectionQueryInfo (
                        getConnection: (p) => ((Product) p).collections (),
                        query: (p, collectionsAfter) => {
                            ((ProductQuery) p).collections (cc => DefaultQueries.products.CollectionConnection (cc),
                                first : DefaultQueries.MaxPageSize, after : collectionsAfter
                            );
                        }
                    )
            };

            foreach (string alias in DefaultImageResolutions.Keys) {
                string currentAlias = alias;

                connectionInfos.Add (new ConnectionQueryInfo (
                    getConnection: (p) => ((Product) p).images (currentAlias),
                    query: (p, imagesAfter) => {
                        ((ProductQuery) p).images (ic => DefaultQueries.products.ImageConnection (ic),
                            first : DefaultQueries.MaxPageSize,
                            after : imagesAfter,
                            maxWidth : DefaultImageResolutions[currentAlias],
                            maxHeight : DefaultImageResolutions[currentAlias],
                            alias : currentAlias
                        );
                    }
                ));
            }

            ConnectionLoader loader = new ConnectionLoader (Loader);
            List<Node> nodes = products.ConvertAll (p => (Node) p);

            loader.QueryConnectionsOnNodes (nodes, connectionInfos, BuildProductQueryOnNode, (nodesResult, error) => {
                callback (nodesResult.ConvertAll (n => (Product) n), error);
            });
        }

        private void BuildProductQueryOnNode (QueryRootQuery query, List<ConnectionQueryInfo> connectionInfosToBuildQuery, string productId, string alias) {
            query.node (n => n
                .onProduct ((p) => {
                    p.id ();

                    foreach (ConnectionQueryInfo info in connectionInfosToBuildQuery) {
                        info.Query (p, info.After);
                    }
                }),
                id : productId, alias : alias
            );
        }

        private void BuildCollectionQueryOnNode (QueryRootQuery query, List<ConnectionQueryInfo> connectionInfosToBuildQuery, string collectionId, string alias) {
            query.node (n => n
                .onCollection (c => {
                    c.id ();

                    foreach (ConnectionQueryInfo info in connectionInfosToBuildQuery) {
                        info.Query (c, info.After);
                    }
                }),
                id : collectionId, alias : alias
            );
        }

        private void GetProductsList (ProductsPaginatedHandler callback, int? first = null, string after = null) {
            ConnectionLoader loader = new ConnectionLoader (Loader);
            int countToLoad = first == null ? int.MaxValue : (int) first;

            loader.QueryConnection (
                (response) => {
                    QueryRootQuery query = null;

                    List<ProductEdge> edges = response != null ? response.data.shop ().products ().edges () : null;

                    if (edges != null) {
                        countToLoad -= edges.Count;
                    }

                    if (response == null || (countToLoad > 0 && response.data.shop ().products ().pageInfo ().hasNextPage ())) {
                        query = new QueryRootQuery ();

                        query = new QueryRootQuery ();
                        DefaultQueries.products.ShopProducts (
                            query : query,
                            imageResolutions : DefaultImageResolutions,
                            first : countToLoad > DefaultQueries.MaxProductPageSize ? DefaultQueries.MaxProductPageSize : countToLoad,
                            after : edges != null ? edges[edges.Count - 1].cursor () : after
                        );
                    }

                    return query;
                },
                (response) => {
                    return ((QueryRoot) response).shop ().products ();
                },
                (response) => {
                    var error = (ShopifyError) response;
                    if (error != null) {
                        callback (null, error, null);
                    } else {
                        string lastCursor = null;

                        if (response.data.shop ().products ().pageInfo ().hasNextPage ()) {
                            List<ProductEdge> productEdges = response.data.shop ().products ().edges ();

                            lastCursor = productEdges[productEdges.Count - 1].cursor ();
                        }

                        callback ((List<Product>) response.data.shop ().products (), error, lastCursor);
                    }
                }
            );
        }

        private void GetCollectionsList (CollectionsPaginatedHandler callback, int? first = null, string after = null) {
            ConnectionLoader loader = new ConnectionLoader (Loader);
            int countToLoad = first == null ? int.MaxValue : (int) first;

            loader.QueryConnection (
                (response) => {
                    QueryRootQuery query = null;

                    List<CollectionEdge> edges = response != null ? response.data.shop ().collections ().edges () : null;

                    if (edges != null) {
                        countToLoad -= edges.Count;
                    }

                    if (response == null || (countToLoad > 0 && response.data.shop ().collections ().pageInfo ().hasNextPage ())) {
                        query = new QueryRootQuery ();

                        query = new QueryRootQuery ();
                        DefaultQueries.collections.ShopCollections (
                            query : query,
                            first : countToLoad > DefaultQueries.MaxPageSize ? DefaultQueries.MaxPageSize : countToLoad,
                            after : edges != null ? edges[edges.Count - 1].cursor () : after,
                            imageResolutions : DefaultImageResolutions
                        );
                    }

                    return query;
                },
                (response) => {
                    return ((QueryRoot) response).shop ().collections ();
                },
                (response) => {
                    var error = (ShopifyError) response;
                    if (error != null) {
                        callback (null, error, null);
                    } else {
                        string lastCursor = null;

                        if (response.data.shop ().collections ().pageInfo ().hasNextPage ()) {
                            List<CollectionEdge> collectionEdges = response.data.shop ().collections ().edges ();

                            lastCursor = collectionEdges[collectionEdges.Count - 1].cursor ();
                        }

                        callback ((List<Collection>) response.data.shop ().collections (), error, lastCursor);
                    }
                }
            );
        }
    }
}