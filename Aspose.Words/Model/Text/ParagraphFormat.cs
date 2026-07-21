// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2005 by Roman Korchagin
using System;
using System.Collections.Generic;
using Aspose.Words.Drawing;

namespace Aspose.Words
{
    /// <summary>
    /// Represents all the formatting for a paragraph.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-paragraphs/">Working with Paragraphs</a> documentation article.</para>
    /// </summary>
    public class ParagraphFormat : IBorderAttrSource, IShadingAttrSource
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="parent">The object that provides paragraph attributes.</param>
        /// <param name="styles">The styles of the document, needed for resolution of some properties.
        /// It is possible to pass <c>null</c> here, but some of the properties will throw.</param>
        internal ParagraphFormat(IParaAttrSource parent, StyleCollection styles)
        {
            mParent = parent;
            mStyles = styles;
        }

        /// <summary>
        /// Resets to default paragraph formatting.
        /// </summary>
        /// <remarks>
        /// Default paragraph formatting is Normal style, left aligned, no indentation,
        /// no spacing, no borders and no shading.
        /// </remarks>
        public void ClearFormatting()
        {
            mParent.ClearParaAttrs();
        }

        /// <summary>
        /// Gets or sets text alignment for the paragraph.
        /// </summary>
        public ParagraphAlignment Alignment
        {
            get { return (ParagraphAlignment)mParent.FetchParaAttr(ParaAttr.Alignment); }
            set { mParent.SetParaAttr(ParaAttr.Alignment, value); }
        }

        /// <summary>
        /// Gets or sets fonts vertical position on a line.
        /// </summary>
        public BaselineAlignment BaselineAlignment
        {
            get { return (BaselineAlignment)mParent.FetchParaAttr(ParaAttr.BaselineAlignment); }
            set { mParent.SetParaAttr(ParaAttr.BaselineAlignment, value); }
        }

        internal bool IsJustified
        {
            get
            {
                return
                    ((Alignment == ParagraphAlignment.Justify) ||
                    (Alignment == ParagraphAlignment.Distributed));
            }
        }

