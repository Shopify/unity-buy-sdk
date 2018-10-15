
#if UNITY_EDITOR
namespace Shopify.Unity.Tests
{
    using System;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.SceneManagement;
    using NUnit.Framework;
    using Shopify.Unity;
    using Shopify.Unity.SDK;
    using System.Collections;
    using System.Collections.Generic;

    public class TestGlobalGameObject : MonoBehaviour {
        [SetUp]
        public void SetUp() {
            // GlobalGameObject might have been created from previous test runs.
            var globalGameObject = GameObject.Find(GlobalGameObject.Name);
            GameObject.Destroy(globalGameObject);
        }

        [TearDown]
        public void TearDown() {
            var globalGameObject = GameObject.Find(GlobalGameObject.Name);
            GameObject.Destroy(globalGameObject);
        }

        [Test]
        public void AddingAComponentCreatesGlobalObject() {
            Assert.IsNull(GlobalGameObject.Name);
            GlobalGameObject.AddComponent<LoaderComponent>();
            var gameObject = GameObject.Find(GlobalGameObject.Name);
            Assert.IsNotNull(gameObject);
        }

        [UnityTest]
        public IEnumerator ObjectLivesWhenNewSceneIsLoadedIn() {
            Assert.IsNull(GlobalGameObject.Name);
            GlobalGameObject.AddComponent<LoaderComponent>();
            var gameObject = GameObject.Find(GlobalGameObject.Name);
            Assert.IsNotNull(gameObject);

            var originalScene = SceneManager.GetActiveScene();

            var loadOp = SceneManager.LoadSceneAsync("IntegrationTestBlankScene");
            loadOp.allowSceneActivation = true;
            while (!loadOp.isDone) {
                yield return null;
            }

            gameObject = GameObject.Find(GlobalGameObject.Name);
            Assert.IsNotNull(gameObject);
        }
    }
}
#endif
