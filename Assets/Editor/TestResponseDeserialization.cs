namespace Shopify.Tests {
    using NUnit.Framework;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;
    
    [TestFixture]
    public class TestResponseDeserialization {
        [Test]
        public void CanDeserializeBasic() {
            string shopName = "test-shop";

            Dictionary<string,object> dataJSONShop = new Dictionary<string,object>() {
                { "name", shopName }   
            }; 

            Dictionary<string,object> dataJSONQueryRoot = new Dictionary<string,object>() {
                { "shop", dataJSONShop }   
            };            

            Dictionary<string,object> dataJSON = new Dictionary<string,object>() {
                { "data", dataJSONQueryRoot }   
            }; 

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual(shopName, response.shop.name);
        }

        [Test]
        public void CanDeserializeEnum() {
            Dictionary<string,object> dataJSONShop = new Dictionary<string,object>() {
                { "currencyCode", "CAD" }   
            }; 

            Dictionary<string,object> dataJSONQueryRoot = new Dictionary<string,object>() {
                { "shop", dataJSONShop }   
            };            

            Dictionary<string,object> dataJSON = new Dictionary<string,object>() {
                { "data", dataJSONQueryRoot }   
            }; 

            QueryRoot response = new QueryRoot(dataJSON);

            Assert.AreEqual(CurrencyCode.CAD, response.shop.currencyCode);
        }

        [Test]
        public void WillReturnUnkownEnum() {
            Dictionary<string,object> dataJSONShop = new Dictionary<string,object>() {
                { "currencyCode", "DOESNT EXIST" }   
            }; 

            Dictionary<string,object> dataJSONQueryRoot = new Dictionary<string,object>() {
                { "shop", dataJSONShop }   
            };            

            Dictionary<string,object> dataJSON = new Dictionary<string,object>() {
                { "data", dataJSONQueryRoot }   
            }; 

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