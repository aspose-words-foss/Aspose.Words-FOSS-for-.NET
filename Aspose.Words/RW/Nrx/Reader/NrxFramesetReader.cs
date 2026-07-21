// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2011 by Alexey Morozov

using System;
using Aspose.Common;
using Aspose.Words.Framesets;
using Aspose.Words.Nrx;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Reader
{
    /// <summary>
    /// Implements framed document reading.
    /// </summary>
    internal static class NrxFramesetReader
    {
        /// <summary>
        /// Reads frameset element.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="frameset">Frame object being read.</param>
        /// <param name="isDocx">Indicates that DOCX is being read.</param>
        internal static void Read(NrxDocumentReaderBase reader, Frameset frameset, bool isDocx)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("frameset"))
            {
                switch (xmlReader.LocalName)
                {
                    case "framesetSplitbar":
                        ReadFramesetSplitBar(reader, frameset);
                        break;
                    case "frameLayout":
                        frameset.LayoutType = StyleConvertUtil.NrxToFrameLayoutType(xmlReader.ReadVal());
                        break;
                    case "sz":
                        SetFrameDividerPosition(frameset, xmlReader.ReadVal());
                        break;
                    case "title": 
                        frameset.Title = xmlReader.ReadVal();
                        break;
                    case "frame":
                        Frameset childFrame = new Frameset();
                        frameset.ChildFramesets.Add(childFrame);
                        ReadFrame(reader, childFrame, isDocx);
                        break;
                    case "frameset":
                        Frameset childFrameset = new Frameset();
                        frameset.ChildFramesets.Add(childFrameset);
                        Read(reader, childFrameset, isDocx);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads framesetSplitbar element which specifies frameset border options located in root frame.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="frame"></param>
        private static void ReadFramesetSplitBar(NrxDocumentReaderBase reader, Frameset frame)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            OoxmlComplianceInfo complianceInfo = reader.ComplianceInfo;

            while (xmlReader.ReadChild("framesetSplitbar"))
            {
                switch (xmlReader.LocalName)
                {
                    case "flatBorders":
                    case "noBorder":
                    {
                        bool val = xmlReader.ReadBoolVal();
                        if (val)
                        {
                            // Although I haven't seen these elements with false value, spec says that it's possible.
                            frame.FramesetBorderType = StyleConvertUtil.NrxToFramesetBorderType(xmlReader.LocalName);
                        }
                        break;
                    }
                    case "color":
                        frame.FramesetBorderColor = NrxXmlUtil.XmlToColor(xmlReader.ReadVal());
                        break;
                    case "w":
                        frame.FramesetBorderWidth = xmlReader.ReadValAsTwips(complianceInfo);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads <see cref="Frameset" /> element.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="frame"></param>
        /// <param name="isDocx"></param>
        private static void ReadFrame(NrxDocumentReaderBase reader, Frameset frame, bool isDocx)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("frame"))
            {
                switch (xmlReader.LocalName)
                {
                    case "title":
                        frame.Title = xmlReader.ReadVal();
                        break;
                    case "name":
                        frame.Name = xmlReader.ReadVal();
                        break;
                    case "sourceFileName":
                        frame.SetFrameDefaultUrlInternal((isDocx) 
                            ? reader.GetRelationshipTarget(xmlReader.ReadAttribute("id", null)) 
                            : xmlReader.ReadVal());
                        break;
                    case "scrollbar":
                        SetFrameScrollType(frame, xmlReader.ReadVal());
                        break;
                    case "noResizeAllowed":
                        frame.NoResize = xmlReader.ReadBoolVal();
                        break;
                    case "linkedToFile":
                        frame.SetIsFrameLinkToFile(xmlReader.ReadBoolVal());
                        break;
                    case "sz":
                        SetFrameDividerPosition(frame, xmlReader.ReadVal());
                        break;
                    case "marH":
                        frame.MarginX = xmlReader.ReadIntVal();
                        break;
                    case "marV":
                        frame.MarginY = xmlReader.ReadIntVal();
                        break;
                    case "longDesc":  
                        // Present in the ISO29500 spec as (Frame Long Description) §17.15.2.23, 
                        // but ignored by MS Word as per ISO29500 MS Impl Notes.
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Extract <see cref="FrameScrollType"/> value from string.
        /// </summary>
        /// <returns></returns>
        private static void SetFrameScrollType(Frameset frame, string value)
        {
            switch (value)
            {
                case "on":
                    frame.ScrollType = FrameScrollType.Yes;
                    break;
                case "off":
                    frame.ScrollType = FrameScrollType.No;
                    break;
                case "auto":
                    frame.ScrollType = FrameScrollType.Auto;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Invalid scroll type value {0}.", value));
            }
        }

        /// <summary>
        /// Extracts <see cref="FrameDividerPositionType"/> and FrameDividerPositionValue from string.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="value"></param>
        private static void SetFrameDividerPosition(Frameset frame, string value)
        {
            // MS Impl Notes lays the following restrictions on the values:
            // 17.15.2.40. sz (Frame Size) (a) The standard allows any string for the val attribute.
            // For the val attribute, Word allows an integer between 1 and 32,767 followed by *; an integer between 1 and 500 followed by %
            // or an integer which divided by pixelsPerInch gives a value between 0 and 22.
            // (b) The standard says if this element is omitted, then no information shall be implied about the size of the current frameset.
            // Word considers the default value to be 1*. Same applies to nested frames.

            if (value.EndsWith("%", StringComparison.Ordinal))
            {
                // Value type is Percentage.
                frame.DividerPositionType = FrameDividerPositionType.Percentage;
            }
            else if (value.EndsWith("*", StringComparison.Ordinal))
            {
                // Value type is Relative.
                frame.DividerPositionType = FrameDividerPositionType.Relative;
            }
            else
            {
                // Value type is Pixel.
                frame.DividerPositionType = FrameDividerPositionType.Pixel;
            }

            value = value.Trim(gDividerCharsToTrim);
            frame.DividerPositionValue = FormatterPal.TryParseInt(value);
        }

        private static readonly char[] gDividerCharsToTrim = new char[] { '*', '%' };
    }
}
