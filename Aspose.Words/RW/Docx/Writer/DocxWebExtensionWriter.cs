// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2019 by Dmitry Sokolov

using Aspose.OpcPackaging;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.WebExtensions;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Exports web extension to appropriate package part.
    /// </summary>
    internal class DocxWebExtensionWriter
    {
        /// <summary>
        /// This class is static. Instance creation is not allowed.
        /// </summary>
        private DocxWebExtensionWriter(DocxDocumentWriter writer)
        {
            mWriter = writer;
        }

        /// <summary>
        /// Writes web extensions document part.
        /// </summary>
        internal static string Write(DocxDocumentWriter writer, OpcPackagePart taskPanesPart, WebExtension extension)
        {
            Debug.Assert(extension != null);

            DocxWebExtensionWriter extensionWriter = new DocxWebExtensionWriter(writer);
            return extensionWriter.WriteCore(taskPanesPart, extension);
        }

        private string WriteCore(OpcPackagePart taskPanesPart, WebExtension extension)
        {
            string partName = string.Format(@"webextension{0}.xml", 
                mWriter.GetNextEmbeddedPartNumber(mWriter.RelTypes.WebExtension));

            string relId;
            DocxBuilder builder = mWriter.CreateChildPartAndBuilder(taskPanesPart, partName,
                DocxContentType.WebExtension, mWriter.RelTypes.WebExtension, out relId);
            mWriter.PushBuilder(builder);

            WriteWebExtensionCore(builder, extension);

            mWriter.PopBuilder();
            return relId;
        }

        /// <summary>
        /// Writes "2.2.7 CT_OsfWebExtension" element.
        /// </summary>
        private void WriteWebExtensionCore(DocxBuilder builder, WebExtension extension)
        {
            const string webExtensionElement = "we:webextension";
            builder.StartDocument(webExtensionElement);

            builder.WriteAttributeString("xmlns:we", "http://schemas.microsoft.com/office/webextensions/webextension/2010/11");
            builder.WriteAttributeString("id", extension.Id);
            builder.WriteAttributeIfTrue("frozen", extension.IsFrozen);

            WriteReference(builder, extension.Reference);
            WriteAlternateReferences(builder, extension.AlternateReferences);
            WriteProperties(builder, extension.Properties);
            WriteBindings(builder, extension.Bindings);

            const string snapshotName = "we:snapshot";
            DmlBlip shanShot = extension.Snapshot == null ? new DmlBlip() : extension.Snapshot;
            DmlFillWriter.WriteBlip(shanShot, mWriter, snapshotName);

            builder.EndDocument();
        }

        /// <summary>
        /// Writes "2.2.5 CT_OsfWebExtensionReference" element.
        /// </summary>
        private static void WriteReference(DocxBuilder builder, WebExtensionReference reference)
        {
            const string webExtensionReferenceElement = "we:reference";
            builder.StartElement(webExtensionReferenceElement);

            builder.WriteAttributeString("id", reference.Id);
            builder.WriteAttributeString("version", reference.Version);
            builder.WriteAttributeString("store", reference.Store);
            builder.WriteAttributeString("storeType", DocxEnum.WebExtensionStoreTypeToDocx(reference.StoreType));

            builder.EndElement(webExtensionReferenceElement);
        }

        /// <summary>
        /// Writes "2.2.6 CT_OsfWebExtensionReferenceList" element.
        /// </summary>
        private static void WriteAlternateReferences(DocxBuilder builder, WebExtensionReferenceCollection references)
        {
            const string webExtensionAlternateReferencesElement = "we:alternateReferences";
            if (CheckEmptyCollection(references, builder, webExtensionAlternateReferencesElement))
                return;

            builder.StartElement(webExtensionAlternateReferencesElement);

            foreach (WebExtensionReference reference in references)
                WriteReference(builder, reference);

            builder.EndElement(webExtensionAlternateReferencesElement);
        }

        /// <summary>
        /// Write "2.2.2 CT_OsfWebExtensionPropertyBag" element.
        /// </summary>
        private static void WriteProperties(DocxBuilder builder, WebExtensionPropertyCollection properties)
        {
            const string webExtensionPropertiesElement = "we:properties";
            if (CheckEmptyCollection(properties, builder, webExtensionPropertiesElement))
                return;

            builder.StartElement(webExtensionPropertiesElement);

            foreach (WebExtensionProperty property in properties)
                builder.WriteElementWithAttributes("we:property", "name", property.Name, "value", property.Value);

            builder.EndElement(webExtensionPropertiesElement);
        }

        /// <summary>
        /// Writes "2.2.4 CT_OsfWebExtensionBindingList" element.
        /// </summary>
        private static void WriteBindings(DocxBuilder builder, WebExtensionBindingCollection bindings)
        {
            const string webExtensionBindingsElement = "we:bindings";
            if (CheckEmptyCollection(bindings, builder, webExtensionBindingsElement))
                return;

            builder.StartElement(webExtensionBindingsElement);

            foreach (WebExtensionBinding binding in bindings)
            {
                builder.WriteElementWithAttributes("we:binding", "id", binding.Id, "type",
                    DocxEnum.WebExtensionBindingTypeToDocx(binding.BindingType), "appref", binding.AppRef);
            }

            builder.EndElement(webExtensionBindingsElement);
        }

        /// <summary>
        /// Returns true when specified collection is empty. And writes empty element at this case.
        /// </summary>
        private static bool CheckEmptyCollection<T>(BaseWebExtensionCollection<T> items, DocxBuilder builder, string elementName)
            where T: class
        {
            bool isEmpty = items.Count == 0;

            if (isEmpty)
                builder.WriteEmptyElement(elementName);

            return isEmpty;
        }

        /// <summary>
        /// Document writer.
        /// </summary>
        private readonly DocxDocumentWriter mWriter;
    }
}
