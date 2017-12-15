// Since we load from the asset database, we need to not compile this file in builds
# if UNITY_EDITOR
namespace Shopify.UIToolkit.Test.Integration {
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEditor;
    using System;
    using Shopify.UIToolkit.Themes;
    using System.Collections.Generic;
    using System.Collections;

    [TestFixture]
    public class TestGenericSingleProductTheme {
        private struct InitializationResult {
            public GenericSingleProductTheme Theme;
            public SingleProductThemeController Controller;
        }

        [UnityTest, TestCaseSource("AllPrefabTestCases"), Timeout(5000)]
        public IEnumerator TestPriceBindsPriceText(string prefabPath) {
            var result = Initialize(prefabPath);
            var theme = result.Theme;
            var controller = result.Controller;

            controller.Show();

            var initialLabelText = theme.PriceLabel.text;

            yield return new WaitUntil(() => {
                return theme.PriceLabel.text != initialLabelText;
            });

            Assert.AreEqual("188.00$", theme.PriceLabel.text);
        }


        [UnityTest, TestCaseSource("AllPrefabTestCases"), Timeout(5000)]
        public IEnumerator TestDescriptionBindsDescriptionText(string prefabPath) {
            var result = Initialize(prefabPath);
            var theme = result.Theme;
            var controller = result.Controller;

            controller.Show();

            var initialLabelText = theme.ProductDescriptionLabel.text;

            yield return new WaitUntil(() => {
                return theme.ProductDescriptionLabel.text != initialLabelText;
            });

            Assert.AreEqual("Lorem ipsum dolor sit amet", theme.ProductDescriptionLabel.text.Substring(0,26));
        }

        [UnityTest, TestCaseSource("AllPrefabTestCases"), Timeout(5000)]
        public IEnumerator TestTitleBindsTitleText(string prefabPath) {
            var result = Initialize(prefabPath);
            var theme = result.Theme;
            var controller = result.Controller;

            controller.Show();

            var initialLabelText = theme.ProductTitleLabel.text;

            yield return new WaitUntil(() => {
                return theme.ProductTitleLabel.text != initialLabelText;
            });

            Assert.AreEqual("Arena Zip Boot", theme.ProductTitleLabel.text);
        }


        private InitializationResult Initialize(string prefabPath) {
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var gameObject = GameObject.Instantiate(asset) as GameObject;
            var theme = gameObject.GetComponent<GenericSingleProductTheme>();
            var controller = gameObject.GetComponent<SingleProductThemeController>();

            controller.AccessToken = "351c122017d0f2a957d32ae728ad749c";
            controller.ShopDomain = "graphql.myshopify.com";
            controller.ProductGID = "Z2lkOi8vc2hvcGlmeS9Qcm9kdWN0Lzk4OTUyODE0NzU=";

            theme.Controller = controller;
            controller.Theme = theme;

            return new InitializationResult() { Theme = theme, Controller = controller };
        }

        private static string[] _prefabPaths = new string[] {
            "Assets/Shopify/UIToolkit/Themes/Generic/Generic Single Product Theme Landscape.prefab"
        };

        public static IEnumerable AllPrefabTestCases() {
            foreach (var prefabPath in _prefabPaths) {
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                yield return new TestCaseData(prefabPath)
                    .SetName(asset.name)
                    .Returns(null);
            }
        }
    }
}
# endif