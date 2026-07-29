// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2015 by Alexey Butalov

using Aspose.Words.Framesets;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Helps to import HTML framesets http://www.w3.org/TR/html4/present/frames.html into a Document object.
    /// </summary>
    internal class HtmlFramesetReader
    {
        internal HtmlFramesetReader(Document document,
                                    string baseUri,
                                    HtmlResourceLoader resourceLoader)
        {
            Debug.Assert(document != null);
            Debug.Assert(resourceLoader != null);
            mDocument = document;
            mBaseUri = baseUri;
            mResourceLoader = resourceLoader;
        }

        /// <summary>
        /// Reads frameset structure declared in the HTML frameset element into <see cref="Document.Frameset"/> object.
        /// </summary>
        /// <param name="framesetNode">HTML frameset element.</param>
        internal void Read(HtmlElementNode framesetNode)
        {
            Debug.Assert(framesetNode != null);
            Debug.Assert(framesetNode.Name == "frameset");

            Frameset rootFrame = new Frameset();
            ReadFrameset(framesetNode, rootFrame);
            mDocument.Frameset = rootFrame;
        }

        private void ReadFrameset(HtmlElementNode framesetNode, Frameset frame)
        {
            string[] frameWidths = new string[0];
            string cols = framesetNode.Attributes.GetAttributeValue("cols", string.Empty);
            if (cols != string.Empty)
            {
                frame.LayoutType = FrameLayoutType.Horizontal;
                frameWidths = cols.Split(',');
            }
            else
            {
                string rows = framesetNode.Attributes.GetAttributeValue("rows", string.Empty);
                if (rows != string.Empty)
                {
                    frame.LayoutType = FrameLayoutType.Vertical;
                    frameWidths = rows.Split(',');
                }
            }
            // Chrome browser shows only the first frame if there are no cols or rows attributes. We do the same.
            if (frameWidths.Length == 0)
                frameWidths = new string[] { "*" };

            int childIndex = 0;
            foreach (string frameWidth in frameWidths)
            {
                HtmlElementNode frameNode = null;
                while ((childIndex < framesetNode.Children.Count) && (frameNode == null))
                {
                    HtmlElementNode childNode = framesetNode.Children[childIndex] as HtmlElementNode;
                    if ((childNode != null) && ((childNode.Name == "frame") || (childNode.Name == "frameset")))
                        frameNode = childNode;
                    childIndex++;
                }
                if (frameNode == null)
                    return;

                Frameset childFrame = new Frameset();
                frame.ChildFramesets.Add(childFrame);

                SetFrameWidth(frameWidth, childFrame);
                switch (frameNode.Name)
                {
                    case "frame":
                    {
                        SetFrameProperties(frameNode, childFrame);
                        break;
                    }
                    case "frameset":
                    {
                        ReadFrameset(frameNode, childFrame);
                        break;
                    }
                    default:
                    {
                        Debug.Assert(false);
                        break;
                    }
                }
            }
        }

        private void SetFrameProperties(HtmlElementNode frameNode, Frameset frame)
        {
            frame.Title = frameNode.Attributes.GetAttributeValue("title", string.Empty);
            frame.Name = frameNode.Attributes.GetAttributeValue("name", string.Empty);
            frame.MarginX = frameNode.Attributes.GetAttributeValue("marginwidth", 0);
            frame.MarginY = frameNode.Attributes.GetAttributeValue("marginheight", 0);
            if (frameNode.Attributes.GetAttributeValue("frameborder", 0) == 1)
            {
                frame.FramesetBorderType = FramesetBorderType.Simple;
                // In DOCX default width of the splitters is 4.5 points wide.
                frame.FramesetBorderWidth = ConvertUtilCore.PointToTwip(4.5);
            }
            frame.NoResize = frameNode.Attributes.GetAttributeValue("noresize", string.Empty) == "noresize";
            switch (frameNode.Attributes.GetAttributeValue("scrolling", string.Empty))
            {
                case "yes":
                    frame.ScrollType = FrameScrollType.Yes;
                    break;
                case "no":
                    frame.ScrollType = FrameScrollType.No;
                    break;
                case "auto":
                    frame.ScrollType = FrameScrollType.Auto;
                    break;
                default:
                    break;
            }

            string fileName = HtmlUtil.ValidateUri(frameNode.Attributes.GetAttributeValue("src", string.Empty));
            if (fileName != string.Empty)
            {
                frame.SetFrameDefaultUrlInternal(fileName);
                frame.FileData = mResourceLoader.LoadHtmlDocument(mBaseUri, fileName);
            }
        }

        private static void SetFrameWidth(string frameWidthStr, Frameset frame)
        {
            CssValue value = CssParser.ParseValue(frameWidthStr.Trim());
            if (value == null)
            {
                frame.DividerPositionType = FrameDividerPositionType.Relative;
            }
            else
            {
                switch (value.ValueType)
                {
                    case CssValueType.Length:
                    {
                        // It doesn't matter which dimension is used - px, pt or another, Chrome uses px in any case. We do the same.
                        frame.DividerPositionType = FrameDividerPositionType.Pixel;
                        frame.DividerPositionValue = (int)((CssLengthValue)value).DoubleValue;
                        break;
                    }
                    case CssValueType.Number:
                    {
                        frame.DividerPositionType = FrameDividerPositionType.Pixel;
                        frame.DividerPositionValue = (int)((CssNumberValue)value).DoubleValue;
                        break;
                    }
                    case CssValueType.Percentage:
                    {
                        frame.DividerPositionType = FrameDividerPositionType.Percentage;
                        frame.DividerPositionValue = (int)((CssPercentageValue)value).DoubleValue;
                        break;
                    }
                    default:
                    {
                        frame.DividerPositionType = FrameDividerPositionType.Relative;
                        break;
                    }
                }
            }
        }

        private readonly Document mDocument;
        private readonly string mBaseUri;
        private readonly HtmlResourceLoader mResourceLoader;
    }
}
