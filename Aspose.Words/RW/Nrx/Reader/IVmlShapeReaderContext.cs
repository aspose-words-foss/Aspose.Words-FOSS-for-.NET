// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/08/2008 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Provides methods needed for VmlShapeReader to operate.
    /// Implemented by DOCX and WordML readers to share same VML reading code.
    /// </summary>
    internal interface IVmlShapeReaderContext
    {
        /// <summary>
        /// The document we are reading into.
        /// </summary>
        DocumentBase Document
        {
            get;
        }

        /// <summary>
        /// The XML reader we are reading from.
        /// </summary>
        NrxXmlReader XmlReader
        {
            get;
        }

        /// <summary>
        /// Specifies whether current nesting level of group shape content.
        /// Zero means we are not in group shape now.
        /// </summary>
        int GroupNestingLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The implementation should return an embedded object or null if not found.
        /// </summary>
        /// <param name="id">For WML this is OLE object id. For DOCX this is relationship id.</param>
        [JavaThrows(true)]
        IEmbeddedObject GetEmbeddedObject(string id);

        /// <summary>
        /// Gets image data.
        /// </summary>
        /// <param name="src">In DOCX this is relationship id. In WordML this is the value of the "src" attribute.</param>
        /// <returns>Image bytes or null if not found.</returns>
        [JavaThrows(true)]
        byte[] GetBinData(string src);

        /// <summary>
        /// In DOCX gets the relationship target for the specified relationship.
        /// Returns empty string if cannot fetch the relationship or if the parameter is null or empty string.
        ///
        /// In WML just returns the parameter value because there are no parts in WML.
        /// </summary>
        string GetRelationshipTarget(string relId);

        /// <summary>
        /// Returns true if the specified url is a link to an external image resource; false if to internal.
        /// </summary>
        /// <param name="src">In DOCX this is relationship id. In WordML this is the value of the "src" attribute.</param>
        [JavaThrows(true)]
        bool IsExternalImage(string src);

        /// <summary>
        /// The implementation should add the shape to the temporary "z-order list" so
        /// proper ZOrder can be calculated after all shapes were read.
        /// </summary>
        void AddToZOrderList(ShapeBase shape);

        /// <summary>
        /// The implementation should add the shape to the id-to-shape map so the shape
        /// can later be retrieved using <see cref="GetShapeById"/>.
        /// </summary>
        void AddToShapeMap(string shapeId, ShapeBase shape);

        /// <summary>
        /// The implementation should retrieve the shape read previously using the provided id.
        /// Used when reading OLE objects and ActiveX controls. Because reading is split into two parts?
        /// </summary>
        ShapeBase GetShapeById(string shapeId);

        /// <summary>
        /// WML implementation should read the WordML "binData" element.
        /// In WordML binary data can occur anywhere before it is referenced. The implementation of this
        /// interface needs to read the binary data and cache it until it is requested.
        ///
        /// DOCX implementation should do nothing because binary data does not occur inline in XML.
        /// </summary>
        [JavaThrows(true)]
        void ReadBinData();

        /// <summary>
        /// The implementation should read context of the textbox and add it to the specified shape.
        /// The XML reader will be positioned on the start of the textbox story.
        /// </summary>
        /// <returns>
        /// Value indicating whether XmlReader's position was already set
        /// to next child during processing of textbox.
        /// </returns>
        [JavaThrows(true)]
        bool ReadTextboxContent(ShapeBase shape);

        /// <summary>
        /// The implementation should add shape type to shape types collection.
        /// </summary>
        void AddShapeType(string id, ShapePr shapePr);

        /// <summary>
        /// The implementation should return shape type property collection.
        /// </summary>
        ShapePr GetShapeTypePr(string id);

        /// <summary>
        /// The implementation should marks the shape as a shape with missing source which is needed to be resolved later.
        /// </summary>
        void MarkMissingSource(Shape shape, string src);

        /// <summary>
        /// The implementation should return shape source if shape is marked as a shape with missing source.
        /// Otherwise the implementation should return null.
        /// </summary>
        string GetMissingSource(Shape shape);
    }
}
