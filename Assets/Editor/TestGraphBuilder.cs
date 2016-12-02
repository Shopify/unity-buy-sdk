namespace Shopify.Tests
{
    using NUnit.Framework;
    using Shopify.Unity;

    [TestFixture]
    public class TestGraphQLGenerator {
        [Test]
        public void queryRootBuildsQuery() {
            QueryRootQuery query = Root.buildQuery();
            query.shop(s =>
                s.name()
            );

            Assert.AreEqual(
                "{shop{name}}",
                query.ToString()
            );
        }

        [Test] 
        public void inputTypeToString() {
            ApiCustomerAccessTokenCreateInputInput input = new ApiCustomerAccessTokenCreateInputInput(email: "email@email.com", password: "123456", clientMutationId: "333");

            Assert.AreEqual(
                "{email:\"email@email.com\",password:\"123456\",clientMutationId:\"333\"}",
                input.ToString()
            );
        }

        [Test]
        public void mutationRootBuildsQuery() {
            MutationQuery query = Root.buildMutation()
                .apiCustomerAccessTokenCreate(r => r
                    .apiCustomerAccessToken(a => a
                        .accessToken()
                    )
                    .clientMutationId(), 
                    new ApiCustomerAccessTokenCreateInputInput(email: "email@email.com", password: "123456", clientMutationId: "333")
                );

            Assert.AreEqual(
                "mutation{apiCustomerAccessTokenCreate(input:{email:\"email@email.com\",password:\"123456\",clientMutationId:\"333\"}){apiCustomerAccessToken{accessToken},clientMutationId}}",
                query.ToString()
            );
        }
    }    
}
