// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/06/2010 by Dmitry Vorobyev

using System.Collections.Generic;
using Aspose.Words.Fields;
using Aspose.Words.Validation;

namespace Aspose.Words.Lists
{
    /// <summary>
    /// Walks through all paragraphs in the document and updates list labels for all numbered or bulleted paragraphs.
    /// </summary>
    /// <remarks>
    /// <para>At the moment the logic based on playing with Word is as follows:</para>
    /// <para>1. There are several areas (normally stories) where numbering goes independently and continuously:
    /// body, headers/footers, notes, comments. It seems that numbering does not reset throughout sections.</para>
    /// <para>2. Textboxes are inside area too. It seems it's Shape area.</para>
    /// <para>3. <see cref="ListNumberGenerator"/> is responsible for numbering inside each area.</para>
    ///
    /// <para>For new <see cref="SaveFormat"/>s that need up-to-date list labels, update <see cref="DocumentValidator.gSaveActions"/>
    /// hashtable with new element containing <see cref="DocumentValidatorActions.UpdateListLabels"/> flag.</para>
    /// <para>The other classes that need up-to-date list labels, must call <see cref="UpdateListLabels"/> function.</para>
    /// <para>For public update of list labels use <see cref="Document.UpdateListLabels"/> function.</para>
    /// </remarks>
    internal class ListLabelUpdater : ListItemProcessor
    {
        /// <summary>
        /// <para>Walks through all paragraphs in the document and updates list labels for all numbered or bulleted paragraphs.</para>
        /// <para>Presence of list label for a paragraph can be checked by <see cref="Paragraph.HasListLabel"/> property.</para>
        /// <para>List label text and other info can be retrieved by <see cref="Paragraph.ListLabel"/> property.</para>
        /// </summary>
        /// <param name="doc">Document, list labels are created for.</param>
        internal static void UpdateListLabels(Document doc)
        {
            ListLabelUpdater originalLabelUpdater = new ListLabelUpdater(doc, RevisionsView.Original);
            doc.Accept(originalLabelUpdater);

            if (!originalLabelUpdater.mHasRevisions)
                return;

            // We have revisions in the document. Do second pass to update final list labels.
            ListLabelUpdater finalLabelUpdater = new ListLabelUpdater(doc, RevisionsView.Final);
            doc.Accept(finalLabelUpdater);
        }

        private ListLabelUpdater(Document doc, RevisionsView revisionsView)
        {
            mRevisionsView = revisionsView;
            mFieldNumListLabelUpdater = new FieldNumListLabelUpdater(doc);

            // AS Some of list number generators may stay unused for simple documents,
            // but creation of ListNumberGenerator is cheap operation. I believe it's better
            // than maintain hashtable of generators with document area as a key.
            for (int i = 0; i < NumberOfAreas; i++)
                mListNumberGenerators[i] = new ListNumberGenerator();

            mBodyListNumberGenerator = mListNumberGenerators[(int)ListNumberingArea.Body];
        }

