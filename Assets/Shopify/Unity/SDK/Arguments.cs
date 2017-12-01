namespace Shopify.Unity.SDK {
    using System.Collections.Specialized;
    using System.Collections;
    using System.Text;
    using System;

    /// <summary>
    /// Is used by GraphQL generator classes to create argument strings.
    /// </summary>
    public class Arguments {
        private StringBuilder query;
        private bool hasArguments = false;

        public Arguments() {
            this.query = new StringBuilder();
        }

        /// <summary>
        /// Adds an argument.
        /// </summary>
        /// <param name="name">Name of the argument to be added</param>
        /// <param name="value">Value of the argument to be added</param>
        public void Add(string name, object value) {
            if (hasArguments) {
                query.Append(",");
            } else {
                query.Append("(");
            }

            query.Append(name);
            query.Append(":");

            query.Append(InputValueToString.Get(value));

            hasArguments = true;
        }

        /// <summary>
        /// Returns a string that is the a GraphQL argument list.
        /// </summary>
        public override string ToString() {
            if (hasArguments) {
                return query.ToString() + ")";
            } else {
                return "";
            }
        }
    }
}