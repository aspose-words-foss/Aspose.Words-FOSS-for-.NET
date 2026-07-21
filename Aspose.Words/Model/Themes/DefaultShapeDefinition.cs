// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/05/2016 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Themes
{
    /// <summary>
    /// This class stores default shape definition of a theme (CT_DefaultShapeDefinition).
    /// </summary>
    /// <remarks>
    /// It is used for elements: 20.1.4.1.27 spDef (Shape Default), 20.1.4.1.20 lnDef (Line Default),
    /// 20.1.4.1.28 txDef (Text Default).
    /// </remarks>
    internal class DefaultShapeDefinition
    {
        /// <summary>
        /// Gets or sets default shape properties.
        /// The property may be <c>null</c> that is its default value.
        /// </summary>
        /// <remarks>
        /// 20.1.2.2.35 spPr (Shape Properties)
        /// This element specifies the visual shape properties that can be applied to a shape.
        /// </remarks>
        internal DefaultShapeProperties ShapePr
        {
            get { return mShapePr; }
            set { mShapePr = value; }
        }

        /// <summary>
        /// Gets or sets default text body properties.
        /// The property may be <c>null</c> that is its default value.
        /// </summary>
        /// <remarks>
        /// 20.4.2.22 bodyPr (Body Properties)
        /// This element defines the body properties for the text body within a shape.
        /// </remarks>
        internal DmlTextBodyProperties TextBodyPr
        {
            get { return mTextBodyPr; }
            set { mTextBodyPr = value; }
        }

        /// <summary>
        /// Gets or sets default style list of text body.
        /// The property may be <c>null</c> that is its default value.
        /// </summary>
        /// <remarks>
        /// 21.1.2.4.12 lstStyle (Text List Styles)
        /// This element specifies the list of styles associated with this body of text.
        /// </remarks>
        internal DmlTextListStyles ListStyles
        {
            get { return mListStyles; }
            set { mListStyles = value; }
        }

        /// <summary>
        /// Gets or sets default shape style.
        /// The property may be <c>null</c> that is its default value.
        /// </summary>
        /// <remarks>
        /// 20.1.2.2.37 style (Shape Style)
        /// This element specifies the style information for a shape.
        /// </remarks>
        internal DmlShapeStyle Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        private DefaultShapeProperties mShapePr;
        private DmlTextBodyProperties mTextBodyPr;
        private DmlTextListStyles mListStyles;
        private DmlShapeStyle mStyle;
    }
}
