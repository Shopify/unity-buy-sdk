namespace Shopify.Tests {
    public class MockLoaderCheckouts : BaseMockLoader {
        // The following query is for a straightforward checkout create
        private const string CREATE_QUERY = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""}]}){checkout {id webUrl requiresShipping subtotalPrice totalTax totalPrice ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}";

        // The following two queries are used for a test where first a create is called and we return false for ready which forces a poll
        // then we return a result that ready is true and the new weburl is there
        private const string CREATE_QUERY_THAT_POLLS = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTPOLL==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""}]}){checkout {id webUrl requiresShipping subtotalPrice totalTax totalPrice ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}";
        private const string POLL_QUERY_AFTER_CREATE = @"{node (id:""checkout-id-poll""){__typename ...on Checkout{id webUrl requiresShipping subtotalPrice totalTax totalPrice ready }}}";

        // The following queries are used to test creating a checkout, then modififying line items and getting a new url
        private const string CREATE_QUERY_THAT_WILL_BE_UPDATED = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyDelete==""}]}){checkout {id webUrl requiresShipping subtotalPrice totalTax totalPrice ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}";
        private const string ADD_UPDATE_DELETE_AFTER_CREATE = @"mutation{checkoutLineItemsRemove (checkoutId:""checkout-id-poll"",lineItemIds:[""line-item-id1"",""line-item-id2""]){userErrors {field message }}checkoutLineItemsAdd (lineItems:[{quantity:10,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate==""},{quantity:3,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NTMNewItem==""}],checkoutId:""checkout-id-poll""){checkout {id webUrl requiresShipping subtotalPrice totalTax totalPrice ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}";

        public MockLoaderCheckouts() {
            SetupCreateResponse();
            SetupCreateResponsesForPolling();
            SetupCreateThenUpdateResponse();
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
                                ""requiresShipping"": true,
                                ""subtotalPrice"": ""267.96"",
                                ""totalTax"": ""34.83"",
                                ""totalPrice"": ""302.79"",
                                ""ready"": true,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"", 
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==""
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id2"", 
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""
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
                            ""userErrors"": []
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
                                ""requiresShipping"": true,
                                ""subtotalPrice"": ""267.96"",
                                ""totalTax"": ""34.83"",
                                ""totalPrice"": ""302.79"",
                                ""ready"": false,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"", 
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTPOLL==""
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id2"", 
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""
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
                            ""userErrors"": []
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
                                ""requiresShipping"": true,
                                ""subtotalPrice"": ""267.96"",
                                ""totalTax"": ""34.83"",
                                ""totalPrice"": ""302.79"",
                                ""ready"": true,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"", 
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate==""
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id2"", 
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyDelete==""
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
                            ""userErrors"": []
                        }
                    }
                }"
            );
            
            AddResponse(
                ADD_UPDATE_DELETE_AFTER_CREATE,
                @"{
                    ""data"": {
                        ""checkoutLineItemsRemove"": {
                            ""userErrors"": []
                        },
                        ""checkoutLineItemsAdd"": {
                            ""checkout"": {
                                ""id"": """",
                                ""webUrl"": ""http://shopify.com/checkout-create-after-update"",
                                ""requiresShipping"": true,
                                ""subtotalPrice"": ""267.96"",
                                ""totalTax"": ""34.83"",
                                ""totalPrice"": ""302.79"",
                                ""ready"": true,
                                ""lineItems"": {
                                    ""edges"": [
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id1"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate=="" 
                                                }
                                            },
                                            ""cursor"": ""line-item-1-cursor""
                                        },
                                        {
                                            ""node"": {
                                                ""id"": ""line-item-id3"",
                                                ""variant"": {
                                                    ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NTMNewItem=="" 
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
                            ""userErrors"": []
                        }
                    }
                }"
            );
        }
    }
}
