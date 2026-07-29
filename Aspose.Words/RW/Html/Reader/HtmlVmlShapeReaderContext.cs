// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2015 by Anton Savko

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.HtmlCommon;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Html.Reader
{
    internal class HtmlVmlShapeReaderContext: IVmlShapeReaderContext
    {
        internal HtmlVmlShapeReaderContext(
            DocumentBuilder builder,
            IDictionary<string, string> namespaces,
            IDictionary<string, ShapePr> shapeTemplates,
            HtmlResourceLoader resourceLoader,
            string shapeXml)
        {
            Debug.Assert(builder != null);
            Debug.Assert(namespaces != null);
            Debug.Assert(shapeTemplates != null);
            Debug.Assert(resourceLoader != null);

            mBuilder = builder;
            mShapeTemplates = shapeTemplates;
            mResourceLoader = resourceLoader;
            mXmlReader = new NrxXmlReader(shapeXml, namespaces);
        }

        DocumentBase IVmlShapeReaderContext.Document
        {
            get { return mBuilder.Document; }
        }

        NrxXmlReader IVmlShapeReaderContext.XmlReader
        {
            get { return mXmlReader; }
        }

        int IVmlShapeReaderContext.GroupNestingLevel { get; set; }

        IEmbeddedObject IVmlShapeReaderContext.GetEmbeddedObject(string id)
        {
            return null;
        }

        byte[] IVmlShapeReaderContext.GetBinData(string src)
        {
            return mResourceLoader.LoadImage(mBuilder.Document.BaseUri, src);
        }

        /// <summary>
        /// There are no related parts, just returns the value of the parameter.
        /// </summary>
        string IVmlShapeReaderContext.GetRelationshipTarget(string relId)
        {
            return relId;
        }

        bool IVmlShapeReaderContext.IsExternalImage(string src)
        {
            byte[] imageBytes = mResourceLoader.LoadImage(mBuilder.Document.BaseUri, src);
            return imageBytes == null;
        }

        void IVmlShapeReaderContext.AddToZOrderList(ShapeBase shape)
        {
            // Do nothing.
        }

        void IVmlShapeReaderContext.AddToShapeMap(string shapeId, ShapeBase shape)
        {
            // Do nothing.
        }

        ShapeBase IVmlShapeReaderContext.GetShapeById(string shapeId)
        {
            return null;
        }

        void IVmlShapeReaderContext.ReadBinData()
        {
            // Do nothing.
        }

        bool IVmlShapeReaderContext.ReadTextboxContent(ShapeBase shape)
        {
            Debug.Assert(shape != null);

            Paragraph para = new Paragraph(mBuilder.Document);
            shape.AppendChild(para);

            Paragraph currentParagraph = mBuilder.CurrentParagraph;
            mBuilder.MoveTo(para);

            StringBuilder outerHtml = new StringBuilder();
            // Only plain HTML is allowed as textbox content (no nested VML shapes), so we don't write namespace declarations
            // on the root HTML element.
            outerHtml.Append("<html>");
            while (!mXmlReader.IsEndElement("textbox") && mXmlReader.ReadChild("textbox"))
            {
                outerHtml.Append(mXmlReader.ReadOuterXml());
            }
            outerHtml.Append("</html>");

            HtmlReader htmlReader = new HtmlReader(new Loading.HtmlReaderSettings(), LoadFormat.Html, mResourceLoader);
            htmlReader.Read(outerHtml.ToString(), mBuilder, HtmlInsertOptions.None, true);

            mBuilder.MoveTo(currentParagraph);

            return false;
        }

        void IVmlShapeReaderContext.AddShapeType(string id, ShapePr shapePr)
        {
            if (StringUtil.HasChars(id))
                mShapeTemplates[id] = shapePr;
        }

        ShapePr IVmlShapeReaderContext.GetShapeTypePr(string id)
        {
            if (id.StartsWith("#", System.StringComparison.Ordinal))
                id = id.Substring(1);

            return (StringUtil.HasChars(id) && mShapeTemplates.ContainsKey(id))
                ? mShapeTemplates[id]
                : null;
        }

        void IVmlShapeReaderContext.MarkMissingSource(Shape shape, string src)
        {
            throw new NotImplementedException("This is intended for NrxDocumentReaderBase only so far.");
        }

        string IVmlShapeReaderContext.GetMissingSource(Shape shape)
        {
            throw new NotImplementedException("This is intended for NrxDocumentReaderBase only so far.");
        }

        private readonly DocumentBuilder mBuilder;
        private readonly IDictionary<string, ShapePr> mShapeTemplates;
        private readonly HtmlResourceLoader mResourceLoader;
        private readonly NrxXmlReader mXmlReader;
    }
}
