namespace Shopify.Tests
{
    using System;
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
            data.Add("id", "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==");
            
            ProductVariant variant = new ProductVariant(data);
            
            cart.LineItems.AddOrUpdate("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==", 33);
            cart.LineItems.AddOrUpdate(variant, 2);
            cart.LineItems.AddOrUpdate("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0LzczMzY1NjgxMzE=");
            
            Assert.AreEqual(3, cart.LineItems.All().Count, "has 3 items in cart");
            Assert.AreEqual(33, cart.LineItems.Get("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==").Quantity, "variant 20756129155 Quantity is 33");
            Assert.AreEqual(2, cart.LineItems.Get("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==").Quantity, "variant 20756129347 Quantity is 2");
            Assert.AreEqual(1, cart.LineItems.Get("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0LzczMzY1NjgxMzE=").Quantity, "variant 7336568131 Quantity is 1");
            Assert.AreEqual(null, cart.LineItems.Get("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0LzczMzY1NjgxMzE=").ID, "variant 7336568131 ID is null");
            
            bool didDelete = cart.LineItems.Delete("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==");
            Assert.AreEqual(2, cart.LineItems.All().Count, "After remove had 2 items in cart");
            Assert.IsTrue(didDelete, "returned true when deleting");
            
            didDelete = cart.LineItems.Delete("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC9pYW1ub3RyZWFs");
            Assert.IsFalse(didDelete, "returned false when did not delete");
        }
        
        [Test]
        public void TestCastingLineItems() {
            ShopifyBuy.Init(new MockLoader());
            
            Cart cart = ShopifyBuy.Client().Cart();
            string variandId = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==";
            
            cart.LineItems.AddOrUpdate(variandId, 33, new Dictionary<string, string>() {
                {"fun", "things"}
            });
            
            CheckoutLineItemInput input = (CheckoutLineItemInput) cart.LineItems.Get(variandId);
            CheckoutLineItemUpdateInput updateInput = (CheckoutLineItemUpdateInput) cart.LineItems.Get(variandId);
            
            Assert.IsNotNull(input);
            Assert.IsNotNull(updateInput);
            Assert.IsNotNull(input.customAttributes);
            Assert.IsNotNull(updateInput.customAttributes);
            Assert.AreEqual("fun", input.customAttributes[0].key);
            Assert.AreEqual("fun", updateInput.customAttributes[0].key);
            Assert.AreEqual("things", input.customAttributes[0].value);
            Assert.AreEqual("things", updateInput.customAttributes[0].value);
        }
        
        [Test]
        public void TestModifyingCustomAttributesMadeLineNotBeSaved() {
            ShopifyBuy.Init(new MockLoader());
            
            Cart cart = ShopifyBuy.Client().Cart();
            string variandId = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==";
            Dictionary<string,object> dataCheckoutLineItem = new Dictionary<string,object>() {
                {"id", "a-checkout-id"},
                {
                    "variant", new Dictionary<string, object>() { 
                        {"id", "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ=="} 
                    }
                } 
            };
            List<CheckoutLineItem> checkoutLineItems = new List<CheckoutLineItem>() {
                { new CheckoutLineItem(dataCheckoutLineItem) }
            };
            
            cart.LineItems.AddOrUpdate(variandId, 33, new Dictionary<string, string>() {
                {"fun", "things"}
            });
            
            Assert.IsFalse(cart.LineItems.IsSaved, "initially is not saved");
            
            cart.LineItems.Get(variandId).GetCheckoutLineItemUpdateInput();
            
            cart.LineItems.UpdateLineItemsFromCheckoutLineItems(checkoutLineItems);

            Assert.IsTrue(cart.LineItems.IsSaved, "after get input is saved");
            
            cart.LineItems.Get(variandId).CustomAttributes["fun"] = "stuff";
            
            Assert.IsFalse(cart.LineItems.IsSaved, "after change is not saved");
        }
        
        [Test]
        public void TestGetCheckoutLink() {
            ShopifyBuy.Init(new MockLoader());
            
            Cart cart = ShopifyBuy.Client().Cart();
            string responseURL = null;
            string responseHttpError = null;
            List<string> responseErrors = null;

            cart.LineItems.AddOrUpdate("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==", 33);
            cart.LineItems.AddOrUpdate("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==", 2);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    responseURL = url;
                },
                failure: (errors, httpError) => {
                    responseHttpError = httpError;
                    responseErrors = errors;
                }
            );

            Assert.IsNull(responseHttpError);
            Assert.IsNull(responseErrors);
            Assert.AreEqual("http://shopify.com/checkout-no-poll", responseURL, "weblink was correct");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set");
            Assert.AreEqual("line-item-id2", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set");
        }

        [Test]
        public void ModifyLineItems() {
            ShopifyBuy.Init(new MockLoader());
            
            string productId1 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==";
            string productId2 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==";
            Dictionary<string, string> attributes1 = new Dictionary<string, string>() {
                {"fancy", "i am fancy"},
                {"boring", "i am boring"}
            };
            Dictionary<string, string> attributes2 = new Dictionary<string, string>() {
                {"animal", "lion"},
                {"spotted", "no"}
            };
            
            Cart cart = ShopifyBuy.Client().Cart();
            
            cart.LineItems.AddOrUpdate(productId1, 33);
            cart.LineItems.AddOrUpdate(productId2, 33);
            
            cart.LineItems.AddOrUpdate(productId1, 100);
            cart.LineItems.AddOrUpdate(productId1, null, attributes1);
            cart.LineItems.AddOrUpdate(productId2, 6, attributes2);
            
            Assert.AreEqual(100, cart.LineItems.Get(productId1).Quantity, "variant 20756129155 Quantity is 100 after change");
            Assert.AreEqual(6, cart.LineItems.Get(productId2).Quantity, "variant 20756129155 Quantity is 100 after change");
            Assert.AreEqual("i am fancy", cart.LineItems.Get(productId1).CustomAttributes["fancy"]);
            Assert.AreEqual("i am boring", cart.LineItems.Get(productId1).CustomAttributes["boring"]);
            Assert.AreEqual("lion", cart.LineItems.Get(productId2).CustomAttributes["animal"]);
            Assert.AreEqual("no", cart.LineItems.Get(productId2).CustomAttributes["spotted"]);
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
                            ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yODQ3MjY3MDUzMQ=="",
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
                            ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yODQ3MjcwNTAyNw=="",
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
                            ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yODQ3MjcwNTA5MQ=="",
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
                            ""id"": ""Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yODQ3MjcwNTE1NQ=="",
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
            
            
            cart.LineItems.AddOrUpdate(product, selectedOptions1, 3);
            cart.LineItems.AddOrUpdate(product, selectedOptions2, 123);
            
            Assert.AreEqual(3, cart.LineItems.Get(product, selectedOptions1).Quantity, "was able to set using selected options");
            Assert.AreEqual(123, cart.LineItems.Get(product, selectedOptions2).Quantity, "was able to set using selected options");
            
            cart.LineItems.AddOrUpdate(product, selectedOptions1, 13);
            
            Assert.AreEqual(13, cart.LineItems.Get(product, selectedOptions1).Quantity, "was able to reset using selected options");
            
            Assert.IsTrue(cart.LineItems.Delete(product, selectedOptions1), "returned true when line item was deleted");
            Assert.IsFalse(cart.LineItems.Delete(product, selectedOptions3), "returned false when no line item existed");
            
            Assert.AreEqual(null, cart.LineItems.Get(product, selectedOptions1), "Get retuned null after attempting access deleted line item");
        }
        
        [Test]
        public void TestGetCheckoutLinkWithPoll() {
            ShopifyBuy.Init(new MockLoader());
            
            Cart cart = ShopifyBuy.Client().Cart();
            string responseURL = null;
            string responseHttpError = null;
            List<string> responseErrors = null;

            cart.LineItems.AddOrUpdate("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTPOLL==", 33);
            cart.LineItems.AddOrUpdate("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==", 2);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    responseURL = url;
                },
                failure: (errors, httpError) => {
                    responseHttpError = httpError;
                    responseErrors = errors;
                }
            );

            Assert.IsNull(responseHttpError);
            Assert.IsNull(responseErrors);
            Assert.AreEqual("http://shopify.com/checkout-with-poll-after-poll", responseURL, "weblink was correct");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set");
            Assert.AreEqual("line-item-id2", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set");
        }
        
        [Test]
        public void TestGetCheckoutLinkAfterLineItemModifications() {
            ShopifyBuy.Init(new MockLoader());
            
            Cart cart = ShopifyBuy.Client().Cart();
            string firstResponseURL = null;
            string firstResponseHttpError = null;
            List<string> firstResponseErrors = null;
            string secondResponseURL = null;
            string secondResponseHttpError = null;
            List<string> secondResponseErrors = null;

            string variantId1 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate==";
            string variantId2 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyDelete==";
            string variantId3 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NTMNewItem==";

            cart.LineItems.AddOrUpdate(variantId1, 33);
            cart.LineItems.AddOrUpdate(variantId2, 2);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    firstResponseURL = url;
                },
                failure: (errors, httpError) => {
                    firstResponseHttpError = httpError;
                    firstResponseErrors = errors;
                }
            );

            Assert.IsNull(firstResponseHttpError);
            Assert.IsNull(firstResponseErrors);
            Assert.AreEqual("http://shopify.com/checkout-create-before-update", firstResponseURL, "weblink was correct");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set");
            Assert.AreEqual("line-item-id2", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set");

            // update item
            cart.LineItems.AddOrUpdate(variantId1, 10);
            // delete item
            cart.LineItems.Delete(variantId2);
            // new item
            cart.LineItems.AddOrUpdate(variantId3, 3);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    secondResponseURL = url;
                },
                failure: (errors, httpError) => {
                    secondResponseHttpError = httpError;
                    secondResponseErrors = errors;
                }
            );

            Assert.IsNull(secondResponseHttpError);
            Assert.IsNull(secondResponseErrors);
            Assert.AreEqual("http://shopify.com/checkout-create-after-update", secondResponseURL, "weblink was correct after update");
            Assert.AreEqual(2, cart.LineItems.All().Count, "Had two line items after update");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set after update");
            Assert.AreEqual("line-item-id3", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set after update");
        }
    }
}
