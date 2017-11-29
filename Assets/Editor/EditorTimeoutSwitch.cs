
namespace Shopify.Tests {
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine.TestTools;

    public class EditorTimeoutWaiter {
        public const float MaxTimeout = 1.0f;
        public const string LatchTimeoutFailureMessage = "Operation timed out.";
        private float _startTime;
        private float _timeout;
        private bool _isComplete;

        public EditorTimeoutWaiter(float timeout = MaxTimeout) {
            _startTime = (float) EditorApplication.timeSinceStartup;
            _timeout = timeout;
        }

        public void Complete() {
            _isComplete = true;
        }

        public bool Await() {
            if (_isComplete) {
                return false;
            }

            if (EditorApplication.timeSinceStartup - _startTime > _timeout) {
                Assert.Fail(LatchTimeoutFailureMessage);
                return false;
            }

            return true;
        }
    }
}