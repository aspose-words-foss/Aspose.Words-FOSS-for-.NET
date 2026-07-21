// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/06/2016 by Alexander Zhiltsov

using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;
using Aspose.Words.RW.Vml;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// This class allows reading the 17.2.1 background (Document Background) element.
    /// </summary>
    internal static class DocxBackgroundReader
    {
        /// <summary>
        /// Reads a DOCX 'w:background' element from the specified reader.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DrColor color = null;
            string themeColor = null;
            string themeShade = null;
            string themeTint = null;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "color": // w:color
                        {
                            color = VmlColor.VmlToColor(xmlReader.Value);
                            // RK Default is white. DOC files do not have this attribute when the color is white.
                            // Let's pretend we don't have it too to make gold ExportImport tests pass.
                            if (color == DrColor.White)
                                color = null;
                            break;
                        }
                    case "themeColor":
                        themeColor = xmlReader.Value;
                        break;
                    case "themeShade":
                        themeShade = xmlReader.Value;
                        break;
                    case "themeTint":
                        themeTint = xmlReader.Value;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
            xmlReader.MoveToElement();

            Shape shape = ReadShape(reader);
            if (shape == null)
                shape = CreateDefaultBackgroundShape(reader.Document);

            reader.Document.SetBackgroundShapeSafe(shape);

            if (color != null)
                shape.SetShapeAttrInternal(ShapeAttr.FillColor, color);
            if (themeColor != null)
                shape.SetShapeAttrInternal(ShapeAttr.ThemeColor, themeColor);
            if (themeShade != null)
                shape.SetShapeAttrInternal(ShapeAttr.ThemeShade, themeShade);
            if (themeTint != null)
                shape.SetShapeAttrInternal(ShapeAttr.ThemeTint, themeTint);
        }

        /// <summary>
        /// Creates a VML shape for document background.
        /// </summary>
        private static Shape CreateDefaultBackgroundShape(DocumentBase document)
        {
            return new Shape(document, ShapeType.Rectangle);
        }

        /// <summary>
        /// Reads a background shape.
        /// </summary>
        private static Shape ReadShape(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            Shape shape = null;
            string tagName = xmlReader.LocalName;
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "background": // v:background
                        shape = CreateDefaultBackgroundShape(reader.Document);
                        VmlShapeReader.ReadShape(reader, shape);
                        break;
                    case "AlternateContent":
                        shape = ReadAlternateContent(reader);
                        break;
                    case "drawing":
                        shape = (Shape)((INrxDmlReader)reader).ReadDrawing(new RunPr());
                        reader.Document.RemoveChild(shape); // shape is inserting into document during reading
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
            return shape;
        }

        /// <summary>
        /// Reads an alternate content block. Returns a shape of the choice part.
        /// </summary>
        private static Shape ReadAlternateContent(DocxDocumentReaderBase reader)
        {
            Shape shape = null;
            Shape fallbackShape = null;
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("AlternateContent"))
            {
                // Choice and Fallback, are used to provide alternates for specified content.
                switch (xmlReader.LocalName)
                {
                    case "Choice":
                        shape = ReadChoice(reader);
                        break;
                    case "Fallback":
                        fallbackShape = ReadShape(reader);
                        break;
                    default:
                        xmlReader.IgnoreElement(WarningType.UnexpectedContent, WarningSource.Nrx, xmlReader.LocalName);
                        break;
                }
            }

            if (fallbackShape != null)
            {
                if (shape == null)
                {
                    if (fallbackShape.MarkupLanguage == ShapeMarkupLanguage.Vml)
                        return fallbackShape;
                    shape = CreateDefaultBackgroundShape(reader.Document);
                    shape.RunPr.AlternateContent = new AlternateContent();
                }
                AlternateContent alternateContent = shape.RunPr.AlternateContent;
                Paragraph fallbackContainer = new Paragraph(reader.Document);
                fallbackContainer.AppendChild(fallbackShape);
                alternateContent.FallBack = fallbackContainer;
            }
            else
            {
                if (shape != null)
                    shape.RunPr.Remove(FontAttr.AlternateContent);
            }

            return shape;
        }

        /// <summary>
        /// Reads a shape of the choice part of an alternate content block.
        /// </summary>
        private static Shape ReadChoice(DocxDocumentReaderBase reader)
        {
            bool choiceMatched = false;
            AlternateContent alternateContent = new AlternateContent();
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "Requires":
                        // The Requires attribute specifies a set of space-delimited namespaces that must be 
                        // understood in order to select that choice.
                        string prefix = xmlReader.Value;
                        if (StringUtil.HasChars(prefix) &&
                            NrxRunReaderBase.IsAllowedChoiceRequirement(xmlReader.LookupNamespace(prefix)))
                        {
                            choiceMatched = true;
                            alternateContent.Requires = prefix;
                        }
                        break;
                    default:
                        xmlReader.Warn(WarningType.UnexpectedContent, WarningSource.Nrx, xmlReader.LocalName);
                        break;
                }
            }
            xmlReader.MoveToElement();

            if (choiceMatched)
            {
                Shape shape = ReadShape(reader);
                if (shape == null) // choice may be empty
                    shape = CreateDefaultBackgroundShape(reader.Document);
                shape.RunPr.AlternateContent = alternateContent;
                return shape;
            }
            else
            {
                xmlReader.IgnoreElement();
                return null;
            }
        }
    }
}
