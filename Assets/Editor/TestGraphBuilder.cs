#if !SHOPIFY_MONO_UNIT_TEST
namespace Shopify.Tests
{
    using System;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using Shopify.Unity.GraphQL;

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
            Assert.IsTrue(Utils.DoesExist("CustomerAccessTokenCreateInput"));
        }

        [Test]
        public void QueryRootBuildsQuery() {
            QueryRootQuery query = new QueryRootQuery();
            query.shop(s => s
                .name()
                .description()
            );

            Assert.AreEqual(
                "{shop {name description }}",
                query.ToString()
            );
        }

        [Test] 
        public void InputObjectToString() {
            CustomerAccessTokenCreateInput input = new CustomerAccessTokenCreateInput(email: "email@email.com", password: "123456");

            Assert.AreEqual(
                "{email:\"email@email.com\",password:\"123456\"}",
                input.ToString()
            );
        }

        [Test]
        public void MutationRootBuildsQuery() {
            MutationQuery query = new MutationQuery();

            query
            .customerAccessTokenCreate(r => r
                .customerAccessToken(a => a
                    .accessToken()
                ),
                input: new CustomerAccessTokenCreateInput(email: "email@email.com", password: "123456")
            );

            Assert.AreEqual(
                "mutation{customerAccessTokenCreate (input:{email:\"email@email.com\",password:\"123456\"}){customerAccessToken {accessToken }}}",
                query.ToString()
            );
            // check that ToString does not mutate
            Assert.AreEqual(
                "mutation{customerAccessTokenCreate (input:{email:\"email@email.com\",password:\"123456\"}){customerAccessToken {accessToken }}}",
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
            QueryRootQuery query = new QueryRootQuery();
            query.node(n => n
                .onProduct(p => p
                    .title()
                ),
                id: "12345"
            );

            Assert.AreEqual("{node (id:\"12345\"){__typename ...on Product{title }}}", query.ToString());
        }
    }    
}
#endif
