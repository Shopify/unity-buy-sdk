namespace Shopify.Tests {
    using NUnit.Framework;
    using System.Collections.Generic;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;

    [TestFixture]
    public class TestRelay {
        [Test]
        public void TestCastConnectionToList() {
            string stringJSON = @"{
                ""edges"": [ 
                    {
                        ""node"": {
                            ""title"": ""Product 1""
                        }
                    },
                    {
                        ""node"": {
                            ""title"": ""Product 2""
                        }
                    }
                ]
            }";

            ProductConnection connection = new ProductConnection((Dictionary<string, object>) Json.Deserialize(stringJSON));
            List<Product> products = (List<Product>) connection;
            bool couldIterateOverConnection = false;

            foreach (Product product in products) {
                couldIterateOverConnection = true;
            }

            Assert.IsNotNull(products, "products is not null");
            Assert.AreEqual(2, products.Count, "products list has two products");
            Assert.AreEqual("Product 2", products[1].title(), "List had data");
            Assert.IsTrue(couldIterateOverConnection, "could iterate over connection");
        }
    }
}