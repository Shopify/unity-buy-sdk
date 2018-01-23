namespace Shopify.UIToolkit.Test.Unit {
    using System.Collections;
    using UnityEngine;
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using Shopify.UIToolkit.Editor;
    using System.Text.RegularExpressions;

    [TestFixture]
    public class TestShopCredentialsVerifier {
        private const string _ValidShopDomain = "graphql.myshopify.com";
        private const string _ValidAccessToken = "351c122017d0f2a957d32ae728ad749c";

        private class MockShopCredentials : IShopCredentials {
            public ShopCredentialsVerificationState CredentialsVerificationState { get; set; }
            public string ShopDomain { get; set; }
            public string AccessToken { get; set; }

            public string GetShopDomain() {
                return ShopDomain;
            }

            public string GetAccessToken() {
                return AccessToken;
            }
        }

        private ShopCredentialsVerifier _Verifier;
        private MockShopCredentials _ObjectWithCredentials;

        [SetUp]
        public void Setup() {
            _ObjectWithCredentials = new MockShopCredentials();
            _ObjectWithCredentials.AccessToken = "";
            _ObjectWithCredentials.ShopDomain = "";
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _Verifier = new ShopCredentialsVerifier(_ObjectWithCredentials);
        }

        [Test]
        public void TestHasVerifiedCredentialsFalseWhenCredentialsUnverified() {
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            Assert.IsFalse(_Verifier.HasVerifiedCredentials());
        }

        [Test]
        public void TestHasVerifiedCredentialsFalseWhenCredentialsInvalid() {
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
            Assert.IsFalse(_Verifier.HasVerifiedCredentials());
        }

        [Test]
        public void TestHasVerifiedCredentialsTrueWhenCredentialsValid() {
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Verified;
            Assert.IsTrue(_Verifier.HasVerifiedCredentials());
        }

        [UnityTest]
        public IEnumerator TestCanVerifyValidCredentials() {
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _ObjectWithCredentials.AccessToken = _ValidAccessToken;
            _ObjectWithCredentials.ShopDomain = _ValidShopDomain;

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

            Assert.IsTrue(_Verifier.HasVerifiedCredentials());
            Assert.IsTrue(_ObjectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Verified);
            Assert.IsTrue(successCallbackCalled);
        }

        [UnityTest]
        public IEnumerator TestCanInvalidateInvalidCredentials() {
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _ObjectWithCredentials.AccessToken = "???";
            _ObjectWithCredentials.ShopDomain = "nope.myshopify.com";

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

            Assert.IsFalse(_Verifier.HasVerifiedCredentials());
            Assert.IsTrue(_ObjectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Invalid);
            Assert.IsTrue(failureCallbackCalled);
        }

        [UnityTest]
        public IEnumerator TestVerifierFailsForBlankDomain() {
            LogAssert.Expect(LogType.Error, new Regex(@"Domain is invalid\. Make sure that it is not empty\/blank\."));

            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _ObjectWithCredentials.AccessToken = "  ";
            _ObjectWithCredentials.ShopDomain = "  ";

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

            Assert.IsFalse(_Verifier.HasVerifiedCredentials());
            Assert.IsTrue(_ObjectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Invalid);
            Assert.IsTrue(failureCallbackCalled);


        }

        [Test]
        public void TestResetCredentialsResetsInvalidState() {
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
            _Verifier.ResetVerificationState();
            Assert.IsTrue(_ObjectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Unverified);
        }

        [Test]
        public void TestResetCredentialsResetsVerifiedState() {
            _ObjectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Verified; 
            _Verifier.ResetVerificationState();
            Assert.IsTrue(_ObjectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Unverified);
        }
    }
}
