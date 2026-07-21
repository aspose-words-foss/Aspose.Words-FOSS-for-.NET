// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/11/2015 by Anton Savko

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.RW.HtmlCommon;
using Aspose.Words.RW.Vml;

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlVmlShapeReader
    {
        internal HtmlVmlShapeReader(DocumentBuilder builder, HtmlResourceLoader resourceLoader)
        {
            Debug.Assert(builder != null);

            mBuilder = builder;
            mResourceLoader = resourceLoader;
            mNamespaces = new Dictionary<string, string>();
            mShapeTemplates = new Dictionary<string, ShapePr>();
        }

        /// <summary>
        /// Processes 'html' element.
        /// </summary>
        internal void HandleHtml(HtmlElementNode node)
        {
            Debug.Assert(node.Name == "html");

            foreach (HtmlAttribute attribute in node.Attributes)
            {
                if ((attribute.Value == NrxNamespaces.Office) ||
                    (attribute.Value == NrxNamespaces.Vml) ||
                    (attribute.Value == NrxNamespaces.Word))
                {
                    const string xmlnsPrefix = "xmlns:";
                    if (attribute.Name.StartsWith(xmlnsPrefix, StringComparison.Ordinal))
                    {
                        string namespacePrefix = attribute.Name.Substring(xmlnsPrefix.Length);
                        if (StringUtil.HasChars(namespacePrefix) && (namespacePrefix.IndexOf(':') < 0))
                        {
                            mNamespaces.Add(namespacePrefix, attribute.Value);
                        }
                    }
                }
            }
        }

        internal HandleNodeAction HandleVml(HtmlElementNode node)
        {
            ShapeBase shape;
            try
            {                
                string serializedXml = node.SerializeToXml();
                HtmlVmlShapeReaderContext context = new HtmlVmlShapeReaderContext(
                    mBuilder,
                    mNamespaces,
                    mShapeTemplates,
                    mResourceLoader,
                    serializedXml);

                shape = VmlShapeReader.ReadShapeLevelElement(context);
            }
            catch
            {
                // WORDSNET-14901, WORDSNET-13555 Ignore invalid VML.
                if (mBuilder.Document.WarningCallback != null)
                {
                    mBuilder.Document.WarningCallback.Warning(new WarningInfo(WarningType.DataLoss, WarningSource.Html,
                        "Malformed VML shape is ignored"));
                }
                return HandleNodeAction.HandledSkipChildren;
            }

            if ((shape != null) && (shape.ParentNode == null))
                mBuilder.InsertNode(shape);

            return HandleNodeAction.HandledSkipChildren;
        }

        private readonly DocumentBuilder mBuilder;
        private readonly HtmlResourceLoader mResourceLoader;
        private readonly Dictionary<string, string> mNamespaces;
        private readonly Dictionary<string, ShapePr> mShapeTemplates;
    }
}
