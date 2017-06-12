namespace Shopify.Tests {
    public class MockLoaderCheckouts : BaseMockLoader {
        private const string CREATE_QUERY = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""}]}){checkout {id webUrl ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}";

        // The following two queries are used for a test where first a create is called and we return false for ready which forces a poll
        // then we return a result that ready is true and the new weburl is there
        private const string CREATE_QUERY_THAT_POLLS = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTPOLL==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""}]}){checkout {id webUrl ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}";
        private const string POLL_QUERY_AFTER_CREATE = @"{node (id:""checkout-id-poll""){__typename ...on Checkout{id webUrl ready }}}";

        public MockLoaderCheckouts() {
            SetupCreateResponse();
            SetupCreateResponsesForPolling();
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
    }
}
