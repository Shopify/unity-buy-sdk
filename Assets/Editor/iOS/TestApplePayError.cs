#if UNITY_IOS
namespace Shopify.Tests {
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using Shopify.Unity.SDK.iOS;
    using Shopify.Unity.MiniJSON;

    [TestFixture]
    public class TestApplePayErrorSerialization {

        [Test]
        public void TestErrorSerialization() {

            var expectedDescription = "test";
            var expectedType = ApplePayError.ErrorType.ShippingAddressUnservicable;

            var applePayError = new ApplePayError(expectedType, expectedDescription);
            var serialized = (Dictionary<string, object>) applePayError.ToJson();

            Assert.AreEqual(serialized["Type"], expectedType);
            Assert.AreEqual(serialized["Description"], expectedDescription);
        }

        [Test]
        public void TestAddressInvalidErrorSerialization() {

            var expectedDescription = "test";
            var expectedType = ApplePayError.ErrorType.PaymentShippingAddress;
            var expectedField = ApplePayAddressInvalidError.AddressField.City;

            var applePayError = new ApplePayAddressInvalidError(expectedType, expectedDescription, expectedField);
            var serialized = (Dictionary<string, object>) applePayError.ToJson();

            Assert.AreEqual(serialized["Type"], expectedType);
            Assert.AreEqual(serialized["Description"], expectedDescription);
            Assert.AreEqual(serialized["Field"], expectedField);
        }

        [Test]
        public void TestShippingAddressInvalidErrorSerialization() {

            var expectedDescription = "test";
            var expectedType = ApplePayError.ErrorType.PaymentShippingAddress;
            var expectedField = ApplePayAddressInvalidError.AddressField.City;

            var applePayError = new ApplePayShippingAddressInvalidError(expectedDescription, expectedField);
            var serialized = (Dictionary<string, object>) applePayError.ToJson();

            Assert.AreEqual(serialized["Type"], expectedType);
            Assert.AreEqual(serialized["Description"], expectedDescription);
            Assert.AreEqual(serialized["Field"], expectedField);
        }

        [Test]
        public void TestBillingAddressInvalidErrorSerialization() {

            var expectedDescription = "test";
            var expectedType = ApplePayError.ErrorType.PaymentBillingAddress;
            var expectedField = ApplePayAddressInvalidError.AddressField.City;

            var applePayError = new ApplePayBillingAddressInvalidError(expectedDescription, expectedField);
            var serialized = (Dictionary<string, object>) applePayError.ToJson();

            Assert.AreEqual(serialized["Type"], expectedType);
            Assert.AreEqual(serialized["Description"], expectedDescription);
            Assert.AreEqual(serialized["Field"], expectedField);
        }

        [Test]
        public void TestContactInvalidErrorSerialization() {

            var expectedDescription = "test";
            var expectedType = ApplePayError.ErrorType.PaymentContactInvalid;
            var expectedField = ApplePayContactInvalidError.ContactField.EmailAddress;

            var applePayError = new ApplePayContactInvalidError(expectedDescription, expectedField);
            var serialized = (Dictionary<string, object>) applePayError.ToJson();

            Assert.AreEqual(serialized["Type"], expectedType);
            Assert.AreEqual(serialized["Description"], expectedDescription);
            Assert.AreEqual(serialized["Field"], expectedField);
        }

        [Test]
        public void TestShippingAddressUnservicableErrorSerialization() {

            var expectedDescription = "test";
            var expectedType = ApplePayError.ErrorType.ShippingAddressUnservicable;

            var applePayError = new ApplePayShippingAddressUnservicableError(expectedDescription);
            var serialized = (Dictionary<string, object>) applePayError.ToJson();

            Assert.AreEqual(serialized["Type"], expectedType);
            Assert.AreEqual(serialized["Description"], expectedDescription);
        }
    }
}
#endif
