#if UNITY_EDITOR
namespace Shopify.UIToolkit.Test.Integration {
	using UnityEngine;
	using UnityEngine.TestTools;
	using NUnit.Framework;

    public class TestSanity : MonoBehaviour {
		[Test]
        public void Start() {
			Assert.Pass ();
        }
    }
}
#endif
