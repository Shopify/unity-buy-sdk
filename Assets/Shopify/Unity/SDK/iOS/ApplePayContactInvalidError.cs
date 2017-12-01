#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;
    using System;

    public class ApplePayContactInvalidError : ApplePayError {
        public enum ContactField {
            PostalAddress,
            EmailAddress,
            PhoneNumber,
            Name,
            PhoneticName
        }

        public readonly ContactField Field;

        /// <summary>
        /// Constructor for ApplePayContactInvalidError
        /// </summary>
        /// <param name="description">Localized description for the error</param>
        /// <param name="field">A <see cref="ContactField"/> that represents the field that caused this error</param>
        public ApplePayContactInvalidError(string description, ContactField field) : base(ErrorType.PaymentContactInvalid, description) {
            Field = field;
        }

        /// <summary>
        /// Constructor for ApplePayContactInvalidError
        /// </summary>
        /// <param name="description">Localized description for the error</param>
        /// <param name="mailingAddressInputField">A field key from <see cref="MailingAddressInput"/> that represents the field that caused this error</param>
        /// <exception cref="ArgumentException">The value given for mailingAddressInputField does not have an equivalent representation as a ContactField</exception>
        public ApplePayContactInvalidError(string description, string mailingAddressInputField) : base(ErrorType.PaymentContactInvalid, description) {
            Field = ContactFieldFromMailingAddressInputField(mailingAddressInputField);
        }

        public override object ToJson() {
            var dict = (Dictionary<string, object>) base.ToJson();
            dict["Field"] = Field;
            return dict;
        }

        /// Field is a string representation of a field key from MailingAddressInput
        private ContactField ContactFieldFromMailingAddressInputField(string field) {
            if (field.Equals(MailingAddressInput.zipFieldKey)) {
                return ContactField.PostalAddress;
            } else if (field.Equals(MailingAddressInput.phoneFieldKey)) {
                return ContactField.PhoneNumber;
            } else if (field.Equals(MailingAddressInput.firstNameFieldKey) || field.Equals(MailingAddressInput.lastNameFieldKey)) {
                return ContactField.Name;
            } else {
                throw new ArgumentException("No matching ContactField for field with value: " + field);
            }
        }
    }
}
#endif