// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2011 by Alexey Morozov

using Aspose.Words.Framesets;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Implements framed document writing.
    /// </summary>
    internal static class NrxFramesetWriter
    {
        internal static void Write(Document doc, NrxXmlBuilder builder, bool isDocx, string docxFramesetRelType)
        {
            // DOCX ISO29500 MS Implementation notes say that:
            // 17.15.2.19. frameset (Root Frameset Definition) (a) The standard does not state the behavior of an empty frameset element.
            // Word treats an empty frameset as though it contains a single frame element (using its default values).
            // same applies to nested framesets, so please rework.
            // 
            // AM. I don't like to write empty Root frameset though. Its unclear for me how frame properties should be applied to Document. 
            // Anyway it can be corrected when AW get frameset public API.
            if ((doc.Frameset == null) || doc.Frameset.ChildFramesets.Count == 0)
                return;

            Write(builder, doc.Frameset, isDocx, true, docxFramesetRelType);
        }

        /// <summary>
        /// Writes frame or frameset element.
        /// </summary>
        /// <remarks>
        /// AM. Althoutgh in DOCX frame and frameset are different elements I use one procedure for writing them both 
        /// because in Model we have one class for frame and frameset and there are several common attributes in both elements.
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="frame"></param>
        /// <param name="isDocx"></param>
        /// <param name="isRoot"></param>
        /// <param name="docxFramesetRelType"></param>
        private static void Write(NrxXmlBuilder builder, Frameset frame, bool isDocx, bool isRoot, string docxFramesetRelType)
        {
            builder.StartElement(frame.HasChild ? "w:frameset" : "w:frame");

            if (isRoot)
            {
                // AM. Althought DOCX can have framesetSplitbar element for nested frame, seems Word doens't support it.
                WriteFramesetBorder(builder, frame);
            }
            else
            {
                // Write sz for all frame/frameset except root frameset.
                WriteSz(builder, frame);
            }

            if (frame.HasChild) 
            {
                // FrameLayoutType.None is only allowed for frame object i.e empty frameset.
                Debug.Assert(frame.LayoutType != FrameLayoutType.None);
                builder.WriteVal("w:frameLayout", StyleConvertUtil.FrameLayoutTypeToNrx(frame.LayoutType));
            }
            else
            {
                builder.WriteValIfPositive("w:MarH", frame.MarginX);
                builder.WriteValIfPositive("w:MarV", frame.MarginY);

                builder.WriteVal("w:name", frame.Name);

                if (frame.FrameDefaultUrl != null)
                {
                    if (isDocx)
                    {
                        string relId =
                            ((DocxBuilder)builder).Part.Rels.Add(docxFramesetRelType, frame.FrameDefaultUrl, true);
                        builder.StartElement("w:sourceFileName");
                        builder.WriteAttributeString("r:id", relId);
                        builder.EndElement();
                    }
                    else
                    {
                        builder.WriteVal("w:sourceFileName", frame.FrameDefaultUrl);
                    }
                }

                if (frame.ScrollType != FrameScrollType.Auto)
                    builder.WriteVal("w:scrollbar", (frame.ScrollType == FrameScrollType.Yes) ? "on" : "off");
            }

            builder.WriteValIfTrue("w:noResizeAllowed", frame.NoResize);
            builder.WriteValIfTrue("w:linkedToFile", frame.IsFrameLinkToFile);

            builder.WriteVal("w:title", frame.Title);

            foreach (Frameset childFrame in frame.ChildFramesets)
                Write(builder, childFrame, isDocx, false, docxFramesetRelType);

            builder.EndElement();
        }

        /// <summary>
        /// Writes frameset border element.
        /// </summary>
        /// <remarks>
        /// AM. Althought DOCX can have framesetSplitbar element for nested frame, seems Word doens't support it. 
        /// At least I couldn't create document with the indepenent borders so I write this only for root frame.
        /// </remarks>
        /// <param name="builder"></param>
        /// <param name="frame"></param>
        private static void WriteFramesetBorder(NrxXmlBuilder builder, Frameset frame)
        {

            builder.StartElement("w:framesetSplitbar");

            builder.WriteValIfNotDefault("w:w", frame.FramesetBorderWidth, 0);
            builder.WriteVal("w:color", frame.FramesetBorderColor);

            string nrxFramesetBorderType = StyleConvertUtil.FramesetBorderTypeToNrx(frame.FramesetBorderType);
            if(StringUtil.HasChars(nrxFramesetBorderType))
                builder.WriteVal(string.Format("w:{0}", nrxFramesetBorderType), true);

            builder.EndElement();
        }

        /// <summary>
        /// Writes frame or frameset sz element. Doesn't write default value.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="frame"></param>
        private static void WriteSz(NrxXmlBuilder builder, Frameset frame)
        {
            string sz = "";
            if (frame.DividerPositionType == FrameDividerPositionType.Percentage)
                sz = "%";
            else if (frame.DividerPositionType == FrameDividerPositionType.Relative)
                sz = "*";

            sz = string.Format("{0}{1}", frame.DividerPositionValue, sz);
            if (sz != "1*")
                builder.WriteVal("w:sz", sz);
        }
    }
}
