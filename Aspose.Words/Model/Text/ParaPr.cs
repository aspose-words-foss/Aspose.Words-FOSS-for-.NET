// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2005 by Roman Korchagin
using System.Collections.Generic;
using Aspose.Words.Drawing;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Provides typed access to paragraph attributes.
    /// </summary>
    internal class ParaPr : WordAttrCollection, IParaAttrSource
    {
        internal override void AcceptFormatRevision()
        {
            if (HasListRevision)
            {
                // WORDSNET-14997 It seems that Word removes left indent when accepts list revision.
                // I believe that actually Word removes all list related attributes
                // (LeftIndent, FirstLineIndent and TabStop) but lets limit solution for a while.
                Remove(ParaAttr.LeftIndent);
                // WORDSNET-19608 Word removes "hanging" attribute when numbering changed within revision.
                Remove(ParaAttr.FirstLineIndent);
            }

            AcceptFormatRevisionCore(ParaAttr.Istd, gRevisionPreservedAttributes);
        }

        /// <summary>
        /// Style index.
        /// </summary>
        internal int Istd
        {
            get { return (int)FetchAttr(ParaAttr.Istd); }
            set { SetAttr(ParaAttr.Istd, value); }
        }

        internal ParagraphAlignment Alignment
        {
            get { return (ParagraphAlignment)FetchAttr(ParaAttr.Alignment); }
            set { SetAttr(ParaAttr.Alignment, value); }
        }

        internal bool NoSpaceBetweenSameStyle
        {
            get { return (bool)FetchAttr(ParaAttr.NoSpaceBetweenSameStyle); }
            set { SetAttr(ParaAttr.NoSpaceBetweenSameStyle, value); }
        }

        internal bool SideBySide
        {
            get { return (bool)FetchAttr(ParaAttr.SideBySide); }
            set { SetAttr(ParaAttr.SideBySide, value); }
        }

        internal bool KeepTogether
        {
            get { return (bool)FetchAttr(ParaAttr.KeepTogether); }
            set { SetAttr(ParaAttr.KeepTogether, value); }
        }

        internal bool KeepWithNext
        {
            get { return (bool)FetchAttr(ParaAttr.KeepWithNext); }
            set { SetAttr(ParaAttr.KeepWithNext, value); }
        }

        internal bool PageBreakBefore
        {
            get { return (bool)FetchAttr(ParaAttr.PageBreakBefore); }
            set { SetAttr(ParaAttr.PageBreakBefore, value); }
        }

        internal bool Kinsoku
        {
            get { return (bool)FetchAttr(ParaAttr.FarEastLineBreakControl); }
            set { SetAttr(ParaAttr.FarEastLineBreakControl, value); }
        }

        internal bool WordWrap
        {
            get { return (bool)FetchAttr(ParaAttr.WordWrap); }
            set { SetAttr(ParaAttr.WordWrap, value); }
        }

        internal bool OverflowPunctuation
        {
            get { return (bool)FetchAttr(ParaAttr.HangingPunctuation); }
            set { SetAttr(ParaAttr.HangingPunctuation, value); }
        }

        internal bool TopLinePunctuation
        {
            get { return (bool)FetchAttr(ParaAttr.TopLinePunctuation); }
            set { SetAttr(ParaAttr.TopLinePunctuation, value); }
        }

        /// <summary>
        /// In Word documents, lists may consist of up to nine levels, numbered 0 to 8.
        /// </summary>
        internal int ListLevel
        {
            get { return (int)FetchAttr(ParaAttr.ListLevel); }
            set
            {
                // WORDSNET-807 list level 10 is specified.
                // Ideally, all setting value in the model should go through one place to make validation in one place only.
                value = Lists.ListLevel.MakeLevelNumberValid(value);

                SetAttr(ParaAttr.ListLevel, value);
            }
        }

        /// <summary>
        /// Zero when the paragraph is not a list item.
        /// </summary>
        internal int ListId
        {
            get { return (int)FetchAttr(ParaAttr.ListId); }
            set { SetAttr(ParaAttr.ListId, value); }
        }

        /// <summary>
        /// Returns true when ListId is present and it is zero.
        /// </summary>
        internal bool IsExplicitlyNotListItem
        {
            get { return (ContainsKey(ParaAttr.ListId) && (ListId == 0)); }
        }

        /// <summary>
        /// Gets the object that represents info about the revision to the list formatting of the paragraph.
        /// After the model passed the document post loader, presence of this object indicates the paragraph has a numbering revision.
        /// </summary>
        internal ParagraphNumberRevision NumberRevision
        {
            get { return (ParagraphNumberRevision)GetDirectAttr(RevisionAttr.NumberRevision); }
            set { SetAttr(RevisionAttr.NumberRevision, value); }
        }

        internal bool HasNumberRevision
        {
            get { return (NumberRevision != null); }
        }

        internal bool SuppressLineNumbers
        {
            get { return (bool)FetchAttr(ParaAttr.SuppressLineNumbers); }
            set { SetAttr(ParaAttr.SuppressLineNumbers, value); }
        }

        /// <summary>
        /// Gets or sets the collection of tab stops directly.
        /// </summary>
        internal TabStopCollection TabStops
        {
            get { return (TabStopCollection)GetDirectAttr(ParaAttr.TabStops); }
            set { SetAttr(ParaAttr.TabStops, value); }
        }

        internal int LeftIndent
        {
            get { return (int)FetchAttr(ParaAttr.LeftIndent); }
            set { SetAttr(ParaAttr.LeftIndent, value); }
        }

        internal int FirstLineIndent
        {
            get { return (int)FetchAttr(ParaAttr.FirstLineIndent); }
            set { SetAttr(ParaAttr.FirstLineIndent, value); }
        }

        internal int RightIndent
        {
            get { return (int)FetchAttr(ParaAttr.RightIndent); }
            set { SetAttr(ParaAttr.RightIndent, value); }
        }

        internal int LineSpacing
        {
            get { return ((LineSpacing)FetchAttr(ParaAttr.LineSpacing)).Value; }
            set { ((LineSpacing)GetOrCreateComplexAttr(ParaAttr.LineSpacing)).Value = value; }
        }

        internal LineSpacingRule LineSpacingRule
        {
            get { return ((LineSpacing)FetchAttr(ParaAttr.LineSpacing)).Rule; }
            set { ((LineSpacing)GetOrCreateComplexAttr(ParaAttr.LineSpacing)).Rule = value; }
        }

        internal int SpaceBefore
        {
            get { return (int)FetchAttr(ParaAttr.SpaceBefore); }
            set { SetAttr(ParaAttr.SpaceBefore, value); }
        }

        internal bool SpaceBeforeAuto
        {
            get { return (bool)FetchAttr(ParaAttr.SpaceBeforeAuto); }
            set { SetAttr(ParaAttr.SpaceBeforeAuto, value); }
        }

        internal int SpaceAfter
        {
            get { return (int)FetchAttr(ParaAttr.SpaceAfter); }
            set { SetAttr(ParaAttr.SpaceAfter, value); }
        }

        internal bool SpaceAfterAuto
        {
            get { return (bool)FetchAttr(ParaAttr.SpaceAfterAuto); }
            set { SetAttr(ParaAttr.SpaceAfterAuto, value); }
        }

        internal OutlineLevel OutlineLevel
        {
            get { return (OutlineLevel)FetchAttr(ParaAttr.OutlineLevel); }
            set { SetAttr(ParaAttr.OutlineLevel, value); }
        }

        internal bool HasFrameHorizontalAlignment
        {
            get { return ContainsKey(ParaAttr.FrameHorizontalAlignment); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal HorizontalAlignment FrameHorizontalAlignment
        {
            get { return (HorizontalAlignment)FetchAttr(ParaAttr.FrameHorizontalAlignment); }
            set { SetAttr(ParaAttr.FrameHorizontalAlignment, value); }
        }


        internal bool HasFrameVerticalAlignment
        {
            get { return ContainsKey(ParaAttr.FrameVerticalAlignment);}
        }

        internal bool HasFrameAttributes
        {
            get
            {
                return HasFrameHeight ||
                    HasFrameWidth ||
                    HasFrameLeft ||
                    HasFrameTop ||
                    HasFrameHorizontalAlignment ||
                    HasFrameVerticalAlignment ||
                    HasFrameRelativeHorizontalPosition ||
                    HasFrameRelativeVerticalPosition;

            }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal VerticalAlignment FrameVerticalAlignment
        {
            get { return (VerticalAlignment)FetchAttr(ParaAttr.FrameVerticalAlignment); }
            set { SetAttr(ParaAttr.FrameVerticalAlignment, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal int FrameLeft
        {
            get { return (int)FetchAttr(ParaAttr.FrameLeft); }
            set { SetAttr(ParaAttr.FrameLeft, value); }
        }

        internal bool HasFrameLeft
        {
            get { return ContainsKey(ParaAttr.FrameLeft); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal int FrameTop
        {
            get { return (int)FetchAttr(ParaAttr.FrameTop); }
            set { SetAttr(ParaAttr.FrameTop, value); }
        }

        internal bool HasFrameTop
        {
            get { return ContainsKey(ParaAttr.FrameTop); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal int FrameWidth
        {
            get { return (int)FetchAttr(ParaAttr.FrameWidth); }
            set { SetAttr(ParaAttr.FrameWidth, value); }
        }

        internal bool HasFrameWidth
        {
            get { return ContainsKey(ParaAttr.FrameWidth); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal int FrameHeight
        {
            get { return ((Height)FetchAttr(ParaAttr.FrameHeight)).Value; }
            set { ((Height)GetOrCreateComplexAttr(ParaAttr.FrameHeight)).Value = value; }
        }
        /// <summary>
        /// Text frame.
        /// </summary>
        internal HeightRule FrameHeightRule
        {
            get { return ((Height)FetchAttr(ParaAttr.FrameHeight)).Rule; }
            set { ((Height)GetOrCreateComplexAttr(ParaAttr.FrameHeight)).Rule = value; }
        }

        internal bool HasFrameHeight
        {
            get { return ContainsKey(ParaAttr.FrameHeight); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal RelativeHorizontalPosition FrameRelativeHorizontalPosition
        {
            get { return (RelativeHorizontalPosition)FetchAttr(ParaAttr.FrameRelativeHorizontalPosition); }
        }

        internal bool HasFrameRelativeHorizontalPosition
        {
            get { return ContainsKey(ParaAttr.FrameRelativeHorizontalPosition); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal RelativeVerticalPosition FrameRelativeVerticalPosition
        {
            get { return (RelativeVerticalPosition)FetchAttr(ParaAttr.FrameRelativeVerticalPosition); }
        }

        internal bool HasFrameRelativeVerticalPosition
        {
            get { return ContainsKey(ParaAttr.FrameRelativeVerticalPosition); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal WrapType FrameWrapType
        {
            get { return (WrapType)FetchAttr(ParaAttr.FrameWrapType); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal bool FrameSuppressOverlap
        {
            get { return (bool)FetchAttr(ParaAttr.FrameSuppressOverlap); }
            set { SetAttr(ParaAttr.FrameSuppressOverlap, value); }
        }

        internal Border BorderTop
        {
            get { return (Border)FetchAttr(ParaAttr.BorderTop); }
            set { SetAttr(ParaAttr.BorderTop, value); }
        }

        internal Border BorderLeft
        {
            get { return (Border)FetchAttr(ParaAttr.BorderLeft); }
            set { SetAttr(ParaAttr.BorderLeft, value); }
        }

        internal Border BorderBottom
        {
            get { return (Border)FetchAttr(ParaAttr.BorderBottom); }
            set { SetAttr(ParaAttr.BorderBottom, value); }
        }

        internal Border BorderRight
        {
            get { return (Border)FetchAttr(ParaAttr.BorderRight); }
            set { SetAttr(ParaAttr.BorderRight, value); }
        }

        internal Border BorderBetween
        {
            get { return (Border)FetchAttr(ParaAttr.BorderBetween); }
            set { SetAttr(ParaAttr.BorderBetween, value); }
        }

        internal Border BorderBar
        {
            get { return (Border)FetchAttr(ParaAttr.BorderBar); }
            set { SetAttr(ParaAttr.BorderBar, value); }
        }

        internal bool SuppressAutoHyphens
        {
            get { return (bool)FetchAttr(ParaAttr.SuppressAutoHyphens); }
            set { SetAttr(ParaAttr.SuppressAutoHyphens, value); }
        }

        internal DropCapPosition DropCapPosition
        {
            get { return (DropCapPosition)FetchAttr(ParaAttr.DropCapPosition); }
            set { SetAttr(ParaAttr.DropCapPosition, value); }
        }

        internal int DropCapLinesToDrop
        {
            get { return (int)FetchAttr(ParaAttr.DropCapLinesToDrop); }
            set { SetAttr(ParaAttr.DropCapLinesToDrop, value); }
        }

        internal Shading Shading
        {
            get { return (Shading)FetchAttr(ParaAttr.Shading); }
            set { SetAttr(ParaAttr.Shading, value); }
        }

        internal bool WidowControl
        {
            get { return (bool)FetchAttr(ParaAttr.WidowControl); }
            set { SetAttr(ParaAttr.WidowControl, value); }
        }

        /// <summary>
        /// HTML related information <see cref="HtmlBlock" /> is applied to this paragraph.
        /// </summary>
        internal int HtmlBlockId
        {
            get { return (int)FetchAttr(ParaAttr.HtmlBlockId); }
            set { SetAttr(ParaAttr.HtmlBlockId, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal TextOrientation FrameTextOrientation
        {
            get { return (TextOrientation)FetchAttr(ParaAttr.FrameTextOrientation); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal int FrameHorizontalDistanceFromText
        {
            get { return (int)FetchAttr(ParaAttr.FrameHorizontalDistanceFromText); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal int FrameVerticalDistanceFromText
        {
            get { return (int)FetchAttr(ParaAttr.FrameVerticalDistanceFromText); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal bool FrameLockAnchor
        {
            get { return (bool)FetchAttr(ParaAttr.FrameLockAnchor); }
        }

        internal BaselineAlignment BaselineAlignment
        {
            get { return (BaselineAlignment)FetchAttr(ParaAttr.BaselineAlignment); }
            set { SetAttr(ParaAttr.BaselineAlignment, value); }
        }

        internal bool Bidi
        {
            get { return (bool)FetchAttr(ParaAttr.Bidi); }
            set { SetAttr(ParaAttr.Bidi, value); }
        }

        /// <summary>
        /// Gets or sets the collapsed state of a paragraph.
        /// </summary>
        internal bool Collapsed
        {
            get { return (bool)FetchAttr(ParaAttr.Collapsed); }
            set { SetAttr(ParaAttr.Collapsed, value); }
        }

        /// <summary>
        /// Top margin of <see cref="HtmlBlock"/>
        /// </summary>
        internal int HtmlMarginTop
        {
            get { return (int)FetchAttr(ParaAttr.HtmlMarginTop); }
        }

        /// <summary>
        /// Bottom margin of <see cref="HtmlBlock"/>
        /// </summary>
        internal int HtmlMarginBottom
        {
            get { return (int)FetchAttr(ParaAttr.HtmlMarginBottom); }
        }

        /// <summary>
        /// Left margin of <see cref="HtmlBlock"/>
        /// </summary>
        internal int HtmlMarginLeft
        {
            get { return (int)FetchAttr(ParaAttr.HtmlMarginLeft); }
        }

        /// <summary>
        /// Right margin of <see cref="HtmlBlock"/>
        /// </summary>
        internal int HtmlMarginRight
        {
            get { return (int)FetchAttr(ParaAttr.HtmlMarginRight); }
        }

        /// <summary>
        /// Returns true if this para should be positioned as floating textframe.
        /// </summary>
        /// <remarks>
        /// 1. None of MS Specs define precise conditions for text frame to be floating,
        /// so this is experimentally derived conditions that relate to the topic of
        /// removal tblpPr in docx
        /// 2. Todo DD: Investigate why is this function works only when so different from <see cref="TablePr.IsFloating"/>, although
        /// Floating properties are the same in iso29500 spec.
        /// </remarks>
        internal bool IsFloating
        {
            get { return FrameFormat.IsFloatingCore(this); }
        }

        /// <summary>
        /// Returns true if the paragraph properties have same absolute positioning as the specified properties.
        /// There are 14 text frame positioning attributes. This efficiently compares all of them if needed.
        /// </summary>
        internal bool IsSameFloatingPositioning(ParaPr rhs)
        {
            bool isApo1 = IsFloating;
            bool isApo2 = rhs.IsFloating;

            // A quick way to get out if both are not absolutely positioned.
            if (!isApo1 && !isApo2)
                return true;

            // A quick way to get out if one is absolutely positioned and the other is not.
            if (isApo1 != isApo2)
                return false;

            // Now we have to compare all frame attributes thoroughly.
            if (FrameRelativeHorizontalPosition != rhs.FrameRelativeHorizontalPosition)
                return false;

            if (FrameRelativeVerticalPosition != rhs.FrameRelativeVerticalPosition)
                return false;

            if (FrameLeft != rhs.FrameLeft)
                return false;

            if (FrameTop != rhs.FrameTop)
                return false;

            if (FrameHorizontalAlignment != rhs.FrameHorizontalAlignment)
                return false;

            if (FrameVerticalAlignment != rhs.FrameVerticalAlignment)
                return false;

            if (FrameWidth != rhs.FrameWidth)
                return false;

            if (FrameHeight != rhs.FrameHeight)
                return false;

            if (FrameHeightRule != rhs.FrameHeightRule)
                return false;

            if (FrameHorizontalDistanceFromText != rhs.FrameHorizontalDistanceFromText)
                return false;

            if (FrameVerticalDistanceFromText != rhs.FrameVerticalDistanceFromText)
                return false;

            if (FrameWrapType != rhs.FrameWrapType)
                return false;

            if (FrameLockAnchor != rhs.FrameLockAnchor)
                return false;

            if (FrameTextOrientation != rhs.FrameTextOrientation)
                return false;

            return true;
        }

        internal void RemoveFloatingAttrs()
        {
            foreach (int attr in FloatingAttrs)
                Remove(attr);

            // The [MS-OI-29500].pdf also says that supressOverlap must not be written into styles,
            // but I am not sure where it is in the model. We would need to delete it here too.
        }

        internal static readonly int[] FloatingAttrs = new int[]
        {
            ParaAttr.DropCapPosition,
            ParaAttr.DropCapLinesToDrop,
            ParaAttr.FrameHorizontalAlignment,
            ParaAttr.FrameVerticalAlignment,
            ParaAttr.FrameLeft,
            ParaAttr.FrameTop,
            ParaAttr.FrameWidth,
            ParaAttr.FrameHeight,
            ParaAttr.FrameRelativeHorizontalPosition,
            ParaAttr.FrameRelativeVerticalPosition,
            ParaAttr.FrameWrapType,
            ParaAttr.FrameTextOrientation,
            ParaAttr.FrameHorizontalDistanceFromText,
            ParaAttr.FrameVerticalDistanceFromText,
            ParaAttr.FrameLockAnchor
        };

        internal static readonly int[] BorderAttrs = new int[]
        {
            ParaAttr.BorderTop,
            ParaAttr.BorderLeft,
            ParaAttr.BorderBottom,
            ParaAttr.BorderRight,
            ParaAttr.BorderBetween,
            ParaAttr.BorderBar
        };

        internal void RemoveClearTabStops()
        {
            TabStopCollection tabStops = this.TabStops;
            if (tabStops != null)
                tabStops.RemoveClearTabStops();
        }

        /// <summary>
        /// Creates and sets the <see cref="NumberRevision"/> object if it does not exist yet.
        /// </summary>
        internal void EnsureHasNumberRevision()
        {
            if (NumberRevision == null)
                NumberRevision = new ParagraphNumberRevision();
        }

        /// <summary>
        /// Reverses a paragraph alignment value.
        /// </summary>
        internal static ParagraphAlignment ReverseAlignment(ParagraphAlignment alignment)
        {
            switch (alignment)
            {
                case ParagraphAlignment.Left:
                    return ParagraphAlignment.Right;
                case ParagraphAlignment.Right:
                    return ParagraphAlignment.Left;
                default:
                    return alignment;
            }
        }

        /// <summary>
        /// Returns the static collection of default attributes.
        /// </summary>
        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        internal static object FetchDefaultAttr(int key)
        {
            return gDefaults.FetchAttr(key);
        }

        /// <summary>
        /// Returns property collection depending on passed flags.
        /// </summary>
        internal ParaPr GetSourceParaPr(ParaPrExpandFlags flags)
        {
            ParaPr sourcePr = this;

            if (FormatRevision != null)
            {
                if ((flags & ParaPrExpandFlags.Revised) != 0)
                {
                    sourcePr = this.Clone();
                    sourcePr.AcceptFormatRevision();
                }
                else if ((flags & ParaPrExpandFlags.AfterChanges) != 0)
                {
                    sourcePr = (ParaPr)this.FormatRevision.RevPr;
                }
            }

            return sourcePr;
        }

        /// <summary>
        /// Removes frame position inheritance issue.
        /// </summary>
        /// <remarks>
        /// AM. FrameTop + FrameVerticalAlignment (FrameLeft + FrameHorizontalAlignment)
        /// should be joined to one attribute the same way as it done for LineSpacing.
        /// This is temporary solution that mimics this.
        /// </remarks>
        internal void FrameInheritanceHack(ParaPr dstParaPr)
        {
            if (FrameVerticalAlignment != VerticalAlignment.None)
                dstParaPr.Remove(ParaAttr.FrameTop);
            else if (FrameTop > 0)
                dstParaPr.Remove(ParaAttr.FrameVerticalAlignment);

            if (FrameHorizontalAlignment != HorizontalAlignment.None)
                dstParaPr.Remove(ParaAttr.FrameLeft);
            else if (FrameLeft > 0)
                dstParaPr.Remove(ParaAttr.FrameHorizontalAlignment);
        }

        /// <summary>
        /// Returns true if a specified indent is valid for the given format.
        /// </summary>
        internal static bool IsValidIndent(int indent, LoadFormat lf)
        {
            switch (lf)
            {
                case LoadFormat.Doc:
                    return (indent <= DocMaxIndent);
                case LoadFormat.Rtf:
                    return (System.Math.Abs(indent) <= RtfMaxIndent);
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns true, if attribute with a specified key should be processed
        /// in a special way in accordance with ISO29500-1, 17.9.18 numId.
        /// </summary>
        internal static bool IsNoListAttr(int key)
        {
            return (key == ParaAttr.LeftIndent) ||
                   (key == ParaAttr.FirstLineIndent) ||
                   (key == ParaAttr.LeftIndentUnits) ||
                   (key == ParaAttr.FirstLineIndentUnits);
        }

        /// <summary>
        /// Indicates that Istd is changed in format revision.
        /// </summary>
        internal bool HasIstdRevision
        {
            get
            {
                if (FormatRevision == null)
                    return false;

                return (int)FetchAttr(ParaAttr.Istd) != (int)FormatRevision.RevPr.FetchAttr(ParaAttr.Istd);
            }
        }

        /// <summary>
        /// Indicates that the paragraph properties contains list revision.
        /// </summary>
        internal bool HasListRevision
        {
            get
            {
                if (FormatRevision == null)
                    return false;

                // Get final list.
                object val = FormatRevision.RevPr[ParaAttr.ListId];
                if (val == null)
                    return false;

                int listIdFinal = (int)val;

                // Get original list.
                val = this[ParaAttr.ListId];
                int listIdOriginal = (val != null) ? (int)val : 0;

                return listIdOriginal != listIdFinal;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Platform specific optimization")]
        protected override int DefaultCapacity
        {
            get
            {
                // WORDSNET-21593 Reduce default capacity to optimize memory consumption.
                return 2;
            }
        }

        #region IParaAttrSource

        object IParaAttrSource.GetDirectParaAttr(int key)
        {
            return GetDirectAttr(key);
        }

        object IParaAttrSource.GetDirectParaAttr(int key, RevisionsView revisionsView)
        {
            return GetDirectAttr(key, revisionsView);
        }

        /// <summary>
        /// IParaAttrSource.
        /// </summary>
        object IParaAttrSource.FetchInheritedParaAttr(int key)
        {
            return FetchInheritedAttr(key);
        }

        /// <summary>
        /// IParaAttrSource.
        /// </summary>
        object IParaAttrSource.FetchParaAttr(int key)
        {
            object value = GetDirectAttr(key);
            return (value != null) ? value : ((IParaAttrSource)this).FetchInheritedParaAttr(key);
        }

        /// <summary>
        /// IParaAttrSource.
        /// </summary>
        void IParaAttrSource.SetParaAttr(int key, object value)
        {
            SetAttr(key, value);
        }

        /// <summary>
        /// IParaAttrSource.
        /// </summary>
        void IParaAttrSource.RemoveParaAttr(int key)
        {
            Remove(key);
        }

        /// <summary>
        /// IParaAttrSource.
        /// </summary>
        void IParaAttrSource.ClearParaAttrs()
        {
            Clear();
        }

        #endregion

        static ParaPr()
        {
            gDefaults = new ParaPr();

            gDefaults.Add(ParaAttr.Istd, StyleIndex.Normal);
            gDefaults.Add(ParaAttr.Alignment, ParagraphAlignment.Left);

            gDefaults.Add(ParaAttr.NoSpaceBetweenSameStyle, false);
            gDefaults.Add(ParaAttr.SideBySide, false);
            gDefaults.Add(ParaAttr.KeepTogether, false);
            gDefaults.Add(ParaAttr.KeepWithNext, false);
            gDefaults.Add(ParaAttr.PageBreakBefore, false);
            gDefaults.Add(ParaAttr.SuppressLineNumbers, false);
            gDefaults.Add(ParaAttr.SuppressAutoHyphens, false);
            gDefaults.Add(ParaAttr.WidowControl, true);
            gDefaults.Add(ParaAttr.MirrorIndents, false);

            gDefaults.Add(ParaAttr.FarEastLineBreakControl, true);              // As per OOXML Part 4, 2.3.1.16
            gDefaults.Add(ParaAttr.WordWrap, true);             // As per OOXML Part 4, 2.3.1.45
            gDefaults.Add(ParaAttr.HangingPunctuation, true);  // As per OOXML Part 4, 2.3.1.21
            gDefaults.Add(ParaAttr.TopLinePunctuation, false);
            gDefaults.Add(ParaAttr.AddSpaceBetweenFarEastAndAlpha, true); // Default value for all formats except RTF and ODT.
            gDefaults.Add(ParaAttr.AddSpaceBetweenFarEastAndDigit, true); // Default value for all formats except RTF and ODT.
            gDefaults.Add(ParaAttr.AutoAdjustRightIndent, true); // See "part 1 reference c051463_ISOIEC 29500-1_2008(E) 17.3.1.1 adjustRightInd"
            gDefaults.Add(ParaAttr.BaselineAlignment, BaselineAlignment.Auto);
            gDefaults.Add(ParaAttr.SnapToGrid, true);
            gDefaults.Add(ParaAttr.Bidi, false);
            gDefaults.Add(ParaAttr.RsidP, 0);

            gDefaults.Add(ParaAttr.ListLevel, 0);
            gDefaults.Add(ParaAttr.ListId, 0);
            gDefaults.Add(ParaAttr.OutlineLevel, OutlineLevel.BodyText);

            gDefaults.Add(ParaAttr.LeftIndent, 0);
            gDefaults.Add(ParaAttr.RightIndent, 0);
            gDefaults.Add(ParaAttr.FirstLineIndent, 0);
            gDefaults.Add(ParaAttr.SpaceBeforeAuto, false);
            gDefaults.Add(ParaAttr.SpaceAfterAuto, false);
            gDefaults.Add(ParaAttr.SpaceBefore, 0);
            gDefaults.Add(ParaAttr.SpaceAfter, 0);
            gDefaults.Add(ParaAttr.SpaceBeforeUnits, 0);
            gDefaults.Add(ParaAttr.SpaceAfterUnits, 0);
            gDefaults.Add(ParaAttr.LeftIndentUnits, 0);
            gDefaults.Add(ParaAttr.RightIndentUnits, 0);
            gDefaults.Add(ParaAttr.FirstLineIndentUnits, 0);

            // These are complex attributes. Be careful not to modify the actual values in them.
            gDefaults.Add(ParaAttr.TabStops, new TabStopCollection());
            gDefaults.Add(ParaAttr.BorderTop, new Border());
            gDefaults.Add(ParaAttr.BorderLeft, new Border());
            gDefaults.Add(ParaAttr.BorderBottom, new Border());
            gDefaults.Add(ParaAttr.BorderRight, new Border());
            gDefaults.Add(ParaAttr.BorderBetween, new Border());
            gDefaults.Add(ParaAttr.BorderBar, new Border());
            gDefaults.Add(ParaAttr.Shading, new Shading());

            gDefaults.Add(ParaAttr.FrameWidth, 0);
            gDefaults.Add(ParaAttr.FrameHeight, new Height(HeightRule.Auto, 0));
            gDefaults.Add(ParaAttr.FrameLeft, 0);
            gDefaults.Add(ParaAttr.FrameTop, 0);
            gDefaults.Add(ParaAttr.FrameHorizontalAlignment, HorizontalAlignment.None);
            gDefaults.Add(ParaAttr.FrameVerticalAlignment, VerticalAlignment.None);
            gDefaults.Add(ParaAttr.FrameLockAnchor, false);
            gDefaults.Add(ParaAttr.FrameRelativeHorizontalPosition, RelativeHorizontalPosition.Default);
            gDefaults.Add(ParaAttr.FrameRelativeVerticalPosition, RelativeVerticalPosition.TableDefault);
            gDefaults.Add(ParaAttr.FrameWrapType, WrapType.Inline);
            gDefaults.Add(ParaAttr.FrameHorizontalDistanceFromText, 0);
            gDefaults.Add(ParaAttr.FrameVerticalDistanceFromText, 0);
            gDefaults.Add(ParaAttr.DropCapLinesToDrop, 0);
            gDefaults.Add(ParaAttr.DropCapPosition, DropCapPosition.None);
            gDefaults.Add(ParaAttr.FrameTextOrientation, TextOrientation.Horizontal);
            gDefaults.Add(ParaAttr.FrameSuppressOverlap, false);
            gDefaults.Add(ParaAttr.HtmlBlockId, 0);
            gDefaults.Add(ParaAttr.TextboxTightWrap, TextboxTightWrap.None);
            gDefaults.Add(ParaAttr.HtmlMarginLeft, 0);
            gDefaults.Add(ParaAttr.HtmlMarginRight, 0);
            gDefaults.Add(ParaAttr.HtmlMarginTop, 0);
            gDefaults.Add(ParaAttr.HtmlMarginBottom, 0);

            gDefaults.Add(ParaAttr.LineSpacing, new LineSpacing(240, LineSpacingRule.Multiple));

            // Need to have defaults for these so we can collapse during RTF import.
            gDefaults.Add(ParaAttr.Sys_Alignment97, ParagraphAlignment.Left);
            gDefaults.Add(ParaAttr.Sys_FirstLineIndent97, 0);
            gDefaults.Add(ParaAttr.Sys_LeftIndent97, 0);
            gDefaults.Add(ParaAttr.Sys_RightIndent97, 0);

            gDefaults.Add(ParaAttr.Collapsed, false);

            PossibleBorderKeys.Add(BorderType.Top, ParaAttr.BorderTop);
            PossibleBorderKeys.Add(BorderType.Left, ParaAttr.BorderLeft);
            PossibleBorderKeys.Add(BorderType.Bottom, ParaAttr.BorderBottom);
            PossibleBorderKeys.Add(BorderType.Right, ParaAttr.BorderRight);
            PossibleBorderKeys.Add(BorderType.Horizontal, ParaAttr.BorderBetween);
            PossibleBorderKeys.Add(BorderType.Vertical, ParaAttr.BorderBar);
        }

        /// <summary>
        /// The key is <see cref="BorderType"/> that represents a border that can occur in a <see cref="ParaPr"/> collection.
        /// The value is an <see cref="ParaAttr"/> integer that represents the attribute key of that border.
        /// </summary>
        internal static readonly SortedList<BorderType, int> PossibleBorderKeys = new SortedList<BorderType, int>();

        private static readonly ParaPr gDefaults;

        /// <summary>
        /// Specifies what attributes to preserve during <see cref="AcceptFormatRevision"/> in presence of changed style.
        /// </summary>
        private static readonly int[] gRevisionPreservedAttributes = new int[] { RevisionAttr.NumberRevision };

        /// <summary>
        /// Maximum allowed in Word indent for DOC format.
        /// AM. Although spec says that indent can be equal to MaxIndent value (31680)
        /// but in Jira4880 such value seems should be ignored.
        /// </summary>
        private const int DocMaxIndent = 31679;

        /// <summary>
        /// Maximum allowed in Word indent for RTF format.
        /// </summary>
        private const int RtfMaxIndent = 31681;
    }
}
