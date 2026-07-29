// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/02/2017 by Alexey Butalov

using Aspose.Drawing;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// This class extends NrxXmlBuilder and helps to write VML-specific attributes.
    /// </summary>
    internal class VmlBuilder
    {
        internal VmlBuilder(NrxXmlBuilder builder)
        {
            Debug.Assert(builder != null);
            mBuilder = builder;
        }

        /// <summary>
        /// Writes attribute with object value. Object is converted to VML string.
        /// Value object should have string, int, double, bool or Color underlying type.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="value">Object value for the attribute.</param>
        /// <param name="defaultValue">Default value for the attribute. If provided object has default value, the attribute is not written.</param>
        internal void WriteVmlAttributeIfNotDefault(string attributeName, object value, object defaultValue)
        {
            if (value != null && !value.Equals(defaultValue))
                WriteVmlAttribute(attributeName, value);
        }

        /// <summary>
        /// Writes attribute with object value. Object is converted to VML string.
        /// Value object should have string, int, double, bool or Color underlying type.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="value">Object value for the attribute.</param>
        internal void WriteVmlAttribute(string attributeName, object value)
        {
            if (value is bool)
                mBuilder.WriteAttribute(attributeName, (bool)value ? "t" : "f");
            else if (value is DrColor)
                mBuilder.WriteAttribute(attributeName, VmlColor.ColorToVml((DrColor)value));
            else
                mBuilder.WriteAttribute(attributeName, value);
        }

        /// <summary>
        /// Writes VML attribute with bool value as 't' or 'f'. Attribute is not written if value equlas specified default value.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="value">Bool value for the attribute.</param>
        /// <param name="defaultValue">Default value for the attribute.</param>
        internal void WriteVmlAttributeIfNotDefault(string attributeName, bool value, bool defaultValue)
        {
            if (value != defaultValue)
                mBuilder.WriteAttributeString(attributeName, value ? "t" : "f");
        }

        private readonly NrxXmlBuilder mBuilder;
    }
}
