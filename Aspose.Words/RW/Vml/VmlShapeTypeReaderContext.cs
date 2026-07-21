// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2008 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;


namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Used to parse VML of standard shape type definitions from an embedded resource.
    /// Most of the implemented methods do nothing because shape type definitions do not
    /// contain image data and so on.
    /// </summary>
    internal class VmlShapeTypeReaderContext : IVmlShapeReaderContext
    {
        private readonly NrxXmlReader mXmlReader;

        int IVmlShapeReaderContext.GroupNestingLevel { get; set; }

        internal VmlShapeTypeReaderContext(string shapeTypeXml)
        {
            mXmlReader = new NrxXmlReader(shapeTypeXml, gShapeTypeNamespaces);
        }

        DocumentBase IVmlShapeReaderContext.Document
        {
            get { return null; }
        }

        NrxXmlReader IVmlShapeReaderContext.XmlReader
        {
            get { return mXmlReader; }
        }

        IEmbeddedObject IVmlShapeReaderContext.GetEmbeddedObject(string id)
        {
            return null;
        }

        byte[] IVmlShapeReaderContext.GetBinData(string src)
        {
            return null;
        }

        string IVmlShapeReaderContext.GetRelationshipTarget(string relId)
        {
            return null;
        }

        bool IVmlShapeReaderContext.IsExternalImage(string src)
        {
            return false;
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
            return false;
        }

        void IVmlShapeReaderContext.AddShapeType(string id, ShapePr shapePr)
        {
            // Do nothing.
        }

        ShapePr IVmlShapeReaderContext.GetShapeTypePr(string id)
        {
            return null;
        }

        void IVmlShapeReaderContext.MarkMissingSource(Shape shape, string src)
        {
            throw new NotImplementedException("This is intended for NrxDocumentReaderBase only so far.");
        }

        string IVmlShapeReaderContext.GetMissingSource(Shape shape)
        {
            throw new NotImplementedException("This is intended for NrxDocumentReaderBase only so far.");
        }

        static VmlShapeTypeReaderContext()
        {
            gShapeTypeNamespaces = new Dictionary<string, string>();
            gShapeTypeNamespaces.Add("v", NrxNamespaces.Vml);
            gShapeTypeNamespaces.Add("o", NrxNamespaces.Office);
        }

        private static readonly Dictionary<string, string> gShapeTypeNamespaces;
    }
}
