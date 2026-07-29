// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2008 by Roman Korchagin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents a single custom XML attribute or a smart tag property.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Used as an item of a <see cref="CustomXmlPropertyCollection"/> collection.</para>
    /// </remarks>
    public class CustomXmlProperty
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="name">The name of the property. Cannot be <c>null</c>.</param>
        /// <param name="uri">The namespace URI of the property. Cannot be <c>null</c>.</param>
        /// <param name="value">The value of the property. Cannot be <c>null</c>.</param>
        public CustomXmlProperty(string name, string uri, string value)
        {
            ArgumentUtil.CheckNotNull(name, "name");
            mName = name;

            Uri = uri;
            Value = value;
        }

        /// <summary>
        /// Specifies the name of the custom XML attribute or smart tag property.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// <para>Default is empty string.</para>
        /// </remarks>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Gets or sets the namespace URI of the custom XML attribute or smart tag property.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// <para>Default is empty string.</para>
        /// </remarks>
        public string Uri
        {
            get { return mUri; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "uri");
                mUri = value;
            }
        }

        /// <summary>
        /// Gets or sets the value of the custom XML attribute or smart tag property.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// <para>Default is empty string.</para>
        /// </remarks>
        public string Value
        {
            get { return mValue; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");
                mValue = value;
            }
        }

        /// <summary>
        /// Makes a deep clone of this object.
        /// </summary>
        internal CustomXmlProperty Clone()
        {
            // Shallow copy is actually enough because all fields are strings.
            return (CustomXmlProperty)MemberwiseClone();            
        }

        private readonly string mName;
        private string mUri;
        private string mValue;
    }
}