        public override VisitorAction VisitSectionStart(Section section)
        {
            mOccuredListIds.Clear();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphStart(Paragraph para)
        {
            mCurrentParagraphs.Push(new ParagraphProcessingState(para));

            // WORDSNET-18475 Restart from a specific list level.
            if (para.IsListItem)
            {
                int listId = para.ListFormat.ListId;
                int listLevel = para.ListFormat.ListLevelNumberOriginal;

                if(!mOccuredListIds.Contains(listId))
                    foreach (ListNumberGenerator listNumberGenerator in mListNumberGenerators)
                        listNumberGenerator.Restart(listId, listLevel);

                mOccuredListIds.Add(listId);
            }

            // SPEED. Do second pass only if document has revisions.
            if (para.HasRevisions)
                mHasRevisions = true;

            ProcessParagraph();

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitParagraphEnd(Paragraph paragraph)
        {
            if (!IsCurrentParagraphProcessed)
            {
                bool isInsideBody = CurrentNumberGenerator == mBodyListNumberGenerator;
                mFieldNumListLabelUpdater.ProcessParagraph(CurrentParagraph, false, isInsideBody);
            }

            mCurrentParagraphs.Pop();
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            mFieldChars.Push(fieldStart);

            mFieldNumListLabelUpdater.ProcessFieldStart(fieldStart);
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            // Protection against standalone field separator.
            if (CurrentFieldChar != null)
            {
                mFieldChars.Push(fieldSeparator);
                mFieldNumListLabelUpdater.ProcessFieldSeparator(fieldSeparator);
            }

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            // There could be field separator, pop it.
            if ((CurrentFieldChar != null) && (CurrentFieldChar.NodeType == NodeType.FieldSeparator))
                mFieldChars.Pop();

            // And pop field start.
            if (CurrentFieldChar != null)
                mFieldChars.Pop();

            mFieldNumListLabelUpdater.ProcessFieldEnd(fieldEnd);

            ProcessParagraph();

            return VisitorAction.Continue;
        }

        protected override void OnNumberingAreaStart(ListNumberingArea area)
        {
            PushNumberGenerator(area);
        }

        protected override void OnNumberingAreaEnd()
        {
            PopNumberGenerator();
        }

        private void ProcessParagraph()
        {
            if (IsCurrentParagraphProcessed)
                return;

            // WORDSNET-18309 When in field, update list labels for the topmost fields only.
            if (IsInField && !IsInTopmostFieldResult)
                return;

            ListNumberGenerator generator = CurrentNumberGenerator;
            bool isInsideBody = generator == mBodyListNumberGenerator;
            Paragraph paragraph = CurrentParagraph;

            bool isListLabelUpdated = ListLabelUtil.UpdateParagraphListLabel(paragraph, generator, mRevisionsView);
            mFieldNumListLabelUpdater.ProcessParagraph(paragraph, isListLabelUpdated, isInsideBody);

            IsCurrentParagraphProcessed = true;
        }

        /// <summary>
        /// Pushes a specific area's number generator to the stack.
        /// </summary>
        private void PushNumberGenerator(ListNumberingArea area)
        {
            // AS Here we put to stack ListNumberGenerator and not ListNumberingArea to optimize Peek operation in
            // CurrentNumberGenerator accessor that returns the topmost ListNumberGenerator from the stack.
            mNumberGeneratorStack.Push(mListNumberGenerators[(int)area]);
            mFieldNumListLabelUpdater.ProcessArea(CurrentNumberGenerator);
        }

        private void PopNumberGenerator()
        {
            mNumberGeneratorStack.Pop();
            if (HasCurrentNumberGenerator)
                mFieldNumListLabelUpdater.ProcessArea(CurrentNumberGenerator);
        }

        private bool HasCurrentNumberGenerator
        {
            get { return mNumberGeneratorStack.Count > 0; }
        }

        private ListNumberGenerator CurrentNumberGenerator
        {
            get { return mNumberGeneratorStack.Peek(); }
        }

        /// <summary>
        /// Indicates that we are currently inside a field.
        /// </summary>
        private bool IsInField
        {
            get { return mFieldChars.Count > 0; }
        }

        /// <summary>
        /// Indicates that we are currently inside field result.
        /// </summary>
        private bool IsInTopmostFieldResult
        {
            get
            {
                // We are in field result if FieldSeparator is at top of stack.
                // WORDSNET-18309 mFieldChars.Count == 2 - means we are in the topmost field result.
                return (mFieldChars.Count == 2) && (CurrentFieldChar.NodeType == NodeType.FieldSeparator);
            }
        }

        private FieldChar CurrentFieldChar
        {
            get { return IsInField ? mFieldChars.Peek() : null; }
        }

        /// <summary>
        /// Collects field characters during document traverse.
        /// </summary>
        private readonly Stack<FieldChar> mFieldChars = new Stack<FieldChar>();

        private readonly ListNumberGenerator[] mListNumberGenerators = new ListNumberGenerator[NumberOfAreas];
        private readonly ListNumberGenerator mBodyListNumberGenerator;
        private readonly Stack<ListNumberGenerator> mNumberGeneratorStack = new Stack<ListNumberGenerator>();
        private readonly FieldNumListLabelUpdater mFieldNumListLabelUpdater;
        private readonly RevisionsView mRevisionsView;
        private readonly Stack<ParagraphProcessingState> mCurrentParagraphs = new Stack<ParagraphProcessingState>();

        // Indicates that document has revision.
        private bool mHasRevisions;

        /// <summary>
        /// Contains list identifiers of numbered paragraph occured after break.
        /// </summary>
        private readonly List<int> mOccuredListIds = new List<int>();

        private bool IsCurrentParagraphProcessed
        {
            get { return mCurrentParagraphs.Peek().IsProcessed; }
            set { mCurrentParagraphs.Peek().IsProcessed = value; }
        }

        private Paragraph CurrentParagraph
        {
            get { return mCurrentParagraphs.Peek().Paragraph; }
        }

        private class ParagraphProcessingState
        {
            internal ParagraphProcessingState(Paragraph paragraph)
            {
                Paragraph = paragraph;
            }

            internal Paragraph Paragraph { get; }
            internal bool IsProcessed { get; set; }
        }
    }
}
