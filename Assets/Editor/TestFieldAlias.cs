namespace Shopify.Tests
{
    using NUnit.Framework;
    using Shopify.Unity;
   
    [TestFixture]
    public class TestFieldAlias {
        [Test]
        public void GenerateQueryWithAlias() {
            QueryRootQuery query = Root.buildQuery();
            query.shop(s => s
                .name()
                .withAlias("an_alias").description()
            );

            Assert.AreEqual(
                "{shop {name an_alias__description:description }}",
                query.ToString()
            );
        }

        [Test]
        public void ExceptionIsThrownForBlankAlias() {
            QueryRootQuery query = Root.buildQuery();
            AliasException caughtError = null;

            try {
                query.shop(s => s
                    .name()
                    .withAlias("").description()
                );
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.AreEqual("When using `withAlias` you must pass in a string which is the alias name", caughtError.Message);
        }

        [Test]
        public void ExceptionIsThrownForTwoWithAliasCalls() {
            QueryRootQuery query = Root.buildQuery();
            AliasException caughtError = null;

            try {
                query.shop(s => s
                    .name()
                    .withAlias("something").withAlias("more").description()
                );
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.AreEqual("You must call `withAlias` then select a field. ex. shop.withAlias(\"myShopName\").name()", caughtError.Message);
        }
    }
}