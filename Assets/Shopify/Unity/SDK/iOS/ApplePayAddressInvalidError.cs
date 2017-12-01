#if UNITY_IOS
namespace Shopify.Unity.SDK.iOS {
    using System.Collections.Generic;
    using System.Collections;
    using System;

    public class ApplePayAddressInvalidError : ApplePayError {
        public enum AddressField {
            Street,
            Sublocality,
            City,
            SubAdministrativeArea,
            State,
            PostalCode,
            Country,
            ISOCountryCode
        }

        public readonly AddressField Field;

        /// <summary>
        /// Constructor for ApplePayAddressInvalidError
        /// </summary>
        /// <param name="type">Type of AddressInvalidError</param>
        /// <param name="description">Localized description for the error</param>
        /// <param name="field">A <see cref="AddressField"/> that represents the field that caused this error</param>
        public ApplePayAddressInvalidError(ErrorType type, string description, AddressField field) : base(type, description) {
            Field = field;
        }

        /// <summary>
        /// Constructor for ApplePayAddressInvalidError
        /// </summary>
        /// <param name="type">Type of AddressInvalidError</param>
        /// <param name="description">Localized description for the error</param>
        /// <param name="mailingAddressInputField">A field key from <see cref="MailingAddressInput"/> that represents the field that caused this error</param>
        /// <exception cref="ArgumentException">The value given for mailingAddressInputField does not have an equivalent representation as a AddressField</exception>
        public ApplePayAddressInvalidError(ErrorType type, string description, string mailingAddressInputField) : base(type, description) {
            Field = AddressFieldFromMailingAddressInputField(mailingAddressInputField);
        }

        public override object ToJson() {
            var dict = (Dictionary<string, object>) base.ToJson();
            dict["Field"] = Field;
            return dict;
        }

        /// Field is a string representation of a field key from MailingAddressInput
        private AddressField AddressFieldFromMailingAddressInputField(string field) {
            if (field.Equals(MailingAddressInput.address1FieldKey) || field.Equals(MailingAddressInput.address2FieldKey)) {
                return AddressField.Street;
            } else if (field.Equals(MailingAddressInput.cityFieldKey)) {
                return AddressField.City;
            } else if (field.Equals(MailingAddressInput.countryFieldKey)) {
                return AddressField.Country;
            } else if (field.Equals(MailingAddressInput.provinceFieldKey)) {
                return AddressField.State;
            } else if (field.Equals(MailingAddressInput.zipFieldKey)) {
                return AddressField.PostalCode;
            } else {
                throw new ArgumentException("No matching AddressField for MailingAddressInputField with value: " + field);
            }
        }
    }
}
#endif