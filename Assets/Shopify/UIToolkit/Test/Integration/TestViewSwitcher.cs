#if UNITY_EDITOR
namespace Shopify.UIToolkit.Test.Integration {
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.TestTools;

    [TestFixture] 
    public class TestViewSwitcher {
        public const float AnimationDuration = 0.1f;
        public GameObject ViewSwitcherPrefab;
        public ViewSwitcher Subject;
        public List<RectTransform> Views;

        [SetUp]
        public void Setup() {
            Views = new List<RectTransform>();

            var fixture = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Shopify/UIToolkit/Components/ViewSwitcher/View Switcher Example.prefab");
            ViewSwitcherPrefab = GameObject.Instantiate(fixture, Vector3.zero, Quaternion.identity);
            Subject = ViewSwitcherPrefab.GetComponentInChildren<ViewSwitcher>();
            Subject.AnimationDuration = AnimationDuration;

            foreach (Transform child in Subject.Container.transform) {
                Views.Add(child.GetComponent<RectTransform>());
            }

            Views.ForEach((x) => Subject.RegisterView(x));
        }

        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(ViewSwitcherPrefab);
        }

        [UnityTest]
        public IEnumerator TestInitFirstViewIsActive() {
            yield return null;
            Assert.IsTrue(Views[0].gameObject.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator TestInitOtherViewsAreInactive() {
            yield return null;
            Assert.IsFalse(Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Views[2].gameObject.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator TestPushViewThrowsErrorIfViewIsNotRegistered() {
            yield return null;
            var exception = Assert.Catch(() => {
                Subject.PushView(Subject.Container);
            });

            Assert.AreEqual("View was not registered with ViewSwitcher before being pushed", exception.Message);
        }

        [UnityTest]
        public IEnumerator TestPushViewWithActiveViewNoops() {
            yield return null;
            Subject.PushView(Views[0]);

            Assert.IsTrue(Views[0].gameObject.activeInHierarchy);
            Assert.IsFalse(Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(0, Views[0].GetSiblingIndex());
            Assert.AreEqual(1, Views[1].GetSiblingIndex());
            Assert.AreEqual(2, Views[2].GetSiblingIndex());
        }

        [UnityTest]
        public IEnumerator TestPushViewWithNewViewSetsActiveAndMovesToBottomOfHeirarchy() {
            yield return null;
            Subject.PushView(Views[1]);

            Assert.IsTrue(Views[0].gameObject.activeInHierarchy);
            Assert.IsTrue(Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(0, Views[0].GetSiblingIndex());
            Assert.AreEqual(2, Views[1].GetSiblingIndex());
            Assert.AreEqual(1, Views[2].GetSiblingIndex());
        }

        [UnityTest]
        public IEnumerator TestPushViewTwiceWithNewViewSetsActiveAndMovesToBottomOfHeirarchy() {
            yield return null;
            Subject.PushView(Views[1]);
            Subject.PushView(Views[2]);

            Assert.IsTrue(Views[0].gameObject.activeInHierarchy);
            Assert.IsTrue(Views[1].gameObject.activeInHierarchy);
            Assert.IsTrue(Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(0, Views[0].GetSiblingIndex());
            Assert.AreEqual(1, Views[1].GetSiblingIndex());
            Assert.AreEqual(2, Views[2].GetSiblingIndex());
        }

        [UnityTest]
        public IEnumerator TestGoBackRestoresPreviousViewAfterAnimationFinishes() {
            yield return null;

            Subject.PushView(Views[1]);
            yield return new WaitForSeconds(AnimationDuration + 0.1f);
            Subject.GoBack();
            yield return new WaitForSeconds(AnimationDuration + 0.1f);

            Assert.IsTrue(Views[0].gameObject.activeInHierarchy);
            Assert.IsFalse(Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(1, Views[0].GetSiblingIndex());
            Assert.AreEqual(2, Views[1].GetSiblingIndex());
            Assert.AreEqual(0, Views[2].GetSiblingIndex());
        }
    }
}
#endif