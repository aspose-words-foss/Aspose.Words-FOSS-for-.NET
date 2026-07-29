// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/27/2014 by Alexey Noskov

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Type of the DrawingML node.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum DmlNodeType
    {
        /// <summary>
        /// 20.1.2.2.33 sp (Shape)
        /// This element specifies the existence of a single shape.
        /// </summary>
        Shape,

        /// <summary>
        /// 20.1.2.2.10 cxnSp (Connection Shape)
        /// This element specifies a connection shape that is used to connect two sp elements.
        /// </summary>
        ConnectorShape,

        /// <summary>
        /// 20.2.2.5 pic (Picture)
        /// This element specifies the existence of a picture object within the document.
        /// </summary>
        Picture,

        /// <summary>
        /// 20.1.2.2.20 grpSp (Group shape)
        /// This element specifies a group shape that represents many shapes grouped together
        /// </summary>
        GroupShape,

        /// <summary>
        /// Represents diagram drawing. actually is a group shape.
        /// </summary>
        SpTree,

        /// <summary>
        /// 20.3.2.1 lockedCanvas (Locked Canvas Container)
        /// The locked canvas element acts as a container for more advanced drawing objects.
        /// </summary>
        LockedCanvas,

        /// <summary>
        /// 21.2.2.29 chartSpace (Chart Space)
        /// This element specifies overall settings for a single chart, and is the root node for the chart part.
        /// </summary>
        Chart,

        /// <summary>
        /// Diagram allows the definition of diagrams using DrawingML objects and constructs.
        /// </summary>
        Diagram,

        /// <summary>
        /// ISO 29500 extension Wordprocessing shape.
        /// This type defines a shape in a WordprocessingML document.
        /// </summary>
        WordprocessingShape,

        /// <summary>
        /// ISO 29500 extension Wordprocessing group shape.
        /// This complex type defines the data that represents a group of graphical objects in WordprocessingML.
        /// </summary>
        WordprocessingGroupShape,

        /// <summary>
        /// ISO 29500 extension Wordprocessing canvas.
        /// This type defines a drawing canvas in a WordprocessingML document.
        /// </summary>
        WordprocessingCanvas,

        /// <summary>
        /// ISO 29500 contentPart.
        /// This element specifies a reference to XML content in a format not defined by ISO/IEC 29500.
        /// </summary>
        ContentPart,

        /// <summary>
        /// 20.4.2.31 graphicFrame (Graphical object container) 
        /// This element type specifies a container for a graphical object in WordprocessingML. 
        /// </summary>
        GraphicFrame,

        /// <summary>
        /// 2.24.1.2 chartSpace [MS-ODRAWXML]
        /// This element specifies the chart container object. It is introduced in MS Word 2016.
        /// </summary>
        ChartEx
    }
}
