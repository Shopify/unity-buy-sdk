namespace Shopify.Tests
{
    using System;
    using NUnit.Framework;
    using Shopify.Unity;

    [TestFixture]
    public class TestGraphQLGenerator {
        [Test]
        public void GeneratedGraphQLEnums() {
            Assert.IsTrue(Utils.DoesExist("CropRegion"));
        }

        [Test]
        public void GeneratedGraphQLInterfaces() {
            Assert.IsTrue(Utils.DoesExist("Node"));
        }

        [Test]
        public void GeneratedGraphQLInputObjects() {
            Assert.IsTrue(Utils.DoesExist("ApiCustomerAccessTokenCreateInput"));
        }

        [Test]
        public void QueryRootBuildsQuery() {
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
        public void InputObjectToString() {
            ApiCustomerAccessTokenCreateInput input = new ApiCustomerAccessTokenCreateInput(email: "email@email.com", password: "123456", clientMutationId: "333");

            Assert.AreEqual(
                "{email:\"email@email.com\",password:\"123456\",clientMutationId:\"333\"}",
                input.ToString()
            );
        }

        [Test]
        public void MutationRootBuildsQuery() {
            MutationQuery query = Root.buildMutation()
                .apiCustomerAccessTokenCreate(r => r
                    .apiCustomerAccessToken(a => a
                        .accessToken()
                    )
                    .clientMutationId(),
                    input: new ApiCustomerAccessTokenCreateInput(email: "email@email.com", password: "123456", clientMutationId: "333")
                );

            Assert.AreEqual(
                "mutation{apiCustomerAccessTokenCreate(input:{email:\"email@email.com\",password:\"123456\",clientMutationId:\"333\"}){apiCustomerAccessToken{accessToken},clientMutationId}}",
                query.ToString()
            );
        }

        [Test]
        public void EnumHasUnknown() {
            Assert.IsTrue(Enum.IsDefined(typeof(CropRegion), "UNKNOWN"));
        }
    }    
}
