#if SHOPIFY_IOS_TEST
namespace Shopify.Tests {
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using Shopify.Unity.SDK.iOS;

    public class Tester : MonoBehaviour {

        [DllImport ("__Internal")]
        protected static extern void _TesterObjectFinishedLoading();

        private bool DidFinishLoading = false;

        void Start() {
            gameObject.AddComponent<ApplePayEventReceiverBridge>();
        }

        void Update() {
            // We call this in Update() instead of Start() to give a chance
            // for all tester scripts to initialize in Start()
            if (!DidFinishLoading) {
                DidFinishLoading = true;
                _TesterObjectFinishedLoading();
            }
        }
    }
}
#endif
