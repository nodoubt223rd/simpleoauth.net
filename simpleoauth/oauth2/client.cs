using System;
using System.Collections.Generic;
using System.Net;
using System.Web;

namespace SimpleOAuth.OAuth2
{
    /// <summary>
    /// A generic OAuth2 Client.
    /// </summary>
    public class OAuth2Client
    {
        #region Nested
        /// <summary>
        /// Represents an explicit authorization request return.
        /// </summary>
        public class AuthorizationReturn
        {
            #region Properties
            /// <summary>
            /// The authorization code.
            /// </summary>
            public string Code { get; internal set; }

            /// <summary>
            /// The state, if provided.
            /// </summary>
            public string State { get; internal set; }
            #endregion Properties
        }
        #endregion Nested

        #region Properties
        /// <summary>
        /// The configuration to use.
        /// </summary>
        protected ClientConfiguration Configuration { get; private set; }
        #endregion Properties

        #region Construction / Destruction
        /// <summary>
        /// Initializes a new instance of <c>OAuth2Client</c>.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configuration"/> was <see langword="null"/>.</exception>
        public OAuth2Client(ClientConfiguration configuration)
        {
            if (null == configuration)
                throw new ArgumentNullException("configuration");

            this.Configuration = configuration;
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Creates an <see cref="HttpWebRequest"/> for a given uri and token.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="token">The <see cref="AccessToken"/>.</param>
        /// <returns>An <see cref="HttpWebRequest"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> was <see langword="null"/> or <see cref="string.Empty"/> or <paramref name="token"/> was null.</exception>
        static public HttpWebRequest CreateAuthorizedRequest(string uri, AccessToken token)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentNullException("uri");

            if (null == token)
                throw new ArgumentNullException("token");

            HttpWebRequest r = (HttpWebRequest)HttpWebRequest.Create(uri);

            r.Headers.Add("Authorization", (TokenType.bearer.Equals(token.TokenType) ? "Bearer" : "MAC") + " " + token.Token);

            return r;
        }

        /// <summary>
        /// Gets an explicit <see cref="AuthorizationReturn"/> from an authorization request,
        /// redirected back from the target server after sending the user to the url in
        /// <see cref="GetAuthorizationUrl"/>.
        /// </summary>
        /// <param name="expectedState">Optional expected state from passing one to <see cref="GetAuthorizationUrl"/>.</param>
        /// <returns>An <see cref="AuthorizationReturn"/> or <see langword="null"/> if none was detected.</returns>
        /// <exception cref="InvalidOperationException">There is no current <see cref="HttpContext"/>.</exception>
        /// <exception cref="OAuth2Exception">Something went wrong.</exception>
        static public AuthorizationReturn GetAuthorizationReturn(string expectedState = null)
        {
            if (null == HttpContext.Current)
                throw new InvalidOperationException("There is no current HttpContext.");

            HttpRequest request = HttpContext.Current.Request;

            string s = request[Parameter.error];

            if (!string.IsNullOrEmpty(s))
                throw new OAuth2Exception(s, request[Parameter.error_description], request[Parameter.error_uri], request[Parameter.state]);

            s = request[Parameter.code];

            if (string.IsNullOrEmpty(s))
                return null;

            AuthorizationReturn r = new AuthorizationReturn
            {
                Code = s,
                State = request[Parameter.state]
            };

            if (!string.IsNullOrWhiteSpace(expectedState) && !string.IsNullOrWhiteSpace(r.State) && !expectedState.Equals(r.State))
                throw new OAuth2Exception("The expected state was not received.");

            return r;
        }
        #endregion Static Methods

        #region Methods
        /// <summary>
        /// Get the authorization url.
        /// </summary>
        /// <param name="state">Optional state.</param>
        /// <param name="implicit"><see langword="true"/> for an implicit authorization; otherwise (default) <see langword="false"/>.</param>
        /// <exception cref="InvalidOperationException">The client is not configured correctly.</exception>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">The client is not configured correctly.</exception>
        /// <exception cref="ArgumentNullException">The configuration <see cref="ClientConfiguration.AuthorizationStateRequired"/> is <see langword="true"/> and <paramref name="state "/> was <see langword="null"/> or <see cref="string.Empty"/>.</exception>
        public string GetAuthorizationUrl(string state = null, bool @implicit = false)
        {
            _ValidateConfiguration();

            if (this.Configuration.AuthorizationStateRequired && string.IsNullOrWhiteSpace(state))
                throw new ArgumentNullException("State is required.");

            Dictionary<string, object> p = new Dictionary<string, object>();

            p.Add(Parameter.response_type, @implicit ? ResponseType.token : ResponseType.code);
            p.Add(Parameter.client_id, this.Configuration.ClientId);

            if (!string.IsNullOrWhiteSpace(this.Configuration.Scope))
                p.Add(Parameter.scope, this.Configuration.Scope);

            if (!string.IsNullOrEmpty(this.Configuration.RedirectUrl))
                p.Add(Parameter.redirect_uri, this.Configuration.RedirectUrl);

            if (!string.IsNullOrWhiteSpace(state))
                p.Add(Parameter.state, state);

            FixupAuthorizationUrlParameters(p);

            return this.Configuration.AuthorizationUrl + p.ToQueryString();
        }

