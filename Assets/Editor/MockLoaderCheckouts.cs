namespace Shopify.Tests {
    public class MockLoaderCheckouts : BaseMockLoader {
        // The following query is for a straightforward checkout create
        private const string CREATE_QUERY = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""}],allowPartialAddresses:true}){checkout {id webUrl currencyCode requiresShipping subtotalPriceV2 {amount currencyCode }totalTaxV2 {amount currencyCode }totalPriceV2 {amount currencyCode }ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}checkoutUserErrors {code field message }}}";

        // The following two queries are used for a test where first a create is called and we return false for ready which forces a poll
        // then we return a result that ready is true and the new weburl is there
        private const string CREATE_QUERY_THAT_POLLS = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTPOLL==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""}],allowPartialAddresses:true}){checkout {id webUrl currencyCode requiresShipping subtotalPriceV2 {amount currencyCode }totalTaxV2 {amount currencyCode }totalPriceV2 {amount currencyCode }ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}checkoutUserErrors {code field message }}}";
        private const string POLL_QUERY_AFTER_CREATE = @"{node (id:""checkout-id-poll""){__typename ...on Checkout{id webUrl currencyCode requiresShipping subtotalPriceV2 {amount currencyCode }totalTaxV2 {amount currencyCode }totalPriceV2 {amount currencyCode }ready }}}";

        // The following queries are used to test creating a checkout, then modififying line items and getting a new url
        private const string CREATE_QUERY_THAT_WILL_BE_UPDATED = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyDelete==""}],allowPartialAddresses:true}){checkout {id webUrl currencyCode requiresShipping subtotalPriceV2 {amount currencyCode }totalTaxV2 {amount currencyCode }totalPriceV2 {amount currencyCode }ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}checkoutUserErrors {code field message }}}";
        private const string ADD_UPDATE_DELETE_AFTER_CREATE = @"mutation{checkoutLineItemsRemove (checkoutId:""checkout-id-poll"",lineItemIds:[""line-item-id1"",""line-item-id2""]){checkoutUserErrors {code field message }}checkoutLineItemsAdd (lineItems:[{quantity:10,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate==""},{quantity:3,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NTMNewItem==""}],checkoutId:""checkout-id-poll""){checkout {id webUrl currencyCode requiresShipping subtotalPriceV2 {amount currencyCode }totalTaxV2 {amount currencyCode }totalPriceV2 {amount currencyCode }ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}checkoutUserErrors {code field message }}}";

        // The following query will return a user error
        private const string CREATE_QUERY_SENDS_USERERROR = @"mutation{checkoutCreate (input:{lineItems:[{quantity:1,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWNFyaWFudC8yMDc1NjEyUserError==""}],allowPartialAddresses:true}){checkout {id webUrl currencyCode requiresShipping subtotalPriceV2 {amount currencyCode }totalTaxV2 {amount currencyCode }totalPriceV2 {amount currencyCode }ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}checkoutUserErrors {code field message }}}";

        // The following queries will return the completedAt date for a checkout for validation tesitng
        private const string QUERY_COMPLETED_AT_WITH_DATE = @"{node (id:""checkout-id""){__typename ...on Checkout{completedAt }}}";
        private const string QUERY_COMPLETED_AT_WITH_NULL_DATE = @"{node (id:""checkout-id-cancelled""){__typename ...on Checkout{completedAt }}}";
        private const string QUERY_COMPLETED_AT_ERROR = @"{node (id:""checkout-id-failed""){__typename ...on Checkout{completedAt }}}";

        public MockLoaderCheckouts() {
            SetupCreateResponse();
            SetupCreateResponsesForPolling();
            SetupCreateThenUpdateResponse();
            SetupUserErrorResponses();
            SetupCompletedAtWithDateResponse();
            SetupCompletedAtWithNullDateResponse();
            SetupCompletedAtWithError();
        }

        private void SetupCreateResponse() {
            AddResponse(
                CREATE_QUERY,
                @"{
                    ""data"": {
                        ""checkoutCreate"": {
                            ""checkout"": {
                                ""id"": ""checkout-id"",
                                ""webUrl"": ""http://shopify.com/checkout-no-poll"",
                                ""currencyCode"": ""CAD"",
                                ""requiresShipping"": true,
                                ""subtotalPriceV2"": {
                                    ""amount"": ""267.96"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""totalTaxV2"": {
                                    ""amount"": ""34.83"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""totalPriceV2"": {
                                    ""amount"": ""302.79"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""ready"": true,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""CAD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id2"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""CAD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        }
                                    ],
                                    ""pageInfo"": {
                                        ""hasNextPage"": false
                                    }
                                }
                            },
                            ""checkoutUserErrors"": []
                        }
                    }
                }"
            );
        }

        private void SetupCreateResponsesForPolling() {
            AddResponse(
                CREATE_QUERY_THAT_POLLS,
                @"{
                    ""data"": {
                        ""checkoutCreate"": {
                            ""checkout"": {
                                ""id"": ""checkout-id-poll"",
                                ""webUrl"": ""http://shopify.com/checkout-with-poll"",
                                ""currencyCode"": ""CAD"",
                                ""requiresShipping"": true,
                                ""subtotalPriceV2"": {
                                    ""amount"": ""267.96"",
                                    ""currencyCode"": ""CAD""
                                }
                                ""totalTaxV2"": {
                                    ""amount"": ""34.83"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""totalPriceV2"": {
                                    ""amount"": ""302.79"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""ready"": false,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTPOLL=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""CAD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id2"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""CAD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        }
                                    ],
                                    ""pageInfo"": {
                                        ""hasNextPage"": false
                                    }
                                }
                            },
                            ""checkoutUserErrors"": []
                        }
                    }
                }"
            );

            AddResponse(
                POLL_QUERY_AFTER_CREATE,
                @"{
                    ""data"": {
                        ""node"": {
                            ""__typename"": ""Checkout"",
                            ""id"": ""checkout-id-poll"",
                            ""webUrl"": ""http://shopify.com/checkout-with-poll-after-poll"",
                            ""ready"": true
                        }
                    }
                }"
            );
        }

        private void SetupCreateThenUpdateResponse() {
            AddResponse(
                CREATE_QUERY_THAT_WILL_BE_UPDATED,
                @"{
                    ""data"": {
                        ""checkoutCreate"": {
                            ""checkout"": {
                                ""id"": ""checkout-id-poll"",
                                ""webUrl"": ""http://shopify.com/checkout-create-before-update"",
                                ""currencyCode"": ""CAD"",
                                ""requiresShipping"": true,
                                ""subtotalPriceV2"": {
                                    ""amount"": ""267.96"",
                                    ""currencyCode"": ""CAD""
                                }
                                ""totalTaxV2"": {
                                    ""amount"": ""34.83"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""totalPriceV2"": {
                                    ""amount"": ""302.79"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""ready"": true,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""USD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id2"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyDelete=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""USD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        }
                                    ],
                                    ""pageInfo"": {
                                        ""hasNextPage"": false
                                    }
                                }
                            },
                            ""checkoutUserErrors"": []
                        }
                    }
                }"
            );

            AddResponse(
                ADD_UPDATE_DELETE_AFTER_CREATE,
                @"{
                    ""data"": {
                        ""checkoutLineItemsRemove"": {
                            ""checkoutUserErrors"": []
                        },
                        ""checkoutLineItemsAdd"": {
                            ""checkout"": {
                                ""id"": """",
                                ""webUrl"": ""http://shopify.com/checkout-create-after-update"",
                                ""currencyCode"": ""CAD"",
                                ""requiresShipping"": true,
                                ""subtotalPriceV2"": {
                                    ""amount"": ""267.96"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""totalTaxV2"": {
                                    ""amount"": ""34.83"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""totalPriceV2"": {
                                    ""amount"": ""302.79"",
                                    ""currencyCode"": ""CAD""
                                },
                                ""ready"": true,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""CAD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id3"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NTMNewItem=="",
                                                    ""priceV2"": {
                                                        ""amount"": ""1.00"",
                                                        ""currencyCode"": ""CAD""
                                                    }
                                                }
                                            },
                                            ""cursor"": ""line-item-3-cursor""
                                        }
                                    ],
                                    ""pageInfo"": {
                                        ""hasNextPage"": false
                                    }
                                }
                            },
                            ""checkoutUserErrors"": []
                        }
                    }
                }"
            );
        }

        private void SetupUserErrorResponses() {
            AddResponse(
                CREATE_QUERY_SENDS_USERERROR,
                @"{
                    ""data"": {
                        ""checkoutCreate"": {
                            ""checkout"": null,
                            ""checkoutUserErrors"": [
                                {
                                    ""code"": ""400""
                                    ""field"": [ ""someField"" ],
                                    ""message"": ""bad things happened""
                                }
                            ]
                        }
                    }
                }"
            );
        }

        private void SetupCompletedAtWithDateResponse() {
            AddResponse(
                QUERY_COMPLETED_AT_WITH_DATE,
                @"{
                    ""data"": {
                        ""node"": {
                            ""__typename"": ""Checkout"",
                            ""completedAt"": ""2017-07-11T16:42:34+00:00""
                        }
                    }
                }"
            );
        }

        private void SetupCompletedAtWithNullDateResponse() {
            AddResponse(
                QUERY_COMPLETED_AT_WITH_NULL_DATE,
                @"{
                    ""data"": {
                        ""node"": {
                            ""__typename"": ""Checkout"",
                            ""completedAt"": null
                        }
                    }
                }"
            );
        }

        private void SetupCompletedAtWithError() {
            AddResponse(
                QUERY_COMPLETED_AT_ERROR,
                @"{
                    ""errors"": [
                        {""message"": ""GraphQL error from mock loader""}
                    ]
                }"
            );
        }
    }
}
