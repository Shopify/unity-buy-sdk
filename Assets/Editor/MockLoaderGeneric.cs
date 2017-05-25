namespace Shopify.Tests {
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;

    public class MockLoaderGeneric : BaseMockLoader {
        public MockLoaderGeneric() {
            SetupShopName();
            SetupCustomerAccessToken();
        }

        private void SetupShopName() {
            QueryRootQuery query = new QueryRootQuery();
            query.shop(s => s.name());

            AddResponse(
                query,
                @"{
                    ""data"": {
                        ""shop"": {
                            ""name"": ""this is the test shop yo""
                        }
                    }
                }"
            );
        }

        private void SetupCustomerAccessToken() {
            MutationQuery mutation = new MutationQuery();

            mutation.customerAccessTokenCreate((a) => a
                .customerAccessToken(at => at
                    .accessToken()
                ),
                input: new CustomerAccessTokenCreateInput("some@email.com", "password")
            );

            AddResponse(
                mutation, 
                @"{
                    ""data"": {
                        ""customerAccessTokenCreate"": {
                            ""customerAccessToken"": {
                                ""accessToken"": ""i am a token""
                            }
                        }
                    }
                }"
            );
        }
    }
}
