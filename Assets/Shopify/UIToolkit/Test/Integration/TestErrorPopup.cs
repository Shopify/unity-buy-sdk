#if UNITY_EDITOR
namespace Shopify.UIToolkit.Test.Integration {
    using System.Collections;
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.UI;
    using UnityEngine.TestTools;
    using Shopify.Unity.SDK;
    using Shopify.UIToolkit.Themes;
    using NUnit.Framework;

    [TestFixture]
    public class TestErrorPopup {
        public Canvas Canvas;
        public ErrorPopup Popup;

        [SetUp]
        public void Setup() {
            var go = new GameObject("Error Popup Test Fixture");
            Canvas = go.AddComponent<Canvas>();

            var errorPopupAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Shopify/UIToolkit/Shops/Generic/Assets/ErrorPopup/Error Popup.prefab");
            var errorPopupGo = GameObject.Instantiate(errorPopupAsset, Vector3.zero, Quaternion.identity);
            errorPopupGo.transform.SetParent(Canvas.transform, false);
            Popup = errorPopupGo.GetComponent<ErrorPopup>();

            // Speed up the animator for the tests...

            Popup.Animator.speed = 10f;
        }

        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(Canvas.gameObject);
        }

        [UnityTest]
        public IEnumerator TestAddErrorChangesErrorTextBoxText() {
            Popup.AddError("Error");
            yield return null;
            Assert.AreEqual("Error", Popup.Text.text);
        }

        [UnityTest]
        public IEnumerator TestAddErrorPlaysAnimation() {
            Popup.AddError("Error");
            yield return null;
            Assert.IsTrue(Popup.Animator.GetCurrentAnimatorStateInfo(0).IsName("ShowError"));
        }

        [UnityTest]
        public IEnumerator TestAddErrorEventallyReturnsToReadyState() {
            Popup.AddError("Error");
            yield return null;

            while(true) {
                if (Popup.Animator.GetCurrentAnimatorStateInfo(0).IsName("Ready")) {
                    yield break;
                }

                yield return null;
            }
        }

        [Test]
        public void TestAddErrorRejectsErrorsAlreadyInQueue() {
            Popup.AddError("Error");
            Assert.IsFalse(Popup.AddError("Error"));
        }

        [UnityTest]
        public IEnumerator TestAddErrorAcceptsDuplicateMessagesAsLongAsTheyHaveFinishedDisplaying() {
            Popup.AddError("Error");
            yield return null;

            while(true) {
                if (Popup.Animator.GetCurrentAnimatorStateInfo(0).IsName("Ready")) {
                    break;
                }

                yield return null;
            }

            Assert.IsTrue(Popup.AddError("Error"));
        }
    }
}
#endif