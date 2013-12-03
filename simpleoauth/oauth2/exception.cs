using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace SimpleOAuth.OAuth2
{
    /// <summary>
    /// An OAuth2 exception.
    /// </summary>
    /// <remarks>
    /// <c>Message</c> is equivalent to the error description when sent by the server.
    /// </remarks>
    [Serializable]
    public class OAuth2Exception : Exception
    {
        #region Properties
        /// <summary>
        /// The error.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The error uri, if provided.
        /// </summary>
        public string ErrorUri { get; private set; }

        /// <summary>
        /// State, if provided.
        /// </summary>
        public string State { get; private set; }
        #endregion Properties

        #region Construction / Destruction
        /// <summary>
        /// Initializes a new instance of <c>OAuth2Exception</c>.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        public OAuth2Exception(string message) : base(message)
        {
            this.Error = message;
        }

        /// <summary>
        /// Initializes a new instance of <c>OAuth2Exception</c>.
        /// </summary>
        /// <param name="error">The error returned from the server.</param>
        /// <param name="errorDescription">The error description returned from the server.</param>
        public OAuth2Exception(string error, string errorDescription) : base(errorDescription)
        {
            this.Error = error;
        }

        /// <summary>
        /// Initializes a new instance of <c>OAuth2Exception</c>.
        /// </summary>
        /// <param name="error">The error returned from the server.</param>
        /// <param name="errorDescription">The error description returned from the server.</param>
        /// <param name="errorUri">The error uri returned from the server.</param>
        public OAuth2Exception(string error, string errorDescription, string errorUri) : base(errorDescription)
        {
            this.Error = error;
            this.ErrorUri = errorUri;
        }

        /// <summary>
        /// Initializes a new instance of <c>OAuth2Exception</c>.
        /// </summary>
        /// <param name="error">The error returned from the server.</param>
        /// <param name="errorDescription">The error description returned from the server.</param>
        /// <param name="errorUri">The error uri returned from the server.</param>
        /// <param name="state">State returned from the server.</param>
        public OAuth2Exception(string error, string errorDescription, string errorUri, string state) : base(errorDescription)
        {
            this.Error = error;
            this.ErrorUri = errorUri;
            this.State = state;
        }

        /// <summary>
        /// Initializes a new instance of <c>OAuth2Exception</c>.
        /// </summary>
        /// <param name="message">A message about the exception.</param>
        /// <param name="innerException">The <see cref="Exception"/> that caused this exception.</param>
        public OAuth2Exception(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of <c>OAuth2Exception</c>.
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> was <see langword="null"/>.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected OAuth2Exception(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (null == info)
                throw new ArgumentNullException("info");

            this.Error = info.GetString("Error");
            this.ErrorUri = info.GetString("ErrorUri");
            this.State = info.GetString("State");
        }
        #endregion Construction / Destruction

        #region ISerializable Implementation
        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">Holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">Contains contextual information about the source or destination</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> was <see langword="null"/>.</exception>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        override public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (null == info)
                throw new ArgumentNullException("info");

            base.GetObjectData(info, context);

            info.AddValue("Error", this.Error);
            info.AddValue("ErrorUri", this.ErrorUri);
            info.AddValue("State", this.State);
        }
        #endregion ISerializable Implementation
    }
}
