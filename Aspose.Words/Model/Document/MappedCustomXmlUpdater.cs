// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2022 by Alexey Morozov

using System;
using System.Collections.Generic;
using System.Xml;
using Aspose.Words.Markup;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words
{
    /// <summary>
    /// Handles document node change and updates mapped CustomXml document accordingly.
    /// </summary>
    /// <remarks>
    /// AM. This is just very basic implementation of runtime mapped XML update.
    /// We need to handle Run.Text change, formatting change and so on but lets wait till actual requests.
    /// </remarks>
    internal class MappedCustomXmlUpdater : INodeChangingCallback
    {
        internal MappedCustomXmlUpdater(Document doc)
        {
            mDoc = doc;
        }

        void INodeChangingCallback.NodeInserting(NodeChangingArgs args)
        {
            // Nothing to do.
        }

        void INodeChangingCallback.NodeRemoving(NodeChangingArgs args)
        {
            // Nothing to do.
        }

        void INodeChangingCallback.NodeInserted(NodeChangingArgs args)
        {
            if (!mDoc.IsMappedCustomXmlUpdateSuspended)
                UpdateMappedCustomXml(args.NewParent);
        }

        void INodeChangingCallback.NodeRemoved(NodeChangingArgs args)
        {
            if (!mDoc.IsMappedCustomXmlUpdateSuspended)
                UpdateMappedCustomXml(args.OldParent);
        }

        private void UpdateMappedCustomXml(Node parentNode)
        {
            // Look for nearest outer SDT.
            StructuredDocumentTag outerSdt = (parentNode.NodeType == NodeType.StructuredDocumentTag)
                ? (StructuredDocumentTag)parentNode
                : parentNode.GetAncestor(NodeType.StructuredDocumentTag) as StructuredDocumentTag;

            // Update found and all ancestor markup.
            while (outerSdt != null)
            {
                UpdateMappedCustomXmlCore(outerSdt);

                // Look for next outer SDT.
                outerSdt = outerSdt.GetAncestor(NodeType.StructuredDocumentTag) as StructuredDocumentTag;
            }
        }

        private void UpdateMappedCustomXmlCore(StructuredDocumentTag sdt)
        {
            // SDT is not mapped, nothing to update.
            if (!sdt.XmlMapping.IsMapped)
                return;

            IList<XmlNode> xmlNodes = sdt.XmlMapping.GetBoundXmlNodes(new XmlMappingContext());
            string mappedContent = CustomXmlUtil.GetOverallInnerText(xmlNodes);

            // Resiliently update mapped content.
            try
            {
                // Update content either Document or simple content.
                if (IsMappedAsDocument(sdt))
                {
                    using (new SuspendMappedCustomXmlUpdateDocument(mDoc))
                    {
                        OoxmlSaveOptions so = new OoxmlSaveOptions(SaveFormat.FlatOpc);
                        so.UpdateXmlMapping = false;
                        OpcDocumentFragmentWriter opcWriter = new OpcDocumentFragmentWriter(so);

                        string wordOpenXML = opcWriter.SaveToString(sdt);
                        sdt.XmlMapping.SetValue(wordOpenXML);
                    }
                }
                else
                {
                    string newContent = sdt.GetText();

                    // WORDSNET-26381 Remove the end-of-cell character if the SDT is at the cell level, otherwise
                    // MS Word will throw an error when opening the document.
                    if (StringUtil.HasChars(newContent) && (sdt.Level == MarkupLevel.Cell))
                    {
                        Debug.Assert(newContent.EndsWith(ControlChar.Cell, StringComparison.InvariantCulture));
                        newContent = newContent.Substring(0, newContent.Length - 1);

                        // Fix Java Error with "\x0007" whitespace
                        newContent = newContent.Replace(ControlChar.Cell, " ");
                    }

                    // Section end character is not allowed in mapped content.
                    newContent = newContent.TrimEnd('\f');

                    sdt.XmlMapping.SetValue(newContent);
                }
            }
            catch (Exception e)
            {
                Debug.Assert(false, e.Message);
            }
        }

        /// <summary>
        /// Check if XML data should be mapped as a document.
        /// </summary>
        private static bool IsMappedAsDocument(StructuredDocumentTag sdt)
        {
            // AM. Check for MarkupLevel.Cell to pass Test26381.
            // It seems some rework is needed here.
            return (sdt.SdtType != SdtType.PlainText) && (sdt.Level != MarkupLevel.Cell) &&
                   (sdt.SdtType != SdtType.Date);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly Document mDoc;
    }
}
