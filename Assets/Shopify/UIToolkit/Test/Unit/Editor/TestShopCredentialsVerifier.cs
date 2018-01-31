namespace Shopify.UIToolkit.Test.Unit {
    using System.Collections;
    using UnityEngine;
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using Shopify.UIToolkit.Editor;
    using System.Text.RegularExpressions;

    [TestFixture]
    public class TestShopCredentialsVerifier {
        private const string _ValidDomain = "graphql.myshopify.com";
        private const string _ValidAccessToken = "351c122017d0f2a957d32ae728ad749c";
        private ShopCredentialsVerifier _Verifier;

        [SetUp]
        public void Setup() {
            _Verifier = new ShopCredentialsVerifier(new ShopCredentials("", ""));
        }

        [Test]
        public void TestHasVerifiedCredentialsFalseWhenCredentialsUnverified() {
            _Verifier.Credentials.State = ShopCredentials.VerificationState.Unverified;
            Assert.IsFalse(_Verifier.HasVerifiedCredentials());
        }

        [Test]
        public void TestHasVerifiedCredentialsFalseWhenCredentialsInvalid() {
            _Verifier.Credentials.State = ShopCredentials.VerificationState.Invalid;
            Assert.IsFalse(_Verifier.HasVerifiedCredentials());
        }

        [Test]
        public void TestHasVerifiedCredentialsTrueWhenCredentialsValid() {
            _Verifier.Credentials.State = ShopCredentials.VerificationState.Verified;
            Assert.IsTrue(_Verifier.HasVerifiedCredentials());
        }

        [UnityTest]
        public IEnumerator TestCanVerifyValidCredentials() {
            _Verifier = new ShopCredentialsVerifier(new ShopCredentials(_ValidDomain, _ValidAccessToken));

            var state = ShopCredentials.VerificationState.Unverified;

            _Verifier.OnCredentialsStateShouldChange += (newState) => {
                state = newState;
            };

            var requestComplete = false;
            var successCallbackCalled = false;

            _Verifier.VerifyCredentials(() => {
                requestComplete = true;
                successCallbackCalled = true;
            }, () => {
                requestComplete = true;
                successCallbackCalled = false;
            });

            while (requestComplete != true) {
                yield return null;
            }

            Assert.AreEqual(ShopCredentials.VerificationState.Verified, state);
            Assert.IsTrue(successCallbackCalled);
        }

        [UnityTest]
        public IEnumerator TestTellsListenerWhenCredentialsAreInvalid() {
            _Verifier = new ShopCredentialsVerifier(new ShopCredentials("nope.myshopify.com", "???"));

            var state = ShopCredentials.VerificationState.Unverified;

            _Verifier.OnCredentialsStateShouldChange += (newState) => {
                state = newState;
            };

            var requestComplete = false;
            var failureCallbackCalled = false;

            _Verifier.VerifyCredentials(() => {
                requestComplete = true;
                failureCallbackCalled = false;
            }, () => {
                requestComplete = true;
                failureCallbackCalled = true;
            });

            while (requestComplete != true) {
                yield return null;
            }

            Assert.AreEqual(ShopCredentials.VerificationState.Invalid, state);
            Assert.IsTrue(failureCallbackCalled);
        }

        [UnityTest]
        public IEnumerator TestVerifierFailsForBlankDomain() {
            LogAssert.Expect(LogType.Error, new Regex(@"Domain is invalid\. Make sure that it is not empty\/blank\."));
            _Verifier = new ShopCredentialsVerifier(new ShopCredentials(" ", " "));

            var state = ShopCredentials.VerificationState.Unverified;

            _Verifier.OnCredentialsStateShouldChange += (newState) => {
                state = newState;
            };

            var requestComplete = false;
            var failureCallbackCalled = false;

            _Verifier.VerifyCredentials(() => {
                requestComplete = true;
                failureCallbackCalled = false;
            }, () => {
                requestComplete = true;
                failureCallbackCalled = true;
            });

            while (requestComplete != true) {
                yield return null;
            }

            Assert.AreEqual(ShopCredentials.VerificationState.Invalid, state);
            Assert.IsTrue(failureCallbackCalled);
        }

        [Test]
        public void TestResetCredentialsResetsInvalidState() {
            var state = ShopCredentials.VerificationState.Unverified;

            _Verifier.OnCredentialsStateShouldChange += (newState) => {
                state = newState;
            };

            _Verifier.Credentials.State = ShopCredentials.VerificationState.Invalid;
            _Verifier.ResetVerificationState();

            Assert.AreEqual(ShopCredentials.VerificationState.Unverified, state);
        }

        [Test]
        public void TestResetCredentialsResetsVerifiedState() {
            var state = ShopCredentials.VerificationState.Unverified;

            _Verifier.OnCredentialsStateShouldChange += (newState) => {
                state = newState;
            };

            _Verifier.Credentials.State = ShopCredentials.VerificationState.Verified;
            _Verifier.ResetVerificationState();

            Assert.AreEqual(ShopCredentials.VerificationState.Unverified, state);
        }
    }
}
