namespace Shopify.Tests {
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;
    
    [TestFixture]
    public class TestResponseDeserialization {
        [Test]
        public void CanDeserializeBasic() {
            string stringJSON = @"{
                ""data"": {
                    ""shop"": { 
                        ""name"": ""test-shop""
                    }
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual("test-shop", response.shop.name);
        }

        [Test]
        public void CanDeserializeEnum() {
            string stringJSON = @"{
                ""data"": {
                    ""shop"": { 
                        ""currencyCode"": ""CAD""
                    }
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual(CurrencyCode.CAD, response.shop.currencyCode);
        }

        [Test]
        public void WillReturnUnkownEnum() {
            string stringJSON = @"{
                ""data"": {
                    ""shop"": { 
                        ""currencyCode"": ""I AM NOT REAL""
                    }
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual(CurrencyCode.UNKNOWN, response.shop.currencyCode);
        }

        [Test]
        public void CanDeserializeLists() {
            string stringJSON = @"{
              ""data"": {
                ""product"": {
                  ""tags"": [
                    ""blue"",
                    ""button"",
                    ""fancy""
                  ]
                }
              }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            CollectionAssert.AreEqual(
                new List<string>() {"blue", "button", "fancy"},
                response.product.tags
            );
        }

        [Test]
        public void CanDeserializeListsWithNull() {
            string stringJSON = @"{
              ""data"": {
                ""product"": {
                  ""tags"": [
                    ""blue"",
                    null,
                    ""fancy""
                  ]
                }
              }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            CollectionAssert.AreEqual(
                new List<string>() {"blue", null, "fancy"},
                response.product.tags
            );
        }
    }
}