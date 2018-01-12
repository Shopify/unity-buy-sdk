#if UNITY_EDITOR
namespace Shopify.UIToolkit.Test.Integration {
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

        [SetUp]
        public void Setup() {
            var fixture = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Shopify/UIToolkit/Components/ViewSwitcher/View Switcher Example.prefab");
            ViewSwitcherPrefab = GameObject.Instantiate(fixture, Vector3.zero, Quaternion.identity);
            Subject = ViewSwitcherPrefab.GetComponentInChildren<ViewSwitcher>();
            Subject.AnimationDuration = AnimationDuration;
        }

        [TearDown]
        public void TearDown() {
            GameObject.DestroyImmediate(ViewSwitcherPrefab);
        }

        [UnityTest]
        public IEnumerator TestInitFirstViewIsActive() {
            yield return null;
            Assert.IsTrue(Subject.Views[0].gameObject.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator TestInitOtherViewsAreInactive() {
            yield return null;
            Assert.IsFalse(Subject.Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Subject.Views[2].gameObject.activeInHierarchy);
        }

        [UnityTest]
        public IEnumerator TestPushViewThrowsErrorIfViewIsNotChildOfContainer() {
            yield return null;
            var exception = Assert.Catch(() => {
                // If its not registered then its gonna throw a different exception!
                System.Array.Resize(ref Subject.Views, 4);
                Subject.Views[3] = Subject.Container;

                // Pushing the container to itself will always trigger this exception
                Subject.PushView(Subject.Container);
            });

            Assert.AreEqual("View was not a child of the ViewSwitcher's container transform", exception.Message);
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
            Subject.PushView(Subject.Views[0]);

            Assert.IsTrue(Subject.Views[0].gameObject.activeInHierarchy);
            Assert.IsFalse(Subject.Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Subject.Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(0, Subject.Views[0].GetSiblingIndex());
            Assert.AreEqual(1, Subject.Views[1].GetSiblingIndex());
            Assert.AreEqual(2, Subject.Views[2].GetSiblingIndex());
        }

        [UnityTest]
        public IEnumerator TestPushViewWithNewViewSetsActiveAndMovesToBottomOfHeirarchy() {
            yield return null;
            Subject.PushView(Subject.Views[1]);

            Assert.IsTrue(Subject.Views[0].gameObject.activeInHierarchy);
            Assert.IsTrue(Subject.Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Subject.Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(0, Subject.Views[0].GetSiblingIndex());
            Assert.AreEqual(2, Subject.Views[1].GetSiblingIndex());
            Assert.AreEqual(1, Subject.Views[2].GetSiblingIndex());
        }

        [UnityTest]
        public IEnumerator TestPushViewTwiceWithNewViewSetsActiveAndMovesToBottomOfHeirarchy() {
            yield return null;
            Subject.PushView(Subject.Views[1]);
            Subject.PushView(Subject.Views[2]);

            Assert.IsTrue(Subject.Views[0].gameObject.activeInHierarchy);
            Assert.IsTrue(Subject.Views[1].gameObject.activeInHierarchy);
            Assert.IsTrue(Subject.Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(0, Subject.Views[0].GetSiblingIndex());
            Assert.AreEqual(1, Subject.Views[1].GetSiblingIndex());
            Assert.AreEqual(2, Subject.Views[2].GetSiblingIndex());
        }

        [UnityTest]
        public IEnumerator TestGoBackRestoresPreviousViewAfterAnimationFinishes() {
            yield return null;

            Subject.PushView(Subject.Views[1]);
            yield return new WaitForSeconds(AnimationDuration + 0.1f);
            Subject.GoBack();
            yield return new WaitForSeconds(AnimationDuration + 0.1f);

            Assert.IsTrue(Subject.Views[0].gameObject.activeInHierarchy);
            Assert.IsFalse(Subject.Views[1].gameObject.activeInHierarchy);
            Assert.IsFalse(Subject.Views[2].gameObject.activeInHierarchy);

            Assert.AreEqual(1, Subject.Views[0].GetSiblingIndex());
            Assert.AreEqual(2, Subject.Views[1].GetSiblingIndex());
            Assert.AreEqual(0, Subject.Views[2].GetSiblingIndex());
        }
    }
}
#endif