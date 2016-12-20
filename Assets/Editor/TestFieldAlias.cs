namespace Shopify.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;
   
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
                "{shop {name an_alias___description:description }}",
                query.ToString()
            );
        }

        [Test]
        public void ExceptionIsThrownForBlankAliasInQuery() {
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

        [Test]
        public void ExceptionIsThrownForInvalidAliasNameForQuery() {
            QueryRootQuery query = Root.buildQuery();
            AliasException caughtError = null;

            try {
                query.shop(s => s
                    .name()
                    .withAlias("$$$$").withAlias("more").description()
                );
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.AreEqual("Alias name was invalid format", caughtError.Message);
        }

        [Test]
        public void DeserializeAliasedField() {
            string stringJSON = @"{
                ""aliasName___name"": ""test-shop""
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            Shop response = new Shop(dataJSON);

            Assert.AreEqual("test-shop", response.withAlias("aliasName").name);
        }

        [Test]
        public void ThrowsExceptionAliasedFieldWhichWasNotQueried() {
            string stringJSON = @"{
                ""name"": ""test-shop""
            }";

            NoQueryException caughtError = null;
            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            Shop response = new Shop(dataJSON);

            try {
                CurrencyCode code = response.withAlias("aliasName").currencyCode;
            } catch(NoQueryException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("It looks like you did not query the field `currencyCode` with alias `aliasName`", caughtError.Message);
        }

        [Test]
        public void ExceptionIsThrownForInvalidAliasNameForResponse() {
            AliasException caughtError = null;
            string stringJSON = @"{
                ""aliasName___name"": ""test-shop""
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            Shop response = new Shop(dataJSON);

            try {
                Assert.AreEqual("test-shop", response.withAlias("$$$").name);
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("Alias name was invalid format", caughtError.Message);
        }

        [Test]
        public void ExceptionIsThrownForBlankAliasInResponse() {
            AliasException caughtError = null;
            string stringJSON = @"{
                ""aliasName___name"": ""test-shop""
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            Shop response = new Shop(dataJSON);

            try {
                Assert.AreEqual("test-shop", response.withAlias("").name);
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("When using `withAlias` you must pass in a string which is the alias name", caughtError.Message);
        }
    }
}