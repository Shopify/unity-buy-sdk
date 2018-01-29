#if UNITY_EDITOR
namespace Shopify.UIToolkit.Test.Integration {
    using UnityEngine;
    using UnityEditor;
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using System.Collections;
    using Shopify.UIToolkit.Shops;

    [TestFixture]
    public class TestLoadingOverlay {
        public Canvas Canvas;
        public LoadingOverlay Subject;

        [SetUp]
        public void Setup() {
           var go = new GameObject("Loading Overlay Test Fixture");
            Canvas = go.AddComponent<Canvas>();

            var loadingOverlayAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Shopify/UIToolkit/Themes/Generic/Assets/Loading Overlay.prefab");
            var loadingOverlayGo = GameObject.Instantiate(loadingOverlayAsset, Vector3.zero, Quaternion.identity);
            loadingOverlayGo.transform.SetParent(Canvas.transform, false);
            Subject = loadingOverlayGo.GetComponent<LoadingOverlay>();
       }

        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(Canvas.gameObject);
        }

        [Test]
        public void TestShowActivatesGameObject() {
            Subject.gameObject.SetActive(false);
            Subject.Show();

            Assert.IsTrue(Subject.isActiveAndEnabled);
        }

        [UnityTest]
        public IEnumerator TestHideEventuallyDeactivatesGameObject() {
            Subject.Show();
            Subject.Hide();

            var elapsed = 0f;

            while(Subject.isActiveAndEnabled) {
                yield return null;
                elapsed += Time.deltaTime;
                if (elapsed > 1.0f) Assert.Fail();
            }
        }

        [UnityTest]
        public IEnumerator TestShowSetsBlockerAlphaTo1f() {
            var canvasRenderer = Subject.Blocker.canvasRenderer;
            canvasRenderer.SetColor(Color.clear);
            Subject.Show();

            var elapsed = 0f;

            while(canvasRenderer.GetColor().a < 0.99f) {
                yield return null;
                elapsed += Time.deltaTime;
                if (elapsed > 1.0f) Assert.Fail();
            }
        }

        [UnityTest]
        public IEnumerator TestHideSetsBlockerAlphaTo0f() {
            var canvasRenderer = Subject.Blocker.canvasRenderer;
            canvasRenderer.SetColor(Color.white);
            Subject.Hide();

            var elapsed = 0f;

            while(canvasRenderer.GetColor().a > 0.01f) {
                yield return null;
                elapsed += Time.deltaTime;
                if (elapsed > 1.0f) Assert.Fail();
            }
        }
    }
}
#endif