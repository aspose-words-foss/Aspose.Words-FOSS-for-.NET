// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/11/2015 by Anton Savko

using Aspose.Bidi;
using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Processes a &lt;ruby&gt; HTML element and writes a corresponding <see cref="Ruby"/> node to the document.
    /// </summary>
    /// <remarks>
    /// Malformed &lt;ruby&gt; elements that have no ruby text associated with them are imported as normal inline text.
    /// This class buffers content imported from a ruby element and then decides how the content should be written:
    /// as a <see cref="Ruby"/> node or as a sequence of <see cref="Run"/> nodes.
    /// </remarks>
    internal class HtmlRubyReader
    {
        internal HtmlRubyReader(
            DocumentBuilder builder,
            CssDeclarationCollection rubyElementDeclarations,
            bool textIsInVisualOrder)
        {
            Debug.Assert(builder != null);

            mBuilder = builder;

            mBidiTextArranger = new HtmlBidiTextArranger(mBuilder, textIsInVisualOrder, false);
            mIsWritingTopText = false;

            mCurrentRuby = new Ruby();
            switch (rubyElementDeclarations.GetIdentifier("ruby-align"))
            {
                case "left":
                    mCurrentRuby.Alignment = RubyAlignment.Left;
                    break;
                case "auto":
                case "center":
                case "line-edge":
                    mCurrentRuby.Alignment = RubyAlignment.Center;
                    break;
                case "right":
                    mCurrentRuby.Alignment = RubyAlignment.Right;
                    break;
                case "distribute-letter":
                    mCurrentRuby.Alignment = RubyAlignment.DistributeLetter;
                    break;
                case "distribute-space":
                default:
                    mCurrentRuby.Alignment = RubyAlignment.DistributeSpace;
                    break;
            }
        }

        /// <summary>
        /// Ends reading of a ruby element and writes the corresponding ruby node into the document model.
        /// </summary>
        internal void Flush()
        {
            if ((mCurrentRuby.Base.Count > 0) && (mCurrentRuby.Top.Count > 0))
            {
                Run run = new Run(mBuilder.Document, string.Empty, mBuilder.GetRunPrCopy());
                run.RunPr.SetAttr(FontAttr.Ruby, mCurrentRuby);
                mBuilder.InsertNode(run);
            }
            else
            {
                mBidiTextArranger.RearrangeAndWriteText();
            }
        }

        internal void WriteText(string text, RunPr runPr, BidiLevelList activeBidiLevels, bool dontCollapseLastSpace)
        {
            if (mIsWritingTopText)
            {
                RubyChunk topChunk = new RubyChunk();
                topChunk.Text = text;
                topChunk.RunPr = runPr;

                mCurrentRuby.Top.Add(topChunk);
                mCurrentRuby.TopSize = runPr.Size;
            }
            else
            {
                RubyChunk baseChunk = new RubyChunk();
                baseChunk.Text = text;
                baseChunk.RunPr = runPr;

                mCurrentRuby.Base.Add(baseChunk);
                mCurrentRuby.BaseSize = runPr.Size;

                mCurrentRuby.Distance = runPr.Size;
            }

            mBidiTextArranger.Append(text, runPr, activeBidiLevels, dontCollapseLastSpace, null);
        }

        internal bool IsWritingTopText
        {
            get { return mIsWritingTopText; }
            set { mIsWritingTopText = value; }
        }

        internal bool IsEmpty
        {
            get { return mBidiTextArranger.IsEmpty; }
        }

        internal bool CollapseWhitespaceAfterText()
        {
            return mBidiTextArranger.CollapseWhitespaceAfterText();
        }

        private readonly DocumentBuilder mBuilder;

        private readonly Ruby mCurrentRuby;
        private readonly HtmlBidiTextArranger mBidiTextArranger;
        private bool mIsWritingTopText;
    }
}
