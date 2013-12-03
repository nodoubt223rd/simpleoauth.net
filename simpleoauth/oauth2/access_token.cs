using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;

using Newtonsoft.Json.Linq;

namespace SimpleOAuth.OAuth2
{
    /// <summary>
    /// Represents an OAuth2 access token.
    /// </summary>
    [Serializable]
    public class AccessToken : ISerializable
    {
        #region Properties
        /// <summary>
        /// Expiration.
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// The scope as requested or returned by the server.
        /// </summary>
        public List<string> Scope { get; private set; }

        /// <summary>
        /// The refresh token, if any.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The token type.
        /// </summary>
        public TokenType TokenType { get; set; }
        #endregion Properties

        #region Construction / Destruction
        /// <summary>
        /// Initializes a new instance of <c>AccessToken</c>
        /// </summary>
        public AccessToken()
        {
            this.Expiration = DateTime.MaxValue;
            this.Scope = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of <c>AccessToken</c>.
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> was <see langword="null"/>.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected AccessToken(SerializationInfo info, StreamingContext context)
        {
            if (null == info)
                throw new ArgumentNullException("info");

            this.Expiration = info.GetDateTime("Expiration");
            this.Scope = (List<string>)info.GetValue("Scope", typeof(List<string>));
            this.RefreshToken = info.GetString("RefreshToken");
            this.Token = info.GetString("Token");
            this.TokenType = (TokenType)info.GetValue("TokenType", typeof(TokenType));
        }

        internal AccessToken(HttpRequest request, string expectedState = null) : this()
        {
            if (null == request)
                throw new ArgumentNullException("request");

            if (!request.ContentType.StartsWith(ApplicationContentType.FormEncoded, StringComparison.InvariantCultureIgnoreCase))
                throw new OAuth2Exception("Unsupported content type \"" + request.ContentType + "\"");

            string e = request[Parameter.error];

            if (!string.IsNullOrEmpty(e))
                throw new OAuth2Exception(e, request[Parameter.error_description], request[Parameter.error_uri], request[Parameter.state]);

            this.Token = request[Parameter.access_token];

            if (string.IsNullOrEmpty(this.Token))
                throw new OAuth2Exception("No " + Parameter.access_token);

            try
            {
                this.TokenType = (TokenType)Enum.Parse(typeof(TokenType), request[Parameter.token_type], true);
            }
            catch
            {
                throw new OAuth2Exception("Unsupported or missing token type.");
            }

            string expires_in = request[Parameter.expires_in];
            string scope = request[Parameter.scope];
            string state = request[Parameter.state];

            if (!string.IsNullOrEmpty(expectedState) && (string.IsNullOrEmpty(state) || !expectedState.Equals(state)))
                throw new OAuth2Exception("The expected state was not received.");

            if (!string.IsNullOrEmpty(expires_in))
            {
                int seconds = 0;

                if (int.TryParse(expires_in, out seconds))
                    this.Expiration = DateTime.Now.AddSeconds(seconds);
            }

            if (!string.IsNullOrEmpty(scope))
                this.Scope.AddRange(scope.Split(' '));
        }

        internal AccessToken(HttpWebResponse response, string expectedState = null) : this()
        {
            if (null == response)
                throw new ArgumentNullException("response");

            if (!(response.ContentType.StartsWith(ApplicationContentType.Json, StringComparison.InvariantCultureIgnoreCase)
                || response.ContentType.StartsWith(ApplicationContentType.Javascript, StringComparison.InvariantCultureIgnoreCase)))
                throw new OAuth2Exception("Unsupported content type \"" + response.ContentType + "\"");

            JObject jr = null;

            using (TextReader tr = new StreamReader(response.GetResponseStream()))
                jr = JObject.Parse(tr.ReadToEnd());

            string e = jr.Value<string>(Parameter.error);

            if (!string.IsNullOrEmpty(e))
                throw new OAuth2Exception(e, jr.Value<string>(Parameter.error_description), jr.Value<string>(Parameter.error_uri), jr.Value<string>(Parameter.state));

            this.Token = jr.Value<string>(Parameter.access_token);

            if (string.IsNullOrEmpty(this.Token))
                throw new OAuth2Exception("No " + Parameter.access_token);

            // assuming bearer
            this.TokenType = TokenType.bearer;

            string tt = jr.Value<string>(Parameter.token_type);

            if (!string.IsNullOrEmpty(tt))
                try
                {
                    this.TokenType = (TokenType)Enum.Parse(typeof(TokenType), tt, true);
                }
                catch
                {
                }

            int expires_in = jr.Value<int>(Parameter.expires_in);
            string scope = jr.Value<string>(Parameter.scope);
            string state = jr.Value<string>(Parameter.state);

            if (!string.IsNullOrEmpty(expectedState) && !string.IsNullOrEmpty(state) && !expectedState.Equals(state))
                throw new OAuth2Exception("The expected state was not received.");

            if (expires_in > 0)
                this.Expiration = DateTime.Now.AddSeconds(expires_in);

            if (!string.IsNullOrEmpty(scope))
                this.Scope.AddRange(scope.Split(' '));

            this.RefreshToken = jr.Value<string>(Parameter.refresh_token);
        }
        #endregion Construction / Destruction

        #region ISerializable Implementation
        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the access token.
        /// </summary>
        /// <param name="info">Holds the serialized object data about the access token.</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> was <see langword="null"/>.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (null == info)
                throw new ArgumentNullException("info");

            info.AddValue("Expiration", this.Expiration);
            info.AddValue("Scope", this.Scope);
            info.AddValue("RefreshToken", this.RefreshToken);
            info.AddValue("Token", this.Token);
            info.AddValue("TokenType", this.TokenType);
        }
        #endregion ISerializable Implementation
    }
}
