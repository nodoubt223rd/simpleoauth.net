using System;

namespace SimpleOAuth.OAuth2
{
    /// <summary>
    /// OAuth2 expected application content types.
    /// </summary>
    static public class ApplicationContentType
    {
        #region Constants
        /// <summary>
        /// <c>application/x-www-form-urlencoded</c>
        /// </summary>
        public const string FormEncoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// <c>text/javascript</c>
        /// </summary>
        public const string Javascript = "text/javascript";

        /// <summary>
        /// <c>application/json</c>
        /// </summary>
        public const string Json = "application/json";
        #endregion Constants
    }

    /// <summary>
    /// Grant type.
    /// </summary>
    static public class GrantType
    {
        #region Constants
        /// <summary>
        /// <c>authorization_code</c>
        /// </summary>
        public const string authorization_code = "authorization_code";

        /// <summary>
        /// <c>refresh_token</c>
        /// </summary>
        public const string refresh_token = "refresh_token";
        #endregion Constants
    }

    /// <summary>
    /// Parameter (request / response).
    /// </summary>
    static public class Parameter
    {
        #region Constants
        /// <summary>
        /// <c>access_token</c>
        /// </summary>
        public const string access_token = "access_token";

        /// <summary>
        /// <c>client_id</c>
        /// </summary>
        public const string client_id = "client_id";

        /// <summary>
        /// <c>client_secret</c>
        /// </summary>
        public const string client_secret = "client_secret";

        /// <summary>
        /// <c>code</c>
        /// </summary>
        public const string code = "code";

        /// <summary>
        /// <c>error</c>
        /// </summary>
        public const string error = "error";

        /// <summary>
        /// <c>error_description</c>
        /// </summary>
        public const string error_description = "error_description";

        /// <summary>
        /// <c>error_uri</c>
        /// </summary>
        public const string error_uri = "error_uri";

        /// <summary>
        /// <c>expires_in</c>
        /// </summary>
        public const string expires_in = "expires_in";

        /// <summary>
        /// <c>grant_type</c>
        /// </summary>
        public const string grant_type = "grant_type";

        /// <summary>
        /// <c>redirect_uri</c>
        /// </summary>
        public const string redirect_uri = "redirect_uri";

        /// <summary>
        /// <c>refresh_token</c>
        /// </summary>
        public const string refresh_token = "refresh_token";

        /// <summary>
        /// <c>response_type</c>
        /// </summary>
        public const string response_type = "response_type";

        /// <summary>
        /// <c>scope</c>
        /// </summary>
        public const string scope = "scope";

        /// <summary>
        /// <c>state</c>
        /// </summary>
        public const string state = "state";

        /// <summary>
        /// <c>token_type</c>
        /// </summary>
        public const string token_type = "token_type";
        #endregion Constants
    }

    /// <summary>
    /// Response type.
    /// </summary>
    static public class ResponseType
    {
        #region Constants
        /// <summary>
        /// <c>code</c>
        /// </summary>
        public const string code = "code";

        /// <summary>
        /// <c>token</c>
        /// </summary>
        public const string token = "token";
        #endregion Constants
    }

    /// <summary>
    /// Token type.
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// <c>bearer</c>
        /// </summary>
        bearer,
        /// <summary>
        /// <c>mac</c>
        /// </summary>
        mac
    }
}
