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
            query.shop(s => s
                .name()
                .description()
            );

            Assert.AreEqual(
                "{shop{name description}}",
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
                "mutation{apiCustomerAccessTokenCreate(input:{email:\"email@email.com\",password:\"123456\",clientMutationId:\"333\"}){apiCustomerAccessToken{accessToken}clientMutationId}}",
                query.ToString()
            );
            // check that ToString does not mutate
            Assert.AreEqual(
                "mutation{apiCustomerAccessTokenCreate(input:{email:\"email@email.com\",password:\"123456\",clientMutationId:\"333\"}){apiCustomerAccessToken{accessToken}clientMutationId}}",
                query.ToString()
            );
        }

        [Test]
        public void EnumHasUnknown() {
            Assert.IsTrue(Enum.IsDefined(typeof(CropRegion), "UNKNOWN"));
        }

        [Test]
        public void ArgumentToString() {
            Arguments args = new Arguments();

            args.Add("firstArg", 0);
            args.Add("secondArg", "I AM SECOND");

            Assert.AreEqual("(firstArg:0,secondArg:\"I AM SECOND\")", args.ToString());
            // this is to test that ToString does not mutate
            Assert.AreEqual("(firstArg:0,secondArg:\"I AM SECOND\")", args.ToString()); 
        }

        [Test]
        public void UsingInlineFragment() {
            QueryRootQuery query = Root.buildQuery().node(n => n
                .onProduct(p => p
                    .title()
                ),
                id: "12345"
            );

            Assert.AreEqual("{node(id:\"12345\"){...on Product{title}}}", query.ToString());
        }
    }    
}
