// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/08/2008 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.Styles;
using Shape = Aspose.Words.Drawing.Shape;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Refactored reading of 'w:pict" for DOCX and WordML.
    /// </summary>
    internal class VmlShapeReader : VmlShapeReaderBase, INrxVmlReader
    {
        /// <summary>
        /// This is the main method to read a VML object.
        ///
        /// Reads a shape container element such as w:pict or w:object.
        /// Reader should be positioned to element start.
        /// </summary>
        public IList<ShapeBase> Read(IVmlShapeReaderContext context)
        {
            NrxXmlReader reader = context.XmlReader;

            List<ShapeBase> shapes = new List<ShapeBase>();

            int dxaOrigin = 0;
            int dyaOrigin = 0;
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "dxaOrig":
                        dxaOrigin = reader.ValueAsInt;
                        break;
                    case "dyaOrig":
                        dyaOrigin = reader.ValueAsInt;
                        break;
                    default:
                        // Ignore.
                        break;
                }
            }

            reader.MoveToElement();
            string tagName = reader.LocalName;
            while (reader.ReadChild(tagName))
            {
                ShapeBase curShape = ReadShapeLevelElement(context);
                // RK There are usually several shape level elements shapetype, shape, OLEObject,
                // some return null. For example, when o:OLEObject occurs after v:shape, we want
                // to keep the shape object, not null.
                // WORDSNET-11137 We should check, that current shape is not within an array already.
                // For example, this possible if current shape is OLE control.
                if ((curShape != null) && (!shapes.Contains(curShape)))
                    shapes.Add(curShape);
            }

            // WORDSNET-28422 Deferred shape source resolution.
            if((shapes.Count > 0) && (shapes[0] is Shape))
            {
                Shape shape = (Shape)shapes[0];
                string possibleMissingSrc = context.GetMissingSource(shape);

                if(possibleMissingSrc != null)
                    shape.ImageData.SetImageSafe(context.GetBinData(possibleMissingSrc));
            }

            // WORDSNET-15024 Update shape bounds with dxaOrigin and dyaOrigin.
            if (shapes.Count > 0)
            {
                const double allowedDelta = 0.1;
                ShapeBase shape = (ShapeBase)shapes[0];
                if (dxaOrigin > 0)
                {
                    // This calculation is based on <see cref="Picf" /> shape bounds calculation.
                    // Picf.mx is an integer value so we round value and convert it back.
                    double mx = System.Math.Round(shape.Width * 20 / dxaOrigin * 1000);
                    double newWidth = mx * dxaOrigin / 1000 / 20;
                    double widthDelta = System.Math.Abs(newWidth - shape.Width);
                    if (widthDelta > allowedDelta)
                        shape.SetWidthSafe(newWidth);
                }

                if (dyaOrigin > 0)
                {
                    double my = System.Math.Round(shape.Height * 20 / dyaOrigin * 1000);
                    double newHeight = my * dyaOrigin / 1000 / 20;
                    double heightDelta = System.Math.Abs(newHeight - shape.Height);
                    if (heightDelta > allowedDelta)
                        shape.SetHeightSafe(newHeight);
                }
            }

            return shapes;
        }

        /// <summary>
        /// Reads a shapeType element and returns parsed shape properties.
        /// </summary>
        internal static ShapePr ReadShapeType(IVmlShapeReaderContext context)
        {
            Debug.Assert(context.XmlReader.LocalName == "shapetype");
            Shape shape = new Shape(context.Document);
            ReadShape(context, shape);
            return shape.ShapePr;
        }

        /// <summary>
        /// Reads a WML 'w:bgPict' element from the specified reader. Reader should be positioned to element start.
        /// </summary>
        internal static void ReadWmlBackground(IVmlShapeReaderContext context)
        {
            Shape shape = new Shape(context.Document, ShapeType.Rectangle);
            context.Document.SetBackgroundShapeSafe(shape);

            NrxXmlReader reader = context.XmlReader;
            while (reader.ReadChild("bgPict"))
            {
                switch (reader.LocalName)
                {
                    case "binData": // w:binData
                        context.ReadBinData();
                        break;
                    case "background": // w:background
                        if (reader.Prefix == "w")
                            ReadWmlBackgroundCore(context, shape);
                        else if (reader.Prefix == "v")
                            ReadShape(context, shape);
                        break;
                    default:
                        reader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 'w:background' element from the specified reader. Reader should be positioned to element start.
        /// </summary>
        private static void ReadWmlBackgroundCore(IVmlShapeReaderContext context, ShapeBase shape)
        {
            NrxXmlReader reader = context.XmlReader;
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;
                switch (reader.LocalName)
                {
                    case "bgcolor": // w:bgcolor
                        if (value != "white")
                            shape.SetShapeAttrInternal(ShapeAttr.FillColor, VmlColor.VmlToColor(value));
                        break;
                    case "background": // w:background
                        shape.SetShapeAttrInternal(ShapeAttr.FillImageBytes, context.GetBinData(value));
                        break;
                    default:
                        // Ignore other attributes.
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a shape level element from the specified reader.
        /// Shape level elements are "top level" elements of a shape such as:
        /// shape, group, rect, line, oval etc and shapetype.
        ///
        /// Reader should be positioned to element start.
        /// </summary>
        internal static ShapeBase ReadShapeLevelElement(IVmlShapeReaderContext context)
        {
            ShapeBase shape = null;

            NrxXmlReader reader = context.XmlReader;
            switch (reader.LocalName)
            {
                case "shapetype":
                {
                    // [ECMA-376] M.5.4 ShapeType Element.
                    // The ShapeType element defines a definition, or template, for a shape.
                    // Such a template is 'instantiated' by creating a Shape element that references the ShapeType.
                    // The shape can override any value specified by its ShapeType, or define attributes and elements the ShapeType does not provide.
                    //
                    // AM. Spec says that there are can be several shapes referred to one shape template so we need to map them.
                    // Currently these shape types used only to obtain valid "o:spt" value.
                    string id = reader.ReadAttribute("id", null);
                    ShapePr shapePr = ReadShapeType(context);
                    ValidateShapeType(shapePr);
                    context.AddShapeType(id, shapePr);
                    break;
                }
                case "binData":
                    // WordML only.
                    context.ReadBinData();
                    break;
                case "group":
                    shape = new GroupShape(context.Document);
                    ReadShape(context, shape);
                    break;
                case "shape":
                case "polyline":
                // [ECMA-376] 6.1.2.10 image (Image File) element.
                case "image":
                    shape = new Shape(context.Document);
                    ReadShape(context, shape);
                    break;
                case "rect":
                    shape = new Shape(context.Document, ShapeType.Rectangle);
                    ReadShape(context, shape);
                    break;
                case "roundrect":
                    shape = new Shape(context.Document, ShapeType.RoundRectangle);
                    ReadShape(context, shape);
                    break;
                case "line":
                    shape = new Shape(context.Document, ShapeType.Line);
                    ReadShape(context, shape);
                    break;
                case "oval":
                    shape = new Shape(context.Document, ShapeType.Ellipse);
                    ReadShape(context, shape);
                    break;
                case "scriptAnchor":
                    // This is not defined in DOCX, but we still read it. Maybe they will fix the spec.
                    shape = ReadScriptAnchor(context);
                    break;
                case "arc":
                case "curve":
                    // These exist in schema but never seen in WordML. Ignore for now.
                    reader.IgnoreElement();
                    break;
                case "ocx": //w:ocx
                    // WordML only?
                    // This appears after v:shape and contains additional OLE-related data for the shape.
                    shape = VmlOleReader.ReadWmlControl(context);
                    break;
                case "control":
                    shape = VmlOleReader.ReadDocxControl(context);
                    break;
                case "OLEObject":
                    // This appears after v:shape and contains additional OLE-related data for the shape.
                    VmlOleReader.ReadOleObject(context);
                    break;
                default:
                    reader.IgnoreElement();
                    break;
            }

            // WORDSNET-22742 Deferred binding of the fill sources.
            // In case some bindings were unavailable at the time the markup was parsed.
            ResolveFillSources(context);

            return shape;
        }

        /// <summary>
        /// Checks that specified shape properties does not match to shape type and updates
        /// shape properties to state which means that shape type can not be written. Also
        /// in this case properties of the type has to be merged with shape properties
        /// (see fix for WORDSNET-13556).
        /// </summary>
        private static void ValidateShapeType(ShapePr shapePr)
        {
            Debug.Assert(shapePr != null);

            object shapeType = shapePr[ShapeAttr.ShapeType];
            ShapePr expectedShapePr = null;

            if (shapeType != null)
                expectedShapePr = ShapeTypeLibrary.GetShapeTypePr((ShapeType)shapeType);

            if (expectedShapePr == null)
                return;

            PathPoint[] expectedVertices = (PathPoint[])expectedShapePr.FetchAttr(ShapeAttr.GeometryVertices);
            PathPoint[] curVertices = (PathPoint[])shapePr.FetchAttr(ShapeAttr.GeometryVertices);

            Handle[] expectedHandles = (Handle[])expectedShapePr.FetchAttr(ShapeAttr.GeometryHandles);
            Handle[] curHandles = (Handle[])shapePr.FetchAttr(ShapeAttr.GeometryHandles);

            // WORDSNET-15308 Remove shape type to avoid loading it from predefined types file.
            // It is necessary when properties of the detected shape type does not match to real
            // properties of the shape type specified in the input stream. Fluent comparison currently is
            // implemented (just compare vertices count). Removed "ShapeAttr.ShapeType" attribute for type of
            // the shape demonstrates that shape type from template can not be found and properties of the type
            // will be merged with shape properties. Also, if type for shape will not set explicitly,
            // such shape will has default "NonPrimitive" type.
            // WORDSNET-26377 see Test26377 for more cases.
            if (((expectedVertices == null) && (curVertices != null)) ||
                ((expectedHandles != null) && (curHandles == null)))
                shapePr.Remove(ShapeAttr.ShapeType);
            if ((expectedVertices != null) && (curVertices != null))
                if ((expectedVertices.Length != curVertices.Length))
                    shapePr.Remove(ShapeAttr.ShapeType);
        }

        /// <summary>
        /// Reads script anchor. Reader should be positioned to 'w:scriptAnchor' element start.
        /// RK Note scriptAnchor is not defined in DOCX but we still read it. Mainly to allow
        /// our gold ExportImport tests to pass.
        /// </summary>
        private static Shape ReadScriptAnchor(IVmlShapeReaderContext context)
        {
            Shape shape = new Shape(context.Document);
            shape.SetShapeType(ShapeType.Image);

            // Set attributes that should be present by default.
            shape.SetShapeAttrInternal(ShapeAttr.Print, false);
            shape.SetShapeAttrInternal(ShapeAttr.LineOn, false);
            shape.SetShapeAttrInternal(ShapeAttr.WrapType, WrapType.Inline);
            shape.SetShapeAttrInternal(ShapeAttr.LockAgainstGrouping, true);
            shape.SetShapeAttrInternal(ShapeAttr.Flip, FlipOrientation.None);
            shape.SetShapeAttrInternal(ShapeAttr.ImageTitle, "Normal");
            shape.SetShapeAttrInternal(ShapeAttr.Hidden, true);
            shape.SetShapeAttrInternal(ShapeAttr.ReallyHidden, false);

            shape.SetShapeAttrInternal(ShapeAttr.ScriptAnchor, true);

            NrxXmlReader reader = context.XmlReader;
            while (reader.ReadChild("scriptAnchor"))
            {
                string value = reader.ReadString();

                switch (reader.LocalName)
                {
                    case "language": //w:language
                    {
                        shape.SetShapeAttrInternal(ShapeAttr.ScriptLanguageName, value);
                        ScriptLanguage scriptLanguage = VmlEnum.VmlToScriptLanguage(value);

                        if (scriptLanguage != ScriptLanguage.None)
                            shape.SetShapeAttrInternal(ShapeAttr.ScriptLanguage, scriptLanguage);

                        break;
                    }
                    case "args": //w:args
                        shape.SetShapeAttrInternal(ShapeAttr.ScriptType, value);
                        break;
                    case "scriptText": //w:scriptText
                        shape.SetShapeAttrInternal(ShapeAttr.ScriptText, value);
                        break;
                    default:
                        reader.IgnoreElement();
                        break;
                }
            }

            return shape;
        }

        /// <summary>
        /// Reads a shape element, such as group, line, rect, shape etc from the specified reader.
        /// The reader should be positioned to element start.
        /// </summary>
        internal static void ReadShape(IVmlShapeReaderContext context, ShapeBase shape)
        {
            NrxXmlReader reader = context.XmlReader;

            string shapeElementName = reader.LocalName;

            string id = null;
            string spid = null;
            string spt = null;
            string type = null;

            // RESILIENCY 17731 - The problem occurred because in the document 'to' attribute was specified before 'from'.
            // That is why width and height of the line shape was calculated improperly.
            // To make code resilient, first read both 'to' and 'from' and then set shape's attributes.
            VmlQuantity[] from = null;
            VmlQuantity[] to = null;

            // Read shape attributes.
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "id":
                        id = value;
                        break;
                    case "spid": // o:spid
                        spid = value;
                        break;
                    case "spt":
                        spt = value;
                        break;
                    case "type":
                        type = value;
                        break;
                    case "editas":
                        shape.SetShapeAttrInternal(ShapeAttr.EditAs, VmlEnum.VmlToEditAs(value));
                        break;
                    case "alt":
                        shape.SetShapeAttrInternal(ShapeAttr.ShapeDescription, value);
                        break;
                    case "style":
                        ReadShapeStyle(shape, value, context, reader);
                        break;
                    case "ole": // o:ole
                        // This attribute is set in WordML for inline OLE controls and objects.
                        // It does not seem to be of any use during import. Hence, ignored.
                        break;
                    case "bordertopcolor": // o:bordertopcolor
                        shape.SetShapeAttrInternal(ShapeAttr.BorderTop, CreateBorderWithColor(value));
                        break;
                    case "borderleftcolor": // o:borderleftcolor
                        shape.SetShapeAttrInternal(ShapeAttr.BorderLeft, CreateBorderWithColor(value));
                        break;
                    case "borderbottomcolor": // o:borderbottomcolor
                        shape.SetShapeAttrInternal(ShapeAttr.BorderBottom, CreateBorderWithColor(value));
                        break;
                    case "borderrightcolor": // o:borderrightcolor
                        shape.SetShapeAttrInternal(ShapeAttr.BorderRight, CreateBorderWithColor(value));
                        break;
                    case "allowincell": // o:allowincell
                        SetBoolAttribute(shape, ShapeAttr.AllowInCell, value);
                        break;
                    case "allowoverlap": // o:allowoverlap
                        SetBoolAttribute(shape, ShapeAttr.AllowOverlap, value);
                        break;
                   case "coordorigin":
                        if (StringUtil.HasChars(value))
                        {
                            int[] values = VmlUtil.VmlToIntArray(value);
                            int x = (values.Length > 0) ? values[0] : 0;
                            int y = (values.Length > 1) ? values[1] : 0;
                            shape.CoordOrigin = new Point(x, y);
                        }
                        break;
                    case "coordsize":
                    {
                        Size coordSize;
                        if (StringUtil.HasChars(value))
                        {
                            int[] values = VmlUtil.VmlToIntArray(value);
                            int width = (values.Length > 0) ? values[0] : 0;
                            int height = (values.Length > 1) ? values[1] : 0;
                            coordSize = new Size(width, height);
                        }
                        else
                        {
                            // WORDSNET-4014 Index was outside the bounds of the array.
                            // RK coordsize is an empty string in this file. MS Word seems to default to 1000.
                            coordSize = new Size(1000, 1000);
                        }
                        shape.SetCoordSizeSafe(coordSize);
                        break;
                    }
                    case "adj":
                        SetAdjustments(shape, value);
                        break;
                    case "path":
                        SetPath(shape, value);
                        break;
                    case "points":
                        SetPoints(shape, value);
                        break;
                    case "preferrelative": // o:preferrelative
                        SetBoolAttribute(shape, ShapeAttr.PreferRelativeResize, value);
                        break;
                    case "fill":
                    case "filled":
                        SetBoolAttribute(shape, ShapeAttr.Filled, value);
                        break;
                    case "fillcolor":
                        shape.SetShapeAttrInternal(ShapeAttr.FillColor, VmlColor.VmlToColor(value));

                        DrColor baseColor = VmlColor.GetBaseColor(value);
                        if (baseColor != null)
                            shape.SetShapeAttrInternal(ShapeAttr.FillColorExt, baseColor);
                        int colorModifier = VmlColor.GetColorModifier(value);
                        if (colorModifier != 0)
                            shape.SetShapeAttrInternal(ShapeAttr.FillColorExtMod, colorModifier);
                        break;
                    case "dgmlayout": // o:dgmlayout
                        shape.SetShapeAttrInternal(ShapeAttr.DiagramNodeLayout, (DiagramNodeLayout)reader.ValueAsInt);
                        break;
                    case "dgmnodekind": // o:dgmnodekind
                        shape.SetShapeAttrInternal(ShapeAttr.DiagramNodeKind, (DiagramNodeKind)reader.ValueAsInt);
                        break;
                    case "connectortype": // o:connectortype
                        shape.SetShapeAttrInternal(ShapeAttr.ConnectorType, VmlEnum.VmlToConnectorType(value));
                        break;
                    case "stroked":
                        SetBoolAttribute(shape, ShapeAttr.LineOn, value);
                        break;
                    case "strokecolor":
                        shape.SetShapeAttrInternal(ShapeAttr.LineColor, VmlColor.VmlToColor(value));

                        DrColor baseStrokeColor = VmlColor.GetBaseColor(value);
                        if (baseStrokeColor != null)
                            shape.SetShapeAttrInternal(ShapeAttr.LineColorExt, baseStrokeColor);
                        int colorStrokeModifier = VmlColor.GetColorModifier(value);
                        if (colorStrokeModifier != 0)
                            shape.SetShapeAttrInternal(ShapeAttr.LineColorExtMod, colorStrokeModifier);
                        break;
                    case "strokeweight":
                        SetStrokeWeight(shape, value);
                        break;
                    case "insetpen":
                        SetBoolAttribute(shape, ShapeAttr.LineInsetPen, value);
                        break;
                    case "href":
                        shape.SetShapeAttrInternal(ShapeAttr.HyperlinkAddress, value);
                        break;
                    case "target":
                        shape.SetShapeAttrInternal(ShapeAttr.HyperlinkTarget, value);
                        break;
                    case "title":
                        shape.SetShapeAttrInternal(ShapeAttr.ScreenTip, value);
                        break;
                    case "bullet":
                        // WordML only?
                        SetBoolAttribute(shape, ShapeAttr.PictureBullet, value);
                        break;
                    case "button": // o:button
                        SetBoolAttribute(shape, ShapeAttr.Button, value);
                        break;
                    case "oleicon": // o:oleicon
                        SetBoolAttribute(shape, ShapeAttr.OleIcon, value);
                        break;
                    case "bwmode": // o:bwmode
                        shape.SetShapeAttrInternal(ShapeAttr.BWMode, VmlEnum.VmlToBWMode(value));
                        break;
                    case "bwpure": // o:bwpure
                        shape.SetShapeAttrInternal(ShapeAttr.BWPure, VmlEnum.VmlToBWMode(value));
                        break;
                    case "bwnormal": // o:bwnormal
                        shape.SetShapeAttrInternal(ShapeAttr.BWNormal, VmlEnum.VmlToBWMode(value));
                        break;
                    case "hr": // o:hr
                        SetBoolAttribute(shape, ShapeAttr.HROn, value);
                        break;
                    case "hrstd": // o:hrstd
                        SetBoolAttribute(shape, ShapeAttr.HRStandard, value);
                        break;
                    case "hrnoshade": // o:hrnoshade
                        SetBoolAttribute(shape, ShapeAttr.HRNoShade, value);
                        break;
                    case "hrpct": // o:hrpct
                        if (StringUtil.HasChars(value))
                            shape.SetShapeAttrInternal(ShapeAttr.HRPct, (int)FormatterPal.ParseDouble(value));
                        break;
                    case "hralign": // o:hralign
                        shape.SetShapeAttrInternal(ShapeAttr.HRAlign, VmlEnum.VmlToHRAlign(value));
                        break;
                    case "from":
                        if (shape.ShapeType == ShapeType.Line)
                            from = VmlToQuantityArray(value, GetDefaultUnits(context));
                        break;
                    case "to":
                        if (shape.ShapeType == ShapeType.Line)
                            to = VmlToQuantityArray(value, GetDefaultUnits(context));
                        break;
                    case "arcsize":
                        ArcSizeToAdj1(shape, value);
                        break;
                    case "wrapcoords":
                        {
                            PathPoint[] wrapPoints = ReadWrapPolygon(reader.Value);
                            if (wrapPoints != null)
                                shape.SetShapeAttrInternal(ShapeAttr.WrapPolygonVertices, wrapPoints);
                            break;
                        }
                    case "equationxml":
                        shape.SetShapeAttrInternal(ShapeAttr.EquationXML, Encoding.UTF8.GetBytes(value));
                        break;
                    case "gfxdata":
                        ReadGfxData(shape, value, reader);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }

            // AN: Set from and to of line shape here to fix the issue WORDSNET-17731.
            // In the document 'to' is specified before 'from', that is why width and height were calculated improperly.
            // WORDSNET-14928 Preserve vertices of the line to avoid inversion of the slope.
            // This code placed before detection of the shape type to avoid processing of the "line" type (o:spt="20").
            FillLinePosAndSize(from, to, shape, context.GroupNestingLevel > 0);

            // Model ShapeType attribute corresponds to 'o:spt' element.
            if (StringUtil.HasChars(spt))
            {
                // Local spt is defined so set it as ShapeType.
                SetShapeType(shape, spt);
            }
            else if (StringUtil.HasChars(type))
            {
                // Local 'spt' is not defined but 'type' is defined instead. Try to find corresponding local shape type.
                ShapePr shapeTypePr = context.GetShapeTypePr(type);
                if((shapeTypePr != null) && shapeTypePr.Contains(ShapeAttr.ShapeType))
                {
                    // Corresponding shape type found, obtain 'spt' value from it.
                    shape.SetShapeAttrInternal(ShapeAttr.ShapeType, shapeTypePr[ShapeAttr.ShapeType]);
                }
                else
                {
                    // Local shape type is not found. In facts we should get 'spt' value from VmlShapeLibrary which
                    // defines global shape types but result will be the same (each global shape type string identifier '_x0000_tXX'
                    // strictly corresponds to XX ShapeType value) so leave it without changes.

                    // WORDSNET-13556 Get shape properties form template when spt is missing.
                    if (shapeTypePr != null)
                    {

                        // AM. Actually we always should do this way.
                        // Currently we relies on spt property but it is not fully correct and works only
                        // with MS Word produced files. According to VML standard shape is defined as
                        // shape template properties + shape itself properties and we can even get
                        // gain in performance since we don't need to load shape template from VmlShapeLibrary.
                        // But this leads us to global VML shape rework as we need to store
                        // both shape template property collection and shape itself in Shape class.
                        //
                        // So this fix is local for a while and handle the case when spt property is missing.
                        ShapePr shapePr = new ShapePr();
                        shapeTypePr.ExpandTo(shapePr);
                        shape.ShapePr.ExpandTo(shapePr);
                        shape.ShapePr = shapePr;
                    }

                    SetShapeType(shape, type);
                    // WORDSNET-26377 ShapeType in the direct attribute shall also be validated.
                    if (shapeTypePr != null)
                        ValidateShapeType(shape.ShapePr);
                }
            }

            // Set shape id and name.
            if (StringUtil.HasChars(spid))
            {
                // RK VML shape has spid. In this case spid is "_x000_s1112" and it becomes shape id in the model.
                SetIdentifierAttrSafe(shape, ShapeAttr.ShapeId, spid);
                // If id is present, it becomes the shape name.
                if (StringUtil.HasChars(id))
                    shape.Name = id;
            }
            else if (StringUtil.HasChars(id))
            {
                // RK VML spid is missing. In this case id is usually "_x000_s1112" and becomes shape id in the model.
                //
                // When reading a shape type, we will have "id    = _x0000_t57" and this code will execute
                // and it will set shape id equal to the shape type. I guess this is alright for us.
                SetIdentifierAttrSafe(shape, ShapeAttr.ShapeId, id);
            }

            // It is important to add the shape to the map using the original "id" value read from VML because
            // we cannot parse it into a model shape id in documents created by some 3rd party generators.
            if (StringUtil.HasChars(id))
                context.AddToShapeMap(id, shape);

            // WORDSNET-24467 Fix Shape type for GroupShape.
            if((shape.NodeType == NodeType.GroupShape) && (shape.ShapeType == ShapeType.Image))
                shape.SetShapeType(ShapeType.Group);

            // Read shape child elements.
            bool isContinueReading = true;
            bool isNextChildAlreadyRead = false;
            while (true)
            {
                if (!isNextChildAlreadyRead)
                    isContinueReading = reader.ReadChild(shapeElementName);

                if (!isContinueReading)
                    break;

                isNextChildAlreadyRead = false;
                switch (reader.LocalName)
                {
                    case "ink":
                        ReadInk(shape, context);
                        break;
                    case "binData":
                        // WordML only?
                        context.ReadBinData();
                        break;
                    case "fill": // v:fill
                        VmlFillReader.ReadFill(shape, context);
                        break;
                    case "stroke": // v:stroke
                        ReadStroke(shape, context);
                        break;
                    case "imagedata": // v:imagedata
                    case "imageData":
                        if (!shape.IsGroup)
                        {
                            Shape shapeInst = (Shape)shape;
                            ReadImageData(shapeInst, context);

                            // andrnosk: WORDSNET-5198 Image disappears after rendering the document.
                            // If Shape has image it means that ShapeType cannot be NonPrimitive.
                            // So we need to set o:spt=100 it is equivalent of ShapeType.CustomShape.
                            // We mimic MS Word behavior here.
                            if (shapeInst.HasImage && (shape.ShapeType == ShapeType.NonPrimitive))
                                shape.SetShapeAttrInternal(ShapeAttr.ShapeType, ShapeType.CustomShape);
                        }
                        break;
                    case "signatureline": // o:signatureline
                        ReadSignatureLine(shape, reader);
                        break;
                    case "shadow": // v:shadow
                        VmlShadowReader.ReadShadow(shape, reader);
                        break;
                    case "extrusion": // o:extrusion
                        VmlExtrusionReader.ReadExtrusion(shape, reader);
                        break;
                    case "formulas": // v:formulas
                        ReadFormulas(shape, reader);
                        break;
                    case "path": // v:path
                        ReadPath(shape, reader);
                        break;
                    case "textpath": // v:textpath
                        ReadTextPath(shape, reader);
                        break;
                    case "handles": // v:handles
                        ReadHandles(shape, reader);
                        break;
                    case "lock": // o:lock
                        ReadLock(shape, reader);
                        break;
                    case "textbox": // v:textbox
                        isNextChildAlreadyRead = ReadTextbox(context, shape);
                        break;
                    case "diagram": // o:diagram
                        VmlDiagramReader.ReadDiagram(shape, reader);
                        break;
                    case "callout": // o:callout
                        ReadCallout(shape, reader);
                        break;
                    case "wrap": // w10:wrap
                        ReadWrap(shape, reader);
                        break;
                    case "anchorlock": // w10:anchorlock
                        shape.SetShapeAttrInternal(ShapeAttr.AnchorLocked, true);
                        break;
                    case "bordertop": // w10:bordertop
                        ReadBorder(shape, ShapeAttr.BorderTop, reader);
                        break;
                    case "borderleft": // w10:borderleft
                        ReadBorder(shape, ShapeAttr.BorderLeft, reader);
                        break;
                    case "borderbottom": // w10:borderbottom
                        ReadBorder(shape, ShapeAttr.BorderBottom, reader);
                        break;
                    case "borderright": // w10:borderright
                        ReadBorder(shape, ShapeAttr.BorderRight, reader);
                        break;
                    default:
                        if (shape.ShapeType == ShapeType.Group)
                        {
                            // WORDSNET-26597 Limit maximum allowed group nesting level.
                            if (context.GroupNestingLevel < MaxGroupNestingLevel)
                            {
                                // Flag is used to determine whether shape is inside group shape,
                                // to decide whether is required to use default units upon reading.
                                context.GroupNestingLevel++;

                                // Read child shapes of a group shape.
                                ShapeBase shapeChild = ReadShapeLevelElement(context);
                                if (shapeChild != null)
                                    shape.AppendChildForLoad(shapeChild);

                                context.GroupNestingLevel--;
                            }
                        }
                        else
                        {
                            reader.IgnoreElement();
                        }
                        break;
                }
            }

            // andrnosk: WORDSNET-4576 Sometimes shape has zero size, in this case it might become invisible during export,
            // so need to update size and position of shape.
            if (shape.RequiresBoundsRecalculation())
                shape.UpdateSizeAndPosition();

            // WORDSNET-22742 Deferred binding of the fill sources.
            // In case some bindings were unavailable at the time the markup was parsed.
            ResolveFillSources(context);
        }

        private static string GetDefaultUnits(IVmlShapeReaderContext context)
        {
            return context.GroupNestingLevel > 0 ? VmlQuantity.PointUnits : VmlQuantity.PixelUnits;
        }

        /// <summary>
        /// Read base64 gfxdata.
        /// </summary>
        /// <remarks>Sometimes value can be corrupted.</remarks>
        private static void ReadGfxData(ShapeBase shape, string value, NrxXmlReader reader)
        {
            try
            {
                shape.SetShapeAttrInternal(ShapeAttr.GfxData, Convert.FromBase64String(value));
            }
            catch (FormatException)
            {
                reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, WarningStrings.CorruptedGfxData);
            }
        }

        private static void SetStrokeWeight(ShapeBase shape, string value)
        {
            // WORDSNET-10198 The problem occurred because line weight was read improperly.
            // According to the specification if units are not specified value is in Emus.
            // VmlQuantity supposes that value without units are in point. Fixed by checking units.
            VmlQuantity quantity = new VmlQuantity(value);
            // DistanceToPoints returns source number if there are no units.
            if (quantity.IsWithoutUnits)
                shape.SetShapeAttrInternal(ShapeAttr.LineWidth, (int)quantity.DistanceToPoints());
            else
                SetEmusAttribute(shape, ShapeAttr.LineWidth, quantity);
        }

        /// <summary>
        /// Populates size, position and coordinates of the line according to specified parameters.
        /// </summary>
        /// <param name="from">Coordinates of the first point of the line.</param>
        /// <param name="to">Coordinates of the second point of the line.</param>
        /// <param name="shape">Current shape which is reading.</param>
        /// <param name="isInGroup">True - shape is nested to group.</param>
        private static void FillLinePosAndSize(VmlQuantity[] from, VmlQuantity[] to,
            ShapeBase shape, bool isInGroup)
        {
            Debug.Assert(shape != null);

            if (shape.ShapeType != ShapeType.Line)
                return;

            PathPoint[] vertices = (PathPoint[])shape.GetDirectShapeAttrInternal(ShapeAttr.GeometryVertices);

            // Expected two vertices for the line.
            if ((vertices == null) || (vertices.Length < 2))
                vertices = new PathPoint[2];

            // Populate vertices points with values according to "from" and "to" attributes values.
            PathPoint fromVal = ProcessLinePoint(from, true, isInGroup, shape);
            PathPoint toVal = ProcessLinePoint(to, false, isInGroup, shape);

            // Calculate and set line size and position.
            int top = (fromVal.Y.Value < toVal.Y.Value) ? fromVal.Y.Value : toVal.Y.Value;
            int left = (fromVal.X.Value < toVal.X.Value) ? fromVal.X.Value : toVal.X.Value;

            shape.Left = isInGroup ? left : ConvertUtilCore.TwipToPoint(left);
            shape.Top = isInGroup ? top : ConvertUtilCore.TwipToPoint(top);

            // WORDSNET-4811 Safe set of shape of props.
            int deltaW = toVal.X.Value - fromVal.X.Value;
            shape.SetWidthSafe(System.Math.Abs(isInGroup ? deltaW : ConvertUtilCore.TwipToPoint(deltaW)));

            int deltaH = toVal.Y.Value - fromVal.Y.Value;
            shape.SetHeightSafe(System.Math.Abs(isInGroup ? deltaH : ConvertUtilCore.TwipToPoint(deltaH)));

            // Actually we do not need to store coordinates exactly to save the document. It is necessary
            // to know start corner and end corner of the rectangle. However, DOC format requires to populate
            // geometry within the local coordinates.
            const int defaultCoordSize = 21600;
            vertices[0] = new PathPoint((deltaW < 0) ? defaultCoordSize : 0,
                (deltaH < 0) ? defaultCoordSize : 0);
            vertices[1] = new PathPoint((deltaW >= 0) ? defaultCoordSize : 0,
                (deltaH >= 0) ? defaultCoordSize : 0);

            // Save vertices for cases when coordinates on existing model parameters Left, Top and
            // Left + width, Top + height can not describe actual geometry of the line.
            if ((deltaW < 0) || (deltaH < 0))
                shape.SetShapeAttrInternal(ShapeAttr.GeometryVertices, vertices);
        }

        /// <summary>
        /// Creates value of the line vertices coordinate.
        /// </summary>
        /// <param name="coordinates">Obtained coordinates values from VML.</param>
        /// <param name="isStartPoint">True - first point of the line is in processing.</param>
        /// <param name="isInGroup">True - shape is nested to group.</param>
        /// <param name="shape">Current shape which is reading.</param>
        private static PathPoint ProcessLinePoint(VmlQuantity[] coordinates, bool isStartPoint, bool isInGroup, ShapeBase shape)
        {
            // According to ooxml spec default value for coordinates of the end point of the line are 10
            // points, however looks like MSW uses 50 points as default value. Also when line is nested to
            // group and last vertice of the line is missed, then MSW put 1000 for each coordinates of the
            // last point.
            // Vertices store in coordinates which represented with "PathValue" type and these
            // values should be in "Twips".
            int defaultCoord = isInGroup ? 1000 : ConvertUtilCore.PointToTwip(50);

            // MSW uses 0 as default for first vertice.
            defaultCoord = isStartPoint ? 0 : defaultCoord;

            int x = GetX(isStartPoint, shape, defaultCoord, isInGroup);
            int y = GetY(isStartPoint, shape, defaultCoord, isInGroup);

            PathValue coordX = GetVerticeCoordinate(coordinates, x, 0, isInGroup);
            PathValue coordY = GetVerticeCoordinate(coordinates, y, 1, isInGroup);

            return new PathPoint(coordX, coordY);
        }

        /// <summary>
        /// Gets x-coordinate with taking left and width of the shape into consideration.
        /// </summary>
        private static int GetX(bool isStartPoint, ShapeBase shape, int defaultCoord, bool isInGroup)
        {
            ShapePr pr = shape.ShapePr;

            if (isStartPoint)
            {
                return pr.ContainsKey(ShapeAttr.Left) ? GetCoordinate(shape.Left, isInGroup) : defaultCoord;
            }
            else
            {
                return (pr.ContainsKey(ShapeAttr.Left) && pr.ContainsKey(ShapeAttr.Width))
                    ? GetCoordinate(shape.Left + shape.Width, isInGroup)
                    : defaultCoord;
            }
        }

        /// <summary>
        /// Gets y-coordinate with taking top and height of the shape into consideration.
        /// </summary>
        private static int GetY(bool isStartPoint, ShapeBase shape, int defaultCoord, bool isInGroup)
        {
            ShapePr pr = shape.ShapePr;

            if (isStartPoint)
            {
                return pr.ContainsKey(ShapeAttr.Top) ? GetCoordinate(shape.Top, isInGroup) : defaultCoord;
            }
            else
            {
                return (pr.ContainsKey(ShapeAttr.Top) && pr.ContainsKey(ShapeAttr.Height))
                    ? GetCoordinate(shape.Top + shape.Height, isInGroup)
                    : defaultCoord;
            }
        }

        private static int GetCoordinate(double pos, bool isInGroup)
        {
            // WORDSNET-19773 Position of nested shape does not need the conversion to "twips".
            return isInGroup ? MathUtil.CastDoubleToInt(pos) : ConvertUtilCore.PointToTwip(pos);
        }

        /// <summary>
        /// Calculates coordinate for line vertices.
        /// </summary>
        /// <param name="coordinates">Obtained coordinates values from VML.</param>
        /// <param name="defaultCoord">Default coordinate value.</param>
        /// <param name="index">Index of the value among coordinates.</param>
        /// <param name="isInGroup">True - shape is nested to group.</param>
        /// <returns>Coordinate.</returns>
        private static PathValue GetVerticeCoordinate(VmlQuantity[] coordinates, int defaultCoord,
            int index, bool isInGroup)
        {
            if ((coordinates == null) || (index > (coordinates.Length - 1)))
                return new PathValue(defaultCoord);

            VmlQuantity quantity = coordinates[index];

            if (!quantity.IsDistance)
                 return new PathValue(defaultCoord);

            return  isInGroup ? new PathValue(MathUtil.DoubleToInt(quantity.DistanceToPoints()))
                : coordinates[index].DistanceToPathValue();
        }

        private static void ReadSignatureLine(ShapeBase shape, NrxXmlReader reader)
        {
            shape.SetShapeAttrInternal(ShapeAttr.SigSetupShowSignDate, true);
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "addlxml":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupAddlXml, reader.Value);
                        break;
                    case "allowcomments":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupAllowComments, reader.ValueAsBool);
                        break;
                    case "ext":
                        //ignore this element.
                        break;
                    case "id":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupId, reader.Value);
                        break;
                    case "issignatureline":
                        shape.SetShapeAttrInternal(ShapeAttr.IsSignatureLine, reader.ValueAsBool);
                        break;
                    case "provid":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupProvId, reader.Value);
                        break;
                    case "showsigndate":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupShowSignDate, reader.ValueAsBool);
                        break;
                    case "signinginstructions":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupSignInst, reader.Value);
                        break;
                    case "signinginstructionsset":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupSignInstSet, reader.ValueAsBool);
                        break;
                    case "sigprovurl":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupProvUrl, reader.Value);
                        break;
                    case "suggestedsigner":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupSuggSigner, reader.Value);
                        break;
                    case "suggestedsigner2":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupSuggSigner2, reader.Value);
                        break;
                    case "suggestedsigneremail":
                        shape.SetShapeAttrInternal(ShapeAttr.SigSetupSuggSignerEmail, reader.Value);
                        break;
                    default:
                        reader.Warn(WarningType.UnexpectedContent, WarningSource.Shapes,
                            string.Format(WarningStrings.UnexpectedTagOrAttribute, reader.LocalName));
                        break;
                }
            }
        }

        /// <summary>
        /// Reads wrapcoords (Shape Bounding Polygon)
        /// This element specifies the bounding polygon that surrounds a shape.
        /// This is used when text is tightly wrapped around a shape.
        /// </summary>
        private static PathPoint[] ReadWrapPolygon(string value)
        {
            Regex numberRegex = new Regex(@"-?\d+");
            MatchCollection numbers = numberRegex.Matches(value);

            string[] values = new string[numbers.Count];

            for (int i = 0; i < numbers.Count; i++ )
                values[i] = numbers[i].Value;

            PathPoint[] wrapPolygon = new PathPoint[values.Length / 2];
            for (int i = 0; i < values.Length / 2; i++)
            {
                wrapPolygon[i] = new PathPoint(
                    VmlPathReader.ParsePathValue(values[2 * i]),
                    VmlPathReader.ParsePathValue(values[2 * i + 1]));
            }

            return (wrapPolygon.Length != 0) ? wrapPolygon : null;
        }

        /// <summary>
        /// Sets the attribute which is an identifier, only if it can parse it from the provided VML string.
        /// Additionally, checks that such shape type identifier exists in the template, when shape type
        /// attribute has to be set.
        /// </summary>
        private static void SetIdentifierAttrSafe(ShapeBase shape, int shapeAttr, string id)
        {
            int value = NrxXmlUtil.TryParseId(id);

            if ((value == int.MinValue) ||
                (shapeAttr == ShapeAttr.ShapeType) && (ShapeTypeLibrary.GetShapeTypeXml((ShapeType)value) == null))
            {
                return;
            }

            shape.SetShapeAttrInternal(shapeAttr, value);
        }

        private static void ReadShapeStyle(ShapeBase shape, string style, IVmlShapeReaderContext context, NrxXmlReader reader)
        {
            VmlNameValue[] props = VmlToNameValueArray(style);

            bool isInline = true;

            for (int i = 0; i < props.Length; i++)
            {
                string name = props[i].Name;
                string value = props[i].Value;

                if ((name == "position") && (value == "absolute"))
                {
                    // alexnosk Part of WORDSNET-8783 Fix: If position of shape is set to absolute, set wrap type to none.
                    // Later we will check whether wrap type is set in direct attributes and determine whether it should be reset.
                    shape.SetShapeAttrInternal(ShapeAttr.WrapType, WrapType.None);
                    isInline = false;
                }

                ReadShapeStyleProperty(shape, name, value, context, reader);
            }

            if (isInline)
                shape.SetShapeAttrInternal(ShapeAttr.WrapType, WrapType.Inline);
        }

        /// <summary>
        /// Reads property from 'style' attribute of a shape.
        /// </summary>
        private static void ReadShapeStyleProperty(
            ShapeBase shape,
            string name,
            string value,
            IVmlShapeReaderContext context,
            NrxXmlReader reader)
        {
            bool inGroup = (context.GroupNestingLevel > 0);

            switch (name)
            {
                case "text-align":
                    // Not found. Ignored.
                    break;
                case "top":
                case "margin-top":
                    shape.Top = ReadDistancePoints(value, 0, !inGroup);
                    break;
                case "left":
                case "margin-left":
                    shape.Left = ReadDistancePoints(value, 0, !inGroup);
                    break;
                case "width":
                    // Do not apply default units for elements inside group shape.
                    shape.SetWidthSafe(ReadDistancePoints(value, ShapeBase.InvalidShapeSizeDefault, !inGroup));
                    break;
                case "height":
                    shape.SetHeightSafe(ReadDistancePoints(value, ShapeBase.InvalidShapeSizeDefault, !inGroup));
                    break;
                case "rotation":
                    shape.SetShapeAttrInternal(ShapeAttr.TransformRotation, ReadDegrees(value));
                    break;
                case "flip":
                    // WORDSNET-14444 In the document flip is specified two times, earlier we take in account only the last value,
                    // MS Word takes in account both. Do the same now.
                    FlipOrientation flip = VmlEnum.VmlToFlipOrientation(value);
                    if (shape.GetDirectShapeAttrInternal(ShapeAttr.Flip) != null)
                        flip |= (FlipOrientation)shape.GetDirectShapeAttrInternal(ShapeAttr.Flip);

                    shape.SetShapeAttrInternal(ShapeAttr.Flip, flip);
                    break;
                case "visibility":
                    SetBoolAttribute(shape, ShapeAttr.Hidden, "hidden", "visible", value);
                    break;
                case "z-index":
                {
                    // WORDSNET-14636 Implement reading of the non-numeric value "auto" for "z-index" attribute.
                    // According to spec: "auto - Uses the order that the shapes appear in the page, bottom to top".
                    int zIndex = StringUtil.EqualsIgnoreCase(value, "auto")
                        ? 0
                        // WORDSNET-22259 Resilient reading of Int64 values (truncating for a while).
                        : ZOrderUtil.TruncateLongToInt(FormatterPal.ParseInt64(value));

                    if (zIndex < 0)
                        shape.SetShapeAttrInternal(ShapeAttr.BehindText, true);

                    // Store z-index in the ZOrder property, it will be recalculated after the document is loaded.
                    // For DOCX convert using z-index base values.
                    shape.ZOrder = ZOrderUtil.ZIndexToZOrder(zIndex, shape.BehindText);

                        context.AddToZOrderList(shape);
                    break;
                }

                case "mso-wrap-edited":
                    SetBoolAttribute(shape, ShapeAttr.EditedWrap, value);
                    break;
                case "mso-wrap-distance-left":
                    SetEmusAttribute(shape, ShapeAttr.DistanceLeft, value);
                    break;
                case "mso-wrap-distance-top":
                    SetEmusAttribute(shape, ShapeAttr.DistanceTop, value);
                    break;
                case "mso-wrap-distance-right":
                    SetEmusAttribute(shape, ShapeAttr.DistanceRight, value);
                    break;
                case "mso-wrap-distance-bottom":
                    SetEmusAttribute(shape, ShapeAttr.DistanceBottom, value);
                    break;
                case "mso-position-horizontal":
                    shape.SetShapeAttrInternal(ShapeAttr.HorizontalAlignment, StyleConvertUtil.XmlToHorizontalAlignment(value));
                    break;
                case "mso-position-horizontal-relative":
                    shape.SetShapeAttrInternal(ShapeAttr.RelativeHorizontalPosition, StyleConvertUtil.XmlToRelativeHorizontalPosition(value));
                    break;
                case "mso-position-vertical":
                    shape.SetShapeAttrInternal(ShapeAttr.VerticalAlignment, StyleConvertUtil.XmlToVerticalAlignment(value));
                    break;
                case "mso-position-vertical-relative":
                    shape.SetShapeAttrInternal(ShapeAttr.RelativeVerticalPosition, StyleConvertUtil.XmlToRelativeVerticalPosition(value, RelativeVerticalPosition.TextFrameDefault));
                    break;
                case "mso-wrap-style":
                    shape.SetShapeAttrInternal(ShapeAttr.TextboxWrapMode, VmlEnum.VmlToTextboxWrapMode(value));
                    break;
                case "position":
                    // andrnosk: Ignored. We do not need to read this attribute, because we write it depending on wrap type.
                    break;

                case "v-text-anchor":
                    shape.SetShapeAttrInternal(ShapeAttr.TextboxAnchor, VmlEnum.VmlToTextboxAnchor(value));
                    break;

                // These are available since Word 2007 and specify shape position and size in percent.
                // WORDSNET-21025 MS Word ignores the decimal part of mso-XXX-percent values.
                case "mso-left-percent":
                    shape.SetShapeAttrInternal(ShapeAttr.LeftPercent, FormatterPal.XmlToInt(value));
                    break;
                case "mso-top-percent":
                    shape.SetShapeAttrInternal(ShapeAttr.TopPercent, FormatterPal.XmlToInt(value));
                    break;
                case "mso-width-percent":
                    shape.SetShapeAttrInternal(ShapeAttr.WidthPercent, FormatterPal.XmlToInt(value));
                    break;
                case "mso-height-percent":
                    shape.SetShapeAttrInternal(ShapeAttr.HeightPercent, FormatterPal.XmlToInt(value));
                    break;
                case "mso-width-relative":
                    shape.SetShapeAttrInternal(ShapeAttr.RelativeWidth, StyleConvertUtil.XmlToRelativeWidth(value));
                    break;
                case "mso-height-relative":
                    shape.SetShapeAttrInternal(ShapeAttr.RelativeHeight, StyleConvertUtil.XmlToRelativeHeight(value));
                    break;
                default:
                    reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                    break;
            }
        }

        private static void ReadInk(ShapeBase shape, IVmlShapeReaderContext context)
        {
            NrxXmlReader reader = context.XmlReader;

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "i":
                        shape.SetShapeAttrInternal(ShapeAttr.InkData, Convert.FromBase64String(value));
                        break;
                    case "annotation":
                        shape.SetShapeAttrInternal(ShapeAttr.InkAnnotation, (value == "t"));
                        break;
                    default:
                        // Ignore other attributes.
                        break;
                }
            }
        }

        private static void ReadStroke(ShapeBase shape, IVmlShapeReaderContext context)
        {
            NrxXmlReader reader = context.XmlReader;
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "id":  // r:id - DOCX
                    case "src": // WordML.
                        {
                            // DOCX only.
                            byte[] binData = context.GetBinData(value);
                            shape.SetShapeAttrInternal(ShapeAttr.LineImageBytes, binData);
                            break;
                        }
                    case "title": // o:title
                        {
                            if (shape.ShapePr.GetDirectAttr(ShapeAttr.LineFillBlipName) == null)
                                shape.SetShapeAttrInternal(ShapeAttr.LineFillBlipName, value);
                            break;
                        }
                    case "joinstyle":
                        shape.SetShapeAttrInternal(ShapeAttr.LineJoinStyle, VmlEnum.VmlToJoinStyle(value));
                        break;
                    case "dashstyle":
                        shape.SetShapeAttrInternal(ShapeAttr.LineDashStyle, VmlEnum.VmlToDashStyle(value));
                        break;
                    case "linestyle":
                        shape.SetShapeAttrInternal(ShapeAttr.LineStyle, VmlEnum.VmlToShapeLineStyle(value));
                        break;
                    case "endcap":
                        shape.SetShapeAttrInternal(ShapeAttr.LineEndCapStyle, VmlEnum.VmlToEndCap(value));
                        break;
                    case "startarrow":
                        shape.SetShapeAttrInternal(ShapeAttr.LineStartArrow, VmlEnum.VmlToArrowType(value));
                        break;
                    case "startarrowwidth":
                        shape.SetShapeAttrInternal(ShapeAttr.LineStartArrowWidth, VmlEnum.VmlToArrowWidth(value));
                        break;
                    case "startarrowlength":
                        shape.SetShapeAttrInternal(ShapeAttr.LineStartArrowLength, VmlEnum.VmlToArrowLength(value));
                        break;
                    case "endarrow":
                        shape.SetShapeAttrInternal(ShapeAttr.LineEndArrow, VmlEnum.VmlToArrowType(value));
                        break;
                    case "endarrowwidth":
                        shape.SetShapeAttrInternal(ShapeAttr.LineEndArrowWidth, VmlEnum.VmlToArrowWidth(value));
                        break;
                    case "endarrowlength":
                        shape.SetShapeAttrInternal(ShapeAttr.LineEndArrowLength, VmlEnum.VmlToArrowLength(value));
                        break;
                    case "opacity":
                        SetFixedAttribute(shape, ShapeAttr.LineOpacity, value);
                        break;
                    case "color":
                        // WORDSNET-11252 Seems, MS Word reads this attribute, but does not write this again to stroke element upon resaving.
                        // Instead, it overrides 'strokecolor' attribute with this color.
                        shape.SetShapeAttrInternal(ShapeAttr.LineColor, VmlColor.VmlToColor(value));
                        break;
                    case "color2":
                        shape.SetShapeAttrInternal(ShapeAttr.LineBackColor, VmlColor.VmlToColor(value));

                        DrColor baseColor = VmlColor.GetBaseColor(value);
                        if (baseColor != null)
                            shape.SetShapeAttrInternal(ShapeAttr.LineBackColorExt, baseColor);
                        int colorModifier = VmlColor.GetColorModifier(value);
                        if (colorModifier != 0)
                            shape.SetShapeAttrInternal(ShapeAttr.LineBackColorExtMod, colorModifier);
                        break;
                    case "filltype":
                        shape.SetShapeAttrInternal(ShapeAttr.LineFillType, VmlEnum.VmlToLineFillType(value));
                        break;
                    case "on":
                        shape.SetShapeAttrInternal(ShapeAttr.LineOn, reader.ValueAsBool);
                        break;
                    case "weight":
                        SetStrokeWeight(shape, value);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        private static void ReadImageData(Shape shape, IVmlShapeReaderContext context)
        {
            NrxXmlReader reader = context.XmlReader;

            bool hasId = (reader.ReadAttribute("id", null) != null);
            bool hasSrc = (reader.ReadAttribute("src", null) != null);

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "id":  // r:id DOCX.
                    {
                        if (reader.Prefix == "r")
                            ReadImageSource(context, value, shape);
                        break;
                    }
                    case "src": // WordML.
                        ReadImageSource(context, value, shape);
                        break;
                    case "pict": // r:pict DOCX.
                    {
                        // MS Word outputs Macintosh PICT images to WordML/Docx in two formats: WMF (primary) and PICT (pcz) (alternate).
                        // r:id - primary, r:pict - alternate.
                        // We import only in primary format, ignoring alternate format.
                        // But for v:image, r:pict is primary image source and we should read it.
                        if (!hasId && !hasSrc)
                            ReadImageSource(context, value, shape);
                        break;
                    }
                    case "href": // r:href
                        shape.SetShapeAttrInternal(ShapeAttr.ImageSourceFullName, context.GetRelationshipTarget(value));
                        break;
                    case "title": // o:title
                        shape.SetShapeAttrInternal(ShapeAttr.ImageTitle, value);
                        break;
                    case "croptop":
                        SetFixedAttribute(shape, ShapeAttr.ImageCropTop, value);
                        break;
                    case "cropbottom":
                        SetFixedAttribute(shape, ShapeAttr.ImageCropBottom, value);
                        break;
                    case "cropleft":
                        SetFixedAttribute(shape, ShapeAttr.ImageCropLeft, value);
                        break;
                    case "cropright":
                        SetFixedAttribute(shape, ShapeAttr.ImageCropRight, value);
                        break;
                    case "chromakey":
                        shape.SetShapeAttrInternal(ShapeAttr.ImageTransparent, VmlColor.VmlToColor(value));
                        break;
                    case "gain":
                        SetFixedAttribute(shape, ShapeAttr.ImageContrast, value);
                        break;
                    case "blacklevel":
                        SetFixedAttribute(shape, ShapeAttr.ImageBrightness, value);
                        break;
                    case "gamma":
                        SetFixedAttribute(shape, ShapeAttr.ImageGamma, value);
                        break;
                    case "grayscale":
                        SetBoolAttribute(shape, ShapeAttr.ImageGrayScale, value);
                        break;
                    case "bilevel":
                        SetBoolAttribute(shape, ShapeAttr.ImageBiLevel, value);
                        break;
                    case "embosscolor":
                        shape.SetShapeAttrInternal(ShapeAttr.ImageDblCrMod, VmlColor.VmlToColor(value));
                        break;
                    // WORDSNET-3801 The problem occurred because recolortarget attribute of image was not supported.
                    case "recolortarget":
                    {
                        DrColor recolorTarget = VmlColor.VmlToColor(value);
                        int recolorTargetValue = (recolorTarget.B << 16) + (recolorTarget.G << 8) + (recolorTarget.R);
                        shape.SetShapeAttrInternal(ShapeAttr.ImageRecolor, recolorTargetValue);
                        break;
                    }
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        private static void ReadImageSource(IVmlShapeReaderContext context, string value, Shape shape)
        {
            if (StringUtil.HasChars(value) && context.IsExternalImage(value))
            {
                // RK If the relationship is external, it means the image is linked only.
                shape.SetShapeAttrInternal(ShapeAttr.ImageSourceFullName, context.GetRelationshipTarget(value));
            }
            else
            {
                // If the relationship is internal, it refers to an image inside the package and
                // the link to the image will come in the href attribute.

                byte[] binData = context.GetBinData(value);

                if (binData == null)
                {
                    // Try to resolve shape source later.
                    context.MarkMissingSource(shape, value);
                }
                else
                    shape.ImageData.SetImageSafe(context.GetBinData(value));
            }
        }

        private static void ReadFormulas(ShapeBase shape, NrxXmlReader reader)
        {
            List<Formula> formulaList = new List<Formula>();

            while (reader.ReadChild("formulas"))
            {
                switch (reader.LocalName)
                {
                    case "f": // v:f
                    {
                        string eqn = reader.ReadAttribute("eqn", "");
                        if (StringUtil.HasChars(eqn))
                            formulaList.Add(ReadFormula(eqn));
                        break;
                    }
                    default:
                        reader.IgnoreElement();
                        break;
                }
            }

            Formula[] formulas = formulaList.ToArray();
            shape.SetShapeAttrInternal(ShapeAttr.GeometryFormulas, formulas);
        }

        private static Formula ReadFormula(string eqn)
        {
            // Formula consists of opcode and 1 to 3 operands, space delimited.
            string[] formulaParts = eqn.Split(' ');

            Formula formula = new Formula();

            formula.Operation = VmlEnum.VmlToOperation(formulaParts[0]);

            if (formulaParts.Length > 1)
            {
                string value = formulaParts[1];

                // WORDSNET-10350 Value can be negative, so should check all integers, not only positive ones.
                // This also done for all other formula Params below.
                if (!FormatterPal.IsInteger(value))
                {
                    formula.Flags = formula.Flags | 0x20;
                    formula.Param1 = GetRefOperand(value);
                }
                else
                {
                    // WORDSNET-10350 MSW converts all values to the values in range 0-65535.
                    formula.Param1 = FormatterPal.XmlToIntAsUnsigned(value);
                }
            }

            if (formulaParts.Length > 2)
            {
                string value = formulaParts[2];

                if (!FormatterPal.IsInteger(value))
                {
                    formula.Flags = formula.Flags | 0x40;
                    formula.Param2 = GetRefOperand(value);
                }
                else
                {
                    formula.Param2 = FormatterPal.XmlToIntAsUnsigned(value);
                }
            }

            if (formulaParts.Length > 3)
            {
                string value = formulaParts[3];

                if (!FormatterPal.IsInteger(value))
                {
                    formula.Flags = formula.Flags | 0x80;
                    formula.Param3 = GetRefOperand(value);
                }
                else
                {
                    formula.Param3 = FormatterPal.XmlToIntAsUnsigned(value);
                }
            }

            return formula;
        }

        private static int GetRefOperand(string value)
        {
            // WORDSNET-21467 Word ignores string case and reads both 'lineDrawn' and 'linedrawn'.
            value = value.ToLower();
            switch (value)
            {
                case "xcenter":
                    return 0x140; // The x ordinate of the center of coordorigin, coordsize (x+w/2).
                case "ycenter":
                    return 0x141; // The y ordinate of the center of coordorigin, coordsize (y+h/2).
                case "width":
                    return 0x142; // The width defined by the coordsize attribute.
                case "height":
                    return 0x143; // The height defined by the coordsize attribute.
                case "xlimo":
                    return 0x153; // The x value of the limo attribute.
                case "ylimo":
                    return 0x154; // The y value of the limo attribute.
                case "linedrawn":
                    return 0x1fc; // unknown
                case "pixellinewidth":
                    // The line width in output device pixels. This is used to outset lines from the edge of a rectangle
                    // on the assumption that the implementation draws to lower right pixel in preference to the upper left pixel when a line is on a pixel boundary.
                    return 0x4f7;
                case "pixelwidth":
                    return 0x4f8; // The width of the shape in device pixels (i.e. the coordsize width transformed into device space).
                case "pixelheight":
                    return 0x4f9; // The height of the coordsize in device pixels.
                case "emuwidth":
                    return 0x4fc; // The width of the coordsize in EMUs.
                case "emuheight":
                    return 0x4fd; // The height of the coordsize in EMUs.
                case "emuwidth2":
                    return 0x4fe; // Half the width of the coordsize in EMUs.
                case "emuheight2":
                    return 0x4ff; // Half the height of the coordsize in EMUs.
                default:
                {
                    if (value.StartsWith("@", StringComparison.Ordinal))
                        return FormatterPal.ParseInt(value.TrimStart('@')) + 0x400;  // formula reference
                    else if (value.StartsWith("#", StringComparison.Ordinal))
                        return FormatterPal.ParseInt(value.TrimStart('#')) + 0x147; // adjustment reference
                    else
                        throw new InvalidOperationException("Unrecognized operand value in the formula.");
                }
            }
        }

        private static void ReadPath(ShapeBase shape, NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "arrowok":
                        SetBoolAttribute(shape, ShapeAttr.LineArrowHeadsOK, value);
                        break;
                    case "fillok":
                        SetBoolAttribute(shape, ShapeAttr.GeometryFillOK, value);
                        break;
                    case "strokeok":
                        SetBoolAttribute(shape, ShapeAttr.GeometryLineOK, value);
                        break;
                    case "shadowok":
                        SetBoolAttribute(shape, ShapeAttr.GeometryShadowOK, value);
                        break;
                    case "extrusionok":
                    {
                        // WORDSNET-22286 Resilent reading of non-standard boolean values as false.
                        SetBoolAttribute(shape, ShapeAttr.GeometryThreeDOK, value, false);
                        break;
                    }
                    case "gradientshapeok":
                        SetBoolAttribute(shape, ShapeAttr.GeometryFillShadeShapeOK, value);
                        break;
                    case "textpathok":
                        SetBoolAttribute(shape, ShapeAttr.GeometryGTextOK, value);
                        break;
                    case "limo":
                    {
                        int[] limo = VmlUtil.VmlToIntArray(value);
                        int x = (limo.Length > 0) ? limo[0] : 0;
                        int y = (limo.Length > 1) ? limo[1] : 0;
                        shape.SetShapeAttrInternal(ShapeAttr.GeometryXLimo, x);
                        shape.SetShapeAttrInternal(ShapeAttr.GeometryYLimo, y);
                        break;
                    }
                    case "connecttype": // o:connecttype
                        shape.SetShapeAttrInternal(ShapeAttr.GeometryConnectionSiteType, VmlEnum.VmlToConnectionSiteType(value));
                        break;
                    case "connectlocs": // o:connectlocs
                        SetConnectLocs(shape, value);
                        break;
                    case "connectangles": // o:connectangles
                        SetConnectAngles(shape, value);
                        break;
                    case "textboxrect":
                        SetTexboxRect(shape, value);
                        break;
                    // andrnosk: WORDSNET-9633 Read Path Definition string containing the commands that define the shape's path.
                    case "v":
                        SetPath(shape, value);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        private static void SetConnectLocs(ShapeBase shape, string value)
        {
            string[] values = value.Split(gConnectLocsSplitChars);
            PathPoint[] connectLocs = new PathPoint[values.Length / 2];

            for (int i = 0; i < values.Length / 2; i++)
            {
                connectLocs[i] = new PathPoint(
                    VmlPathReader.ParsePathValue(values[2 * i]),
                    VmlPathReader.ParsePathValue(values[2 * i + 1]));
            }

            shape.SetShapeAttrInternal(ShapeAttr.GeometryConnectLocs, connectLocs);
        }

        private static void SetConnectAngles(ShapeBase shape, string value)
        {
            string[] values = value.Split(',');
            int[] connectAngles = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
                connectAngles[i] = ReadDegrees(values[i]);

            shape.SetShapeAttrInternal(ShapeAttr.GeometryConnectAngles, connectAngles);
        }

        private static void SetTexboxRect(ShapeBase shape, string value)
        {
            string[] rectStrings = value.Split(';');
            PathRectangle[] rects = new PathRectangle[rectStrings.Length];

            for (int i = 0; i < rects.Length; i++)
            {
                string[] values = rectStrings[i].Split(',');
                PathRectangle rect = new PathRectangle();
                if (values.Length == 4)
                {
                    rect.Left = VmlPathReader.ParsePathValue(values[0]);
                    rect.Top = VmlPathReader.ParsePathValue(values[1]);
                    rect.Right = VmlPathReader.ParsePathValue(values[2]);
                    rect.Bottom = VmlPathReader.ParsePathValue(values[3]);
                }
                rects[i] = rect;
            }

            shape.SetShapeAttrInternal(ShapeAttr.GeometryPathTextBoxRects, rects);
        }

        private static void ReadTextPath(ShapeBase shape, NrxXmlReader reader)
        {
            // The following attributes are 'true' by default for all GeoText(TextPath) shapes.
            shape.SetShapeAttrInternal(ShapeAttr.GeoTextOn, true);
            shape.SetShapeAttrInternal(ShapeAttr.GeoTextStretch, true);

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "on":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextOn, value);
                        break;
                    case "style":
                        ReadTextPathStyle(shape, value);
                        break;
                    case "fitshape":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextStretch, value);
                        break;
                    case "trim":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextShrinkFit, value);
                        break;
                    case "fitpath":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextBestFit, value);
                        break;
                    case "xscale":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextDxMeasure, value);
                        break;
                    case "string":
                        shape.SetShapeAttrInternal(ShapeAttr.GeoTextText, value);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        private static void ReadTextPathStyle(ShapeBase shape, string style)
        {
            VmlNameValue[] props = VmlToNameValueArray(style);

            for (int i = 0; i < props.Length; i++)
            {
                string value = props[i].Value;

                switch (props[i].Name)
                {
                    case "font-family":
                        // Normally in MS generated WordML the font-family name is enclosed in double quotes ("). But we should expect it to be enclosed in single quotes(') as well.
                        shape.SetShapeAttrInternal(ShapeAttr.GeoTextFont, value.Trim(new char[]{'"', '\''}));
                        break;
                    case "font-size":
                        // andrnosk: Pass UseDefaultUnits=false because according to specification ECMA 376 6.1.2.23 the font size is always defined in points,
                        // and we do not need to change units if it is not specified.
                        shape.SetShapeAttrInternal(ShapeAttr.GeoTextSize, ConvertUtilCore.DoubleToFixed(ReadDistancePoints(value, ShapePr.GeoTextSizeDefault, false)));
                        break;
                    case "font-style":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextItalic, "italic", "normal", value);
                        break;
                    case "font-weight":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextBold, "bold", "normal", value);
                        break;
                    case "font-variant":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextSmallCaps, "small-caps", "normal", value);
                        break;
                    case "text-decoration":
                    {
                        switch (value)
                        {
                            case "underline":
                                shape.SetShapeAttrInternal(ShapeAttr.GeoTextUnderline, true);
                                break;
                            case "line-through":
                                shape.SetShapeAttrInternal(ShapeAttr.GeoTextStrikeThrough, true);
                                break;
                            default:
                                // Skip other values.
                                break;
                        }
                        break;
                    }
                    case "mso-text-shadow":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextShadow, "auto", "normal", value);
                        break;
                    case "v-text-align":
                        shape.SetShapeAttrInternal(ShapeAttr.GeoTextAlign, VmlEnum.VmlToTextPathAlignment(value));
                        break;
                    case "v-text-spacing-mode":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextTight, "tightening", "tracking", value);
                        break;
                    case "v-text-spacing":
                        SetFixedAttribute(shape, ShapeAttr.GeoTextSpacing, value);
                        break;
                    case "v-text-kern":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextKerning, value);
                        break;
                    case "v-text-reverse":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextReverseRows, value);
                        break;
                    case "v-same-letter-heights":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextNormalize, value);
                        break;
                    case "v-rotate-letters":
                        SetBoolAttribute(shape, ShapeAttr.GeoTextVertical, value);
                        break;
                    default:
                        // Ignore others.
                        break;
                }
            }
        }

        private static void ReadHandles(ShapeBase shape, NrxXmlReader reader)
        {
            List<Handle> handleList = new List<Handle>();

            while (reader.ReadChild("handles"))
            {
                switch (reader.LocalName)
                {
                    case "h": // v:h
                        handleList.Add(ReadHandle(reader));
                        break;
                    default:
                        reader.IgnoreElement();
                        break;
                }
            }

            Handle[] handles = handleList.ToArray();
            shape.SetShapeAttrInternal(ShapeAttr.GeometryHandles, handles);
        }

        private static Handle ReadHandle(NrxXmlReader reader)
        {
            Handle h = new Handle();

            // Set these values to 'empty' values.
            h.XMin = new PathValue(int.MinValue);
            h.XMax = new PathValue(int.MaxValue);

            h.YMin = new PathValue(int.MinValue);
            h.YMax = new PathValue(int.MaxValue);

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "position":
                    {
                        HandlePoint position = ReadHandlePoint(value);
                        h.PositionX = position.X;
                        h.PositionY = position.Y;
                        break;
                    }
                    case "switch":
                        h.HasSwitch = true;
                        break;
                    case "polar":
                    {
                        h.HasPolar = true;
                        PathPoint polar = ReadPathPoint(value);
                        h.PolarX = polar.X;
                        h.PolarY = polar.Y;
                        break;
                    }
                    case "map":
                    {
                        h.HasMap = true;
                        PathPoint polar = ReadPathPoint(value);
                        h.PolarX = polar.X;
                        h.PolarY = polar.Y;
                        break;
                    }
                    case "radiusrange":
                    {
                        h.HasRadiusRange = true;
                        PathPoint range = ReadPathPoint(value);
                        h.XMin = range.X;
                        h.XMax = range.Y;
                        break;
                    }
                    case "xrange":
                    {
                        h.HasXYRange = true;
                        PathPoint range = ReadPathPoint(value);
                        h.XMin = range.X;
                        h.XMax = range.Y;
                        break;
                    }
                    case "yrange":
                    {
                        h.HasXYRange = true;
                        PathPoint range = ReadPathPoint(value);
                        h.YMin = range.X;
                        h.YMax = range.Y;
                        break;
                    }
                    default:
                        // Ignore other attributes.
                        break;
                }
            }

            return h;
        }

        private static HandlePoint ReadHandlePoint(string value)
        {
            string[] subvalues = value.Split(',');
            return new HandlePoint(GetHandleValue(subvalues[0]), GetHandleValue(subvalues[1]));
        }

        private static PathPoint ReadPathPoint(string value)
        {
            string[] subvalues = value.Split(',');
            return new PathPoint(
                GetPathValue(subvalues[0]),
                GetPathValue(subvalues[1]));
        }

        private static PathValue GetPathValue(string value)
        {
            switch (value)
            {
                case "topLeft":
                    return new PathValue(0, true);
                case "bottomRight":
                    return new PathValue(1, true);
                case "center":
                    return new PathValue(2, true);
                default:
                    // Process other values below.
                    break;
            }

            if (value.StartsWith("@", StringComparison.Ordinal))
                return new PathValue(FormatterPal.ParseInt(value.TrimStart('@')) + 3, true);

            if (value.StartsWith("#", StringComparison.Ordinal))
                return new PathValue(FormatterPal.ParseInt(value.TrimStart('#')) + 256, true);

            return new PathValue(FormatterPal.ParseInt(value), false);
        }

        private static HandlePositionValue GetHandleValue(string value)
        {
            switch (value)
            {
                case "topLeft":
                    return new HandlePositionValue(HandlePositionType.LeftTop, 0);
                case "bottomRight":
                    return new HandlePositionValue(HandlePositionType.RightBottom, 0);
                case "center":
                    return new HandlePositionValue(HandlePositionType.Center, 0);
                default:
                    // Process other values below.
                    break;
            }

            if (value.StartsWith("@", StringComparison.Ordinal))
                return new HandlePositionValue(HandlePositionType.Formula, FormatterPal.ParseInt(value.TrimStart('@')));

            if (value.StartsWith("#", StringComparison.Ordinal))
                return new HandlePositionValue(HandlePositionType.Adjust, FormatterPal.ParseInt(value.TrimStart('#')));

            return new HandlePositionValue(HandlePositionType.Constant, FormatterPal.ParseInt(value));
        }

        private static void ReadLock(ShapeBase shape, NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "ext": // v:ext
                        // Always "edit", ignore.
                        break;
                    case "aspectratio":
                        SetBoolAttribute(shape, ShapeAttr.LockAspectRatio, value);
                        break;
                    case "verticies":
                        SetBoolAttribute(shape, ShapeAttr.LockVertices, value);
                        break;
                    case "text":
                        SetBoolAttribute(shape, ShapeAttr.LockText, value);
                        break;
                    case "shapetype":
                        SetBoolAttribute(shape, ShapeAttr.LockShapeType, value);
                        break;
                    case "grouping":
                        SetBoolAttribute(shape, ShapeAttr.LockAgainstGrouping, value);
                        break;
                    default:
                        // Ignore other attributes.
                        break;
                }
            }
        }

        private static bool ReadTextbox(IVmlShapeReaderContext context, ShapeBase shape)
        {
            NrxXmlReader reader = context.XmlReader;

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "style":
                        ReadTextboxStyle(shape, reader.Value);
                        break;
                    case "inset":
                        SetInset(shape, reader.Value);
                        break;
                    default:
                        // Ignore other attributes.
                        break;
                }
            }

            // When shape is loaded, it is not added to the document tree immediately,
            // it is returned to the caller and the caller adds it to the tree.
            return context.ReadTextboxContent(shape);
        }

        private static void ReadTextboxStyle(ShapeBase shape, string style)
        {
            VmlNameValue[] props = VmlToNameValueArray(style);

            string layoutFlow = null;
            string layoutFlowAlt = null;

            for (int i = 0; i < props.Length; i++)
            {
                string value = props[i].Value;

                switch (props[i].Name)
                {
                    case "layout-flow":
                        layoutFlow = value;
                        break;
                    case "mso-layout-flow-alt":
                        layoutFlowAlt = value;
                        break;
                    case "mso-fit-shape-to-text":
                        SetBoolAttribute(shape, ShapeAttr.TextboxFitShapeToText, value);
                        break;
                    case "mso-next-textbox":
                        // Textboxes linked by "id" rather than by "spid" in DOCX.
                        shape.SetShapeAttrInternal(ShapeAttr.Sys_TextboxNextShapeIdRaw, value.Trim('#'));
                        break;
                    default:
                        // Ignore other properties.
                        break;
                }
            }

            if (layoutFlow != null)
            {
                if (layoutFlow == "vertical" && layoutFlowAlt == "bottom-to-top")
                    shape.SetShapeAttrInternal(ShapeAttr.TextboxLayoutFlow, LayoutFlow.BottomToTop);
                else
                    shape.SetShapeAttrInternal(ShapeAttr.TextboxLayoutFlow, VmlEnum.VmlToLayoutFlow(layoutFlow));
            }
        }

        private static void ReadCallout(ShapeBase shape, NrxXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "ext":
                        // Always "edit", ignore.
                        break;
                    case "type":
                        shape.SetShapeAttrInternal(ShapeAttr.CalloutType, VmlEnum.VmlToCalloutType(value));
                        break;
                    case "gap":
                        SetEmusAttribute(shape, ShapeAttr.CalloutGap, value);
                        break;
                    case "angle":
                        shape.SetShapeAttrInternal(ShapeAttr.CalloutAngle, VmlEnum.VmlToCalloutAngle(value));
                        break;
                    case "drop":
                        shape.SetShapeAttrInternal(ShapeAttr.CalloutDropType, VmlEnum.VmlToCalloutDropType(value));
                        break;
                    case "distance":
                        SetEmusAttribute(shape, ShapeAttr.CalloutDropDistance, value);
                        break;
                    case "length":
                        SetEmusAttribute(shape, ShapeAttr.CalloutLength, value);
                        break;
                    case "on":
                        SetBoolAttribute(shape, ShapeAttr.CalloutOn, value);
                        break;
                    case "accentbar":
                        SetBoolAttribute(shape, ShapeAttr.CalloutAccentBar, value);
                        break;
                    case "textborder":
                        SetBoolAttribute(shape, ShapeAttr.CalloutTextBorder, value);
                        break;
                    case "minusx":
                        SetBoolAttribute(shape, ShapeAttr.CalloutMinusX, value);
                        break;
                    case "minusy":
                        SetBoolAttribute(shape, ShapeAttr.CalloutMinusY, value);
                        break;
                    case "dropauto":
                        SetBoolAttribute(shape, ShapeAttr.CalloutDropAuto, value);
                        break;
                    case "lengthspecified":
                        SetBoolAttribute(shape, ShapeAttr.CalloutLengthSpecified, value);
                        break;
                    default:
                        reader.Warn(WarningType.MinorFormattingLoss, WarningSource.Shapes, string.Format(WarningMessageFormat, reader.LocalName));
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 'w10:wrap' element.
        /// </summary>
        private static void ReadWrap(ShapeBase shape, NrxXmlReader reader)
        {
            // alexnosk Part of WORDSNET-8783 Fix: Do not set default wrap type,
            // instead default value will be taken from inherited defaults.

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "type":
                    {
                        // alexnosk Part of WORDSNET-8783 Fix: if wrap type is inline
                        // and wrap type is already set in direct attributes - leave attribute unchanged.
                        // It is already set to None or Inline value depending on position.
                        WrapType wrapType = VmlEnum.VmlToWrapType(value);
                        if (shape.ShapePr.Contains(ShapeAttr.WrapType) && (shape.WrapType == WrapType.Inline))
                            break;

                        shape.SetShapeAttrInternal(ShapeAttr.WrapType, wrapType);
                        break;
                    }
                    case "side":
                        shape.SetShapeAttrInternal(ShapeAttr.WrapSide, VmlEnum.VmlToWrapSide(value));
                        break;
                    case "anchorx":
                    case "anchory":
                        // RK These settings seem to duplicate mso-horizontal-position-relative
                        // and mso-vertical-position-relative, yet they sometimes contradict them.
                        // I think it is safer to ignore these values.
                        break;
                    default:
                        // Ignore other attributes.
                        break;
                }
            }
        }

        private static void ReadBorder(ShapeBase shape, int borderAttr, NrxXmlReader reader)
        {
            ShapePr shapePr = shape.ShapePr;
            Border border = (Border)shapePr.GetDirectAttr(borderAttr);

            // If there is no border in shape yet, then create it.
            if (border == null)
            {
                border = new Border();
                shapePr.SetAttr(borderAttr, border);
            }

            while (reader.MoveToNextAttribute())
            {
                string value = reader.Value;

                switch (reader.LocalName)
                {
                    case "type":
                        border.LineStyleInternal = VmlEnum.VmlToLineStyle(value);
                        break;
                    case "width":
                        border.SetLineWidthSafe(FormatterPal.ParseDouble(value) / 8);
                        border.ValidateNoneBorder();
                        break;
                    case "shadow":
                        border.Shadow = (value == "t");
                        break;
                    default:
                        // Ignore other attributes.
                        break;
                }
            }
        }

        private static Border CreateBorderWithColor(string vmlcolor)
        {
            Border border = new Border();
            border.ColorInternal = VmlColor.VmlToColor(vmlcolor);
            return border;
        }

        private static void SetAdjustments(ShapeBase shape, string adjustments)
        {
            if (!StringUtil.HasChars(adjustments))
                return;

            string[] values = adjustments.Split(',');

            for (int i = 0; i < values.Length; i++)
            {
                string value = values[i];

                if (value != "")
                    shape.SetShapeAttrInternal(ShapeAttr.GeometryAdjust1 + i, FormatterPal.ParseInt(value));
            }
        }

        private static void SetPath(ShapeBase shape, string path)
        {
            if (!StringUtil.HasChars(path))
                return;

            VmlPathReader pathReader = new VmlPathReader(path);
            shape.SetShapeAttrInternal(ShapeAttr.GeometrySegmentInfo, pathReader.PathInfos);
            shape.SetShapeAttrInternal(ShapeAttr.GeometryVertices, pathReader.Points);
        }

        private static void SetPoints(ShapeBase shape, string value)
        {
            if (!StringUtil.HasChars(value))
                return;

            VmlQuantity[] values = VmlToQuantityArray(value);

            // Build an array of points and store in the model.
            PathPoint[] points = new PathPoint[values.Length / 2];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = new PathPoint(
                    values[2 * i].DistanceToPathValue(),
                    values[2 * i + 1].DistanceToPathValue());
            }
            shape.SetShapeAttrInternal(ShapeAttr.GeometryVertices, points);

            // Build an array of path infos and store in the model.
            // This seems to be how MS Word does it for DOC files.
            PathInfo[] pathInfos = new PathInfo[points.Length * 2 + 1];
            pathInfos[0] = new PathInfo(PathType.MoveTo, 0);
            for (int i = 1; i < pathInfos.Length - 1; i++)
            {
                if (MathUtil.IsOdd(i))
                    pathInfos[i] = new PathInfo(PathType.EscapeAutoLine, 0);
                else
                    pathInfos[i] = new PathInfo(PathType.LineTo, 1);
            }
            pathInfos[pathInfos.Length - 1] = new PathInfo(PathType.End, 0);
            shape.SetShapeAttrInternal(ShapeAttr.GeometrySegmentInfo, pathInfos);
        }

        private static void SetInset(ShapeBase shape, string inset)
        {
            string[] values = inset.Split(',');

            int index = 0;
            int maxIndex = values.Length - 1;

            for (int attr = ShapeAttr.TextboxLeft; attr <= ShapeAttr.TextboxBottom; attr++)
            {
                SetInsetAttribute(shape, attr, values[index]);

                index++;

                if (index > maxIndex)
                    return;
            }
        }

        /// <summary>
        /// Sets the specified inset margin attribute.
        /// </summary>
        private static void SetInsetAttribute(ShapeBase shape, int shapeAttr, string value)
        {
            VmlQuantity quantity = new VmlQuantity(value);
            if (!quantity.IsDistance)
                return;

            // VmlQuantity uses points as default units when units are not specified, but inset margins expect EMUs
            // as default units. So, use VmlQuantity to perform the necessary conversions only if units are specified
            // in 'value'.
            // Since quantity.IsDistance is 'true', it means that the value was successfully parsed as double, do the
            // same here and convert to 'int'.
            int margin = quantity.IsWithoutUnits
                ? MathUtil.DoubleToInt(FormatterPal.ParseDouble(value))
                : quantity.DistanceToEmus();

            shape.SetShapeAttrInternal(shapeAttr, margin);
        }

        /// <summary>
        /// Converts distance from WordML string to numeric value in points.
        /// If units in specified string are 'in' or 'mm' then the numeric value is converted to points.
        /// If units are "pt" or not specified then the value is returned as is.
        /// If the value is not recognized, the defaultValue is returned.
        /// </summary>
        private static double ReadDistancePoints(string value, double defaultValue, bool usePixelDefaultUnits)
        {
            // andrnosk: WORDSNET-7668 According to specification ECMA 376 (6.1.2.1 page 4362).
            // Upon height/weight specifying if no units are given, pixels (px) is assumed.
            // This rule is not applied for shapes and other elements inside group shape (like lines etc).
            VmlQuantity q = new VmlQuantity(value, usePixelDefaultUnits ? VmlQuantity.PixelUnits : VmlQuantity.PointUnits);

            return q.IsDistance ? q.DistanceToPoints() : defaultValue;
        }

        /// <summary>
        /// Converts angle magnitude from WordML string to numeric value.
        /// If units in specified string are 'fd' then the value is returned as is.
        /// If units are not specified then the value is returned multiplied by 0x10000.
        /// If the units are not recognized or no valid numeric value is found then 0 is returned.
        /// </summary>
        private static int ReadDegrees(string value)
        {
            VmlQuantity q = new VmlQuantity(value);
            return q.IsDegrees ? q.DegreesToFixed() : 0;
        }

        /// <summary>
        /// Reads shape's style attribute.
        /// </summary>
        private static VmlNameValue[] VmlToNameValueArray(string style)
        {
            string[] props = style.Split(';');

            VmlNameValue[] nameValues = new VmlNameValue[props.Length];

            for (int i = 0; i < props.Length; i++)
                nameValues[i] = VmlNameValue.FromString(props[i]);

            // WORDSNET-10921 The problem occurred because there was empty entry in the props array.
            // WORDSNET-13610 The problem is very similar to 10921, but in this case in props array there is invalid data.
            // In both cases final nameValues array can contain null entries. so to fix we need to remove null entries from array.
            return RemoveNullEntries(nameValues);
        }

        /// <summary>
        /// Removes null entries from VmlNameValue array.
        /// </summary>
        private static VmlNameValue[] RemoveNullEntries(VmlNameValue[] input)
        {
            // First we should optimize the array, i.e. move all not null entries to the beginning of the array.
            int newIndex = 0;
            for (int oldIndex = 0; oldIndex < input.Length; oldIndex++)
            {
                if (input[oldIndex] != null)
                    input[newIndex++] = input[oldIndex];
            }

            // Now just copy array from 0 to newIndex position to another array.
            VmlNameValue[] output = new VmlNameValue[newIndex];
            for (int i = 0; i < output.Length; i++)
                output[i] = input[i];

            return output;
        }

        /// <summary>
        /// In VML roundrect arcsize is stored in the "arcsize" attribute, but in other formats
        /// and in the model it is stored as adj1 and in different units.
        /// </summary>
        private static void ArcSizeToAdj1(ShapeBase shape, string value)
        {
            VmlQuantity q = new VmlQuantity(value);

            // The arcsize value in VML is percent but can be specified either as percent or fixed.
            double percent;
            if (q.IsPercent)
                percent = (double)q.PercentToInt() / 100;
            else if (q.IsFixed)
                percent = ConvertUtilCore.FixedToDouble(q.ToFixed());
            else
                return;

            // In the model arcsize is stored as adj1 value from 0 to coordsizewidth.
            int adj1 = MathUtil.DoubleToInt(percent * shape.CoordSizeWidth);
            shape.SetShapeAttrInternal(ShapeAttr.GeometryAdjust1, adj1);
        }

        /// <summary>
        /// Sets shape type from the specified string <paramref name="type"/>.
        /// </summary>
        private static void SetShapeType(ShapeBase shape, string type)
        {
            // WORDSNET-14341 It seems, MS Word recognizes "#imageNN" in type string as custom shape type.
            if (type.StartsWith("#image", StringComparison.Ordinal))
                shape.SetShapeAttrInternal(ShapeAttr.ShapeType, ShapeType.CustomShape);
            else
                SetIdentifierAttrSafe(shape, ShapeAttr.ShapeType, type);
        }

        /// <summary>
        /// Sets the <see cref="ShapeAttr.FillImageBytes"/> attributes from the deferred bindings.
        /// </summary>
        private static void ResolveFillSources(IVmlShapeReaderContext context)
        {
            Debug.Assert((context != null) && (context.XmlReader != null) &&
                (context.XmlReader.FillSourceMap != null));

            Dictionary<ShapeBase, string> fillSources = context.XmlReader.FillSourceMap;
            if (fillSources.Count == 0)
                return;

            List<ShapeBase> processedShapes = new List<ShapeBase>();

            foreach (ShapeBase shape in fillSources.Keys)
                if (ProcessFillSource(context, shape, fillSources[shape]))
                    processedShapes.Add(shape);

            foreach (ShapeBase shape in processedShapes)
                fillSources.Remove(shape);

            processedShapes.Clear();
        }

        /// <summary>
        /// Processes the fill source attribute using the specified context.
        /// </summary>
        private static bool ProcessFillSource(IVmlShapeReaderContext context, ShapeBase shape, string sourcePath)
        {
            Debug.Assert((context != null) && (shape != null) && (sourcePath != null));

            bool isExternalImage = context.IsExternalImage(sourcePath);

            // andrnosk: WORDSNET-6001 In the document there were fill with the Gif image,
            // we do not fully support Gif in model, so convert it to PNG as we do for image data.
            byte[] imageBytes = (isExternalImage)
                ? ImageDataUtil.LoadImageBytes(context.GetRelationshipTarget(sourcePath), context.Document)
                : ImageDataCore.GetImageBytes(context.GetBinData(sourcePath));

            if ((imageBytes == null) || (imageBytes.Length == 0))
                return isExternalImage;

            shape.SetShapeAttrInternal(ShapeAttr.FillImageBytes, imageBytes);
            return true;
        }

        private static readonly char[] gConnectLocsSplitChars = new char[] { ',', ';' };

        /// <summary>
        /// Specifies maximum allowed nesting level for group shapes.
        /// </summary>
        private const int MaxGroupNestingLevel = 500;
    }
}
