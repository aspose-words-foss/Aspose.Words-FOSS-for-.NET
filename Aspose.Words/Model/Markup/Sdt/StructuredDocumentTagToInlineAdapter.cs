// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2013 by Denis Shvydkiy

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Adapter for <see cref="StructuredDocumentTag"/> class to <see cref="IInline"/> interface.
    /// This class is used for extracting run properties from StructuredDocumentTag.
    /// The class is not intended and not tested for other purposes.
    /// </summary>
    internal class StructuredDocumentTagToInlineAdapter : IInline
    {
        internal StructuredDocumentTagToInlineAdapter(StructuredDocumentTag sdt, bool isStart = true)
        {
            Debug.Assert(sdt != null);
            mSdt = sdt;
            mIsStart = isStart;
        }

        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return ((IRunAttrSource)this).GetDirectRunAttr(key, RevisionsView.Original);
        }

        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return RunPr.GetDirectAttr(key, revisionsView);
        }

        object IRunAttrSource.FetchInheritedRunAttr(int fontAttr)
        {
            return InlineHelper.FetchInheritedAttr(this, fontAttr);
        }

        void IRunAttrSource.SetRunAttr(int fontAttr, object value)
        {
            RunPr.SetAttr(fontAttr, value);
        }

        void IRunAttrSource.RemoveRunAttr(int key)
        {
            RunPr.Remove(key);
        }

        void IRunAttrSource.ClearRunAttrs()
        {
            RunPr.Clear();
        }

        Paragraph IInline.ParentParagraph_IInline
        {
            get { return mSdt.ParentNode as Paragraph; }
        }

        DocumentBase IInline.Document_IInline
        {
            get { return mSdt.Document; }
        }

        RunPr IInline.GetExpandedRunPr_IInline(RunPrExpandFlags flags)
        {
            return InlineHelper.GetExpandedRunPr(this, flags);
        }

        RunPr IInline.RunPr_IInline
        {
            get { return RunPr; }
            set
            {
                if (mIsStart)
                    mSdt.ContentsRunPr = value;
                else
                    mSdt.EndCharacterRunPr = value;
            }
        }

        private RunPr RunPr
        {
            get { return mIsStart ? mSdt.ContentsRunPr : mSdt.EndCharacterRunPr; }
        }

        private readonly bool mIsStart;
        private readonly StructuredDocumentTag mSdt;
    }
}
