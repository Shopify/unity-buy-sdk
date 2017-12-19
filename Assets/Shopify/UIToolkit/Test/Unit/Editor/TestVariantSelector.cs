namespace Shopify.UIToolkit.Test.Unit {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using Shopify.Unity;
    using Shopify.Unity.MiniJSON;

    [TestFixture]
    public class TestVariantSelector {

        [Test]
        public void TestDefaultVariant() {
            string oneVariantProduct = @"
            [
                {
                    ""node"": {
                      ""title"": ""Default Title"",
                      ""selectedOptions"": [
                        {
                          ""name"": ""Title"",
                          ""value"": ""Default Title""
                        }
                      ]
                    }
                }
            ]";

            var selector = new VariantSelector(VariantsFromString(oneVariantProduct));
            var options = selector.AllOptions();
            Assert.IsNull(options);
        }

        [Test]
        public void TestCoupleOfVariants() {
            string twoVariantProduct = @"
            [
                {
                    ""node"": {
                      ""title"": ""Black / Small"",
                      ""selectedOptions"": [
                        {
                          ""name"": ""Color"",
                          ""value"": ""Black""
                        },
                        {
                          ""name"": ""Size"",
                          ""value"": ""Small""
                        }
                      ]
                    }
                }
            ]";

            var selector = new VariantSelector(VariantsFromString(twoVariantProduct));
            var options = selector.AllOptions();
            Assert.AreEqual(options.Count, 2);
            Assert.AreEqual(options["Color"], new List<string>{ "Black" });
            Assert.AreEqual(options["Size"], new List<string>{ "Small" });
        }

        [Test]
        public void TestAllOptions() {
            var selector = new VariantSelector(ManyVariants());
            var options = selector.AllOptions();
            Assert.AreEqual(options.Count, 3);
            Assert.AreEqual(options["Color"], new List<string>{ "Blue" });
            Assert.AreEqual(options["Size"], new List<string>{ "Small", "Medium" });
            Assert.AreEqual(options["Language"], new List<string>{ "English", "French" });
        }

        [Test]
        public void TestSelectingOptionsExcludesInvalidChoices() {
            var selector = new VariantSelector(ManyVariants());
            var colorSelected = selector.ValidOptionsForSelections(new Dictionary<string, string>() {
              {"Color", "Blue"}
            });

            Assert.AreEqual(new List<string>() {"Blue"}, colorSelected["Color"]);
            Assert.AreEqual(new List<string>() {"Small", "Medium"}, colorSelected["Size"]);
            Assert.AreEqual(new List<string>() {"English", "French"}, colorSelected["Language"]);


            var sizeColorSelected = selector.ValidOptionsForSelections(new Dictionary<string, string>() {
              {"Color", "Blue"},
              {"Size", "Medium"}
            });
            Assert.AreEqual(new List<string>() {"Blue"}, sizeColorSelected["Color"]);
            Assert.AreEqual(new List<string>() {"Medium"}, sizeColorSelected["Size"]);

            // Only French should be available since "Blue/Medium/English" isn't a valid product variant.
            Assert.AreEqual(new List<string>() {"French"}, sizeColorSelected["Language"]);
        }

        private List<ProductVariant> VariantsFromString(string input) {
            var objects = (List<object>)Json.Deserialize(input);
            List<ProductVariant> variants = new List<ProductVariant>();
            foreach (var obj in objects) {
                var dict = (Dictionary<string, object>)obj;
                variants.Add(new ProductVariant((Dictionary<string, object>)dict["node"]));
            }
            return variants;
        }

        private List<ProductVariant> ManyVariants() {
            string threeVariantProduct = @"
            [
                {
                    ""node"": {
                      ""title"": ""Blue / Small / English"",
                      ""selectedOptions"": [
                        {
                          ""name"": ""Color"",
                          ""value"": ""Blue""
                        },
                        {
                          ""name"": ""Size"",
                          ""value"": ""Small""
                        },
                        {
                          ""name"": ""Language"",
                          ""value"": ""English""
                        }
                      ]
                    }
                },
                {
                    ""node"": {
                      ""title"": ""Blue / Medium / French"",
                      ""selectedOptions"": [
                        {
                          ""name"": ""Color"",
                          ""value"": ""Blue""
                        },
                        {
                          ""name"": ""Size"",
                          ""value"": ""Medium""
                        },
                        {
                          ""name"": ""Language"",
                          ""value"": ""French""
                        }
                      ]
                    }
                }
            ]";

            return VariantsFromString(threeVariantProduct);
        }
    }
}
