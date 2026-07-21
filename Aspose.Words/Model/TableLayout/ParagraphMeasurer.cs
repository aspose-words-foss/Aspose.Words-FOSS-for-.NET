// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2006 by Dmitry Vorobyev

using System;
using Aspose.Drawing.Fonts;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// The main purpose of this class is calculating layout (min and max) paragraph width.
    /// </summary>
    internal class ParagraphMeasurer
    {
        internal LayoutWidth GetLayoutWidth(Paragraph paragraph)
        {
            LayoutWidth layoutWidth = new LayoutWidth();
            int lockCount = 0;

            foreach (Node childNode in paragraph.GetChildNodes(NodeType.Any, false))
            {
                // TODO: Add support of explicit line breaks
                switch (childNode.NodeType)
                {
                    case NodeType.FieldStart:
                    {
                        lockCount++;
                        break;
                    }
                    case NodeType.FieldSeparator:
                    {
                        lockCount--;
                        break;
                    }
                    case NodeType.FieldEnd:
                    {
                        FieldEnd fieldEnd = (FieldEnd)childNode;
                        if (!fieldEnd.HasSeparator)
                            lockCount--;
                        
                        break;
                    }
                    default:
                    {
                        if (lockCount == 0)
                            ProcessNode(childNode, layoutWidth);

                        break;
                    }
                }

                if (mIsInsideWord)
                    AddWordWidth(layoutWidth);
            }

            ParagraphFormat pf = paragraph.ParagraphFormat;

            double toAdd = pf.LeftIndent + pf.RightIndent;

            if (pf.FirstLineIndent < 0)
                toAdd -= pf.FirstLineIndent;

            layoutWidth.Add(toAdd);

            return layoutWidth;
        }

        private void ProcessNode(Node node, LayoutWidth width)
        {
            // TODO: Add support of explicit line breaks
            switch (node.NodeType)
            {
                case NodeType.Run:
                {
                    Run run = (Run)node;
                    // WORDSJAVA-862/864: Fetch font only once per Run and pass it to
                    // all child method calls. Fetching font and it's attributes is very expensive
                    // operation: this fix raised performance up to 2 times on IBM performance tests.
                    DrFont font = GetDrawingFont(run.Font, run.FetchDocumentOrGlossaryMain());
                    string text = run.Text;
                    UpdateMinWidth(font, text, width);
                    width.Max += font.GetTextWidthPoints(text);
                    break;
                }
                case NodeType.Shape:
                case NodeType.GroupShape:
                {
                    ShapeBase shape = (ShapeBase)node;

                    // WORDSNET-16273 Take in attention shapes with "Square" wrap type.
                    if (shape.IsInline || ((shape.WrapType == WrapType.Square) && (shape.WrapSide == WrapSide.Both)))
                    {
                        width.Min = System.Math.Max(width.Min, shape.Width);
                        width.Max += shape.Width;
                    }

                    break;
                }
                default:
                {
                    // Skip other types of nodes.

                    break;
                }
            }
        }

        private void UpdateMinWidth(DrFont font, string text, LayoutWidth layoutWidth)
        {
            int startIndex = 0, length = 0;

            for (int i = 0; i < text.Length; i++)
            {
                Char c = text[i];

                if (Char.IsWhiteSpace(c) || (c == '-'))
                {
                    if (mIsInsideWord)
                    {
                        AddSubstringWidth(font, text, startIndex, length);
                        AddWordWidth(layoutWidth);
                        length = 0;
                    }
                }
                else
                {
                    if (!mIsInsideWord)
                    {
                        mIsInsideWord = true;
                        startIndex = i;
                    }
                    length++;
                }
            }

            if (mIsInsideWord)
                AddSubstringWidth(font, text, startIndex, length);
        }

        private void AddSubstringWidth(DrFont font, string text, int startIndex, int length)
        {
            string substring = text.Substring(startIndex, length);
            mCurrentWordWidth += font.GetTextWidthPoints(substring);
        }

        private void AddWordWidth(LayoutWidth layoutWidth)
        {
            layoutWidth.Min = System.Math.Max(layoutWidth.Min, mCurrentWordWidth);
            mCurrentWordWidth = 0;
            mIsInsideWord = false;
        }

        private static DrFont GetDrawingFont(Font font, Document doc)
        {
            return doc.FontProvider.FetchDrFont(font.Name, (float)font.Size, font.FontStyle);
        }

        private bool mIsInsideWord;
        private double mCurrentWordWidth;
    }
}