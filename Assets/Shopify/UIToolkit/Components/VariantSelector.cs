namespace Shopify.UIToolkit {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Shopify.Unity;

    using UnityEngine;

    /// <summary>
    /// Converts the list of product variant and their respective selectable options that looks like:
    /// [{
    ///    "selectedOptions": [{
    ///       "name": "optionName1",
    ///       "value": "value1"
    ///     }, {
    ///       "name": "optionName2",
    ///       "value": "value1"
    ///     }]
    /// },
    /// {
    ///    "selectedOptions": [{
    ///       "name": "optionName1",
    ///       "value": "value2"
    ///     }, {
    ///       "name": "optionName2",
    ///       "value": "value2"
    ///     }]
    /// }]
    /// into a map that looks like:
    /// {
    ///   "optionName1": ["value1", "value2", ...],
    ///   "optionName2": ["value1", "value2", ...],
    /// }
    ///
    /// </summary>
    public class VariantSelector {
        private const string SingleVariantTitle = "Default Title";

        private List<ProductVariant> _variants;

        public VariantSelector(List<ProductVariant> variants) {
            _variants = variants;
        }

        public Dictionary<string, List<string>> AllOptions() {
            return ValidOptionsForSelections(new Dictionary<string, string>());
        }

        public Dictionary<string, List<string>> ValidOptionsForSelections(Dictionary<string, string> selections) {
            if (IsSingleOrEmpty()) {
                return null;
            }

            var allOptions = new Dictionary<string, List<string>>();
            foreach (var variant in _variants) {
                var options = variant.selectedOptions();

                var containsSelections = selections.Aggregate(true, (accum, entry) => {
                    return accum && options.Find(o => {
                        return o.name().Equals(entry.Key) && o.value().Equals(entry.Value);
                    }) != null;
                });

                // Skip this variant if it's not valid with the selections.
                if (containsSelections) {
                    options.Aggregate(allOptions, (accum, option) => {
                        List<string> values;
                        if (!accum.ContainsKey(option.name())) {
                            accum.Add(option.name(), new List<string>());
                        }
                        values = accum[option.name()];

                        // Avoid duplicates.
                        if (values.Find(v => v.Equals(option.value())) == null) {
                            values.Add(option.value());
                        }

                        return accum;
                    });
                }
            }
            return allOptions;
        }

        private bool IsSingleOrEmpty() {
            if (_variants.Count == 0)  {
                return true;
            }

            // This is what the Storefront API returns back to us when there are no variants...
            if (_variants.Count == 1 && _variants[0].title() == SingleVariantTitle) {
                return true;
            }

            return false;
        }
    }
}