        /// <summary>
        /// When <c>true</c>, <see cref="SpaceBefore"/> and <see cref="SpaceAfter"/> will be ignored
        /// between the paragraphs of the same style.
        /// </summary>
        /// <remarks>
        /// <p>This setting only takes affect when applied to a paragraph style. If applied to
        /// a paragraph directly, it has no effect.</p>
        /// </remarks>
        public bool NoSpaceBetweenParagraphsOfSameStyle
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.NoSpaceBetweenSameStyle); }
            set { mParent.SetParaAttr(ParaAttr.NoSpaceBetweenSameStyle, value); }
        }

        internal bool SideBySide
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.SideBySide); }
            set { mParent.SetParaAttr(ParaAttr.SideBySide, value); }
        }

        /// <summary>
        /// True if all lines in the paragraph are to remain on the same page.
        /// </summary>
        public bool KeepTogether
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.KeepTogether); }
            set { mParent.SetParaAttr(ParaAttr.KeepTogether, value); }
        }

        /// <summary>
        /// True if the paragraph is to remains on the same page as the paragraph that follows it.
        /// </summary>
        public bool KeepWithNext
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.KeepWithNext); }
            set { mParent.SetParaAttr(ParaAttr.KeepWithNext, value); }
        }

        /// <summary>
        /// True if a page break is forced before the paragraph.
        /// </summary>
        public bool PageBreakBefore
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.PageBreakBefore); }
            set { mParent.SetParaAttr(ParaAttr.PageBreakBefore, value); }
        }

        /// <summary>
        /// Specifies whether the current paragraph's lines should be exempted from line numbering
        /// which is applied in the parent section.
        /// </summary>
        public bool SuppressLineNumbers
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.SuppressLineNumbers); }
            set { mParent.SetParaAttr(ParaAttr.SuppressLineNumbers, value); }
        }

        /// <summary>
        /// Specifies whether the current paragraph should be exempted from any hyphenation which
        /// is applied in the document settings.
        /// </summary>
        public bool SuppressAutoHyphens
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.SuppressAutoHyphens); }
            set { mParent.SetParaAttr(ParaAttr.SuppressAutoHyphens, value); }
        }

        /// <summary>
        /// True if the first and last lines in the paragraph are to remain on the same page as the rest of the paragraph.
        /// </summary>
        public bool WidowControl
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.WidowControl); }
            set { mParent.SetParaAttr(ParaAttr.WidowControl, value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether inter-character spacing is automatically adjusted between regions
        /// of Latin text and regions of East Asian text in the current paragraph.
        /// </summary>
        public bool AddSpaceBetweenFarEastAndAlpha
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.AddSpaceBetweenFarEastAndAlpha); }
            set { mParent.SetParaAttr(ParaAttr.AddSpaceBetweenFarEastAndAlpha, value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether inter-character spacing is automatically adjusted between regions
        /// of numbers and regions of East Asian text in the current paragraph.
        /// </summary>
        public bool AddSpaceBetweenFarEastAndDigit
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.AddSpaceBetweenFarEastAndDigit); }
            set { mParent.SetParaAttr(ParaAttr.AddSpaceBetweenFarEastAndDigit, value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether East Asian line-breaking rules are applied to the current paragraph.
        /// </summary>
        public bool FarEastLineBreakControl
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.FarEastLineBreakControl); }
            set { mParent.SetParaAttr(ParaAttr.FarEastLineBreakControl, value); }
        }

        /// <summary>
        /// If this property is <c>false</c>, Latin text in the middle of a word can be wrapped for
        /// the current paragraph. Otherwise Latin text is wrapped by whole words.
        /// </summary>
        public bool WordWrap
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.WordWrap); }
            set { mParent.SetParaAttr(ParaAttr.WordWrap, value); }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether hanging punctuation is enabled for the current paragraph.
        /// </summary>
        public bool HangingPunctuation
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.HangingPunctuation); }
            set { mParent.SetParaAttr(ParaAttr.HangingPunctuation, value); }
        }

        internal bool TopLinePunctuation
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.TopLinePunctuation); }
            set { mParent.SetParaAttr(ParaAttr.TopLinePunctuation, value); }
        }

        internal bool AutoAdjustRightIndent
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.AutoAdjustRightIndent); }
            set { mParent.SetParaAttr(ParaAttr.AutoAdjustRightIndent, value); }
        }

        /// <summary>
        /// Gets or sets whether this is a right-to-left paragraph.
        /// </summary>
        /// <remarks>
        /// <p>When <c>true</c>, the runs and other inline objects in this paragraph
        /// are laid out right to left.</p>
        /// </remarks>
        public bool Bidi
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.Bidi); }
            set { mParent.SetParaAttr(ParaAttr.Bidi, value); }
        }

        /// <summary>
        /// Gets or sets the value (in points) that represents the left indent for paragraph.
        /// </summary>
        public double LeftIndent
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.LeftIndent)); }
            set
            {
                mParent.RemoveParaAttr(ParaAttr.LeftIndentUnits);
                mParent.SetParaAttr(ParaAttr.LeftIndent, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets or sets the left indent value (in characters) for the specified paragraphs.
        /// </summary>
        public double CharacterUnitLeftIndent
        {
            get { return ((int)mParent.FetchParaAttr(ParaAttr.LeftIndentUnits)) / 100.0; }
            set { mParent.SetParaAttr(ParaAttr.LeftIndentUnits, (int)(value * 100)); }
        }

        /// <summary>
        /// Gets or sets the value (in points) that represents the right indent for paragraph.
        /// </summary>
        public double RightIndent
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.RightIndent)); }
            set
            {
                mParent.RemoveParaAttr(ParaAttr.RightIndentUnits);
                mParent.SetParaAttr(ParaAttr.RightIndent, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets or sets the right indent value (in characters) for the specified paragraphs.
        /// </summary>
        public double CharacterUnitRightIndent
        {
            get { return ((int)mParent.FetchParaAttr(ParaAttr.RightIndentUnits)) / 100.0; }
            set { mParent.SetParaAttr(ParaAttr.RightIndentUnits, (int)(value * 100)); }
        }

        /// <summary>
        /// Gets or sets the value (in points) for a first line or hanging indent.
        /// <p>Use positive values to set the first-line indent, and negative values to set the hanging indent.</p>
        /// </summary>
        public double FirstLineIndent
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FirstLineIndent)); }
            set
            {
                mParent.RemoveParaAttr(ParaAttr.FirstLineIndentUnits);
                mParent.SetParaAttr(ParaAttr.FirstLineIndent, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets or sets the value (in characters) for the first-line or hanging indent.
        /// <p>Use positive values to set the first-line indent, and negative values to set the hanging indent.</p>
        /// </summary>
        public double CharacterUnitFirstLineIndent
        {
            get { return ((int)mParent.FetchParaAttr(ParaAttr.FirstLineIndentUnits)) / 100.0; }
            set { mParent.SetParaAttr(ParaAttr.FirstLineIndentUnits, (int)(value * 100)); }
        }

        /// <summary>
        /// True if the amount of spacing before the paragraph is set automatically.
        /// </summary>
        /// <remarks>
        /// <p>When set to <c>true</c>, overrides the effect of <see cref="SpaceBefore"/>.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="ParagraphFormat.AutoSpacing"]/*'/>
        public bool SpaceBeforeAuto
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.SpaceBeforeAuto); }
            set { mParent.SetParaAttr(ParaAttr.SpaceBeforeAuto, value); }
        }

        /// <summary>
        /// True if the amount of spacing after the paragraph is set automatically.
        /// </summary>
        /// <remarks>
        /// <p>When set to <c>true</c>, overrides the effect of <see cref="SpaceAfter"/>.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="ParagraphFormat.AutoSpacing"]/*'/>
        public bool SpaceAfterAuto
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.SpaceAfterAuto); }
            set { mParent.SetParaAttr(ParaAttr.SpaceAfterAuto, value); }
        }

        /// <summary>
        /// Gets or sets the amount of spacing (in points) before the paragraph.
        /// </summary>
        /// <remarks>
        /// <p>Has no effect when <see cref="SpaceBeforeAuto"/> is <c>true</c>.</p>
        /// <p>Valid values range from 0 to 1584 inclusive.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when argument was out of the range of valid values.
        /// </exception>
        public double SpaceBefore
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.SpaceBefore)); }
            set
            {
                ArgumentUtil.ValidateRange(value, 0, 0, 1584, 1584, true, "SpaceBefore");

                // AM. Actually Word does not remove units when normal spacing/indent set but
                // lets postpone till customer requests.
                mParent.RemoveParaAttr(ParaAttr.SpaceBeforeUnits);
                mParent.SetParaAttr(ParaAttr.SpaceBefore, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets or sets the amount of spacing (in gridlines) before the paragraphs.
        /// </summary>
        public double LineUnitBefore
        {
            get { return ((int)mParent.FetchParaAttr(ParaAttr.SpaceBeforeUnits)) / 100.0; }
            set { mParent.SetParaAttr(ParaAttr.SpaceBeforeUnits, (int)(value * 100)); }
        }

        /// <summary>
        /// Gets or sets the amount of spacing (in points) after the paragraph.
        /// </summary>
        /// <remarks>
        /// <p>Has no effect when <see cref="SpaceAfterAuto"/> is <c>true</c>.</p>
        /// <p>Valid values ​​range from 0 to 1584 inclusive.</p>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when argument was out of the range of valid values.
        /// </exception>
        public double SpaceAfter
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.SpaceAfter)); }
            set
            {
                // WORDSNET-19480 Added validation.
                ArgumentUtil.ValidateRange(value, 0, 0, 1584, 1584, true, "SpaceAfter");

                mParent.RemoveParaAttr(ParaAttr.SpaceAfterUnits);
                mParent.SetParaAttr(ParaAttr.SpaceAfter, ConvertUtilCore.PointToTwip(value));
            }
        }

        /// <summary>
        /// Gets or sets the amount of spacing (in gridlines) after the paragraphs.
        /// </summary>
        public double LineUnitAfter
        {
            get { return ((int)mParent.FetchParaAttr(ParaAttr.SpaceAfterUnits)) / 100.0; }
            set { mParent.SetParaAttr(ParaAttr.SpaceAfterUnits, (int)(value * 100)); }
        }

        /// <summary>
        /// Gets or sets the line spacing for the paragraph.
        /// </summary>
        public LineSpacingRule LineSpacingRule
        {
            get { return ((LineSpacing)mParent.FetchParaAttr(ParaAttr.LineSpacing)).Rule; }
            set
            {
                LineSpacing lineSpacing = (LineSpacing)FetchOrCreateComplexParaAttr(ParaAttr.LineSpacing);
                lineSpacing.Rule = value;
            }
        }

        /// <summary>
        /// Gets or sets the line spacing (in points) for the paragraph.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="LineSpacingRule"/> property is set to <see cref="LineSpacingRule.AtLeast"/>, the line spacing can be greater than or equal to,
        /// but never less than the specified <see cref="LineSpacing"/> value.</p>
        /// <p>When <see cref="LineSpacingRule"/> property is set to <see cref="LineSpacingRule.Exactly"/>, the line spacing never changes from
        /// the specified <see cref="LineSpacing"/> value, even if a larger font is used within the paragraph.</p>
        /// </remarks>
        public double LineSpacing
        {
            get { return ConvertUtilCore.TwipToPoint(((LineSpacing)mParent.FetchParaAttr(ParaAttr.LineSpacing)).Value); }
            set
            {
                LineSpacing lineSpacing = (LineSpacing)FetchOrCreateComplexParaAttr(ParaAttr.LineSpacing);
                lineSpacing.Value = ConvertUtilCore.PointToTwip(value);
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the left and right indents are of the same width.
        /// </summary>
        public bool MirrorIndents
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.MirrorIndents); }
            set { mParent.SetParaAttr(ParaAttr.MirrorIndents, value); }
        }

        /// <summary>
        /// Gets the LineSpacing / 12 value.
        /// </summary>
        internal double LineSpacingInLines
        {
            get { return ConvertUtilCore.PointToLine(LineSpacing); }
        }

        /// <summary>
        /// True when the paragraph style is one of the built-in Heading styles.
        /// </summary>
        public bool IsHeading
        {
            get { return Style.IsHeading; }
        }

        /// <summary>
        /// True when the paragraph is an item in a bulleted or numbered list.
        /// </summary>
        public bool IsListItem
        {
            get { return (ListId != 0); }
        }

        /// <summary>
        /// In Word documents, lists may consist of up to nine levels, numbered 0 to 8.
        /// </summary>
        internal int ListLevel
        {
            get { return (int)mParent.FetchParaAttr(ParaAttr.ListLevel); }
            set { mParent.SetParaAttr(ParaAttr.ListLevel, value); }
        }

        /// <summary>
        /// This is actually ilfo. I should probably rename it into ilfo.
        /// </summary>
        internal int ListId
        {
            // RK: Here used to be code that looked like Istd getter but I think it was no longer needed.
            get { return (int)mParent.FetchParaAttr(ParaAttr.ListId); }
            set { mParent.SetParaAttr(ParaAttr.ListId, value); }
        }

        /// <summary>
        /// Specifies the outline level of the paragraph in the document.
        /// </summary>
        public OutlineLevel OutlineLevel
        {
            get { return (OutlineLevel)mParent.FetchParaAttr(ParaAttr.OutlineLevel); }
            set
            {
                if ((value < OutlineLevel.Level1) || (value > OutlineLevel.BodyText))
                    throw new ArgumentOutOfRangeException("value");

                mParent.SetParaAttr(ParaAttr.OutlineLevel, value);
            }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal double FrameWidth
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameWidth)); }
            set { mParent.SetParaAttr(ParaAttr.FrameWidth, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal HeightRule FrameHeightRule
        {
            get
            {
                Height height = (Height)mParent.FetchParaAttr(ParaAttr.FrameHeight);
                return height.Rule;
            }
            set
            {
                Height height = (Height)FetchOrCreateComplexParaAttr(ParaAttr.FrameHeight);
                height.Rule = value;
            }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal double FrameHeight
        {
            get
            {
                Height height = (Height)mParent.FetchParaAttr(ParaAttr.FrameHeight);
                return ConvertUtilCore.TwipToPoint(height.Value);
            }
            set
            {
                Height height = (Height)FetchOrCreateComplexParaAttr(ParaAttr.FrameHeight);
                height.Value = ConvertUtilCore.PointToTwip(value);
            }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal double FrameLeft
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameLeft)); }
            set { mParent.SetParaAttr(ParaAttr.FrameLeft, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal double FrameTop
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameTop)); }
            set { mParent.SetParaAttr(ParaAttr.FrameTop, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)mParent.FetchParaAttr(ParaAttr.FrameHorizontalAlignment); }
            set { mParent.SetParaAttr(ParaAttr.FrameHorizontalAlignment, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)mParent.FetchParaAttr(ParaAttr.FrameVerticalAlignment); }
            set { mParent.SetParaAttr(ParaAttr.FrameVerticalAlignment, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal bool FrameLockAnchor
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.FrameLockAnchor); }
            set { mParent.SetParaAttr(ParaAttr.FrameLockAnchor, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal RelativeHorizontalPosition RelativeHorizontalPosition
        {
            get { return (RelativeHorizontalPosition)mParent.FetchParaAttr(ParaAttr.FrameRelativeHorizontalPosition); }
            set { mParent.SetParaAttr(ParaAttr.FrameRelativeHorizontalPosition, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal RelativeVerticalPosition RelativeVerticalPosition
        {
            get { return (RelativeVerticalPosition)mParent.FetchParaAttr(ParaAttr.FrameRelativeVerticalPosition); }
            set { mParent.SetParaAttr(ParaAttr.FrameRelativeVerticalPosition, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal WrapType WrapType
        {
            get { return (WrapType)mParent.FetchParaAttr(ParaAttr.FrameWrapType); }
            set { mParent.SetParaAttr(ParaAttr.FrameWrapType, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal double FrameHorizontalDistanceFromText
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameHorizontalDistanceFromText)); }
            set { mParent.SetParaAttr(ParaAttr.FrameHorizontalDistanceFromText, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal double FrameVerticalDistanceFromText
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameVerticalDistanceFromText)); }
            set { mParent.SetParaAttr(ParaAttr.FrameVerticalDistanceFromText, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal TextOrientation FrameTextOrientation
        {
            get { return (TextOrientation)mParent.FetchParaAttr(ParaAttr.FrameTextOrientation); }
            set { mParent.SetParaAttr(ParaAttr.FrameTextOrientation, value); }
        }

        /// <summary>
        /// Text frame.
        /// </summary>
        internal bool FrameSuppressOverlap
        {
            get { return (bool) mParent.FetchParaAttr(ParaAttr.FrameSuppressOverlap); }
            set { mParent.SetParaAttr(ParaAttr.FrameSuppressOverlap, value); }
        }

        /// <summary>
        /// Gets or sets the number of lines of the paragraph text used to calculate the drop cap height.
        /// </summary>
        public int LinesToDrop
        {
            get { return (int)mParent.FetchParaAttr(ParaAttr.DropCapLinesToDrop); }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");

                mParent.SetParaAttr(ParaAttr.DropCapLinesToDrop, value);
            }
        }

        /// <summary>
        /// Gets or sets the position for a drop cap text.
        /// </summary>
        public DropCapPosition DropCapPosition
        {
            get { return (DropCapPosition)mParent.FetchParaAttr(ParaAttr.DropCapPosition); }
            set
            {
                if ((mParent.GetDirectParaAttr(ParaAttr.DropCapPosition) == null) && (value == DropCapPosition.None))
                    return;

                mParent.SetParaAttr(ParaAttr.DropCapPosition, value);

                // andrnosk: WORDSNET-6606 We need to set specific FrameRelativeHorizontalPosition and
                // FrameRelativeVerticalPosition upon specifying DropCapPosition.
                SetDropCapFrameRelativePosition(value);
            }
        }

        /// <summary>
        /// Set specific DropCap Frame Relative position.
        /// </summary>
        private void SetDropCapFrameRelativePosition(DropCapPosition value)
        {
            switch (value)
            {
                case DropCapPosition.Margin:
                    {
                        mParent.SetParaAttr(ParaAttr.FrameRelativeHorizontalPosition, RelativeHorizontalPosition.Page);
                        mParent.SetParaAttr(ParaAttr.FrameRelativeVerticalPosition, RelativeHorizontalPosition.Default);
                        break;
                    }
                case DropCapPosition.Normal:
                    {
                        mParent.SetParaAttr(ParaAttr.FrameRelativeHorizontalPosition, RelativeHorizontalPosition.Default);
                        mParent.SetParaAttr(ParaAttr.FrameRelativeVerticalPosition, RelativeHorizontalPosition.Default);
                        break;
                    }
                case DropCapPosition.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Returns a <see cref="Aspose.Words.Shading"/> object that refers to the shading formatting for the paragraph.
        /// </summary>
        public Shading Shading
        {
            get
            {
                //<<GetOrCreateComplexAttr>> pattern
                Shading shading = (Shading)mParent.GetDirectParaAttr(ParaAttr.Shading);
                if (shading == null)
                {
                    ParagraphFormat parent = this;
                    shading = new Shading(parent, ParaAttr.Shading);
                    shading = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(shading, parent);
                    mParent.SetParaAttr(ParaAttr.Shading, shading);
                }
                return shading;
            }
        }

        /// <summary>
        /// Gets collection of borders of the paragraph.
        /// </summary>
        public BorderCollection Borders
        {
            get
            {
                if (mBorders == null)
                    mBorders = new BorderCollection(this);
                return mBorders;
            }
        }

        /// <summary>
        /// Gets or sets the paragraph style applied to this formatting.
        /// </summary>
        public Style Style
        {
            get
            {
                if (mStyles == null)
                    return null;

                RevisionsView revisionsView = (mStyles.Document.NodeType == NodeType.Document)
                    ? ((Document)mStyles.Document).RevisionsView
                    : RevisionsView.Original;

                // Try to get RevisionViews aware Istd.
                int istd = (mParent.GetDirectParaAttr(ParaAttr.Istd, revisionsView) != null)
                    ? (int)mParent.GetDirectParaAttr(ParaAttr.Istd, revisionsView)
                    : Istd;

                return mStyles.FetchByIstd(istd, StyleIndex.Normal);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value.Document != mStyles.Document)
                    throw new ArgumentException("This style belongs to a different document.");

                if (value.Type != StyleType.Paragraph)
                    throw new ArgumentException("This style is not a paragraph style.");

                Istd = value.Istd;
            }
        }

        /// <summary>
        /// Gets or sets the name of the paragraph style applied to this formatting.
        /// </summary>
        public string StyleName
        {
            get { return Style.Name; }
            set { Style = mStyles.FetchByName(value); }
        }

        /// <summary>
        /// Gets or sets the locale independent style identifier of the paragraph style applied to this formatting.
        /// </summary>
        public StyleIdentifier StyleIdentifier
        {
            get { return Style.StyleIdentifier; }
            set { Style = mStyles.FetchBySti(value); }
        }

        /// <summary>
        /// Specifies whether the current paragraph should use the document grid lines per page settings
        /// when laying out the contents in the paragraph.
        /// </summary>
        public bool SnapToGrid
        {
            get { return (bool)mParent.FetchParaAttr(ParaAttr.SnapToGrid); }
            set { mParent.SetParaAttr(ParaAttr.SnapToGrid, value); }
        }

        internal int Istd
        {
            get
            {
                object istd = mParent.GetDirectParaAttr(ParaAttr.Istd);
                // When getting the style of the paragraph, don't try to retrieve the value from the parent
                // since the parent is the style and it will result in an endless loop.
                return (istd != null) ? (int)istd : (int)ParaPr.FetchDefaultAttr(ParaAttr.Istd);
            }
            set
            {
                mParent.SetParaAttr(ParaAttr.Istd, value);
            }
        }

        /// <summary>
        /// Gets the collection of custom tab stops defined for this object.
        /// </summary>
        public TabStopCollection TabStops
        {
            get
            {
                //<<GetOrCreateComplexAttr>> pattern
                TabStopCollection tabStops = (TabStopCollection)mParent.GetDirectParaAttr(ParaAttr.TabStops);
                if (tabStops == null)
                {
                    tabStops = new TabStopCollection();
                    mParent.SetParaAttr(ParaAttr.TabStops, tabStops);
                }
                return tabStops;
            }
        }

        /// <summary>
        /// Gets the attribute from the attribute collection or from one of the parents.
        /// </summary>
        internal object FetchAttr(int key)
        {
            return mParent.FetchParaAttr(key);
        }

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return mParent.GetDirectParaAttr(key);
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            return mParent.FetchInheritedParaAttr(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            mParent.SetParaAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get { return ParaPr.PossibleBorderKeys; }
        }

        object IShadingAttrSource.FetchInheritedShadingAttr(int key)
        {
            return mParent.FetchInheritedParaAttr(key);
        }

        private object FetchOrCreateComplexParaAttr(int key)
        {
            object directValue = mParent.GetDirectParaAttr(key);

            if (directValue != null)
                return directValue;

            object defaultValue = ParaPr.FetchDefaultAttr(key);

            object clonedValue = ((IComplexAttr)defaultValue).DeepCloneComplexAttr();
            mParent.SetParaAttr(key, clonedValue);

            return clonedValue;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IParaAttrSource mParent;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly StyleCollection mStyles;
        private BorderCollection mBorders;

        /// <summary>
        /// Microsoft Word adds 14 points when auto space after or before is turned on.
        /// </summary>
        internal const float AutoSpaceSize = 14f;

#if DEBUG
        public override string ToString()
        {
            return base.ToString() + '.' + GetHashCode();
        }
#endif
    }
}
