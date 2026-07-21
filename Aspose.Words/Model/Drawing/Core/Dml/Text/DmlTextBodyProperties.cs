// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Scene3D;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 21.1.2.1.1 bodyPr (Body Properties)
    /// This element defines the body properties for the text body within a shape.
    /// </summary>
    internal class DmlTextBodyProperties : DmlExtensionListSource
    {
        internal DmlTextBodyProperties()
        {
            mPropertyBag.ParentBagProvider = gDefaultParentBagProvider;
        }

        internal DmlTextBodyProperties Clone()
        {
            DmlTextBodyProperties lhs = (DmlTextBodyProperties)MemberwiseClone();

            if (mPropertyBag != null)
                lhs.mPropertyBag = mPropertyBag.Clone();

            if (mFlatText != null)
                lhs.mFlatText = mFlatText.Clone();

            if (mPresetTextWrap != null)
                lhs.mPresetTextWrap = mPresetTextWrap.Clone();

            if (mAutoFitMode != null)
                lhs.mAutoFitMode = mAutoFitMode.Clone();

            if (mScene3DProperties != null)
                lhs.mScene3DProperties = mScene3DProperties.Clone();

            if (mShape3DProperties != null)
                lhs.mShape3DProperties = mShape3DProperties.Clone();

            if (HasExtensions)
                lhs.Extensions = CloneExtensions();

            return lhs;
        }

        internal void SetParentProperties(DmlTextBodyProperties parentProperties)
        {
            mPropertyBag.ParentBagProvider = new DmlHierarchicalPropertyBagParentContainer(parentProperties.mPropertyBag);
        }

        private void SetProperty(DmlTextBodyPropertiesDefaultsIds propertyId, object value)
        {
            mPropertyBag.SetProperty((int)propertyId, value);
        }

        private object GetProperty(DmlTextBodyPropertiesDefaultsIds propertyId)
        {
            return mPropertyBag.GetProperty((int)propertyId);
        }

        internal object GetDirectProperty(DmlTextBodyPropertiesDefaultsIds propertyId)
        {
            return mPropertyBag.GetDirectProperty((int)propertyId);
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="propertyId">the property id</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlTextBodyPropertiesDefaultsIds propertyId)
        {
            return mPropertyBag.IsPropertySpecified((int)propertyId);
        }

        internal void Remove(DmlTextBodyPropertiesDefaultsIds propertyId)
        {
            mPropertyBag.Remove((int)propertyId);
        }

        internal DmlFlatText FlatText
        {
            get { return mFlatText; }
            set { mFlatText = value; }
        }

        internal DmlPresetTextWarp PresetTextWrap
        {
            get
            {
                if (mPresetTextWrap == null)
                    mPresetTextWrap = new DmlPresetTextWarp();

                return mPresetTextWrap;
            }
            set { mPresetTextWrap = value; }
        }

        internal DmlAutoFitMode AutoFitMode
        {
            get
            {
                if (mAutoFitMode == null)
                    mAutoFitMode = new DmlNoAutoFitMode();

                return mAutoFitMode;
            }
            set { mAutoFitMode = value; }
        }

        internal DmlScene3DProperties Scene3DProperties
        {
            get { return mScene3DProperties; }
            set { mScene3DProperties = value; }
        }

        internal DmlShape3DProperties Shape3DProperties
        {
            get { return mShape3DProperties; }
            set { mShape3DProperties = value; }
        }

      
        /// <summary>
        /// Indicates whether insets are specified.
        /// </summary>
        internal bool HasInsets
        {
            get
            {
                return IsPropertySpecified(DmlTextBodyPropertiesDefaultsIds.LeftInset) ||
                    IsPropertySpecified(DmlTextBodyPropertiesDefaultsIds.TopInset) ||
                    IsPropertySpecified(DmlTextBodyPropertiesDefaultsIds.RightInset) ||
                    IsPropertySpecified(DmlTextBodyPropertiesDefaultsIds.BottomInset);
            }
        }

        /// <summary>
        /// Specifies the bottom inset of the bounding rectangle. 
        /// Insets are used just as internal margins for text boxes within shapes. 
        /// If this attribute is omitted, a value of 45720 or 0.05 inches is implied.
        /// </summary>
        internal int BottomInset
        {
            get { return (int)GetProperty(DmlTextBodyPropertiesDefaultsIds.BottomInset); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.BottomInset, value); }
        }

        /// <summary>
        /// Specifies the top inset of the bounding rectangle. 
        /// Insets are used just as internal margins for text boxes within shapes. 
        /// If this attribute is omitted, then a value of 45720 or 0.05 inches is implied.
        /// </summary>
        internal int TopInset
        {
            get { return (int)GetProperty(DmlTextBodyPropertiesDefaultsIds.TopInset); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.TopInset, value); }
        }

        /// <summary>
        /// Specifies the right inset of the bounding rectangle. 
        /// Insets are used just as internal margins for text boxes within shapes. 
        /// If this attribute is omitted, then a value of 91440 or 0.1 inches is implied.
        /// </summary>
        internal int RightInset
        {
            get { return (int)GetProperty(DmlTextBodyPropertiesDefaultsIds.RightInset); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.RightInset, value); }
        }

        /// <summary>
        /// Specifies the left inset of the bounding rectangle. 
        /// Insets are used just as internal margins for text boxes within shapes. 
        /// If this attribute is omitted, then a value of 91440 or 0.1 inches is implied.
        /// </summary>
        internal int LeftInset
        {
            get { return (int)GetProperty(DmlTextBodyPropertiesDefaultsIds.LeftInset); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.LeftInset, value); }
        }

        /// <summary>
        /// Specifies the anchoring position of the txBody within the shape. 
        /// If this attribute is omitted, then a value of t, or top is implied.
        /// </summary>
        internal DmlTextAnchoringType Anchor
        {
            get { return (DmlTextAnchoringType)GetProperty(DmlTextBodyPropertiesDefaultsIds.Anchor); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.Anchor, value); }
        }

        /// <summary>
        /// Specifies the centering of the text box. The way it works fundamentally 
        /// is to determine the smallest possible "bounds box" for the text and then 
        /// to center that "bounds box" accordingly. This is different than paragraph alignment, 
        /// which aligns the text within the "bounds box" for the text. This flag is compatible 
        /// with all of the different kinds of anchoring. If this attribute is omitted, 
        /// then a value of 0 or false is implied.
        /// </summary>
        internal bool AnchorCenter
        {
            get { return (bool)GetProperty(DmlTextBodyPropertiesDefaultsIds.AnchorCenter); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.AnchorCenter, value); }
        }

        /// <summary>
        /// Specifies that the line spacing for this text body is decided in a simplistic manner using the font scene. 
        /// If this attribute is omitted, a value of 0 or false is implied.
        /// </summary>
        internal bool UseCompatibleLineSpacing
        {
            get { return (bool)GetProperty(DmlTextBodyPropertiesDefaultsIds.UseCompatibleLineSpacing); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.UseCompatibleLineSpacing, value); }
        }

        /// <summary>
        /// Forces the text to be rendered anti-aliased regardless of the font size. 
        /// Certain fonts can appear grainy around their edges unless they are anti-aliased. 
        /// Therefore this attribute allows for the specifying of which bodies of text should always 
        /// be anti-aliased and which ones should not. If this attribute is omitted, then a value of 0 or false is implied.
        /// </summary>
        internal bool ForceAntiAlias
        {
            get { return (bool)GetProperty(DmlTextBodyPropertiesDefaultsIds.ForceAntiAlias); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.ForceAntiAlias, value); }
        }

        /// <summary>
        /// Specifies that text within this textbox is converted text from a WordArt object. 
        /// This is more of a backwards compatibility attribute that is useful to the application 
        /// from a tracking perspective. WordArt was the former way to apply text effects and therefore 
        /// this attribute is useful in document conversion scenarios. 
        /// If this attribute is omitted, then a value of 0 or false is implied.
        /// </summary>
        internal bool FromWordArt
        {
            get { return (bool)GetProperty(DmlTextBodyPropertiesDefaultsIds.FromWordArt); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.FromWordArt, value); }
        }

        /// <summary>
        /// Determines whether the text can flow out of the bounding box horizontally. 
        /// This is used to determine what happens in the event that the text within a shape is too 
        /// large for the bounding box it is contained within. 
        /// If this attribute is omitted, then a value of overflow is implied.
        /// </summary>
        internal DmlTextHorizontalOverflowType TextHorizontalOverflow
        {
            get { return (DmlTextHorizontalOverflowType)GetProperty(DmlTextBodyPropertiesDefaultsIds.TextHorizontalOverflow); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.TextHorizontalOverflow, value); }
        }

        /// <summary>
        /// Specifies the number of columns of text in the bounding rectangle. 
        /// When applied to a text run this property takes the width of the bounding box for the text 
        /// and divides it by the number of columns specified. 
        /// These columns are then treated as overflow containers in that when the previous column 
        /// has been filled with text the next column acts as the repository for additional text. 
        /// When all columns have been filled and text still remains then the overflow properties 
        /// set for this text body are used and the text is reflowed to make room for additional text. 
        /// If this attribute is omitted, then a value of 1 is implied.
        /// </summary>
        internal int ColumnNumber
        {
            get { return (int)GetProperty(DmlTextBodyPropertiesDefaultsIds.ColumnNumber); }
            set
            {
                // In error cases we will render in one column
                SetProperty(DmlTextBodyPropertiesDefaultsIds.ColumnNumber, ((value < 1) ? 1 : value));
            }
        }

        /// <summary>
        /// Specifies the rotation that is being applied to the text within the bounding box. 
        /// If it not specified, the rotation of the accompanying shape is used. 
        /// If it is specified, then this is applied independently from the shape. 
        /// That is the shape can have a rotation applied in addition to the text itself having 
        /// a rotation applied to it. 
        /// If this attribute is omitted, then a value of 0, is implied.
        /// </summary>
        internal DmlAngle Rotation
        {
            get { return (DmlAngle)GetProperty(DmlTextBodyPropertiesDefaultsIds.Rotation); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.Rotation, value); }
        }

        /// <summary>
        /// Specifies whether default rotation is specified.
        /// For some reason MS Word 2013 sets axis labels rotation to -1000 degrease by default.
        /// But this value is treated as zero in MS Word. So to fix the problem simply reset value to zero. 
        /// but to preserve this value upon open/save DOCX this flag is introduced.
        /// </summary>
        internal bool HasDefaultRotation
        {
            get { return (bool)GetProperty(DmlTextBodyPropertiesDefaultsIds.HasDefaultRotation); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.HasDefaultRotation, value); }
        }

        /// <summary>
        /// Specifies whether columns are used in a right-to-left or left-to-right order. 
        /// The usage of this attribute only sets the column order that is used to determine 
        /// which column overflow text should go to next. 
        /// If this attribute is omitted, then a value of left-to-right is implied in which 
        /// case text starts in the leftmost column and flow to the right.
        /// </summary>
        internal DmlTextColumnOrder ColumnOrder
        {
            get { return (DmlTextColumnOrder)GetProperty(DmlTextBodyPropertiesDefaultsIds.ColumnOrder); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.ColumnOrder, value); }
        }

        /// <summary>
        /// Specifies the space between text columns in the text area. 
        /// This should only apply when there is more than 1 column present. 
        /// If this attribute is omitted, then a value of 0 is implied.
        /// </summary>
        internal int SpaceBetweenColumns
        {
            get { return (int)GetProperty(DmlTextBodyPropertiesDefaultsIds.SpaceBetweenColumns); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.SpaceBetweenColumns, value); }
        }

        /// <summary>
        /// Specifies whether the before and after paragraph spacing defined by the user is to be respected. 
        /// While the spacing between paragraphs is helpful, it is additionally useful to be able to set a 
        /// flag as to whether this spacing is to be followed at the edges of the text body, in other words 
        /// the first and last paragraphs in the text body. More precisely since this is a text body level 
        /// property it should only effect the before paragraph spacing of the first paragraph and the after 
        /// paragraph spacing of the last paragraph for a given text body. If this attribute is omitted, 
        /// then a value of 0, or false is implied.
        /// </summary>
        internal bool AreFirstAndLastParagraphsUseSpacing
        {
            get { return (bool)GetProperty(DmlTextBodyPropertiesDefaultsIds.AreFirstAndLastParagraphsUseSpacing); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.AreFirstAndLastParagraphsUseSpacing, value); }
        }

        /// <summary>
        /// Specifies whether text should remain upright, regardless of the transform applied to it and the 
        /// accompanying shape transform. If this attribute is omitted, then a value of 0, or false is implied.
        /// </summary>
        internal bool IsTextUpright
        {
            get { return (bool)GetProperty(DmlTextBodyPropertiesDefaultsIds.IsTextUpright); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.IsTextUpright, value); }
        }

        /// <summary>
        /// Determines if the text within the given text body should be displayed vertically. 
        /// If this attribute is omitted, then a value of horz, or no vertical text is implied.
        /// </summary>
        internal ShapeTextOrientation TextOrientation
        {
            get { return (ShapeTextOrientation)GetProperty(DmlTextBodyPropertiesDefaultsIds.TextOrientation); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.TextOrientation, value); }
        }

        /// <summary>
        /// Determines whether the text can flow out of the bounding box vertically. 
        /// This is used to determine what happens in the event that the text within a shape is too 
        /// large for the bounding box it is contained within. 
        /// If this attribute is omitted, then a value of overflow is implied.
        /// </summary>
        internal DmlTextVerticalOverflowType TextVerticalOverflow
        {
            get { return (DmlTextVerticalOverflowType)GetProperty(DmlTextBodyPropertiesDefaultsIds.TextVerticalOverflow); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.TextVerticalOverflow, value); }
        }

        /// <summary>
        /// Specifies the wrapping options to be used for this text body. 
        /// If this attribute is omitted, then a value of square is implied which wraps the text using the bounding text box. 
        /// </summary>
        internal TextBoxWrapMode TextWrappingType
        {
            get { return (TextBoxWrapMode)GetProperty(DmlTextBodyPropertiesDefaultsIds.TextWrappingType); }
            set { SetProperty(DmlTextBodyPropertiesDefaultsIds.TextWrappingType, value); }
        }

        /// <summary>
        /// Returns <c>true</c> if no properties are defined in this collection.
        /// </summary>
        internal bool IsEmpty
        {
            get
            {
                return
                    (mPropertyBag.Count == 0) &&
                    (mPropertyBag.ExtensionProperties == null) &&
                    (mAutoFitMode == null) &&
                    (mFlatText == null) &&
                    !HasInsets &&
                    (mPresetTextWrap == null) &&
                    (mScene3DProperties == null) &&
                    (mShape3DProperties == null);
            }
        }

        private DmlAutoFitMode mAutoFitMode;
        private DmlFlatText mFlatText;
        private DmlPresetTextWarp mPresetTextWrap;
        private DmlScene3DProperties mScene3DProperties;
        private DmlShape3DProperties mShape3DProperties;

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();

        private static readonly IDmlHierarchicalPropertyBagParentProvider gDefaultParentBagProvider =
            new DmlHierarchicalPropertyBagParentContainer(DmlTextBodyPropertiesDefaults.Instance);
    }
}