        /// <summary>
        /// When overridden in derived classes, allows the opportunity to examine and modify the
        /// authorization url parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        virtual protected void FixupAuthorizationUrlParameters(Dictionary<string, object> parameters)
        {
        }

        /// <summary>
        /// Gets an access token given a redirection back to the <see cref="ClientConfiguration.RedirectUrl"/> from an authorization url.
        /// </summary>
        /// <param name="code">The authorization code, required if <paramref name="implicit"/> is <see langword="false"/>.</param>
        /// <param name="expectedState">Optional expected state, if any.</param>
        /// <param name="implicit"><see langword="true"/> for an implicit token request; otherwise (default) <see langword="false"/>.</param>
        /// <returns>The <see cref="AccessToken"/> constructed by <see cref="OnGotTokenResponse"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="implicit"/> is <see langword="false"/> and <paramref name="code"/> was <see langword="null"/> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="InvalidOperationException">The client is not configured correctly.</exception>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">The client is not configured correctly.</exception>
        /// <exception cref="OAuth2Exception">Something went wrong.</exception>
        public AccessToken GetAccessToken(string code = null, string expectedState = null, bool @implicit = false)
        {
            _ValidateConfiguration();

            if (@implicit)
                return new AccessToken(HttpContext.Current.Request);

            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("code");

            try
            {
                Dictionary<string, object> p = new Dictionary<string, object>();

                p.Add(Parameter.grant_type, GrantType.authorization_code);
                p.Add(Parameter.code, code);
                p.Add(Parameter.client_id, this.Configuration.ClientId);
                p.Add(Parameter.client_secret, this.Configuration.ClientSecret);

                if (!string.IsNullOrWhiteSpace(this.Configuration.RedirectUrl))
                    p.Add(Parameter.redirect_uri, this.Configuration.RedirectUrl);

                FixupGetAccessTokenParameters(p);

                string uri = this.Configuration.TokenUrl + p.ToQueryString();

                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);

                req.ContentType = ApplicationContentType.FormEncoded;

                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                    return OnGotTokenResponse(res, expectedState);
            }
            catch (OAuth2Exception)
            {
                throw;
            }
            catch (Exception e)
            {
                if (e is WebException)
                    return OnGotTokenResponse((HttpWebResponse)((WebException)e).Response);

                throw new OAuth2Exception("Could not get an access token -- " + e.Message, e);
            }
        }

        /// <summary>
        /// When overridden in derived classes, allows the opportunity to examine and modify the
        /// access token url parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        virtual protected void FixupGetAccessTokenParameters(Dictionary<string, object> parameters)
        {
        }

        /// <summary>
        /// Refreshes an access token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <returns>The <see cref="AccessToken"/> constructed by <see cref="OnGotTokenResponse"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="refreshToken"/> was <see langword="null"/> or <see cref="string.Empty"/>.</exception>
        /// <exception cref="InvalidOperationException">The client is not configured correctly.</exception>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">The client is not configured correctly.</exception>
        /// <exception cref="OAuth2Exception">Something went wrong.</exception>
        public AccessToken RefreshToken(string refreshToken)
        {
            _ValidateConfiguration();

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentNullException("refreshToken");

            try
            {
                Dictionary<string, object> p = new Dictionary<string, object>();

                p.Add(Parameter.grant_type, GrantType.refresh_token);
                p.Add(Parameter.refresh_token, refreshToken);

                if (!string.IsNullOrWhiteSpace(this.Configuration.Scope))
                    p.Add(Parameter.scope, this.Configuration.Scope);

                FixupRefreshTokenParameters(p);

                string uri = this.Configuration.TokenUrl + p.ToQueryString();

                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uri);

                req.ContentType = ApplicationContentType.FormEncoded;

                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                    return OnGotTokenResponse(res);
            }
            catch (OAuth2Exception)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new OAuth2Exception("Could not get an access token -- " + e.Message, e);
            }
        }

        /// <summary>
        /// When overridden in derived classes, allows the opportunity to examine and modify the
        /// refresh token url parameters.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        virtual protected void FixupRefreshTokenParameters(Dictionary<string, object> parameters)
        {
        }

        /// <summary>
        /// Constructs an <see cref="AccessToken"/> from the server response.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/>.</param>
        /// <param name="state">Optional expected state.</param>
        /// <returns>An <see cref="AccessToken"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response"/> was <see langword="null"/>.</exception>
        /// <exception cref="OAuth2Exception">Something wend wrong, e.g. the response contained an error from the server.</exception>
        virtual protected AccessToken OnGotTokenResponse(HttpWebResponse response, string state = null)
        {
            if (null == response)
                throw new ArgumentNullException("response");

            return new AccessToken(response, state);
        }
        #endregion Methods

        #region Implementation
        private void _ValidateConfiguration()
        {
            if (null == HttpContext.Current)
                throw new InvalidOperationException("There is no current HttpContext.");

            this.Configuration.Validate();
        }
        #endregion Implementation
    }
}
