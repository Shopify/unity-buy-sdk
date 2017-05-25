namespace Shopify.Tests {
    public class MockLoaderCheckouts : BaseMockLoader {
        private const string CREATE_QUERY = @"mutation{checkoutCreate (input:{lineItems:[{quantity:33,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==""},{quantity:2,variantId:""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==""}]}){checkout {id webUrl ready lineItems (first:250){edges {node {id variant {id }}cursor }pageInfo {hasNextPage }}}userErrors {field message }}}";

        public MockLoaderCheckouts() {
            SetupCreateResponse();
        }

        public void SetupCreateResponse() {
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
    }
}
