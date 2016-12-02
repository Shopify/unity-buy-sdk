namespace Shopify.Tests
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using Shopify.Unity;

    [TestFixture]
    public class TestGraphQLGenerator {
        [Test]
        public void generatedGraphQLEnums() {
            Assert.IsTrue(DoesExist("CropRegion"));
        }

        [Test]
        public void generatedGraphQLInterfaces() {
            Assert.IsTrue(DoesExist("Node"));
        }

        [Test]
        public void generatedGraphQLInputObjects() {
            Assert.IsTrue(DoesExist("ApiCustomerAccessTokenCreateInputInput"));
        }

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
        public void inputObjectToString() {
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

        private bool DoesExist(string className) {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            from type in assembly.GetTypes()
            where type.Name == className
            select type).Any();
        }
    }    
}
