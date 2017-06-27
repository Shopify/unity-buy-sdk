namespace Shopify.Tests {
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;
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

            Assert.IsNotNull(response.DataJSON);
            Assert.IsNull(response.errors);
        }

        [Test]
        public void TopLevelHandlesStringExceptions() {
            string stringJSON = @"{
                ""errors"": ""Parameter Missing or Invalid""
            }";

            InvalidServerResponseException caughtError = null;
            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            try {
                new TopLevelResponse(dataJSON);
            } catch(InvalidServerResponseException error) {
                caughtError = error;
            }

            Assert.IsNotNull(caughtError);
            Assert.AreEqual("Response JSON at `errors` did not contain an Array of Object's", caughtError.Message);
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

            Assert.IsNull(response.DataJSON);
            Assert.IsNotNull(response.errors);
            Assert.AreEqual("[\"Field 'doesntExist' doesn't exist on type 'Shop'\"]", response.errors);
        }

        [Test]
        public void TopLevelCastableToShopifyError() {
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

            Assert.IsNull(response.DataJSON);
            Assert.IsNotNull(response.errors);

            ShopifyError error = (ShopifyError) response;
            Assert.IsNotNull(error);
            Assert.IsTrue(error is ShopifyError);
            Assert.AreEqual(ShopifyError.ErrorType.GraphQL, error.error);
            Assert.AreEqual("[\"Field 'doesntExist' doesn't exist on type 'Shop'\"]", error.description);
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

            Assert.AreEqual("test-shop", response.shop().name());
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

            Assert.AreEqual(CurrencyCode.CAD, response.shop().currencyCode());
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

            Assert.AreEqual(CurrencyCode.UNKNOWN, response.shop().currencyCode());
        }

        [Test]
        public void CanDeserializeLists() {
            string stringJSON = @"{
                ""node"": {
                    ""__typename"": ""Product"",
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
                ((Product) response.node()).tags()
            );
        }

        [Test]
        public void CanDeserializeListsWithNull() {
            string stringJSON = @"{
                ""node"": {
                    ""__typename"": ""Product"",
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
                ((Product) response.node()).tags()
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
                response.shop().currencyCode();
            } catch(NoQueryException e) {
                error = e;
            }

            Assert.IsNotNull(error);
            Assert.AreEqual("It looks like you did not query the field `currencyCode`", error.Message);
        }

        [Test]
        public void CanDeserializeInterfaces() {
            string stringJSON = @"{
                ""node"": { 
                    ""__typename"": ""Product"",
                    ""title"": ""I AM A PRODUCT""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot query = new QueryRoot(dataJSON);

            Assert.IsTrue(query.node() is Node);
            Assert.IsTrue(query.node() is Product);
        }

        [Test]
        public void InterfaceCanDeseralizeUnknownType() {
            string stringJSON = @"{
                ""node"": { 
                    ""__typename"": ""IAmNotReal"",
                    ""title"": ""I AM A PRODUCT""
                }
            }";

            Dictionary<string,object> dataJSON = (Dictionary<string,object>) Json.Deserialize(stringJSON);

            QueryRoot query = new QueryRoot(dataJSON);

            Assert.IsTrue(query.node() is Node);
            Assert.IsTrue(query.node() is UnknownNode);
        }

        [Test]
        public void TestCustomScalarDateTime() {
            string json = @"{
                ""updatedAt"": ""2016-09-11T21:32:43Z""
            }";

            Collection collection = new Collection((Dictionary<string,object>) Json.Deserialize(json));
            DateTimeOffset date = new DateTimeOffset(collection.updatedAt());
            date = date.ToUniversalTime();

            Assert.AreEqual(2016, date.Year);
            Assert.AreEqual(9, date.Month);
            Assert.AreEqual(11, date.Day);
            Assert.AreEqual(21, date.Hour); 
            Assert.AreEqual(32, date.Minute);
            Assert.AreEqual(43, date.Second);
        }


        [Test]
        public void TestCustomScalarMoney() {
            string json = @"{
                ""price"": ""3.23""
            }";

            ProductVariant variant = new ProductVariant((Dictionary<string,object>) Json.Deserialize(json));
        
            Assert.AreEqual(3.23M, variant.price());
        }
    }
}
