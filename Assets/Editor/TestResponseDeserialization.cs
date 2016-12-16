namespace Shopify.Tests {
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;
    
    [TestFixture]
    public class TestResponseDeserialization {
        [Test]
        public void TopLevelReturnsData() {
            string stringJSON = @"{
                ""data"": {
                    ""shop"": { 
                        ""name"": ""test-shop""
                    }
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            TopLevelResponse response = new TopLevelResponse(dataJSON);

            Assert.IsNotNull(response.data);
            Assert.IsNull(response.errors);
        }

        [Test]
        public void TopLevelHandlesStringExceptions() {
            string stringJSON = @"{
                ""errors"": ""Parameter Missing or Invalid""
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            TopLevelResponse response = new TopLevelResponse(dataJSON);

            Assert.IsNull(response.data);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual("Parameter Missing or Invalid", response.errors[0]);
        }

        [Test]
        public void TopLevelHandlesListExceptions() {
            string stringJSON = @"{
                ""errors"": [
                    {
                        ""message"": ""Field 'doesntExist' doesn't exist on type 'Shop'"",
                        ""locations"": [
                            {
                                ""line"": 3,
                                ""column"": 5
                            }
                        ],
                        ""fields"": [
                            ""query"",
                            ""shop"",
                            ""doesntExist""
                        ]
                    }
                ]
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            TopLevelResponse response = new TopLevelResponse(dataJSON);

            Assert.IsNull(response.data);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual("Field 'doesntExist' doesn't exist on type 'Shop'", response.errors[0]);
        }

        [Test]
        public void CanDeserializeBasic() {
            string stringJSON = @"{
                ""shop"": { 
                    ""name"": ""test-shop""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual("test-shop", response.shop.name);
        }

        [Test]
        public void CanDeserializeEnum() {
            string stringJSON = @"{
                ""shop"": { 
                    ""currencyCode"": ""CAD""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual(CurrencyCode.CAD, response.shop.currencyCode);
        }

        [Test]
        public void WillReturnUnkownEnum() {
            string stringJSON = @"{
                ""shop"": { 
                    ""currencyCode"": ""I AM NOT REAL""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual(CurrencyCode.UNKNOWN, response.shop.currencyCode);
        }

        [Test]
        public void CanDeserializeLists() {
            string stringJSON = @"{
                ""product"": {
                    ""tags"": [
                        ""blue"",
                        ""button"",
                        ""fancy""
                    ]
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
                ""product"": {
                    ""tags"": [
                        ""blue"",
                        null,
                        ""fancy""
                    ]
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            CollectionAssert.AreEqual(
                new List<string>() {"blue", null, "fancy"},
                response.product.tags
            );
        }

        [Test]
        public void AccessingFieldNotQueriedThrowsException() {
            Exception error = null;
            string stringJSON = @"{
                ""shop"": { 
                    ""name"": ""test-shop""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot response = new QueryRoot(dataJSON);

            try {
                CurrencyCode code = response.shop.currencyCode;
            } catch(NoQueryException e) {
                error = e;
            }

            Assert.IsNotNull(error);
            Assert.AreEqual("It looks like you did not query the field: currencyCode", error.Message);
        }
    }
}