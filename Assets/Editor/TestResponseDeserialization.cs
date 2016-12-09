namespace Shopify.Tests {
    using NUnit.Framework;
    using System.Collections.Generic;
    using Shopify.Unity;
    
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
            string shopName = "test-shop";

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
            string shopName = "test-shop";

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
    }
}