namespace Shopify.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;
    using Shopify.Unity.GraphQL;
    using Shopify.Unity.SDK;
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
        public void TestReset() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            // Add something to the cart.
            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ=="), 33);
            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw=="), 2);

            Assert.AreEqual(2, cart.LineItems.All().Count, "cart has 2 line items");

            // Start a checkout.
            string responseURL = null;
            ShopifyError error = null;
            cart.GetWebCheckoutLink(
                success: (url) => {
                    responseURL = url;
                },
                failure: (shopError) => {
                    error = shopError;
                }
            );

            Assert.IsNotNull(responseURL, "Was able to generate a web checkout url");
            Assert.IsNull(error, "No error from getting web checkout");
            Assert.IsNotNull(cart.CurrentCheckout, "Cart has a checkout");

            cart.Reset();

            // Verify that we cleared out the line items and checkout.
            Assert.AreEqual(0, cart.LineItems.All().Count, "has 0 items in cart");
            Assert.IsNull(cart.CurrentCheckout, "cart has a null Checkout");
        }

        [Test]
        public void TestSubtotal() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ=="), 33);

            Assert.AreEqual(33, cart.Subtotal());

            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw=="), 2);

            Assert.AreEqual(35, cart.Subtotal());

            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0LzczMzY1NjgxMzE=", 3.00m));

            Assert.AreEqual(38, cart.Subtotal());
        }

        [Test]
        public void TestAddRemoveLineItems() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ=="), 33);
            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw=="), 2);
            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0LzczMzY1NjgxMzE="));

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

            var variantId = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==";
            var variant = CreateProductVariant(variantId);

            cart.LineItems.AddOrUpdate(variant, 33, new Dictionary<string, string>() {
                {"fun", "things"}
            });

            CheckoutLineItemInput input = (CheckoutLineItemInput) cart.LineItems.Get(variantId);
            CheckoutLineItemUpdateInput updateInput = (CheckoutLineItemUpdateInput) cart.LineItems.Get(variantId);

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
            string variantId = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==";
            var variant = CreateProductVariant(variantId);
            Dictionary<string,object> dataCheckoutLineItem = new Dictionary<string,object>() {
                {"id", "a-checkout-id"},
                {
                    "variant", new Dictionary<string, object>() {
                        {"id", "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ=="},
                        {"price", "1.00"},
                    }
                }
            };
            List<CheckoutLineItem> checkoutLineItems = new List<CheckoutLineItem>() {
                { new CheckoutLineItem(dataCheckoutLineItem) }
            };

            cart.LineItems.AddOrUpdate(variant, 33, new Dictionary<string, string>() {
                {"fun", "things"}
            });

            Assert.IsFalse(cart.LineItems.IsSaved, "initially is not saved");

            cart.LineItems.Get(variantId).GetCheckoutLineItemUpdateInput();

            cart.LineItems.UpdateLineItemsFromCheckoutLineItems(checkoutLineItems);

            Assert.IsTrue(cart.LineItems.IsSaved, "after get input is saved");

            cart.LineItems.Get(variantId).CustomAttributes["fun"] = "stuff";

            Assert.IsFalse(cart.LineItems.IsSaved, "after change is not saved");
        }

        [Test]
        public void TestGetCheckoutLink() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();
            string responseURL = null;
            ShopifyError error = null;

            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ=="), 33);
            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw=="), 2);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    responseURL = url;
                },
                failure: (shopError) => {
                    error = shopError;
                }
            );

            Assert.IsNull(error);
            Assert.AreEqual("http://shopify.com/checkout-no-poll", responseURL, "weblink was correct");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set");
            Assert.AreEqual("line-item-id2", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set");
        }

        [Test]
        public void ModifyLineItems() {
            ShopifyBuy.Init(new MockLoader());

            string variantId1 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTE1NQ==";
            var variant1 = CreateProductVariant(variantId1);

            string variantId2 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw==";
            var variant2 = CreateProductVariant(variantId2);

            Dictionary<string, string> attributes1 = new Dictionary<string, string>() {
                {"fancy", "i am fancy"},
                {"boring", "i am boring"}
            };
            Dictionary<string, string> attributes2 = new Dictionary<string, string>() {
                {"animal", "lion"},
                {"spotted", "no"}
            };

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.AddOrUpdate(variant1, 33);
            cart.LineItems.AddOrUpdate(variant2, 33);

            cart.LineItems.AddOrUpdate(variant1, 100);
            cart.LineItems.AddOrUpdate(variant1, null, attributes1);
            cart.LineItems.AddOrUpdate(variant2, 6, attributes2);

            Assert.AreEqual(100, cart.LineItems.Get(variantId1).Quantity, "variant 20756129155 Quantity is 100 after change");
            Assert.AreEqual(6, cart.LineItems.Get(variantId2).Quantity, "variant 20756129155 Quantity is 100 after change");
            Assert.AreEqual("i am fancy", cart.LineItems.Get(variantId1).CustomAttributes["fancy"]);
            Assert.AreEqual("i am boring", cart.LineItems.Get(variantId1).CustomAttributes["boring"]);
            Assert.AreEqual("lion", cart.LineItems.Get(variantId2).CustomAttributes["animal"]);
            Assert.AreEqual("no", cart.LineItems.Get(variantId2).CustomAttributes["spotted"]);
        }

        [Test]
        public void AddOrUpdateWithVariantId() {
            ShopifyBuy.Init(new MockLoader());

            string variantId1 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8xMTI0Mzc=";

            Cart cart = ShopifyBuy.Client().Cart();

            cart.LineItems.AddOrUpdate(variantId1, 100);

            Assert.AreEqual(100, cart.LineItems.Get(variantId1).Quantity, "variant 20756129155 Quantity is 100 after change");

            NoMatchingVariantException exception = null;

            try {
                cart.LineItems.AddOrUpdate("error", 100);
            } catch (NoMatchingVariantException e) {
                exception = e;
            }

            Assert.IsNotNull(exception);
            Assert.AreEqual("Could not `AddOrUpdate` line item as no matching variant could be found for given id", exception.Message);
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
                            ""price"": ""1.00"",
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
                            ""price"": ""1.00"",
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
                            ""price"": ""1.00"",
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
                            ""price"": ""1.00"",
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
            ShopifyError error = null;

            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTPOLL=="), 33);
            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyOTM0Nw=="), 2);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    responseURL = url;
                },
                failure: (shopError) => {
                    error = shopError;
                }
            );

            Assert.IsNull(error);
            Assert.AreEqual("http://shopify.com/checkout-with-poll-after-poll", responseURL, "weblink was correct");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set");
            Assert.AreEqual("line-item-id2", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set");
        }

        [Test]
        public void TestGetCheckoutLinkAfterLineItemModifications() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();
            string firstResponseURL = null;
            ShopifyError firstError = null;
            string secondResponseURL = null;
            ShopifyError secondError = null;

            string variantId1 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyUpdate==";
            string variantId2 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NjEyDelete==";
            string variantId3 = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0VmFyaWFudC8yMDc1NTMNewItem==";

            cart.LineItems.AddOrUpdate(CreateProductVariant(variantId1), 33);
            cart.LineItems.AddOrUpdate(CreateProductVariant(variantId2), 2);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    firstResponseURL = url;
                },
                failure: (shopError) => {
                    firstError = shopError;
                }
            );

            Assert.IsNull(firstError);
            Assert.AreEqual("http://shopify.com/checkout-create-before-update", firstResponseURL, "weblink was correct");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set");
            Assert.AreEqual("line-item-id2", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set");

            // update item
            cart.LineItems.AddOrUpdate(CreateProductVariant(variantId1), 10);
            // delete item
            cart.LineItems.Delete(variantId2);
            // new item
            cart.LineItems.AddOrUpdate(CreateProductVariant(variantId3), 3);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    secondResponseURL = url;
                },
                failure: (shopError) => {
                    secondError = shopError;
                }
            );

            Assert.IsNull(secondError);
            Assert.AreEqual("http://shopify.com/checkout-create-after-update", secondResponseURL, "weblink was correct after update");
            Assert.AreEqual(2, cart.LineItems.All().Count, "Had two line items after update");
            Assert.AreEqual("line-item-id1", cart.LineItems.All()[0].ID, "Line item 1 has the correct ID set after update");
            Assert.AreEqual("line-item-id3", cart.LineItems.All()[1].ID, "Line item 2 has the correct ID set after update");
        }

        [Test]
        public void TestUserError() {
            ShopifyBuy.Init(new MockLoader());

            Cart cart = ShopifyBuy.Client().Cart();
            string resultUrl = null;
            ShopifyError resultError = null;

            cart.LineItems.AddOrUpdate(CreateProductVariant("Z2lkOi8vc2hvcGlmeS9Qcm9kdWNFyaWFudC8yMDc1NjEyUserError=="), 1);

            cart.GetWebCheckoutLink(
                success: (url) => {
                    resultUrl = url;
                },
                failure: (shopError) => {
                    resultError = shopError;
                }
            );

            Assert.IsNull(resultUrl, "no url was returned");
            Assert.IsNotNull(resultError, "returned an errror");
            Assert.AreEqual(ShopifyError.ErrorType.UserError, resultError.Type);
            Assert.AreEqual("There were issues with some of the fields sent. See `cart.UserErrors`", resultError.Description);
            Assert.AreEqual(1, cart.UserErrors.Count);
            Assert.AreEqual("someField", cart.UserErrors[0].field()[0], "fields was correct");
            Assert.AreEqual("bad things happened", cart.UserErrors[0].message(), "messaged was correct");
        }

        private ProductVariant CreateProductVariant(string id, decimal price = 1.00m) {
            Dictionary<string,object> data = new Dictionary<string,object>();
            data.Add("id", id);
            data.Add("price", price);

            return new ProductVariant(data);
        }
    }
}
