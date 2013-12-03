using System;
using System.Configuration;

namespace SimpleOAuth
{
    /// <summary>
    /// Configuration element which has a name.
    /// </summary>
    public class NamedConfigurationElement : ConfigurationElement
    {
        #region Constants
        private const string NAME = "name";
        #endregion Constants

        #region Properties
        /// <summary>
        /// The site name. (<c>name</c>.)
        /// </summary>
        [ConfigurationProperty(NAME, IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base[NAME]; }
            set { base[NAME] = value; }
        }
        #endregion Properties
    }

    /// <summary>
    /// Collection of <see cref="NamedConfigurationElement"/>-derived objects.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="NamedConfigurationElement"/> object.</typeparam>
    public class NamedConfigurationElementCollection<T> : ConfigurationElementCollection where T : NamedConfigurationElement, new()
    {
        #region ConfigurationElementCollection Overrides
        #region Properties
        /// <summary>
        /// Gets / sets the element at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The element, if found.</returns>
        /// <exception cref="ConfigurationErrorsException"><paramref name="index"/> is less than 0.  - or - There is no element at the specified index.</exception>
        public T this[int index]
        {
            get { return (T)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);

                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Gets the element with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The element, if found, otherwise <see langword="null"/>.</returns>
        new public T this[string name]
        {
            get { return (T)BaseGet(name); }
        }

        /// <summary>
        /// Gets the type of collection.
        /// </summary>
        /// <remarks><see cref="ConfigurationElementCollectionType.AddRemoveClearMap"/></remarks>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Creates a new element.
        /// </summary>
        /// <returns>The new element.</returns>
        override protected ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element.
        /// </summary>
        /// <param name="element">The element whose key will be returned.</param>
        /// <returns>The element key.</returns>
        override protected object GetElementKey(ConfigurationElement element)
        {
            return ((T)element).Name;
        }
        #endregion Methods
        #endregion ConfigurationElementCollection Overrides
    }
}
