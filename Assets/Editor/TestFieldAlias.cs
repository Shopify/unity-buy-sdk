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
            QueryRootQuery query = new QueryRootQuery();
            query.customerAddress(
                a => a.city(),
                alias: "an_alias", id: "1234"
            );

            Assert.AreEqual(
                @"{an_alias___customerAddress:customerAddress (id:""1234""){city }}",
                query.ToString()
            );
        }

        [Test]
        public void ExceptionIsThrownForBlankAliasInQuery() {
            AliasException caughtError = null;
            QueryRootQuery query = new QueryRootQuery();
            
            try {
                query.customerAddress(
                    a => a.city(),
                    alias: "", id: "1234"
                );
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("`alias` cannot be a blank string", caughtError.Message);
        }

        [Test]
        public void ExceptionIsThrownForInvalidAliasNameForQuery() {
            QueryRootQuery query = new QueryRootQuery();
            AliasException caughtError = null;

            try {
                query.customerAddress(
                    a => a.city(),
                    alias: "$$$", id: "1234"
                );
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("`alias` was invalid format", caughtError.Message);
        }

        [Test]
        public void DeserializeAliasedField() {
            string stringJSON = @"{
                ""aliasName___customerAddress"": {
                    ""city"": ""Toronto""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            QueryRoot response = new QueryRoot(dataJSON);
            
            Assert.AreEqual("Toronto", response.customerAddress(alias: "aliasName").city());
        }

        [Test]
        public void ThrowsExceptionAliasedFieldWhichWasNotQueried() {
            string stringJSON = @"{}";

            NoQueryException caughtError = null;
            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            QueryRoot response = new QueryRoot(dataJSON);

            try {
                response.customerAddress(alias: "aliasName").city();
            } catch(NoQueryException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("It looks like you did not query the field `customerAddress` with alias `aliasName`", caughtError.Message);
        }

        [Test]
        public void ExceptionIsThrownForInvalidAliasNameForResponse() {
            string stringJSON = @"{
                ""aliasName___customerAddress"": {
                    ""city"": ""Toronto""
                }
            }";

            AliasException caughtError = null;
            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            QueryRoot response = new QueryRoot(dataJSON);

            try {
                response.customerAddress(alias: "$$$").city();
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("`alias` was invalid format", caughtError.Message);
        }

        [Test]
        public void ExceptionIsThrownForBlankAliasInResponse() {
            string stringJSON = @"{
                ""aliasName___customerAddress"": {
                    ""city"": ""Toronto""
                }
            }";

            AliasException caughtError = null;
            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            QueryRoot response = new QueryRoot(dataJSON);

            try {
                response.customerAddress(alias: "").city();
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("`alias` cannot be a blank string", caughtError.Message);
        }
    }
}