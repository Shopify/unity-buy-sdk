#if UNITY_EDITOR
namespace Shopify.UIToolkit.Test.Integration {
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools;
    using Shopify.UIToolkit.Shops;

    [TestFixture] 
    public class TestGenericSingleProductShop {
        public GameObject SingleProductShopPrefab;
        public GenericSingleProductShop Subject;
        
        [SetUp]
        public void Setup() {
            var fixture = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Shopify/UIToolkit/Shops/Generic/Generic Single Product Shop Landscape.prefab");
            SingleProductShopPrefab = GameObject.Instantiate(fixture, Vector3.zero, Quaternion.identity);
            Subject = SingleProductShopPrefab.GetComponentInChildren<GenericSingleProductShop>();
        }

        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(SingleProductShopPrefab);
        }

        // TODO: Once the controller gets factored out of the Shop I'll be able to inject it 
        //       easier to run tests on the Single Product view.
    }
}
#endif
