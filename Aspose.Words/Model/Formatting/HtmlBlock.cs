// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2013 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Represents HTML related information about paragraph or row.
    /// </summary>
    /// <remarks>
    /// 17.15.2.6 div (Information About Single HTML div Element)
    /// This element specifies information about a single HTML div, body, or blockquote element which was
    /// included in this document, so that information (which is stored on a logical structure with no direct analog
    /// in WordprocessingML) can be maintained when an HTML document is stored in the WordprocessingML format.
    /// The div element stores the following information about these structures:
    /// · The child HTML div, and blockquote elements
    /// · The borders for the element
    /// · The margins for the element
    /// When the resulting WordprocessingML document is displayed by an application, the settings specified by this
    /// information shall be reflected in the formatting of the resulting paragraphs (i.e. this information shall not only
    /// be used when the document is resaved in the HTML format).
    /// 
    /// AM. There is no common naming across formats about this feature and usage of 'Pgp' is seems to be confusing because of
    /// PrettyGoodPrivacy so we decided to use HtmlBlock.
    /// </remarks>
    internal class HtmlBlock
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal HtmlBlock(int id)
        {
            mId = id;
        }

        public override int GetHashCode()
        {
            int hashCode = mId.GetHashCode();
            hashCode = (hashCode * 397) ^ mParentId.GetHashCode();
            hashCode = (hashCode * 397) ^ ((mParaPr != null) ? mParaPr.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ mHtmlBlockType.GetHashCode();
            hashCode = (hashCode * 397) ^ mItap.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Unique value that is used to identify this HtmlBlock.
        /// </summary>
        internal int Id
        {
            get { return mId; }
        }

        /// <summary>
        /// The identifier of the immediate parent HtmlBlock.
        /// </summary>
        internal int ParentId
        {
            get { return mParentId; }
            set { mParentId = value; }
        }

        // The table depth to which this HtmlBlock is applied.
        internal int Itap
        {
            get { return mItap; }
            set { mItap = value; }
        }

        /// <summary>
        /// Paragraph properties collection for this HtmlBlock.
        /// </summary>
        /// <remarks>
        /// AM. This property collection might has only limited set of paragraph attributes.
        /// So if it goes public we need to restrict it somehow.
        /// </remarks>
        internal ParaPr ParaPr
        {
            get
            {
                if(mParaPr == null)
                    mParaPr = new ParaPr();

                return mParaPr;
            }
        }

        internal HtmlBlockType HtmlBlockType
        {
            set { mHtmlBlockType = value; }
            get { return mHtmlBlockType; }
        }

        /// <summary>
        /// Indicates that the HtmlBlock has child blocks.
        /// </summary>
        /// <remarks>
        /// AM. This feature is rare so I implemented this in a straight way.
        /// Method might be optimized later if needed.
        /// </remarks>
        internal bool HasChild
        {
            get
            {
                if (mHtmlBlockCollection == null)
                    return false;

                for (int i = 0; i < mHtmlBlockCollection.Count; i++)
                    if (mHtmlBlockCollection.GetHtmlBlockByIndex(i).ParentId == this.Id)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Removes ignorable attribute values from property collection.
        /// </summary>
        /// <remarks>
        /// AM. Current implementation removes zero values and empty borders. 
        /// The reason for it is to get unified model regardless of format being read.
        /// So far I'm not sure that zeros/empty borders should be just removed. 
        /// Maybe they need to be collapsed over parent item? Will see later.
        /// </remarks>
        internal void Refine()
        {
            foreach (int key in gMarginKeys)
                if ((int)ParaPr.FetchAttr(key) == 0)
                    ParaPr.Remove(key);

            foreach (int key in ParaPr.PossibleBorderKeys.Values)
                if (object.Equals(ParaPr.GetDirectAttr(key), Border.Empty))
                    ParaPr.Remove(key);
        }

        internal HtmlBlock Clone()
        {
            HtmlBlock lhs = (HtmlBlock)MemberwiseClone();
            if (mParaPr != null)
                lhs.mParaPr = mParaPr.Clone();
            return lhs;
        }

        /// <summary>
        /// HtmlBlockCollection to which this HtmlBlock is belong.
        /// </summary>
        internal HtmlBlockCollection HtmlBlockCollection
        {
            get { return mHtmlBlockCollection; }
            set { mHtmlBlockCollection = value; }
        }

        private readonly int mId;
        private int mParentId;
        private ParaPr mParaPr;
        private HtmlBlockType mHtmlBlockType = HtmlBlockType.Div;
        private int mItap;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private HtmlBlockCollection mHtmlBlockCollection;

        private static readonly int[] gMarginKeys = new int[]
                                            {
                                                ParaAttr.HtmlMarginLeft, ParaAttr.HtmlMarginRight, ParaAttr.HtmlMarginTop, ParaAttr.HtmlMarginBottom
                                            };

    }
}
