using System;
using System.Configuration;

namespace SimpleOAuth.OAuth2
{
    /// <summary>
    /// The OAuth2 client configuration section.
    /// </summary>
    public class OAuth2Configuration : ConfigurationSection
    {
        #region Constants
        /// <summary>
        /// The configuration section name. (<c>simpleoauth.oauth2</c>)
        /// </summary>
        public const string SectionName = "simpleoauth.oauth2";

        private const string CLIENTS = "clients";
        #endregion Constants

        #region Static Properties
        /// <summary>
        /// Gets the current client configuration.
        /// </summary>
        static public OAuth2Configuration Current { get { return (OAuth2Configuration)ConfigurationManager.GetSection(SectionName); } }
        #endregion Static Properties

        #region Properties
        /// <summary>
        /// Gets the client configurations. (<c>clients</c>)
        /// </summary>
        [ConfigurationProperty(CLIENTS, IsRequired = true)]
        public ClientConfigurationCollection Clients
        {
            get { return (ClientConfigurationCollection)base[CLIENTS]; }
        }
        #endregion Properties
    }

    /// <summary>
    /// Represents a client configuration.
    /// </summary>
    public class ClientConfiguration : NamedConfigurationElement
    {
        #region Constants
        private const string AUTHORIZATIONSTATEREQUIRED = "authorizationStateRequired";
        private const string AUTHORIZATIONURL = "authorizationUrl";
        private const string CLIENTID = "clientId";
        private const string CLIENTSECRET = "clientSecret";
        private const string REDIRECTURL = "redirectUrl";
        private const string SCOPE = "scope";
        private const string TOKENURL = "tokenUrl";
        #endregion Constants

        #region Properties
        /// <summary>
        /// Gets / sets if state is required for <see cref="OAuth2Client.GetAuthorizationUrl"/>. (<c>authorizationStateRequired</c>)
        /// </summary>
        [ConfigurationProperty(AUTHORIZATIONSTATEREQUIRED, IsRequired = false, DefaultValue = false)]
        public bool AuthorizationStateRequired
        {
            get { return (bool)base[AUTHORIZATIONSTATEREQUIRED]; }
            set { base[AUTHORIZATIONSTATEREQUIRED] = value; }
        }

        /// <summary>
        /// Gets / sets the authorization url. (<c>authorizationUrl</c>)
        /// </summary>
        [ConfigurationProperty(AUTHORIZATIONURL, IsRequired = true)]
        public string AuthorizationUrl
        {
            get { return (string)base[AUTHORIZATIONURL]; }
            set { base[AUTHORIZATIONURL] = value; }
        }

        /// <summary>
        /// Gets / sets the client id. (<c>clientId</c>)
        /// </summary>
        [ConfigurationProperty(CLIENTID, IsRequired = true)]
        public string ClientId
        {
            get { return (string)base[CLIENTID]; }
            set { base[CLIENTID] = value; }
        }

        /// <summary>
        /// Gets / sets the client secret. (<c>clientSecret</c>)
        /// </summary>
        [ConfigurationProperty(CLIENTSECRET, IsRequired = true)]
        public string ClientSecret
        {
            get { return (string)base[CLIENTSECRET]; }
            set { base[CLIENTSECRET] = value; }
        }

        /// <summary>
        /// Gets / sets the redirect url. (<c>redirectUrl</c>)
        /// </summary>
        [ConfigurationProperty(REDIRECTURL, IsRequired = true)]
        public string RedirectUrl
        {
            get { return (string)base[REDIRECTURL]; }
            set { base[REDIRECTURL] = value; }
        }

        /// <summary>
        /// Gets / sets the desired permissions scope. (<c>scope</c>)
        /// </summary>
        [ConfigurationProperty(SCOPE, IsRequired = false)]
        public string Scope
        {
            get { return (string)base[SCOPE]; }
            set { base[SCOPE] = value; }
        }

        /// <summary>
        /// Gets / sets the token url. (<c>tokenUrl</c>)
        /// </summary>
        [ConfigurationProperty(TOKENURL, IsRequired = true)]
        public string TokenUrl
        {
            get { return (string)base[TOKENURL]; }
            set { base[TOKENURL] = value; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Validates the configuration.
        /// </summary>
        /// <exception cref="ConfigurationErrorsException">Something was misconfigured.</exception>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.AuthorizationUrl))
                throw new ConfigurationErrorsException("AuthorizationUrl is not set.");

            if (string.IsNullOrWhiteSpace(this.ClientId))
                throw new ConfigurationErrorsException("ClientId is not set.");

            if (string.IsNullOrWhiteSpace(this.ClientSecret))
                throw new ConfigurationErrorsException("ClientSecret is not set.");

            if (string.IsNullOrWhiteSpace(this.TokenUrl))
                throw new ConfigurationErrorsException("TokenUrl is not set.");
        }
        #endregion Methods
    }

    /// <summary>
    /// Represents a collection of <see cref="ClientConfiguration"/>s
    /// </summary>
    public class ClientConfigurationCollection : NamedConfigurationElementCollection<ClientConfiguration> { }
}
