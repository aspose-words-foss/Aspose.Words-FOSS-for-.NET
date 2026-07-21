// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2003 by Roman Korchagin

using System.IO;
using System.Reflection;
using System.Text;
using Aspose.Fonts;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Txt.Writer
{
    /// <summary>
    /// The base class for saving document in plain text formats.
    /// </summary>
    internal abstract class TxtWriterBase : DocumentVisitor, IDocumentWriter
    {
        /// <summary>
        /// Saves a document or a fragment of a document in a plain text format.
        /// </summary>
        SaveOutputParameters IDocumentWriter.SaveToStream(SaveInfo saveInfo)
        {
            Debug.Assert(saveInfo != null);
            SaveInfo = saveInfo;

            Init(saveInfo.Document, (TxtSaveOptionsBase)saveInfo.SaveOptions);

            for (Node child = mDocument.FirstChild; child != null; child = child.NextSibling)
                WriteSection((Section)child);
            Document.AcceptEnd(this);

            // Save the text to the stream.
            StreamWriter writer = new StreamWriter(saveInfo.Stream, mSaveOptionsBase.Encoding);
            writer.Write(mBuilder.ToString());
            writer.Write(mHeadersFootersBuilder.ToString());

            // Just flush, but do not close the stream because closing is the client's responsibility.
            writer.Flush();

            return new SaveOutputParameters("text/plain");
        }

        /// <summary>
        /// Saves a specified node to a string.
        /// </summary>
        internal string SaveToString(Node node, TxtSaveOptionsBase saveOptions)
        {
            // FOSS

            Init(node.Document, saveOptions);

            BeforeNodeAccepted(node);
            node.Accept(this);
            AfterNodeAccepted(node);

            return Builder.ToString();
        }

        protected virtual void BeforeNodeAccepted(Node node)
        {
            if (node.NodeType == NodeType.StructuredDocumentTagRangeStart)
                ((StructuredDocumentTagRangeStart)node).AcceptAsCompositeNode(this);
        }

        protected virtual void AfterNodeAccepted(Node node)
        {
        }

        /// <summary>
        /// Gets text of a specified run with applied formatting.
        /// </summary>
        internal static string GetText(Run run)
        {
            string text = run.Text;

            if (run.RunPr.Contains(FontAttr.Ruby))
            {
                Ruby ruby = (Ruby)run.RunPr[FontAttr.Ruby];
                text = ruby.GetText();
            }

            text = run.Font.AllCaps || IsParagraphAllCaps(run)
                ? text.ToUpper()
                : text;

            // WORDSNET-25757 Convert text with single character (that is most probably symbol entity (<w:sym> element in DOCX))
            // with Wingding font from PUA to analogue character from some other Unicode area. This allows to avoid converting
            // it later to Symbol character with FontUtil.UnicodeToSymbol(), as in some cases this is not correct.
            if ((text.Length == 1) && (run.RunPr.ComplexNameAscii.Resolve(run.Document.GetThemeInternal()) == "Wingdings"))
                return FontUtil.WingdingToUnicode(text[0]).ToString();

            return text;
        }

        /// <summary>
        /// Switches current builder to a specified one.
        /// </summary>
        private void SwitchToBuilder(StringBuilder builder)
        {
            mBuilder = builder;
        }

        /// <summary>
        /// Writes all headers and footers of a specified section.
        /// </summary>
        protected virtual void WriteAllHeadersFooters(Section section)
        {
            // Switch to specified string builder.
            SwitchToBuilder(HeadersFootersBuilder);

            foreach (HeaderFooterType type in gHeadersFootersSortedTypes)
            {
                WriteHeaderFooter(section, type);
                // Insert paragraph break after each HeaderFooter to mimic Word behavior.
                if (section.HeadersFooters[type] != null)
                    mBuilder.Append(SaveOptionsBase.ParagraphBreak);
            }

            SwitchToBuilder(MainBuilder);
        }

        /// <summary>
        /// Writes HeaderFooter to a current string builder.
        /// </summary>
        protected void WriteHeaderFooter(Section section, HeaderFooterType headerFooterType)
        {
            HeaderFooter header = section.HeadersFooters[headerFooterType];
            if (header != null)
                WriteStory(header);
        }

        /// <summary>
        /// Writes section.
        /// </summary>
        private void WriteSection(Section section)
        {

            if (SaveOptionsBase.ExportHeadersFootersMode == TxtExportHeadersFootersMode.AllAtEnd)
            {
                // When headers and footers are exporting using this mode, we have to collect their text and
                // write it at the very end of a document instead of writing it just here at the current section.
                // So, we write them into dedicated string builder.
                WriteAllHeadersFooters(section);
            }

            if (SaveOptionsBase.ExportHeadersFootersMode == TxtExportHeadersFootersMode.PrimaryOnly)
                WriteHeaderFooter(section, HeaderFooterType.HeaderPrimary);

            if (section.Body != null)
                WriteStory(section.Body);

            if (SaveOptionsBase.ExportHeadersFootersMode == TxtExportHeadersFootersMode.PrimaryOnly)
                WriteHeaderFooter(section, HeaderFooterType.FooterPrimary);

        }

        /// <summary>
        /// Writes story.
        /// </summary>
        private void WriteStory(Story story)
        {
            story.Accept(this);
        }

        /// <summary>
        /// Returns true, if a text of a specified run should be converted to upper case.
        /// </summary>
        private static bool IsParagraphAllCaps(Run run)
        {
            Paragraph parentParagraph = run.ParentParagraph;

            return ((parentParagraph != null) &&
                    (parentParagraph.ParagraphStyle != null) &&
                    (parentParagraph.ParagraphStyle.Font != null) &&
                    parentParagraph.ParagraphStyle.Font.AllCaps);
        }

        /// <summary>
        /// Initializes writer.
        /// </summary>
        private void Init(DocumentBase doc, TxtSaveOptionsBase saveOptions)
        {
            // Handling of char array TxtWriter.gSimplifiedBulletsCharacters is based on these constants.
            // Fix this handling inside ListNumberGenerator class if assertions are violated.
            Debug.Assert((ListLevel.MinLevel == 0) && (ListLevel.MaxLevels == 9));
            Debug.Assert(doc != null);
            Debug.Assert(saveOptions != null);

            mDocument = doc;
            mSaveOptionsBase = saveOptions;

            mMainBuilder = new StringBuilder();
            mHeadersFootersBuilder = new StringBuilder();

            SwitchToBuilder(mMainBuilder);
        }

        /// <summary>
        /// Gets a document being written.
        /// </summary>
        internal DocumentBase Document
        {
            get { return mDocument; }
        }

        /// <summary>
        /// Gets current builder.
        /// </summary>
        internal StringBuilder Builder
        {
            get { return mBuilder; }
        }

        /// <summary>
        /// Gets a builder that will be eventually written into a text file.
        /// </summary>
        protected StringBuilder MainBuilder
        {
            get { return mMainBuilder; }
        }

        /// <summary>
        /// Gets builder for writing headers and footers.
        /// </summary>
        protected StringBuilder HeadersFootersBuilder
        {
            get { return mHeadersFootersBuilder; }
        }

        /// <summary>
        /// Gets save options.
        /// </summary>
        protected TxtSaveOptionsBase SaveOptionsBase
        {
            get { return mSaveOptionsBase; }
        }

        /// <summary>
        /// Encapsulates parameters passed into a document writer.
        /// </summary>
        protected SaveInfo SaveInfo { get; private set; }

        /// <summary>
        /// The array of headers and footers types sorted in the same order as Word writes them in plain text format.
        /// </summary>
        private static readonly HeaderFooterType[] gHeadersFootersSortedTypes =
        {
            HeaderFooterType.HeaderEven,
            HeaderFooterType.HeaderPrimary,
            HeaderFooterType.FooterEven,
            HeaderFooterType.FooterPrimary,
            HeaderFooterType.HeaderFirst,
            HeaderFooterType.FooterFirst
        };

        private TxtSaveOptionsBase mSaveOptionsBase;

        private DocumentBase mDocument;

        private StringBuilder mBuilder;
        private StringBuilder mMainBuilder;
        private StringBuilder mHeadersFootersBuilder;
    }
}
