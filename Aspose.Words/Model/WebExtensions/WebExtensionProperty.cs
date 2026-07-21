// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

namespace Aspose.Words.WebExtensions
{
    /// <summary>
    /// Specifies a web extension custom property.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/work-with-office-add-ins/">Work with Office Add-ins</a> documentation article.</para>
    /// </summary>
    /// <dev>2.2.1 CT_OsfWebExtensionProperty</dev>
    public class WebExtensionProperty
    {
        /// <summary>
        /// Hide default ctr.
        /// </summary>
        internal WebExtensionProperty() { }

        /// <summary>
        /// Creates web extension custom property with specified name and value.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        public WebExtensionProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///  Specifies a custom property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  Specifies a custom property value.
        /// </summary>
        public string Value { get; set; }
    }
}
