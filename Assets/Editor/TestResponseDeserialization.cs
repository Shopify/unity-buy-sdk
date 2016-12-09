namespace Shopify.Tests {
    using NUnit.Framework;
    using System.Collections.Generic;
    using Shopify.Unity;
    
    [TestFixture]
    public class TestResponseDeserialization {
        [Test]
        public void CanDeserializeResponse() {
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
    }
}