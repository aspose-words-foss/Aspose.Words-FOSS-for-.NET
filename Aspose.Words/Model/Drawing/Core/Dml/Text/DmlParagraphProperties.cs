// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Text.Bullets;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 21.1.2.2.7 pPr (Text Paragraph Properties)
    /// This element contains all paragraph level text properties for the containing paragraph. 
    /// These paragraph properties should override any and all conflicting properties that are 
    /// associated with the paragraph in question.
    /// </summary>
    internal class DmlParagraphProperties : DmlExtensionListSource
    {
        internal DmlParagraphProperties()
        {
            mPropertyBag.ParentBagProvider = gDefaultParentBagProvider;
        }

        internal DmlParagraphProperties Clone()
        {
            DmlParagraphProperties lhs = (DmlParagraphProperties)MemberwiseClone();

            if (mPropertyBag != null)
                lhs.mPropertyBag = mPropertyBag.Clone();

            if (mDefaultRunProperties != null)
                lhs.mDefaultRunProperties = mDefaultRunProperties.Clone();

            DmlTextBullet bullet = (DmlTextBullet)GetDirectProperty(DmlParagraphPropertiesIds.Bullet);
            if (bullet != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.Bullet, bullet.Clone());

            DmlTextBulletColor bulletColor = (DmlTextBulletColor)GetDirectProperty(DmlParagraphPropertiesIds.BulletColor);
            if (bulletColor != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.BulletColor, bulletColor.Clone());

            DmlTextBulletSize bulletSize = (DmlTextBulletSize)GetDirectProperty(DmlParagraphPropertiesIds.BulletSize);
            if (bulletSize != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.BulletSize, bulletSize.Clone());

            DmlTextBulletFont bulletFont = (DmlTextBulletFont)GetDirectProperty(DmlParagraphPropertiesIds.BulletFont);
            if (bulletFont != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.BulletFont, bulletFont.Clone());

            DmlTextSpacing spacing = (DmlTextSpacing)GetDirectProperty(DmlParagraphPropertiesIds.LineSpacing);
            if (spacing != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.LineSpacing, spacing.Clone());

            spacing = (DmlTextSpacing)GetDirectProperty(DmlParagraphPropertiesIds.SpaceAfter);
            if (spacing != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.SpaceAfter, spacing.Clone());

            spacing = (DmlTextSpacing)GetDirectProperty(DmlParagraphPropertiesIds.SpaceBefore);
            if (spacing != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.SpaceBefore, spacing.Clone());

            TabStopCollection tabStops = (TabStopCollection)GetDirectProperty(DmlParagraphPropertiesIds.TabList);
            if (tabStops != null)
                lhs.SetProperty(DmlParagraphPropertiesIds.TabList, tabStops.Clone());

            if (HasExtensions)
                lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Clears the properties so that default formatting is used.
        /// </summary>
        internal void Clear()
        {
            mPropertyBag.RemoveAll();
            mHasDefaultRunProperties = false;
            DefaultRunProperties = null;
        }

        internal object GetDirectProperty(DmlParagraphPropertiesIds propertyId)
        {
            return mPropertyBag.GetDirectProperty((int)propertyId);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="attr">the attr</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlParagraphPropertiesIds attr)
        {
            return mPropertyBag.IsPropertySpecified((int)attr);
        }

        internal void SetParentProperties(DmlTextListStyles textListStyles)
        {
            SetParentProperties(new DmlParagraphParentBagProvider(this, textListStyles));
        }

        internal void SetParentProperties(DmlParagraphProperties parentProperties)
        {
            SetParentProperties(new DmlHierarchicalPropertyBagParentContainer(parentProperties.mPropertyBag));
        }

        internal void SetParentProperties(DmlParagraphPropertiesDefaults parentProperties)
        {
            SetParentProperties(new DmlHierarchicalPropertyBagParentContainer(parentProperties));
        }

        internal void SetParentProperties(IDmlHierarchicalPropertyBagParentProvider parentProvider)
        {
            mPropertyBag.ParentBagProvider = parentProvider;
        }

        private void SetProperty(DmlParagraphPropertiesIds propertyId, object value)
        {
            mPropertyBag.SetProperty((int)propertyId, value);
        }

        private object GetProperty(DmlParagraphPropertiesIds propertyId)
        {
            return mPropertyBag.GetProperty((int)propertyId);
        }

        public override StringToObjDictionary<DmlExtension> Extensions
        {
            get { return (StringToObjDictionary<DmlExtension>)GetProperty(DmlParagraphPropertiesIds.Extensions); }
            set { SetProperty(DmlParagraphPropertiesIds.Extensions, value); }
        }

        internal DmlTextBullet Bullet
        {
            get { return (DmlTextBullet)GetProperty(DmlParagraphPropertiesIds.Bullet); }
            set { SetProperty(DmlParagraphPropertiesIds.Bullet, value); }
        }

        internal DmlTextBulletColor BulletColor
        {
            get { return (DmlTextBulletColor)GetProperty(DmlParagraphPropertiesIds.BulletColor); }
            set { SetProperty(DmlParagraphPropertiesIds.BulletColor, value); }
        }

        internal DmlTextBulletSize BulletSize
        {
            get { return (DmlTextBulletSize)GetProperty(DmlParagraphPropertiesIds.BulletSize); }
            set { SetProperty(DmlParagraphPropertiesIds.BulletSize, value); }
        }

        internal DmlTextBulletFont BulletFont
        {
            get { return (DmlTextBulletFont)GetProperty(DmlParagraphPropertiesIds.BulletFont); }
            set { SetProperty(DmlParagraphPropertiesIds.BulletFont, value); }
        }

        /// <summary>
        /// Specifies the particular level text properties that this paragraph follows. 
        /// The value for this attribute is numerical and formats the text according to 
        /// the corresponding level paragraph properties that are listed within the lstStyle element. 
        /// Since there are nine separate level properties defined, this tag has an effective range of 0-8 = 9 available values.
        /// </summary>
        internal int Level
        {
            get { return (int)GetProperty(DmlParagraphPropertiesIds.Level); }
            set { SetProperty(DmlParagraphPropertiesIds.Level, value); }
        }

        /// <summary>
        /// Specifies the alignment that is to be applied to the paragraph. Possible values for this include left, 
        /// right, centered, justified and distributed. If this attribute is omitted, then a value of left is implied.
        /// </summary>
        internal ParagraphAlignment Alignment
        {
            get { return (ParagraphAlignment)GetProperty(DmlParagraphPropertiesIds.Alignment); }
            set { SetProperty(DmlParagraphPropertiesIds.Alignment, value); }
        }

        /// <summary>
        /// Specifies the default size for a tab character within this paragraph. This attribute should be used to describe 
        /// the spacing of tabs within the paragraph instead of a leading indentation tab. For indentation tabs there are 
        /// the marL and indent attributes to assist with this. 
        /// </summary>
        internal int DefaultTabSize
        {
            get { return (int)GetProperty(DmlParagraphPropertiesIds.DefaultTabSize); }
            set { SetProperty(DmlParagraphPropertiesIds.DefaultTabSize, value); }
        }

        /// <summary>
        /// Specifies whether an East Asian word can be broken in half and wrapped onto the next line without 
        /// a hyphen being added. To determine whether an East Asian word can be broken the presentation 
        /// application would use the kinsoku settings here. This attribute is to be used specifically when 
        /// there is a word that cannot be broken into multiple pieces without a hyphen. That is it is not present 
        /// within the existence of normal breakable East Asian words but is when a special case word arises that 
        /// should not be broken for a line break. If this attribute is omitted, then a value of 1 or true is implied.
        /// </summary>
        internal bool IsEastAsianLineBreakAllowed
        {
            get { return (bool)GetProperty(DmlParagraphPropertiesIds.IsEastAsianLineBreakAllowed); }
            set { SetProperty(DmlParagraphPropertiesIds.IsEastAsianLineBreakAllowed, value); }
        }

        /// <summary>
        /// Determines where vertically on a line of text the actual words are positioned. This deals with vertical 
        /// placement of the characters with respect to the baselines. For instance having text anchored to the top 
        /// baseline, anchored to the bottom baseline, centered in between, etc. To understand this attribute and 
        /// it's use it is helpful to understand what baselines are. A diagram describing these different cases 
        /// is shown below. If this attribute is omitted, then a value of base is implied.
        /// </summary>
        internal DmlFontAlignment FontAlignment
        {
            get { return (DmlFontAlignment)GetProperty(DmlParagraphPropertiesIds.FontAlignment); }
            set { SetProperty(DmlParagraphPropertiesIds.FontAlignment, value); }
        }

        /// <summary>
        /// Specifies whether punctuation is to be forcefully laid out on a line of text or put on a different line of text. 
        /// That is, if there is punctuation at the end of a run of text that should be carried over to a separate line 
        /// does it actually get carried over. A true value allows for hanging punctuation forcing the punctuation to 
        /// not be carried over and a value of false allows the punctuation to be carried onto the next text line. 
        /// If this attribute is omitted, then a value of 0, or false is implied.
        /// </summary>
        internal bool IsHangingPunctuationAllowed
        {
            get { return (bool)GetProperty(DmlParagraphPropertiesIds.IsHangingPunctuationAllowed); }
            set { SetProperty(DmlParagraphPropertiesIds.IsHangingPunctuationAllowed, value); }
        }

        /// <summary>
        /// Specifies the indent size that is applied to the first line of text in the paragraph. 
        /// An indentation of 0 is considered to be at the same location as marL attribute. 
        /// If this attribute is omitted, then a value of -342900 is implied.
        /// </summary>
        internal int TextIdentation
        {
            get { return (int)GetProperty(DmlParagraphPropertiesIds.TextIdentation); }
            set { SetProperty(DmlParagraphPropertiesIds.TextIdentation, value); }
        }

        /// <summary>
        /// Specifies whether a Latin word can be broken in half and wrapped onto the next line without a hyphen being added. 
        /// This attribute is to be used specifically when there is a word that cannot be broken into multiple pieces without 
        /// a hyphen. It is not present within the existence of normal breakable Latin words but is when a special case word 
        /// arises that should not be broken for a line break. If this attribute is omitted, then a value of 1 or true is implied.
        /// </summary>
        internal bool IsLatinLineBreakAllowed
        {
            get { return (bool)GetProperty(DmlParagraphPropertiesIds.IsLatinLineBreakAllowed); }
            set { SetProperty(DmlParagraphPropertiesIds.IsLatinLineBreakAllowed, value); }
        }

        /// <summary>
        /// Specifies the left margin of the paragraph. This is specified in addition to the text body inset and applies 
        /// only to this text paragraph. That is the text body inset and the marL attributes are additive with respect 
        /// to the text position. If this attribute is omitted, then a value of 347663 is implied.
        /// </summary>
        internal int LeftMargin
        {
            get { return (int)GetProperty(DmlParagraphPropertiesIds.LeftMargin); }
            set { SetProperty(DmlParagraphPropertiesIds.LeftMargin, value); }
        }

        /// <summary>
        /// Specifies the right margin of the paragraph. This is specified in addition to the text body inset and applies 
        /// only to this text paragraph. That is the text body inset and the marR attributes are additive with respect to 
        /// the text position. If this attribute is omitted, then a value of 0 is implied.
        /// </summary>
        internal int RightMargin
        {
            get { return (int)GetProperty(DmlParagraphPropertiesIds.RightMargin); }
            set { SetProperty(DmlParagraphPropertiesIds.RightMargin, value); }
        }

        /// <summary>
        /// Specifies whether the text is right-to-left or left-to-right in its flow direction. 
        /// If this attribute is omitted, then a value of 0, or left-to-right is implied.
        /// </summary>
        internal bool RightToLeftFlowDirection
        {
            get { return (bool)GetProperty(DmlParagraphPropertiesIds.RightToLeftFlowDirection); }
            set { SetProperty(DmlParagraphPropertiesIds.RightToLeftFlowDirection, value); }
        }

        internal DmlRunProperties DefaultRunProperties
        {
            get
            {
                if (mDefaultRunProperties == null)
                    mDefaultRunProperties = new DmlRunProperties();
                return mDefaultRunProperties;
            }

            set { mDefaultRunProperties = value; }
        }

        /// <summary>
        /// Specifies the vertical line spacing that is to be used within a paragraph. 
        /// This can be specified in two different ways, percentage spacing and font point spacing. 
        /// If this element is omitted then the spacing between two lines of text should be determined by 
        /// the point size of the largest piece of text within a line.
        /// </summary>
        internal DmlTextSpacing LineSpacing
        {
            get { return (DmlTextSpacing)GetProperty(DmlParagraphPropertiesIds.LineSpacing); }
            set { SetProperty(DmlParagraphPropertiesIds.LineSpacing, value); }
        }

        /// <summary>
        /// Specifies the amount of vertical white space that is present after a paragraph. 
        /// </summary>
        internal DmlTextSpacing SpaceAfter
        {
            get { return (DmlTextSpacing)GetProperty(DmlParagraphPropertiesIds.SpaceAfter); }
            set { SetProperty(DmlParagraphPropertiesIds.SpaceAfter, value); }
        }

        /// <summary>
        /// Specifies the amount of vertical white space that is present before a paragraph.
        /// </summary>
        internal DmlTextSpacing SpaceBefore
        {
            get { return (DmlTextSpacing)GetProperty(DmlParagraphPropertiesIds.SpaceBefore); }
            set { SetProperty(DmlParagraphPropertiesIds.SpaceBefore, value); }
        }

        /// <summary>
        /// Specifies the list of all tab stops that are to be used within a paragraph.
        /// </summary>
        internal TabStopCollection TabList
        {
            get { return (TabStopCollection)GetProperty(DmlParagraphPropertiesIds.TabList); }
            set { SetProperty(DmlParagraphPropertiesIds.TabList, value); }
        }

        /// <summary>
        /// Returns number of properties set explicitly.
        /// </summary>
        internal int Count
        {
            get { return mPropertyBag.Count; }
        }

        /// <summary>
        /// Returns true if paragraph has bullet.
        /// </summary>
        internal bool HasBullet
        {
            get { return (Bullet != null) && (Bullet.BulletType != DmlTextBulletType.None); }
        }

        /// <summary>
        /// Flag indicates that paragraph properties has default run properties set in the document.
        /// Sometimes paragraph properties has empty DefaultRunProperties, that must be preserved.
        /// Flag is used exactly for this.
        /// </summary>
        internal bool HasDefaultRunProperties
        {
            get { return mHasDefaultRunProperties; }
            set { mHasDefaultRunProperties = value; }
        }

        /// <summary>
        /// Returns <c>true</c> if no properties are defined in this collection.
        /// </summary>
        internal bool IsEmpty
        {
            get { return (Count == 0) && !HasDefaultRunProperties && (DefaultRunProperties.Count == 0); }
        }

        private static readonly IDmlHierarchicalPropertyBagParentProvider gDefaultParentBagProvider =
            new DmlHierarchicalPropertyBagParentContainer(DmlParagraphPropertiesDefaults.Instance);

        private DmlRunProperties mDefaultRunProperties;
        private bool mHasDefaultRunProperties;
        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        internal class DmlParagraphParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            public DmlParagraphParentBagProvider(
                DmlParagraphProperties paragraphProperties, DmlTextListStyles textListStyles)
            {
                mParentBag = textListStyles.GetTextListStyle(paragraphProperties.Level).mPropertyBag;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return mParentBag; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlParagraphParentBagProvider lhs = (DmlParagraphParentBagProvider)MemberwiseClone();
                if (mParentBag != null)
                    lhs.mParentBag = mParentBag.Clone();
                return lhs;
            }

            private IDmlHierarchicalPropertyBag mParentBag;
        }
    }
}
