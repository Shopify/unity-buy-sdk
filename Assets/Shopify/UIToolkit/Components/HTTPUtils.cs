namespace Shopify.UIToolkit {
    using System;
    using System.Text.RegularExpressions;

    public static class HTTPUtils {
        /// <summary>
        /// Parses the 'Status XXX' HTTP header field for the status code number.
        /// </summary>
        /// <param name="statusLine">HTTP STATUS field</param>
        /// <returns>HTTP Status code</returns>
        public static int ParseResponseCode(string statusLine) {
            int ret = 0;
            var regex = new Regex("HTTP\\/1.1 (\\d*)");
            var match = regex.Match(statusLine);
            if (match.Success && match.Groups.Count >= 2) {
                Group g = match.Groups[1];
                CaptureCollection cc = g.Captures;
                if (cc.Count > 0) {
                    Capture c = cc[0];
                    int.TryParse(c.ToString(), out ret);
                }
            }
            return ret;
        }
    }
}
