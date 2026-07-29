// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.JavaAttributes;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a run of characters with the same font formatting.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/programming-with-documents/">Programming with Documents</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>All text of the document is stored in runs of text.</p>
    /// <p><see cref="Run"/> can only be a child of <see cref="Paragraph"/> or inline <see cref="Aspose.Words.Markup.StructuredDocumentTag"/>.</p>
    /// </remarks>
    public class Run : Inline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Run"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Run"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Run"/> to the document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the paragraph where you want the run inserted.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Run(DocumentBase doc) : this(doc, "")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <b>Run</b> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Run"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Run"/> to the document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the paragraph where you want the run inserted.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        /// <param name="text">The text of the run.</param>
        public Run(DocumentBase doc, string text) : this(doc, text, new RunPr())
        {
        }

        internal Run(DocumentBase doc, string text, RunPr runPr) : base(doc, runPr)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            mText = text;
        }

        /// <summary>
        /// Returns <see cref="NodeType.Run"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Run; }
        }

        /// <summary>
        /// Gets or sets the text of the run.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Public API, as designed.")]
        public string Text
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mText; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (mText == value)
                    return;

                if (Document.IsTrackRevisionsEnabled)
                {
                    EditSession editSession = FetchDocumentOrGlossaryMain().EditSession;

                    if (mText != String.Empty)
                    {
                        // Make copy of this run. It will be marked for deletion.
                        Run oldTextRun = new Run(Document, mText, RunPr.Clone());

                        using (new SuspendTrackRevisionsDocument(Document))
                        {
                            // WORDSNET-16417 Use ParentNode instead of ParentParagraph to insert it into SDT
                            // Have to suspend tracking because actually we insert run but it should be marked for deletion.
                            ParentNode.InsertAfter(oldTextRun, this);
                            RevisionTrackingUtil.AddDeleteRevision(oldTextRun, editSession);
                        }
                    }

                    RevisionTrackingUtil.AddInsertRevision(this, editSession);
                }

                mText = value;
            }
        }

        /// <summary>
        /// Gets a boolean value indicating either the run is a phonetic guide.
        /// </summary>
        public bool IsPhoneticGuide
        {
            get { return RunPr.Contains(FontAttr.Ruby); }
        }

        /// <summary>
        /// Gets a <see cref="PhoneticGuide"/> object.
        /// </summary>
        public PhoneticGuide PhoneticGuide
        {
            get
            {
                if (mPhoneticGuide == null)
                    mPhoneticGuide = new PhoneticGuide(this);

                return mPhoneticGuide;
            }
        }

        /// <summary>
        /// Indicates that run contains only symbols.
        /// </summary>
        internal bool IsSymbolic
        {
            get
            {
                if (mText.Length == 0)
                    return false;

                for (int i = 0; i < mText.Length; i++)
                    if (!StringUtil.IsPrivateUseCategory(mText[i]))
                        return false;

                return true;
            }
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitRun"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><c>false</c> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitRun(this));
        }

        /// <summary>
        /// Gets the text of the run.
        /// </summary>
        /// <returns>The text of the run.</returns>
        [JavaDelete("Don't need this in Java because the Text property getter is auto ported as getText() and all methods in Java are virtual.")]
        public override string GetText()
        {
            return mText;
        }

        /// <summary>
        /// Cuts the text of the run.
        /// </summary>
        /// <param name="startIndex">A text position the new text should be started from.</param>
        /// <param name="length">The length of the new text.</param>
        /// <param name="isClone">True to return a new run, false to alter this run and return a reference to it.</param>
        /// <returns></returns>
        internal Run CutText(int startIndex, int length, bool isClone)
        {
            Run run = (!isClone) ? this : (Run)Clone(false);

            if (length == 0)
                run.Text = string.Empty;
            else
                run.Text = run.Text.Substring(startIndex, length);

            return run;
        }

        /// <summary>
        /// Splits the run before leftPartLength position.
        /// First leftPartLength chars go to a new run before the remaining part of an old run.
        /// </summary>
        /// <remarks>It does nothing when the whole run would go in one part of split.</remarks>
        /// <param name="leftPartLength"></param>
        /// <returns>A Run object representing the newly inserted run or null if the run was not actually split.</returns>
        internal Run SplitBefore(int leftPartLength)
        {
            Run insertedRun = null;

            if ((leftPartLength > 0) && (leftPartLength < Text.Length))
            {
                insertedRun = (Run) ParentNode.InsertBefore(
                    new Run(Document, Text.Substring(0, leftPartLength), RunPr.Clone()), this);
                CutText(leftPartLength, Text.Length - leftPartLength, false);
            }

            return insertedRun;
        }

        /// <summary>
        /// Splits the run after leftPartLength position.
        /// First leftPartLength chars remain in the old run, the rest goes to a new run after it.
        /// </summary>
        /// <remarks>It does nothing when the whole run would go in one part of split.</remarks>
        /// <param name="leftPartLength"></param>
        /// <returns>A Run object representing the newly inserted run or null if the run was not actually split.</returns>
        internal Run SplitAfter(int leftPartLength)
        {
            Run insertedRun = null;

            if ((leftPartLength > 0) && (leftPartLength < Text.Length))
            {
                insertedRun = (Run) ParentNode.InsertAfter(
                    new Run(Document, Text.Substring(leftPartLength, Text.Length - leftPartLength), RunPr.Clone()), this);
                CutText(0, leftPartLength, false);
            }

            return insertedRun;
        }

        /// <summary>
        /// Returns true if a passed string represents a symbol run.
        /// </summary>
        /// <remarks>
        /// More info in NrxRunReaderBase.ReadSymbol().
        /// It seems to me that the symbol characters are stored as one-character run in the model.
        /// So to find them I check if the run has the first character that belongs to Unicode 'Private Use' category.
        /// 'Private Use' means that no font substitution can be done on such characters because
        /// they are providing glyphs entirely different from conventional glyphs for these characters.
        ///</remarks>
        internal static bool IsSymbolicCharacter(string text)
        {
            return (StringUtil.HasChars(text)) && StringUtil.IsPrivateUseCategory(text[0]);
        }

        private string mText;
        private PhoneticGuide mPhoneticGuide;

        /// <summary>
        /// These keys will be ignored in run properties comparison on join of LTR runs. Keep sorted since we use binary search.
        /// FontAttr.RsidRPr and FontAttr.RsidR should be ignored since they are the unique editing session IDs.
        /// Other keys are specific for LTR runs.
        /// </summary>
        internal static readonly int[] KeysToIgnoreInComparisonOnJoinLtrRuns =
        {
            FontAttr.RsidRPr, FontAttr.RsidR, FontAttr.BoldBi, FontAttr.ItalicBi, FontAttr.NameBi, FontAttr.LocaleIdBi,
            FontAttr.SizeBi
        };

        /// <summary>
        /// These keys will be ignored in run properties comparison on join RTL runs. Keep sorted since we use binary search.
        /// FontAttr.RsidRPr and FontAttr.RsidR should be ignored since they are the unique editing session IDs.
        /// Other keys are specific for RTL runs.
        /// </summary>
        internal static readonly int[] KeysToIgnoreInComparisonOnJoinRtlRuns =
        {
            FontAttr.RsidRPr, FontAttr.RsidR, FontAttr.Bold, FontAttr.Italic, FontAttr.Size, FontAttr.NameAscii,
            FontAttr.NameFarEast, FontAttr.NameOther, FontAttr.LocaleId, FontAttr.LocaleIdFarEast
        };

#if DEBUG
        public override string ToString()
        {
            return String.Format("{0} {1}", base.ToString(), Text);
        }
#endif
    }
}
