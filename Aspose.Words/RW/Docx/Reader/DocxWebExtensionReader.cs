// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2019 by Dmitry Sokolov

using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.WebExtensions;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Implements reading of the "2.2.7 CT_OsfWebExtension" element.
    /// </summary>
    internal class DocxWebExtensionReader
    {
        /// <summary>
        /// Reads "2.2.7 CT_OsfWebExtension" element.
        /// </summary>
        /// <param name="relId">Relation identifier of the package part with web extension.</param>
        /// <param name="reader">Document reader object.</param>
        /// <param name="extension">Extension which receives read data.</param>
        internal static void ReadWebExtension(string relId, DocxDocumentReader reader, WebExtension extension)
        {
            Debug.Assert(extension != null);

            DocxXmlReader webExtensionReader = reader.SwitchToPartReaderByRelId(relId);
 
            while (webExtensionReader.MoveToNextAttribute())
            {
                switch (webExtensionReader.LocalName)
                {
                    case "id":
                        extension.Id = webExtensionReader.Value;
                        break;
                    case "frozen":
                        extension.IsFrozen = webExtensionReader.ValueAsBool;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            while (webExtensionReader.ReadChild("webextension")) // we:webextension
            {
                switch (webExtensionReader.LocalName)
                {
                    case "reference":
                        ReadWebExtensionReference(webExtensionReader, extension.Reference);
                        break;
                    case "alternateReferences":
                        ReadAlternateReferences(webExtensionReader, extension.AlternateReferences);
                        break;
                    case "properties":
                        ReadWebExtensionPropertyBag(webExtensionReader, extension.Properties);
                        break;
                    case "bindings":
                        ReadWebExtensionBindingList(webExtensionReader, extension.Bindings);
                        break;
                    case "snapshot":
                        extension.Snapshot = DmlFillReader.ReadBlip(reader);
                        break;
                    default:
                        // For example "extLst", which may be ignored according to MS-OWEXML spec.
                        webExtensionReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }

        /// <summary>
        /// Read "2.2.5 CT_OsfWebExtensionReference" element.
        /// </summary>
        private static void ReadWebExtensionReference(DocxXmlReader xmlReader, WebExtensionReference reference)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "id":
                        reference.Id = xmlReader.Value;
                        break;
                    case "store":
                        reference.Store = xmlReader.Value;
                        break;
                    case "version":
                        reference.Version = xmlReader.Value;
                        break;
                    case "storeType":
                        reference.StoreType = DocxEnum.DocxToWebExtensionStoreType(xmlReader.Value.ToLower());
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            // "WebExtensionReference" has one child node "extLst" which may be ignored according to MS-OWEXML spec.
        }

        /// <summary>
        /// Reads "2.2.6 CT_OsfWebExtensionReferenceList" element.
        /// </summary>
        private static void ReadAlternateReferences(DocxXmlReader xmlReader, WebExtensionReferenceCollection references)
        {
            while (xmlReader.ReadChild("alternateReferences")) // we:webextension
            {
                switch (xmlReader.LocalName)
                {
                    case "reference":
                    {
                        WebExtensionReference reference = new WebExtensionReference();
                        references.Add(reference);

                        ReadWebExtensionReference(xmlReader, reference);
                        break;
                    }
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads "2.2.2 CT_OsfWebExtensionPropertyBag" element.
        /// </summary>
        private static void ReadWebExtensionPropertyBag(DocxXmlReader xmlReader, WebExtensionPropertyCollection properties)
        {
            while (xmlReader.ReadChild("properties")) // we:properties
            {
                switch (xmlReader.LocalName)
                {
                    case "property":
                        WebExtensionProperty property = ReadWebExtensionProperty(xmlReader);
                        properties.Add(property);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads "2.2.1 CT_OsfWebExtensionProperty" element.
        /// </summary>
        private static WebExtensionProperty ReadWebExtensionProperty(DocxXmlReader xmlReader)
        {
            WebExtensionProperty property = new WebExtensionProperty();

            while (xmlReader.MoveToNextAttribute()) // we:property
            {
                switch (xmlReader.LocalName)
                {
                    case "name":
                        property.Name = xmlReader.Value;
                        break;
                    case "value":
                        property.Value = xmlReader.Value;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            return property;
        }

        /// <summary>
        /// Reads "2.2.4 CT_OsfWebExtensionBindingList" element.
        /// </summary>
        private static void ReadWebExtensionBindingList(DocxXmlReader xmlReader, WebExtensionBindingCollection bindings)
        {
            while (xmlReader.ReadChild("bindings")) // we:bindings
            {
                switch (xmlReader.LocalName)
                {
                    case "binding":
                        WebExtensionBinding binding = ReadWebExtensionBinding(xmlReader);
                        bindings.Add(binding);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads "2.2.3 CT_OsfWebExtensionBinding" element.
        /// </summary>
        private static WebExtensionBinding ReadWebExtensionBinding(DocxXmlReader xmlReader)
        {
            WebExtensionBinding binding = new WebExtensionBinding();

            while (xmlReader.MoveToNextAttribute()) // we:binding
            {
                switch (xmlReader.LocalName)
                {
                    case "id":
                        binding.Id = xmlReader.Value;
                        break;
                    case "appref":
                        binding.AppRef = xmlReader.Value;
                        break;
                    case "type":
                        binding.BindingType = DocxEnum.DocxToWebExtensionBindingType(xmlReader.Value.ToLower());
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            // "WebExtensionBinding" has one child node "extLst" which may be ignored according to MS-OWEXML spec.

            return binding;
        }
    }
}
