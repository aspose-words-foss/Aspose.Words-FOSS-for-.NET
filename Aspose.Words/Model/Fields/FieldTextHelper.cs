// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2013 by Ivan Lyagin

using System;
using System.Collections.Generic;
using Aspose.Bidi;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides a set of utility BIDI-aware methods to process text within the Field engine.
    /// </summary>
    internal class FieldTextHelper : IBidiParagraphCharDataModifier
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private FieldTextHelper()
        {
            // Hide from using outside.
        }

        /// <summary>
        /// Writes BIDI text to the given document builder properly basing on <see cref="IBidiParagraphLevelOverride"/>
        /// instance provided by the given field.
        /// </summary>
        internal static void WriteTextBidiAware(Field field, DocumentBuilder builder, string text)
        {
            WriteTextBidiAware(field, builder, text, false);
        }

        /// <summary>
        /// Writes BIDI text to the given document builder properly using the specified BIDI embedding.
        /// </summary>
        internal static void WriteTextBidiAware(DocumentBuilder builder, string text, bool isRtlEmbedding)
        {
            WriteTextBidiAware(null, builder, text, isRtlEmbedding);
        }

        /// <summary>
        /// Writes BIDI text to the given document builder properly basing on <see cref="IBidiParagraphLevelOverride"/>
        /// instance provided by the given field.
        /// </summary>
        internal static void WriteTextBidiAware(Field field, DocumentBuilder builder, FieldFormattingResult result)
        {
            Debug.Assert(field != null);

            if (!result.PreserveRichFormatting || builder.Document.FieldOptions.IsBidiTextSupportedOnUpdate)
            {
                WriteTextBidiAware(field, builder, result.Text.ToSystemString(), false);
                return;
            }

            WriteRichString(builder, result.Text);
        }

        /// <summary>
        /// Writes BIDI text to the given document builder properly. If the specified field is null, uses the specified
        /// BIDI embedding. Otherwise, uses <see cref="IBidiParagraphLevelOverride"/> instance provided by the given field.
        /// </summary>
        private static void WriteTextBidiAware(Field field, DocumentBuilder builder, string text, bool isRtlEmbedding)
        {
            if (!builder.Document.FieldOptions.IsBidiTextSupportedOnUpdate)
            {
                builder.Write(text);
                return;
            }

            WriteBidiText(field, builder, text, isRtlEmbedding);
        }

        private static void WriteBidiText(Field field, DocumentBuilder builder, string text, bool isRtlEmbedding)
        {
            // Write BIDI-compatible text runs and paragraphs.
            IBidiParagraphLevelOverride levelOverride = (field != null)
                ? field.GetBidiParagraphLevelOverride()
                : ConstantBidiParagraphLevelOverride.GetInstance(isRtlEmbedding);

            IList<BidiParagraph> bidiParagraphs = BidiParagraph.SplitStringToParagraphs(text, levelOverride, gInstance);
            DocumentBuilderAdapter adapter = CreateDocumentBuilderAdapter(builder);

            foreach (BidiParagraph bidiParagraph in bidiParagraphs)
            {
                AppendBidiParagraphRuns(bidiParagraph, builder.Document, adapter, adapter);
                if (StringUtil.HasChars(bidiParagraph.ParagraphSeparator))
                    builder.InsertParagraph();
            }
        }

        /// <summary>
        /// Returns an <see cref="IList{T}"/> instance filled with runs obtained while the specified single-line text
        /// partitioning. Considers BIDI text. Throws, if the specified text is not single-line.
        /// </summary>
        internal static IList<Run> GetSingleLineTextRunListBidiAware(
            string text,
            Document document,
            IFieldRunPrProvider runPrProvider,
            bool isRtlEmbedding)
        {
            RunListBuilder builder = CreateRunListBuilder();
            AppendSingleLineTextRunsBidiAware(text, document, builder, runPrProvider, isRtlEmbedding);
            return builder.GetList();
        }

        /// <summary>
        /// Returns a <see cref="NodeRange"/> instance consisting of runs obtained while the specified single-line text
        /// partitioning. Considers BIDI text. Throws, if the specified text is not single-line.
        /// </summary>
        internal static NodeRange GetSingleLineTextRunRangeBidiAware(
            string text,
            Document document,
            IFieldRunPrProvider runPrProvider,
            bool isRtlEmbedding)
        {
            RunRangeBuilder builder = CreateRunRangeBuilder();
            AppendSingleLineTextRunsBidiAware(text, document, builder, runPrProvider, isRtlEmbedding);
            return builder.GetNodeRange();
        }

        private static void WriteRichString(DocumentBuilder builder, RichString text)
        {
            // EV: This method is adapted copy of the DocumentBuilder.WriteCore method.

            //Convert CrLf and Lf into Crs.
            RichString normalizedText = text
                .ReplaceInternal(ControlChar.CrLf, ControlChar.Cr)
                .ReplaceInternal(ControlChar.Lf, ControlChar.Cr);

            //If text had any Crs insert paragraphs as appropriate.
            int paraStart = 0;
            while (paraStart <= normalizedText.Length)
            {
                int paraEnd = normalizedText.IndexOf(ControlChar.ParagraphBreakChar.ToString(), paraStart);
                if (paraEnd != -1)
                {
                    int length = paraEnd - paraStart;
                    if (length > 0)
                        InsertRun(builder, normalizedText.Substring(paraStart, length));

                    switch (builder.ParagraphBreakCharReplacement)
                    {
                        case ParagraphBreakCharReplacement.Paragraph:
                            Paragraph lastParagraph = builder.CurrentParagraph;
                            Paragraph newParagraph = builder.InsertParagraph();

                            newParagraph.ParagraphBreakRunPr = lastParagraph.ParagraphBreakRunPr;
                            lastParagraph.ParagraphBreakRunPr = normalizedText.GetInternal(paraEnd).RunPr;
                            break;
                        case ParagraphBreakCharReplacement.ParagraphBreakChar:
                            InsertRun(builder, ControlChar.ParagraphBreak, normalizedText.GetInternal(paraEnd).RunPr);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    paraStart = paraEnd + 1;
                }
                else
                {
                    //No more paragraph breaks are found.
                    int length = normalizedText.Length - paraStart;
                    if (length > 0)
                        InsertRun(builder, normalizedText.Substring(paraStart, length));

                    break;
                }
            }
        }

        private static void InsertRun(DocumentBuilder builder, RichString text)
        {
            foreach (RichStringChunk chunk in text.ToChunks())
                InsertRun(builder, chunk.Value, chunk.RunPr);
        }

        private static void InsertRun(DocumentBuilder builder, string text, RunPr runPr)
        {
            runPr = runPr.Clone();
            builder.GetRunPrCopy().MirrorTo(runPr, FontAttr.RsidR, FontAttr.RsidRPr);

            Run newRunNode = new Run(builder.Document, text, runPr);

            builder.InsertNode(newRunNode);
        }

        /// <summary>
        /// Appends runs obtained while the specified single-line text partitioning to the specified <see cref="IRunAppender"/>.
        /// Considers BIDI text. Throws, if the specified text is not single-line.
        /// </summary>
        private static void AppendSingleLineTextRunsBidiAware(
            string text,
            Document document,
            IRunAppender runAppender,
            IFieldRunPrProvider runPrProvider,
            bool isRtlEmbedding)
        {
            if (BidiParagraph.IsMultiParagraphString(text))
                throw new ArgumentException("A single-line text was expected.", "text");

            if (document.FieldOptions.IsBidiTextSupportedOnUpdate)
            {
                // Build and append BIDI-compatible text runs.
                BidiParagraph bidiParagraph = new BidiParagraph(text, Convert.ToInt32(isRtlEmbedding), gInstance);
                AppendBidiParagraphRuns(bidiParagraph, document, runAppender, runPrProvider);
            }
            else
            {
                // Build and append a single text run regardless of whether the text contains any RTL characters.
                runAppender.Append(new Run(document, text, runPrProvider.GetRunPr()));
            }
        }

        /// <summary>
        /// Appends runs of the specified <see cref="BidiParagraph"/> to the given <see cref="IRunAppender"/>.
        /// </summary>
        private static void AppendBidiParagraphRuns(
            BidiParagraph bidiParagraph,
            Document document,
            IRunAppender runAppender,
            IFieldRunPrProvider runPrProvider)
        {
            foreach (BidiRun bidiRun in bidiParagraph.BidiRuns)
            {
                if (!StringUtil.HasChars(bidiRun.Text))
                    continue;

                RunPr runPr = runPrProvider.GetRunPr();
                if (bidiRun.Rtl)
                {
                    runPr.Bidi = AttrBoolEx.True;
                }
                else
                {
                    runPr.Remove(FontAttr.Bidi);
                }

                Run run = new Run(document, bidiRun.Text, runPr);
                runAppender.Append(run);
            }
        }

        void IBidiParagraphCharDataModifier.Modify(BidiParagraph paragraph, BidiCharData[] textData)
        {
            // Change BidiCharData items to produce MS-Word-like BIDI field result.
            int embeddingLevel = paragraph.EmbeddingLevel;
            foreach (BidiCharData bidiChar in textData)
            {
                switch (bidiChar.CharType)
                {
                    case BidiCharacterType.AN:
                    case BidiCharacterType.EN:
                        // It seems like MS Word sets an embedding level for every numeric character equal to the previous
                        // character's embedding level or to the paragraph's embedding level in case of the first character,
                        // although it breaks clause 3.3.5 of unicode BIDI algorithm (see http://www.unicode.org/reports/tr9/).
                        bidiChar.EmbLevel = embeddingLevel;
                        break;
                    default:
                        embeddingLevel = bidiChar.EmbLevel;
                        break;
                }
            }
        }

        /// <summary>
        /// A factory method to create a <see cref="RunListBuilder"/> instance.
        /// </summary>
        /// <remarks>
        /// This method should be an instance member the code to be autoportable to Java since
        /// instances of nested classes should be created within instance members of an outer class.
        /// </remarks>
        private static RunListBuilder CreateRunListBuilder()
        {
            return new RunListBuilder();
        }

        /// <summary>
        /// A factory method to create a <see cref="RunRangeBuilder"/> instance.
        /// </summary>
        /// <remarks>
        /// This method should be an instance member the code to be autoportable to Java since
        /// instances of nested classes should be created within instance members of an outer class.
        /// </remarks>
        private static RunRangeBuilder CreateRunRangeBuilder()
        {
            return new RunRangeBuilder();
        }

        /// <summary>
        /// A factory method to create a <see cref="DocumentBuilderAdapter"/> instance.
        /// </summary>
        /// <remarks>
        /// This method should be an instance member the code to be autoportable to Java since
        /// instances of nested classes should be created within instance members of an outer class.
        /// </remarks>
        private static DocumentBuilderAdapter CreateDocumentBuilderAdapter(DocumentBuilder builder)
        {
            return new DocumentBuilderAdapter(builder);
        }

        /// <summary>
        /// Represents an entity which can collect runs.
        /// </summary>
        private interface IRunAppender
        {
            [JavaThrows(true)]
            void Append(Run run);
        }

        /// <summary>
        /// Implements <see cref="IRunAppender"/> in the way to build an <see cref="List{T}"/> of appended runs.
        /// </summary>
        private class RunListBuilder : IRunAppender
        {
            internal IList<Run> GetList()
            {
                return mList;
            }

            void IRunAppender.Append(Run run)
            {
                mList.Add(run);
            }

            private readonly List<Run> mList = new List<Run>();
        }

        /// <summary>
        /// Implements <see cref="IRunAppender"/> in the way to build a <see cref="NodeRange"/> consisting of appended runs.
        /// </summary>
        private class RunRangeBuilder : IRunAppender
        {
            internal NodeRange GetNodeRange()
            {
                return ((mStartRun != null) && (mEndRun != null)) ? new NodeRange(mStartRun, mEndRun) : null;
            }

            void IRunAppender.Append(Run run)
            {
                if (mStartRun == null)
                    mStartRun = run;

                if (mEndRun == null)
                {
                    mEndRun = run;
                }
                else
                {
                    CompositeNode parent = mEndRun.ParentNode;
                    if (parent == null)
                    {
                        // We should stick multiple runs together by a parent node,
                        // NodeEnumerator to be able to move between them.
                        parent = new Paragraph(mEndRun.Document);
                        parent.InsertAfter(mEndRun, null);
                    }

                    parent.InsertAfter(run, mEndRun);
                    mEndRun = run;
                }
            }

            private Run mStartRun;
            private Run mEndRun;
        }

        /// <summary>
        /// Adapts a <see cref="DocumentBuilder"/> instance to <see cref="IRunAppender"/> and <see cref="IFieldRunPrProvider"/>
        /// interfaces.
        /// </summary>
        private class DocumentBuilderAdapter : IRunAppender, IFieldRunPrProvider
        {
            internal DocumentBuilderAdapter(DocumentBuilder builder)
            {
                mBuilder = builder;
            }

            void IRunAppender.Append(Run run)
            {
                mBuilder.InsertNode(run);
            }

            RunPr IFieldRunPrProvider.GetRunPr()
            {
                return mBuilder.GetRunPrCopy();
            }

            private readonly DocumentBuilder mBuilder;
        }

        /// <summary>
        /// The singleton instance of the class.
        /// </summary>
        private static readonly FieldTextHelper gInstance = new FieldTextHelper();
    }
}
