// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/06/2023 by Ilya Navrotskiy

using Aspose.Collections.Generic;
using Aspose.Words.Markup;

namespace Aspose.Words
{
    /// <summary>
    /// Class for collecting all necessary information about cloned nodes in <see cref="DocumentMerger"/>.
    /// </summary>
    internal class DocMergerInfoCollector : INodeCloningListener
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal DocMergerInfoCollector()
        {
            CorePropSdts = new HashSetGeneric<StructuredDocumentTag>();
        }

        /// <summary>
        /// This method is invoked when the listened node is cloned.
        /// </summary>
        void INodeCloningListener.NotifyNodeCloned(Node source, Node clone)
        {
            switch (clone.NodeType)
            {
                case NodeType.StructuredDocumentTag:
                {
                    StructuredDocumentTag sdt = (StructuredDocumentTag)clone;
                    if ((sdt.XmlMapping != null) && sdt.XmlMapping.IsCoreProperties)
                        CorePropSdts.Add(sdt);
                    break;
                }

                case NodeType.Footnote:
                {
                    HasFootnotes = true;
                    break;
                }

                case NodeType.FormField:
                {
                    HasFormFields = true;
                    break;
                }
            }
        }

        /// <summary>
        /// The collection of cloned SDTs that are map to document core properties.
        /// </summary>
        internal HashSetGeneric<StructuredDocumentTag> CorePropSdts { get; }

        /// <summary>
        /// Gets a boolean value indicating, if source document being merged, has footnotes.
        /// </summary>
        internal bool HasFootnotes { get; private set; }

        /// <summary>
        /// Gets a boolean value indicating, if source document being merged, has form fields.
        /// </summary>
        internal bool HasFormFields { get; private set; }
    }
}
