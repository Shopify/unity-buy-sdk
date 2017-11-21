namespace Shopify.UIToolkit.Test.Unit {
    using System.Collections;
    using UnityEngine;
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using Shopify.UIToolkit.Editor;

    [TestFixture]
    public class TestShopCredentialsVerifier {
        private const string _validShopDomain = "graphql.myshopify.com";
        private const string _validAccessToken = "351c122017d0f2a957d32ae728ad749c";

        private class MockShopCredentials : IShopCredentials {
            public ShopCredentialsVerificationState CredentialsVerificationState { get; set; }
            public string ShopDomain { get; set; }
            public string AccessToken { get; set; }
        }

        private ShopCredentialsVerifier _verifier;
        private MockShopCredentials _objectWithCredentials;

        [SetUp]
        public void Setup() {
            _objectWithCredentials = new MockShopCredentials();
            _objectWithCredentials.AccessToken = "";
            _objectWithCredentials.ShopDomain = "";
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _verifier = new ShopCredentialsVerifier(_objectWithCredentials);
        }

        [Test]
        public void TestHasVerifiedCredentialsFalseWhenCredentialsUnverified() {
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            Assert.IsFalse(_verifier.HasVerifiedCredentials());
        }

        [Test]
        public void TestHasVerifiedCredentialsFalseWhenCredentialsInvalid() {
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
            Assert.IsFalse(_verifier.HasVerifiedCredentials());
        }

        [Test]
        public void TestHasVerifiedCredentialsTrueWhenCredentialsValid() {
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Verified;
            Assert.IsTrue(_verifier.HasVerifiedCredentials());
        }

        [UnityTest]
        public IEnumerator TestCanVerifyValidCredentials() {
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _objectWithCredentials.AccessToken = _validAccessToken;
            _objectWithCredentials.ShopDomain = _validShopDomain;

            var requestComplete = false;
            var successCallbackCalled = false;

            _verifier.VerifyCredentials(() => {
                requestComplete = true;
                successCallbackCalled = true;
            }, () => {
                requestComplete = true;
                successCallbackCalled = false;
            });

            while (requestComplete != true) {
                yield return null;
            }

            Assert.IsTrue(_verifier.HasVerifiedCredentials());
            Assert.IsTrue(_objectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Verified);
            Assert.IsTrue(successCallbackCalled);
        }

        [UnityTest]
        public IEnumerator TestCanInvalidateInvalidCredentials() {
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Unverified;
            _objectWithCredentials.AccessToken = "???";
            _objectWithCredentials.ShopDomain = "nope.myshopify.com";

            var requestComplete = false;
            var failureCallbackCalled = false;

            _verifier.VerifyCredentials(() => {
                requestComplete = true;
                failureCallbackCalled = false;
            }, () => {
                requestComplete = true;
                failureCallbackCalled = true;
            });

            while (requestComplete != true) {
                yield return null;
            }

            Assert.IsFalse(_verifier.HasVerifiedCredentials());
            Assert.IsTrue(_objectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Invalid);
            Assert.IsTrue(failureCallbackCalled);
        }

        [Test]
        public void TestResetCredentialsResetsInvalidState() {
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Invalid;
            _verifier.ResetVerificationState();
            Assert.IsTrue(_objectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Unverified);
        }

        [Test]
        public void TestResetCredentialsResetsVerifiedState() {
            _objectWithCredentials.CredentialsVerificationState = ShopCredentialsVerificationState.Verified; 
            _verifier.ResetVerificationState();
            Assert.IsTrue(_objectWithCredentials.CredentialsVerificationState == ShopCredentialsVerificationState.Unverified);
        }
    }
}
