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
            query.node(
                node => node.id(),
                alias: "an_alias", id: "1234"
            );

            Assert.AreEqual(
                @"{an_alias___node:node (id:""1234""){__typename id }}",
                query.ToString()
            );
        }

        [Test]
        public void ExceptionIsThrownForBlankAliasInQuery() {
            AliasException caughtError = null;
            QueryRootQuery query = new QueryRootQuery();
            
            try {
                query.node(
                    node => node.id(),
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
                query.node(
                    node => node.id(),
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
                ""aliasName___product"": {
                    ""title"": ""This is a product""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            QueryRoot response = new QueryRoot(dataJSON);
            
            Assert.AreEqual("This is a product", response.product(alias: "aliasName").title());
        }

        [Test]
        public void ThrowsExceptionAliasedFieldWhichWasNotQueried() {
            string stringJSON = @"{}";

            NoQueryException caughtError = null;
            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            QueryRoot response = new QueryRoot(dataJSON);

            try {
                response.node(alias: "aliasName").id();
            } catch(NoQueryException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("It looks like you did not query the field `node` with alias `aliasName`", caughtError.Message);
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
                response.node(alias: "$$$").id();
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("`alias` was invalid format", caughtError.Message);
        }

        [Test]
        public void ExceptionIsThrownForBlankAliasInResponse() {
            string stringJSON = @"{
                ""aliasName___product"": {
                    ""title"": ""This is a product""
                }
            }";

            AliasException caughtError = null;
            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);
            
            QueryRoot response = new QueryRoot(dataJSON);

            try {
                response.product(alias: "").title();
            } catch(AliasException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("`alias` cannot be a blank string", caughtError.Message);
        }
    }
}