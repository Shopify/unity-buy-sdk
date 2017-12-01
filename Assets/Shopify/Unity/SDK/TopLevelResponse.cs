namespace Shopify.Unity.SDK {
    using System.Collections.Generic;
    using System;
    using MiniJSON;

    /// <summary>
    /// Base class for <see ref="QueryResponse">QueryResponse </see> and <see ref="MutationResponse">MutationResponse </see>,
    /// which are top-level responses for all GraphQL queries.
    /// </summary>
    public class TopLevelResponse {
        private const string KEY_DATA = "data";
        private const string KEY_ERRORS = "errors";

        /// <summary>
        /// Contains a string representing an http error. Is <c>null</c> if there was no http error.
        /// </summary>
        public string HTTPError {
            get {
                return _HTTPError;
            }
        }

        /// <summary>
        /// Contains JSON response data from the GraphQL query. Is <c>null</c> if no data was returned.
        /// </summary>
        public Dictionary<string, object> DataJSON {
            get {
                return _DataJSON;
            }
        }

        /// <summary>
        /// Contains a list of errors as a JSON string returned by the GraphQL end point. Is <c>null</c> if no errors were returned.
        /// </summary>
        public string errors {
            get {
                return _errors;
            }
        }

        private string _HTTPError;
        private Dictionary<string, object> _DataJSON;
        private string _errors;

        public TopLevelResponse(Dictionary<string, object> dataJSON) {
            if (dataJSON == null) {
                throw new InvalidServerResponseException("Server returned invalid JSON");
            }

            object errors;
            object data;

            _HTTPError = null;

            dataJSON.TryGetValue(KEY_ERRORS, out errors);
            dataJSON.TryGetValue(KEY_DATA, out data);

            if (errors == null && data == null) {
                throw new InvalidServerResponseException(String.Format("Response JSON did not contain `{0}`` or `{1}`", KEY_DATA, KEY_ERRORS));
            }

            if (errors != null) {
                _errors = ParseErrors((Dictionary<string, object>) dataJSON);
            }

            if (data != null) {
                _DataJSON = (Dictionary<string, object>) data;
            }
        }

        public TopLevelResponse(string httpError) {
            _HTTPError = httpError;
            _DataJSON = null;
            _errors = null;
        }

        private string ParseErrors(Dictionary<string, object> dataJSON) {
            List<string> errors = new List<string>();

            if (dataJSON[KEY_ERRORS] is List<object>) {
                List<object> errorsJSON = (List<object>) dataJSON[KEY_ERRORS];

                foreach (Dictionary<string, object> error in errorsJSON) {
                    errors.Add((string) error["message"]);
                }
            } else {
                throw new InvalidServerResponseException(String.Format("Response JSON at `{0}` did not contain an Array of Object's", KEY_ERRORS));
            }

            return Json.Serialize(errors);
        }
    }
}