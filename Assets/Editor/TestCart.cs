namespace Shopify.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;
    using Shopify.Unity.GraphQL;
    using System.Text.RegularExpressions;

    [TestFixture]
    public class TestCart {
        [SetUp]
        public void Setup() {
            #if (SHOPIFY_TEST)
            ShopifyBuy.Reset();
            #endif
        }

        [Test]
        public void TestGenericQuery() {
            ShopifyBuy.Init(new MockLoader());
            string cartWithId = "cartId";

            Assert.IsNotNull(ShopifyBuy.Client().Cart(), "created a cart");
            Assert.IsNotNull(ShopifyBuy.Client().Cart(cartWithId), "created a cart with id");
            Assert.AreNotEqual(ShopifyBuy.Client().Cart(), ShopifyBuy.Client().Cart(cartWithId));
        }

        [Test]
        public void TestAddRemoveLineItems() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            Dictionary<string,object> data = new Dictionary<string,object>();
            data.Add("id", "gid://shopify/ProductVariant/20756129347");

            Shopify.Unity.GraphQL.ProductVariant variant = new Shopify.Unity.GraphQL.ProductVariant(data);

            cart.LineItems.Set("gid://shopify/ProductVariant/20756129155", 33);
            cart.LineItems.Set(variant, 2);
            cart.LineItems.Set("gid://shopify/Product/7336568131");

            Assert.AreEqual(3, cart.LineItems.All().Count, "has 3 items in cart");
            Assert.AreEqual(33, cart.LineItems.Get("gid://shopify/ProductVariant/20756129155").quantity, "variant 20756129155 quantity is 33");
            Assert.AreEqual(2, cart.LineItems.Get("gid://shopify/ProductVariant/20756129347").quantity, "variant 20756129347 quantity is 2");
            Assert.AreEqual(1, cart.LineItems.Get("gid://shopify/Product/7336568131").quantity, "variant 7336568131 quantity is 1");

            bool didDelete = cart.LineItems.Delete("gid://shopify/ProductVariant/20756129155");
            Assert.AreEqual(2, cart.LineItems.All().Count, "After remove had 2 items in cart");
            Assert.IsTrue(didDelete, "returned true when deleting");

            didDelete = cart.LineItems.Delete("gid://shopify/ProductVariant/iamnotreal");
            Assert.IsFalse(didDelete, "returned false when did not delete");
        }

        [Test]
        public void TestPermalink() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.Set("gid://shopify/ProductVariant/20756129155", 33);
            cart.LineItems.Set("gid://shopify/ProductVariant/20756129347", 2);

            Assert.AreEqual("http://graphql.myshopify.com/cart/20756129155:33,20756129347:2", cart.GetWebCheckoutLink());
            Assert.AreEqual("http://graphql.myshopify.com/cart/20756129155:33,20756129347:2?note=i-am-a-note", cart.GetWebCheckoutLink("i-am-a-note"));
        }

        [Test]
        public void ModifyLineItems() {
            ShopifyBuy.Init(new MockLoader());

            string productId1 = "gid://shopify/ProductVariant/20756129155";
            string productId2 = "gid://shopify/ProductVariant/20756129347";
            List<AttributeInput> attributes1 = new List<AttributeInput>() {
                new AttributeInput("fancy", "i am fancy"),
                new AttributeInput("boring", "i am boring")
            };
            List<AttributeInput> attributes2 = new List<AttributeInput>() {
                new AttributeInput("animal", "lion"),
                new AttributeInput("spotted", "no")
            };

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.Set(productId1, 33);
            cart.LineItems.Set(productId2, 33);
            
            cart.LineItems.Set(productId1, 100);
            cart.LineItems.Set(productId1, null, attributes1);
            cart.LineItems.Set(productId2, 6, attributes2);

            Assert.AreEqual(100, cart.LineItems.Get(productId1).quantity, "variant 20756129155 quantity is 100 after change");
            Assert.AreEqual(6, cart.LineItems.Get(productId2).quantity, "variant 20756129155 quantity is 100 after change");
            Assert.AreEqual("i am fancy", cart.LineItems.Get(productId1).customAttributes[0].value);
            Assert.AreEqual("i am boring", cart.LineItems.Get(productId1).customAttributes[1].value);
            Assert.AreEqual("lion", cart.LineItems.Get(productId2).customAttributes[0].value);
            Assert.AreEqual("no", cart.LineItems.Get(productId2).customAttributes[1].value);
        }

        [Test]
        public void AddEditRemoveViaSelectedOptions() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            string jsonString = @"
                {
                    ""title"": ""Many Variant"",
                    ""options"": [
                        {
                        ""name"": ""Size"",
                        ""values"": [
                            ""Large"",
                            ""Small""
                        ]
                        },
                        {
                        ""name"": ""Color"",
                        ""values"": [
                            ""Red"",
                            ""Blue""
                        ]
                        }
                    ],
                    ""variants"": {
                        ""edges"": [
                        {
                            ""node"": {
                            ""id"": ""gid://shopify/ProductVariant/28472670531"",
                            ""selectedOptions"": [
                                {
                                ""name"": ""Size"",
                                ""value"": ""Large""
                                },
                                {
                                ""name"": ""Color"",
                                ""value"": ""Red""
                                }
                            ]
                            }
                        },
                        {
                            ""node"": {
                            ""id"": ""gid://shopify/ProductVariant/28472705027"",
                            ""selectedOptions"": [
                                {
                                ""name"": ""Size"",
                                ""value"": ""Large""
                                },
                                {
                                ""name"": ""Color"",
                                ""value"": ""Blue""
                                }
                            ]
                            }
                        },
                        {
                            ""node"": {
                            ""id"": ""gid://shopify/ProductVariant/28472705091"",
                            ""selectedOptions"": [
                                {
                                ""name"": ""Size"",
                                ""value"": ""Small""
                                },
                                {
                                ""name"": ""Color"",
                                ""value"": ""Red""
                                }
                            ]
                            }
                        },
                        {
                            ""node"": {
                            ""id"": ""gid://shopify/ProductVariant/28472705155"",
                            ""selectedOptions"": [
                                {
                                ""name"": ""Size"",
                                ""value"": ""Small""
                                },
                                {
                                ""name"": ""Color"",
                                ""value"": ""Blue""
                                }
                            ]
                            }
                        }
                        ]
                    }
                }
            ";

            Product product = new Product((Dictionary<string, object>) Json.Deserialize(jsonString));

            Dictionary<string, string> selectedOptions1 = new Dictionary<string, string>() {
                {"Size", "Small"},
                {"Color", "Red"}
            };

            Dictionary<string, string> selectedOptions2 = new Dictionary<string, string>() {
                {"Size", "Large"},
                {"Color", "Blue"}
            };

            Dictionary<string, string> selectedOptions3 = new Dictionary<string, string>() {
                {"Size", "Large"},
                {"Color", "Red"}
            };


            cart.LineItems.Set(product, selectedOptions1, 3);
            cart.LineItems.Set(product, selectedOptions2, 123);

            Assert.AreEqual(3, cart.LineItems.Get(product, selectedOptions1).quantity, "was able to set using selected options");
            Assert.AreEqual(123, cart.LineItems.Get(product, selectedOptions2).quantity, "was able to set using selected options");

            cart.LineItems.Set(product, selectedOptions1, 13);

            Assert.AreEqual(13, cart.LineItems.Get(product, selectedOptions1).quantity, "was able to reset using selected options");

            Assert.IsTrue(cart.LineItems.Delete(product, selectedOptions1), "returned true when line item was deleted");
            Assert.IsFalse(cart.LineItems.Delete(product, selectedOptions3), "returned false when no line item existed");

            Assert.AreEqual(null, cart.LineItems.Get(product, selectedOptions1), "Get retuned null after attempting access deleted line item");
        }
    }
}
