// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2007 by Vladimir Averkin
using System.Drawing;
using System.Text;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Ole.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Vml
{
    /// <summary>
    /// Writes shape to WordML.
    /// This class is static.
    /// TODO 2 Refactor this class, get rid of 'props' hashtable and put all shape sub elements writers into separate classes. See VmlDiagramWriter for example.
    /// </summary>
    internal static class VmlShapeWriter
    {
        /// <summary>
        /// RK I think this method does not write the closing XML tag!
        /// </summary>
        internal static void Write(ShapeBase shape, NrxXmlBuilder builder, IVmlShapeWriterContext context)
        {
            Write(shape, false, builder, context);
        }

        /// <summary>
        /// RK I think this method does not write the closing XML tag!
        ///
        /// RK This needs some serious refactoring!
        /// </summary>
        internal static void Write(ShapeBase shape, bool isBackGround, NrxXmlBuilder builder, IVmlShapeWriterContext context)
        {
            string shapeElementName = "v:shape";
            string shapeTypeId = null;
            bool needShapeTypeXml = false;

            switch (shape.ShapeType)
            {
                case ShapeType.Group:
                    shapeElementName = "v:group";
                    break;
                case ShapeType.NonPrimitive:
                    // TODO 3 Sometimes NonPrimitive is output as polyline and sometimes as shape (see TestNonPrimitiveShape.doc).
                    // Don't know how to distinguish between them yet.
                    shapeElementName = "v:shape";
                    break;
                case ShapeType.Rectangle:
                    shapeElementName = "v:rect";
                    break;
                case ShapeType.RoundRectangle:
                    shapeElementName = "v:roundrect";
                    break;
                case ShapeType.Ellipse:
                    shapeElementName = "v:oval";
                    break;
                case ShapeType.Line:
                    shapeElementName = "v:line";
                    break;
                case ShapeType.OleControl:
                {
                    // Word exports Forms2OleControls as Image shape type.
                    IEmbeddedObject embeddedObject = shape.EmbeddedObject;
                    ShapeType shapeType = ((embeddedObject != null) && embeddedObject.IsForms2OleControlInternal)
                        ? ShapeType.Image
                        : shape.ShapeType;
                    shapeTypeId = string.Format("#_x0000_t{0}", (int)shapeType);
                    needShapeTypeXml = true;
                    break;
                }
                case ShapeType.OleObject:
                {
                    // This shape type in the model uses the Image shape type in the document.
                    shapeTypeId = string.Format("#_x0000_t{0}", (int)ShapeType.Image);
                    needShapeTypeXml = true;
                    break;
                }
                default:
                {
                    shapeTypeId = string.Format("#_x0000_t{0}", (int)shape.ShapeType);
                    needShapeTypeXml = true;
                    break;
                }
            }

            // Other predefined shapes from VML spec are arc, image and curve. They are defined in the schema but not seen in the existing tests.

            if (isBackGround)
                shapeElementName = "v:background";

            VmlFillWriter fillWriter = new VmlFillWriter(shape, builder, context);
            VmlStrokeWriter strokeWriter = new VmlStrokeWriter(builder, context);
            VmlImagedataWriter imagedataWriter = new VmlImagedataWriter(shape, builder, context);
            VmlShadowWriter shadowWriter = new VmlShadowWriter(builder, context);
            VmlExtrusionWriter extrusionWriter = new VmlExtrusionWriter(builder, context);
            VmlGeoWriter geoWriter = new VmlGeoWriter(shape, builder);
            VmlGeoTextWriter geoTextWriter = new VmlGeoTextWriter(builder);
            VmlLockWriter lockWriter = new VmlLockWriter(builder);
            VmlDiagramWriter diagramWriter = new VmlDiagramWriter(context, builder);
            VmlCalloutWriter calloutWriter = new VmlCalloutWriter(builder);
            VmlSignatureLinesWriter signWriter = new VmlSignatureLinesWriter(builder);

            string diagramLayout = null;
            string diagramNodeKind = null;

            // Collect existing attributes.
            IntToObjDictionary<object> props = new IntToObjDictionary<object>();

            ShapePr shapePr = shape.ShapePr;
            for (int i = 0; i < shapePr.Count; i++)
            {
                int key = shapePr.GetKey(i);
                object value = shapePr.GetByIndex(i);

                // Perform check what sub-elements of shape should be written.
                switch (key & ShapeAttrCategory.CategoryMask)
                {
                    case ShapeAttrCategory.Fill:
                        fillWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.Line:
                        if (key == ShapeAttr.LineArrowHeadsOK)
                            geoWriter.AddAttribute(key, value);
                        else
                            strokeWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.Blip:
                        imagedataWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.Shadow:
                        shadowWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.ThreeDObject:
                    case ShapeAttrCategory.ThreeDStyle:
                        extrusionWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.Geometry:
                        geoWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.GeoText:
                        geoTextWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.Protection:
                        lockWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.Diagram:
                        diagramWriter.AddAttribute(key, value);
                        break;
                    case ShapeAttrCategory.Callout:
                        calloutWriter.AddAttribute(key, value);
                        break;
                    default:
                    {
                        // All other attribute categories are handled to here. Act on the attribute key.
                        switch (key)
                        {
                            case ShapeAttr.FillImageBytes:
                                fillWriter.AddAttribute(key, value);
                                break;
                            case ShapeAttr.LineImageBytes:
                                strokeWriter.AddAttribute(key, value);
                                break;
                            case ShapeAttr.ImageBytes:
                            case ShapeAttr.ImageSourceFullName:
                            case ShapeAttr.ImageTitle:
                                imagedataWriter.AddAttribute(key, value);
                                break;
                            case ShapeAttr.LockShapeType:
                                lockWriter.AddAttribute(key, value);
                                break;
                            case ShapeAttr.BWMode:
                            case ShapeAttr.BWNormal:
                            case ShapeAttr.BWPure:
                                props[key] = VmlEnum.BWModeToVml((BWMode)value);
                                break;
                            case ShapeAttr.DiagramNodeLayout:
                                diagramLayout = FormatterPal.IntToXml((int)value);
                                break;
                            case ShapeAttr.DiagramNodeKind:
                                diagramNodeKind = FormatterPal.IntToXml((int)value);
                                break;
                            case ShapeAttr.CoordOriginX:
                            case ShapeAttr.CoordOriginY:
                            case ShapeAttr.CoordSizeWidth:
                            case ShapeAttr.CoordSizeHeight:
                                props[key] = value;
                                break;
                            case ShapeAttr.Flip:
                                props[key] = VmlEnum.FlipOrientationToVml((FlipOrientation)value);
                                break;
                            case ShapeAttr.HorizontalAlignment:
                            {
                                HorizontalAlignment ha = (HorizontalAlignment)value;
                                if (ha != HorizontalAlignment.None)
                                    props[key] = StyleConvertUtil.HorizontalAlignmentToXml(ha);
                                break;
                            }
                            case ShapeAttr.RelativeHorizontalPosition:
                            {
                                RelativeHorizontalPosition rhp = (RelativeHorizontalPosition)value;
                                if (rhp != RelativeHorizontalPosition.Column)
                                    props[key] = StyleConvertUtil.RelativeHorizontalPositionToXml(rhp);
                                break;
                            }
                            case ShapeAttr.VerticalAlignment:
                            {
                                VerticalAlignment va = (VerticalAlignment)value;
                                if (va != VerticalAlignment.None)
                                    props[key] = StyleConvertUtil.VerticalAlignmentToXml(va);
                                break;
                            }
                            case ShapeAttr.RelativeVerticalPosition:
                            {
                                RelativeVerticalPosition rvp = (RelativeVerticalPosition)value;
                                if (rvp != RelativeVerticalPosition.Paragraph)
                                    props[key] = StyleConvertUtil.RelativeVerticalPositionToXml(rvp);
                                break;
                            }
                            case ShapeAttr.DistanceBottom:
                            case ShapeAttr.DistanceLeft:
                            case ShapeAttr.DistanceRight:
                            case ShapeAttr.DistanceTop:
                                props[key] = ConvertUtilCore.EmuToPoint((int)value);
                                break;
                            case ShapeAttr.ConnectorType:
                                props[key] = VmlEnum.ConnectorTypeToVml((ConnectorType)value);
                                break;
                            case ShapeAttr.TextboxWrapMode:
                                props[key] = VmlEnum.TextboxWrapModeToVml((TextBoxWrapMode)value);
                                break;
                            case ShapeAttr.TextboxAnchor:
                                props[key] = VmlEnum.TextboxAnchorToVml((TextBoxAnchor)value);
                                break;
                            case ShapeAttr.TextboxFitShapeToText:
                                if ((bool)value)
                                    props[key] = "t";
                                break;
                            case ShapeAttr.HRAlign:
                            {
                                HorizontalRuleAlignment horizontalRuleAlignment = (HorizontalRuleAlignment)value;
                                if (horizontalRuleAlignment != HorizontalRuleAlignment.Left)
                                    props[key] = VmlEnum.HRAlignToVml(horizontalRuleAlignment);
                                break;
                            }

                            case ShapeAttr.LeftPercent:
                            case ShapeAttr.TopPercent:
                            case ShapeAttr.WidthPercent:
                            case ShapeAttr.HeightPercent:
                                props[key] = FormatterPal.IntToStr((int)value);
                                break;
                            case ShapeAttr.RelativeWidth:
                                props[key] = StyleConvertUtil.RelativeWidthToXml((RelativeHorizontalSize)value);
                                break;
                            case ShapeAttr.RelativeHeight:
                                props[key] = StyleConvertUtil.RelativeHeightToXml((RelativeVerticalSize)value);
                                break;
                            case ShapeAttr.SigSetupAddlXml:
                            case ShapeAttr.SigSetupAllowComments:
                            case ShapeAttr.SigSetupId:
                            case ShapeAttr.IsSignatureLine:
                            case ShapeAttr.SigSetupProvId:
                            case ShapeAttr.SigSetupShowSignDate:
                            case ShapeAttr.SigSetupSignInst:
                            case ShapeAttr.SigSetupProvUrl:
                            case ShapeAttr.SigSetupSuggSigner:
                            case ShapeAttr.SigSetupSuggSigner2:
                            case ShapeAttr.SigSetupSuggSignerEmail:
                            case ShapeAttr.SigSetupSignInstSet:
                                signWriter.AddAttribute(key, value);
                                break;
                            default:
                                props[key] = value;
                                break;
                        }
                        break;
                    }
                }
            }

            // RK Here writing begins.

            if (!context.IsDocx && (props[ShapeAttr.ScriptAnchor] != null) && (bool)props[ShapeAttr.ScriptAnchor])
            {
                string scriptLanguageName;

                if (props[ShapeAttr.ScriptLanguageName] != null)
                    scriptLanguageName = props[ShapeAttr.ScriptLanguageName] as string;
                else
                    scriptLanguageName = VmlEnum.ScriptLanguageToVml(props[ShapeAttr.ScriptLanguage]);

                builder.StartElement("w:scriptAnchor");
                builder.WriteElement("w:language", scriptLanguageName);
                builder.WriteElement("w:args", props[ShapeAttr.ScriptType] as string);
                builder.WriteElement("w:scriptText", props[ShapeAttr.ScriptText] as string);

                // RK Note we exit here.
                return;
            }

            // Write shape type definition from Resources\ShapeTypes.txt.
            if (needShapeTypeXml && !context.ShapeTypesWritten.Contains(shapeTypeId))
            {
                context.ShapeTypesWritten.Add(shapeTypeId);
                // shapeTypeId string has format "#_x0000_t{0}"
                string shapeTypeXml = ShapeTypeLibrary.GetShapeTypeXml(shapeTypeId);
                if (StringUtil.HasChars(shapeTypeXml))
                {
                    builder.WriteRaw("\r\n");
                    builder.WriteRaw(shapeTypeXml);
                    builder.WriteRaw("\r\n");
                }
            }

            if (imagedataWriter.ImageBytes != null)
            {
                imagedataWriter.ImageName = context.WriteImageBinData(imagedataWriter.ImageBytes);

                // andrnosk: WORDSNET-6431 If SourceFullName is an empty string or null, the image is not linked.
                if (StringUtil.HasChars(imagedataWriter.ImageSourceFullName) && (imagedataWriter.ImageSourceFullName != imagedataWriter.ImageName))
                    imagedataWriter.ImageSourceFullName = context.WriteImageLink(imagedataWriter.ImageSourceFullName);
            }
            else if (StringUtil.HasChars(imagedataWriter.ImageSourceFullName))
                imagedataWriter.ImageName = context.WriteImageLink(imagedataWriter.ImageSourceFullName);

            if (fillWriter.ImageBytes != null)
                fillWriter.ImageName = context.WriteImageBinData(fillWriter.ImageBytes);

            if (strokeWriter.ImageBytes != null)
                strokeWriter.ImageName = context.WriteImageBinData(strokeWriter.ImageBytes);

            VmlBuilder vmlBuilder = new VmlBuilder(builder);
            builder.StartElement(shapeElementName);

            if (shape.ShapeType == ShapeType.CustomShape)
            {
                // RK This seems to be what MS Word is writing for custom shapes.
                string shapeName = (string)props[ShapeAttr.ShapeName];
                if (StringUtil.HasChars(shapeName))
                {
                    vmlBuilder.WriteVmlAttribute("id", shapeName);
                    vmlBuilder.WriteVmlAttribute("o:spid", NrxXmlUtil.GetShapeId(shape));
                }
                else
                {
                    vmlBuilder.WriteVmlAttribute("id", NrxXmlUtil.GetShapeId(shape));
                }

                // If the following attributes are null, then we assign some default values to them.
                if (strokeWriter.LineJoinStyle == null)
                    strokeWriter.LineJoinStyle = "miter";

                // RK Custom shapes seem to always output coord size in MS Word.
                if (props[ShapeAttr.CoordSizeWidth] == null)
                    props[ShapeAttr.CoordSizeWidth] = shape.CoordSizeWidth;
                if (props[ShapeAttr.CoordSizeHeight] == null)
                    props[ShapeAttr.CoordSizeHeight] = shape.CoordSizeHeight;
            }
            else if (StringUtil.HasChars((string)props[ShapeAttr.ShapeName]) && !context.SaveInfo.LinkedShapeNameConflict)
            {
                // RK Shape has name. In this case it is written is VML shape id
                // and the shape id from the model is written as VML shape spid.
                // WORDSNET-11823 If shape is OLE control, despite this shape has name
                // MS Word writes shape id to correctly link shape with OLE control.
                if (shape.IsOleControl)
                    vmlBuilder.WriteVmlAttribute("id", NrxXmlUtil.GetShapeId(shape));
                else
                {
                    string shapeName = (string)props[ShapeAttr.ShapeName];
                    vmlBuilder.WriteVmlAttribute("id", shapeName);
                    vmlBuilder.WriteVmlAttribute("o:spid", NrxXmlUtil.GetShapeId(shape));
                }
                vmlBuilder.WriteVmlAttribute("type", shapeTypeId);
            }
            else
            {
                // RK Shape has no name. Shape id is written as VML shape id.
                string shapeName = NrxXmlUtil.GetShapeId(shape);

                vmlBuilder.WriteVmlAttribute("id", shapeName);
                vmlBuilder.WriteVmlAttribute("type", shapeTypeId);
            }

            // WORDSNET-19028 We need write 'editas' if it present, the same as word.
            if (StringUtil.HasChars(diagramWriter.EditAs))
                vmlBuilder.WriteVmlAttribute("editas", diagramWriter.EditAs);

            // WORDSNET-16036 Write coordsize if only one dimension (width or height) is defined.
            if ((props[ShapeAttr.CoordSizeWidth] == null) && (props[ShapeAttr.CoordSizeHeight] != null))
                props[ShapeAttr.CoordSizeWidth] = shape.CoordSizeWidth;
            else if ((props[ShapeAttr.CoordSizeWidth] != null) && (props[ShapeAttr.CoordSizeHeight] == null))
                props[ShapeAttr.CoordSizeHeight] = shape.CoordSizeHeight;

            string altText = ShapeBase.GenerateAltText((string)props[ShapeAttr.ShapeTitle], (string)props[ShapeAttr.ShapeDescription]);


            if (!(shape.IsInline && shape.IsImage))
            {
                // These attributes are not written for inline pictures.
                // Inline pictures are enclosed in hyperlink field instead.
                vmlBuilder.WriteVmlAttribute("href", props[ShapeAttr.HyperlinkAddress]);
                vmlBuilder.WriteVmlAttribute("target", props[ShapeAttr.HyperlinkTarget]);
                vmlBuilder.WriteVmlAttribute("title", props[ShapeAttr.ScreenTip]);
            }

            // andrnosk: WORDSNET-6836 Preserve shape description. Previously we preserve shape description
            // only if inline shape is signature line, but according to MS Word behavior we need to preserve it always.
            if (StringUtil.HasChars(altText))
                vmlBuilder.WriteVmlAttribute("alt", altText);

            // Style attribute can have the following CSS attributes:
            //   position
            //   text-align: left
            //   top | margin-top
            //   left | margin-left
            //   width
            //   height
            //   rotation
            //   flip
            //   visibility - never seen in WordML
            //   z-index
            //   mso-wrap-distance-bottom
            //   mso-wrap-distance-left
            //   mso-wrap-distance-right
            //   mso-wrap-distance-top
            //   mso-position-horizontal
            //   mso-position-horizontal-relative
            //   mso-position-vertical
            //   mso-position-vertical-relative
            //   mso-wrap-style: none, square (TextboxWrapMode)
            //   v-text-anchor: bottom (TextboxAnchor)

            if (!isBackGround)
            {
                VmlCssStyleBuilder style = new VmlCssStyleBuilder();

                // Mimic MS Word behavior and write only "width"/"height" for picture bullet shape.
                if (shape.IsPictureBullet)
                {
                    style.Add("width", VmlUtil.DistanceToVml(shape.Width, shape.IsTopLevel));
                    style.Add("height", VmlUtil.DistanceToVml(shape.Height, shape.IsTopLevel));
                }
                else
                {
                    if (!shape.IsInline)
                        style.Add("position", "absolute");

                    // Top level distances seem to be always written in points.
                    // Nested shape distances seem to be always written without measurement units, they are relative to coordinates defined in containing shape.
                    string marginPrefix = "";

                    if (shape.IsTopLevel)
                        marginPrefix = "margin-";

                    if (shape.ShapeType != ShapeType.Line)
                    {
                        if (!shape.IsInline)
                        {
                            if (shape.IsTopLevel || shape.Left != 0)
                                style.Add(marginPrefix + "left", VmlUtil.DistanceToVml(shape.Left, shape.IsTopLevel));

                            if (shape.IsTopLevel || shape.Top != 0)
                                style.Add(marginPrefix + "top", VmlUtil.DistanceToVml(shape.Top, shape.IsTopLevel));
                        }

                        style.Add("width", VmlUtil.DistanceToVml(shape.Width, shape.IsTopLevel));
                        style.Add("height", VmlUtil.DistanceToVml(shape.Height, shape.IsTopLevel));
                    }

                    object rotationObj = props[ShapeAttr.TransformRotation];
                    if (rotationObj != null)
                    {
                        int rotation = (int)rotationObj;
                        if (rotation != 0)
                            style.Add("rotation", VmlUtil.FixedDegreesToVml(rotation));
                    }

                    if (StringUtil.HasChars((string)props[ShapeAttr.Flip]))
                        style.Add("flip", (string)props[ShapeAttr.Flip]);

                    if (shape.IsTopLevel && !shape.IsInline)
                        style.Add("z-index", context.SaveInfo.GetShapeZIndex(shape));

                    if (props[ShapeAttr.Hidden] != null)
                    {
                        // Occurs in Demo2007\NewsletterFamily.doc and Demo2007\NewsletterTravel.doc.
                        style.Add("visibility", (bool)props[ShapeAttr.Hidden] ? "hidden" : "visible");
                    }

                    if (props[ShapeAttr.EditedWrap] != null)
                    {
                        // Only 'false' values are written.
                        style.Add("mso-wrap-edited", (bool)props[ShapeAttr.EditedWrap] ? "" : "f");
                    }

                    style.Add("mso-wrap-distance-left",
                        VmlUtil.DistanceToVml(props[ShapeAttr.DistanceLeft], shape.IsTopLevel));
                    style.Add("mso-wrap-distance-top",
                        VmlUtil.DistanceToVml(props[ShapeAttr.DistanceTop], shape.IsTopLevel));
                    style.Add("mso-wrap-distance-right",
                        VmlUtil.DistanceToVml(props[ShapeAttr.DistanceRight], shape.IsTopLevel));
                    style.Add("mso-wrap-distance-bottom",
                        VmlUtil.DistanceToVml(props[ShapeAttr.DistanceBottom], shape.IsTopLevel));

                    style.Add("mso-position-horizontal", (string)props[ShapeAttr.HorizontalAlignment]);
                    style.Add("mso-position-horizontal-relative", (string)props[ShapeAttr.RelativeHorizontalPosition]);
                    style.Add("mso-position-vertical", (string)props[ShapeAttr.VerticalAlignment]);
                    style.Add("mso-position-vertical-relative", (string)props[ShapeAttr.RelativeVerticalPosition]);

                    // These are Word 2007 only attributes.
                    if (context.IsDocx)
                    {
                        style.Add("mso-left-percent", (string)props[ShapeAttr.LeftPercent]);
                        style.Add("mso-top-percent", (string)props[ShapeAttr.TopPercent]);
                        style.Add("mso-width-percent", (string)props[ShapeAttr.WidthPercent]);
                        style.Add("mso-height-percent", (string)props[ShapeAttr.HeightPercent]);
                        style.Add("mso-width-relative", (string)props[ShapeAttr.RelativeWidth]);
                        style.Add("mso-height-relative", (string)props[ShapeAttr.RelativeHeight]);
                    }

                    style.Add("mso-wrap-style", (string)props[ShapeAttr.TextboxWrapMode]);
                    style.Add("v-text-anchor", (string)props[ShapeAttr.TextboxAnchor]);
                }

                builder.WriteAttribute("style", style.ToCss());
            }

            if(props[ShapeAttr.EquationXML] != null)
            {
                builder.WriteAttribute("equationxml", Encoding.UTF8.GetString((byte[])props[ShapeAttr.EquationXML]));
            }

            builder.WriteAttribute("w:themeColor", props[ShapeAttr.ThemeColor]);
            builder.WriteAttribute("w:themeShade", props[ShapeAttr.ThemeShade]);
            builder.WriteAttribute("w:themeTint", props[ShapeAttr.ThemeTint]);

            builder.WriteAttribute("o:bwmode", props[ShapeAttr.BWMode]);
            builder.WriteAttribute("o:bwpure", props[ShapeAttr.BWPure]);
            builder.WriteAttribute("o:bwnormal", props[ShapeAttr.BWNormal]);

            if(!isBackGround)
                vmlBuilder.WriteVmlAttribute("o:oleicon", props[ShapeAttr.OleIcon]);

            if (shape.IsInline && shape.IsOle)
                builder.WriteAttributeString("o:ole", "");

            if (shape.IsInline)
            {
                builder.WriteAttribute("o:bordertopcolor",
                    VmlUtil.BorderToVmlColor((Border)props[ShapeAttr.BorderTop], context));
                builder.WriteAttribute("o:borderleftcolor",
                    VmlUtil.BorderToVmlColor((Border)props[ShapeAttr.BorderLeft], context));
                builder.WriteAttribute("o:borderbottomcolor",
                    VmlUtil.BorderToVmlColor((Border)props[ShapeAttr.BorderBottom], context));
                builder.WriteAttribute("o:borderrightcolor",
                    VmlUtil.BorderToVmlColor((Border)props[ShapeAttr.BorderRight], context));
            }

            // write attributes for predefined shapes
            if (shape.ShapeType == ShapeType.Line)
                WriteLineCoordinates(shape, vmlBuilder);

            if (shape.ShapeType == ShapeType.RoundRectangle)
                vmlBuilder.WriteVmlAttribute("arcsize", VmlUtil.FixedToVml(Adj1ToArcSize(shape)));

            // RK Is it supposed to be written always?
            WritePoint(builder, props, ShapeAttr.CoordOriginX, ShapeAttr.CoordOriginY, "coordorigin");
            WritePoint(builder, props, ShapeAttr.CoordSizeWidth, ShapeAttr.CoordSizeHeight, "coordsize");

            vmlBuilder.WriteVmlAttributeIfNotDefault("o:allowincell", props[ShapeAttr.AllowInCell], true);
            vmlBuilder.WriteVmlAttributeIfNotDefault("o:allowoverlap", props[ShapeAttr.AllowOverlap], true);

            if (shape.ShapeType == ShapeType.CustomShape)
                vmlBuilder.WriteVmlAttribute("o:spt", "100");

            if (diagramLayout != null)
                builder.WriteAttribute("o:dgmlayout", diagramLayout);

            if (diagramNodeKind != null)
                builder.WriteAttribute("o:dgmnodekind", diagramNodeKind);

            vmlBuilder.WriteVmlAttribute("o:connectortype", props[ShapeAttr.ConnectorType]);

            geoWriter.WriteAdjAttribute();

            geoWriter.WritePathAttribute();

            vmlBuilder.WriteVmlAttribute("o:preferrelative", props[ShapeAttr.PreferRelativeResize]);
            vmlBuilder.WriteVmlAttribute("o:button", props[ShapeAttr.Button]);

            // VA: MS Word does not write o:hrheight and o:hrwidth values to WordML. So I won't either.
            // RK: I agree. In fact there HRWidth and HRHeight attributes are no longer in the model,
            // the height and width of the horizontal rule is determined by the shape height and width.
            vmlBuilder.WriteVmlAttribute("o:hrpct", props[ShapeAttr.HRPct]);
            vmlBuilder.WriteVmlAttribute("o:hralign", props[ShapeAttr.HRAlign]);
            vmlBuilder.WriteVmlAttribute("o:hrstd", props[ShapeAttr.HRStandard]);
            vmlBuilder.WriteVmlAttribute("o:hrnoshade", props[ShapeAttr.HRNoShade]);
            vmlBuilder.WriteVmlAttribute("o:hr", props[ShapeAttr.HROn]);

            vmlBuilder.WriteVmlAttribute("o:bullet", props[ShapeAttr.PictureBullet]);

            WriteWrapPolygon(builder, (PathPoint[])props[ShapeAttr.WrapPolygonVertices]);

            fillWriter.WriteAttributes();

            if (!isBackGround)
                strokeWriter.WriteAttributes();

            // Write 'v:fill' element.
            fillWriter.Write();

            // Write 'v:stroke' element.
            strokeWriter.Write();

            // Write 'v:imagedata' element.
            imagedataWriter.Write();

            // Write 'v:shadow' element.
            shadowWriter.Write();

            // Write 'o:extrusion' element.
            extrusionWriter.Write();

            // andrnosk: WORDSNET-7738 v:path should be written for v:shape and for v:rect too.
            if ((shapeElementName == "v:shape") || (shapeElementName == "v:rect"))
            {
                geoWriter.WriteFormulas();
                geoWriter.WritePath();
                geoTextWriter.Write();
                geoWriter.WriteHandles();
            }

            // Write 'o:lock' element.
            if (!isBackGround)
                lockWriter.Write();

            // Write 'o:ink' element.
            if (shape.InkData != null)
            {
                builder.StartElement("o:ink");
                builder.WriteAttribute("i", shape.InkData);
                vmlBuilder.WriteVmlAttribute("annotation", shape.InkAnnotation);
                builder.EndElement();
            }

            // Write 'o:signatureline' element.
            signWriter.Write();

            // Write 'o:diagram' element.
            diagramWriter.WriteDiagramElement();

            // Write 'o:callout' element.
            calloutWriter.Write();

            WriteW10Border("w10:bordertop", props[ShapeAttr.BorderTop], builder);
            WriteW10Border("w10:borderleft", props[ShapeAttr.BorderLeft], builder);
            WriteW10Border("w10:borderbottom", props[ShapeAttr.BorderBottom], builder);
            WriteW10Border("w10:borderright", props[ShapeAttr.BorderRight], builder);

            bool isShapeLinked = context.SaveInfo.LinkedShapeIds.Contains(shape.Id);

            if (!shape.IsGroup && (shape.HasChildNodes || isShapeLinked))
            {
                builder.StartElement("v:textbox");

                VmlCssStyleBuilder textboxStyle = new VmlCssStyleBuilder();

                if (props.ContainsKey(ShapeAttr.TextboxLayoutFlow))
                {
                    LayoutFlow layoutFlow = (LayoutFlow)props[ShapeAttr.TextboxLayoutFlow];
                    if (layoutFlow == LayoutFlow.BottomToTop)
                    {
                        textboxStyle.Add("layout-flow", "vertical");
                        textboxStyle.Add("mso-layout-flow-alt", "bottom-to-top");
                    }
                    else
                    {
                        textboxStyle.Add("layout-flow", VmlEnum.LayoutFlowToVml(layoutFlow));
                    }
                }

                int nextShapeId = shape.TextboxNextShapeId;
                if (nextShapeId != 0)
                {
                    ShapeBase nextShape = context.SaveInfo.Shapes[nextShapeId];

                    string link = StringUtil.HasChars(nextShape.Name) && !context.SaveInfo.LinkedShapeNameConflict
                                      ? string.Format("#{0}", nextShape.Name)
                                      : string.Format("#{0}", NrxXmlUtil.GetShapeId(nextShape));

                    textboxStyle.Add("mso-next-textbox", link);
                }

                textboxStyle.Add("mso-fit-shape-to-text", (string)props[ShapeAttr.TextboxFitShapeToText]);
                vmlBuilder.WriteVmlAttribute("style", textboxStyle.ToCss());
                vmlBuilder.WriteVmlAttribute("inset", VmlUtil.GetInset(props));

                builder.StartElement("w:txbxContent");

                if (!shape.HasChildNodes && isShapeLinked)
                {
                    builder.EndElement("w:txbxContent");
                    builder.EndElement("v:textbox");
                }
            }
        }

        /// <summary>
        /// Write 'wrapcoords' attribute of 'w:shape' element.
        /// </summary>
        private static void WriteWrapPolygon(NrxXmlBuilder builder, PathPoint[] wrapCoords)
        {
            if ((wrapCoords != null) && (wrapCoords.Length > 0))
                builder.WriteAttribute("wrapcoords", VmlUtil.PathPointsToVml(wrapCoords, ' ', ' '));
        }

        private static void WritePoint(NrxXmlBuilder builder, IntToObjDictionary<object> props, int keyX, int keyY, string attrName)
        {
            object x = props[keyX];
            object y = props[keyY];
            if ((x != null) && (y != null))
            {
                VmlBuilder vmlBuilder = new VmlBuilder(builder);
                vmlBuilder.WriteVmlAttribute(attrName, VmlUtil.PointToVml(new Point((int)x, (int)y)));
            }
        }

        internal static void WriteWrapAndAnchorLock(ShapeBase shape, NrxXmlBuilder builder)
        {
            ShapePr shapePr = shape.ShapePr;

            string wrapType = shapePr.Contains(ShapeAttr.WrapType) ?
                VmlEnum.WrapTypeToVml(shape.WrapType) : "";

            if (shape.ShapePr.Contains(ShapeAttr.WrapType))
            {
                VmlBuilder vmlBuilder = new VmlBuilder(builder);
                string relativeHorizontalPosition = StyleConvertUtil.RelativeHorizontalPositionToXml(shape.RelativeHorizontalPosition);

                if (relativeHorizontalPosition == "text")
                    relativeHorizontalPosition = "";

                string relativeVerticalPosition = StyleConvertUtil.RelativeVerticalPositionToXml(shape.RelativeVerticalPosition);

                if (relativeVerticalPosition == "text")
                    relativeVerticalPosition = "";

                string wrapSide = shapePr.Contains(ShapeAttr.WrapSide) ?
                    VmlEnum.WrapSideToVml(shape.WrapSide) : "";

                if (shape.IsInline)
                {
                    if ((relativeHorizontalPosition == "char") && (relativeVerticalPosition == "line") && StringUtil.HasChars(wrapType))
                    {
                        builder.StartElement("w10:wrap");
                        vmlBuilder.WriteVmlAttribute("type", wrapType);
                        builder.EndElement(); //w10:wrap
                    }
                }
                else if (StringUtil.HasChars(wrapType))
                {
                    builder.StartElement("w10:wrap");
                    vmlBuilder.WriteVmlAttribute("type", wrapType);
                    vmlBuilder.WriteVmlAttribute("side", wrapSide);
                    builder.EndElement(); //w10:wrap
                }
                else if (StringUtil.HasChars(relativeHorizontalPosition) || StringUtil.HasChars(relativeVerticalPosition))
                {
                    bool writeWrapSide = StringUtil.HasChars(wrapSide);
                    bool writeAnchorX = IsSupportedPropertyAndNotDefault(relativeHorizontalPosition);
                    bool writeAnchorY = IsSupportedPropertyAndNotDefault(relativeVerticalPosition);

                    // Skip writing an empty element.
                    if (writeWrapSide || writeAnchorX || writeAnchorY)
                    {
                        builder.StartElement("w10:wrap");
                        vmlBuilder.WriteVmlAttribute("side", wrapSide);

                        // Do not write not supported and default values.
                        if (writeAnchorX)
                            vmlBuilder.WriteVmlAttribute("anchorx", relativeHorizontalPosition);
                        if (writeAnchorY)
                            vmlBuilder.WriteVmlAttribute("anchory", relativeVerticalPosition);
                        builder.EndElement(); //w10:wrap
                    }
                }
            }

            if (shape.AnchorLocked)
                builder.WriteEmptyElement("w10:anchorlock");
        }

        /// <summary>
        /// According to spec, default value for anchorx and anchory is 'page'.
        /// So we can omit writing these attributes if they have default value.
        /// </summary>
        private static bool IsSupportedPropertyAndNotDefault(string value)
        {
            return (IsSupportedProperty(value) && (value != "page"));
        }

        /// <summary>
        /// Word supports only these props (margin, page, text) for anchors.
        /// See "[MS-OI29500] 2.1.1827 Part 4 Section 14.3.3.3, ST_HorizontalAnchor (Horizontal Anchor Type), 2.1.1828 Part 4 Section 14.3.3.4, ST_VerticalAnchor (Vertical Anchor Type)",
        /// "part 4 transitional c059578_ISO_IEC_29500-4_2011 19.3.3.3 ST_HorizontalAnchor (Horizontal Anchor Type), 19.3.3.4 ST_VerticalAnchor (Vertical Anchor Type)".
        /// </summary>
        private static bool IsSupportedProperty(string value)
        {
            return ((value == "margin") || (value == "page") || (value == "text"));
        }

        private static void WriteW10Border(string borderName, object borderAttr, NrxXmlBuilder builder)
        {
            if (borderAttr is Border)
            {
                Border border = (Border)borderAttr;

                if (border.LineStyle != LineStyle.None)
                {
                    builder.StartElement(borderName);
                    builder.WriteAttribute("type", VmlEnum.LineStyleToVml(border.LineStyle));
                    builder.WriteAttribute("width", FormatterPal.DoubleToStr2Decimals(border.LineWidth * 8));
                    builder.WriteAttribute("shadow", VmlUtil.BoolToVml(border.Shadow, false));
                    builder.EndElement();
                }
            }
        }

        private static int Adj1ToArcSize(ShapeBase shape)
        {
            object adj1Obj = shape.GetDirectShapeAttrInternal(ShapeAttr.GeometryAdjust1);
            if (adj1Obj != null)
            {
                // In the model arcsize is stored in adj1. In VML it is stored as percentage.
                int adj1 = (int)adj1Obj;
                double percent = (double)adj1 / shape.CoordSizeWidth;
                return ConvertUtilCore.DoubleToFixed(percent);
            }
            else
            {
                const int DefaultArcSize = 10923;
                return DefaultArcSize;
            }
        }

        /// <summary>
        /// Writes line coordinates of start and end points.
        /// </summary>
        private static void WriteLineCoordinates(ShapeBase shape, VmlBuilder builder)
        {
            Debug.Assert((shape != null) && (builder != null));

            double[] vertices =
                VmlUtil.GetLineVertices(shape, shape.Top, shape.Left, shape.Width, shape.Height);

            double fromX = (vertices.Length > 0) ? vertices[0] : shape.Left;
            double fromY = (vertices.Length > 0) ? vertices[1] : shape.Top;
            double toX = (vertices.Length > 0) ? vertices[2] : shape.Left + shape.Width;
            double toY = (vertices.Length > 0) ? vertices[3] : shape.Top + shape.Height;

            builder.WriteVmlAttribute("from", VmlUtil.PointToVml(fromX, fromY, shape.IsTopLevel));
            builder.WriteVmlAttribute("to", VmlUtil.PointToVml(toX, toY, shape.IsTopLevel));
        }
    }
}
