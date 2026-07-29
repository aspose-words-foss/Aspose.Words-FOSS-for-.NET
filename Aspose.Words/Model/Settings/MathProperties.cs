// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Denis Darkin
using System;
using Aspose.Words.Math;

namespace Aspose.Words.Settings
{   
    /// <summary>
    /// Specifies the document-level properties for all math markup in the document.
    /// </summary>
    internal class MathProperties
    {
        internal MathProperties Clone()
        {
            return (MathProperties)MemberwiseClone();
        }
        
        /// <summary>
        /// Returns true if any property contained here has non-default value.
        /// </summary>
        internal bool IsNonDefault
        {
            get
            {
                return ((mBrkBin != MathBreakOnBinary.Default) ||
                        (mBrkBinSub != MathBreakOnBinarySubtraction.Default) ||
                        (mDefJc != OfficeMathJustification.Default) ||
                        !mDispDef ||
                        (mIntLim != MathLimitLocation.SubscriptSuperscript) ||
                        (mInterSp != 0) ||
                        (mIntraSp != 0) ||
                        (mLMargin != 0) || 
                        (mRMargin != 0) || 
                        (mMathFont != "") || 
                        (mNaryLim != MathLimitLocation.UnderOver) || 
                        (mPostSp != 0) ||
                        (mPreSp != 0) ||
                         mSmallFrac ||
                        (mWrapIndent != DefaultWrapIndent) || 
                         mWrapRight);
            }
        }
        
        /// <summary>
        /// Specifies how binary operators are treated when they coincide with a line break.
        /// Default: <see cref="MathBreakOnBinary.Default"/>
        /// </summary>
        internal MathBreakOnBinary BreakOnBinary
        {
            get { return mBrkBin; }
            set { mBrkBin = value; }
        }
        
        /// <summary>
        /// Specifies how the subtraction operator is treated when it coincides with a line break, when
        /// <see cref="BreakOnBinary"/> is set to repeat. 
        /// By default the subtraction operator is repeated before and after the break.
        /// </summary>
        internal MathBreakOnBinarySubtraction BreakOnBinarySubtraction
        {
            get { return mBrkBinSub; }
            set { mBrkBinSub = value; }
        }
        
        /// <summary>
        /// Specifies the default justification of display math, at the document level. Individual instances of
        /// mathematical text can overrule the default setting.
        /// Default is <see cref="OfficeMathJustification.Default"/>
        /// </summary>
        internal OfficeMathJustification DefaultJustification
        {
            get { return mDefJc; }
            set { mDefJc = value; }
        }
        
        /// <summary>
        /// Specifies the document-level property to overwrite paragraph settings for mathematical text.
        /// Default is true and special math settings are applied.
        /// </summary>
        /// <remarks>
        /// When this value is false, then MS Word ignores the following settings:
        /// <see cref="DefaultJustification"/>, <see cref="RightMargin"/>, <see cref="LeftMargin"/>,
        /// <see cref="WrapIndent"/> and <see cref="WrapRight"/>.
        /// Instead paragraph-level setting are used.
        /// </remarks>
        internal bool UseDisplayMathDefaults
        {
            get { return mDispDef; }
            set { mDispDef = value; }   
        }
        
        /// <summary>
        /// Specifies spacing between equations, expressions, or other instances of mathematical text within a
        /// display math paragraph, in twips.
        /// Default 0, meaning no spacing applied between equations.
        /// </summary>
        internal int InterEquationSpacing
        {
            get { return mInterSp; }
            set { mInterSp = value; }    
        }
        
        
        /// <summary>
        /// Specifies the spacing between adjacent display math paragraphs, in twips. 
        /// Default 0, meaning no spacing is applied between adjacent math paragraphs.
        /// </summary>
        internal int IntraEquationSpacing
        {
            get { return mIntraSp; }
            set { mIntraSp = value; }
        }
        
        /// <summary>
        /// Specifies the document setting for the default placement of integral limits, when converted from a
        /// linear form to a two-dimensional output (professional form).
        /// Default: <see cref="MathLimitLocation.SubscriptSuperscript"/>
        /// </summary>
        internal MathLimitLocation IntegralLimitLocation
        {
            get { return mIntLim; }
            set
            {
                if (value == MathLimitLocation.Undefined)
                    throw new ArgumentException("This enum value is not allowed.");
                mIntLim = value;
            }
        }

        /// <summary>
        /// Specifies the left margin for math, in twips. 
        /// Default: 0.
        /// </summary>
        internal int LeftMargin
        {
            get { return mLMargin; }
            set { mLMargin = value; }
        }

        /// <summary>
        /// Specifies the right margin for math, in twips. 
        /// Default is 0.
        /// </summary>
        internal int RightMargin
        {
            get { return mRMargin; }
            set { mRMargin = value; }
        }
        
        /// <summary>
        /// Specifies the default math font to be used in the document. If not specified, font
        /// substitution should be used to determine the most appropriate font for use throughout the document.
        /// Default: "" - empty string.
        /// </summary>
        /// <remarks>
        /// It seems that for Office documents default font is often"Cambria Math", 
        /// but for now we will consider empty string to be default.
        /// </remarks>
        internal string DefaultFont
        {
            get { return mMathFont; }
            set
            {
                if (StringUtil.HasChars(value))
                    mMathFont = value;
            }
        }
        
        /// <summary>
        /// Specifies the document setting for the default placement of n-ary limits other than integrals
        /// when converted from a built down form to a two-dimensional output (professional form).
        /// Default: <see cref="MathLimitLocation.UnderOver"/>
        /// </summary>
        internal MathLimitLocation NaryLimitLocation
        {
            get { return mNaryLim; }
            set
            {
                if (value == MathLimitLocation.Undefined)
                    throw new ArgumentException("This enum value is not allowed.");
                mNaryLim = value;
            }
        }
        
        /// <summary>
        /// Specifies the spacing after a math paragraph, in twips. 
        /// Default 0. 
        /// </summary>
        internal int PostParagraphSpacing
        {
            get { return mPostSp; }
            set { mPostSp = value; }
        }
        
        /// <summary>
        /// Specifies the spacing before a math paragraph, in twips. 
        /// Default 0.
        /// </summary>
        internal int PreParagraphSpacing
        {
            get { return mPreSp; }
            set { mPreSp = value; }
        }
        
        /// <summary>
        /// This element specifies a reduced fraction size display math, such that the numerator and denominator are
        /// written in script size instead of at the size of regular text.
        /// Default value: false, meaning that this option is not applied.
        /// </summary>
        internal bool IsSmallFraction
        {
            get { return mSmallFrac; }
            set { mSmallFrac = value; }
        }
        
        /// <summary>
        /// Specifies the indent of the wrapped line of an instance of mathematical text, in twips.
        /// The default: 1440 twips (or 1 inch).
        /// </summary>
        /// <remarks>
        /// The line or lines of a wrapped instance of mathematical text after the line break can either be indented by a
        ///  specified amount from the left margin, or right aligned. 
        /// </remarks>
        internal int WrapIndent
        {
            get { return mWrapIndent; }
            set { mWrapIndent = value; }
        }
        
        /// <summary>
        /// Specifies the right justification of the wrapped line of an instance of mathematical text.
        /// Default: false. 
        /// </summary>
        /// <remarks>
        /// The line or lines of a wrapped instance of mathematical text after the line break can 
        /// either be indented by a specified amount from the left margin, or right aligned. 
        /// If this element is present, the continuation is right aligned.
        /// </remarks>
        internal bool WrapRight
        {
            get { return mWrapRight; }
            set { mWrapRight = value; }
        }

        /// <summary>
        /// (Break on Binary Operators) §22.1.2.16
        /// </summary>
        private MathBreakOnBinary mBrkBin = MathBreakOnBinary.Default;

        /// <summary>
        /// (Break on Binary Subtraction) §22.1.2.17
        /// </summary>
        private MathBreakOnBinarySubtraction mBrkBinSub = MathBreakOnBinarySubtraction.Default;

        // (Default Justification) §22.1.2.25
        private OfficeMathJustification mDefJc = OfficeMathJustification.Default;
        
        /// <summary>
        /// (Use Display Math Defaults) §22.1.2.30
        /// </summary>
        private bool mDispDef = true;

        /// <summary>
        /// (Integral Limit Locations) §22.1.2.49
        /// </summary>
        private MathLimitLocation mIntLim = MathLimitLocation.SubscriptSuperscript;

        /// <summary>
        /// (Inter-Equation Spacing) §22.1.2.48
        /// </summary>
        private int mInterSp; 

        /// <summary>
        /// (Intra-Equation Spacing) §22.1.2.50
        /// </summary>
        private int mIntraSp;

        /// <summary>
        /// (Left Margin) §22.1.2.59
        /// </summary>
        private int mLMargin;

        /// <summary>
        /// (Right Margin) §22.1.2.90
        /// </summary>
        private int mRMargin;

        /// <summary>
        /// (Math Font) §22.1.2.61
        /// </summary>
        private string mMathFont = "";
        
        /// <summary>
        /// (n-ary Limit Location) §22.1.2.71
        /// </summary>
        private MathLimitLocation mNaryLim = MathLimitLocation.UnderOver;

        /// <summary>
        /// (Post-Paragraph Spacing) §22.1.2.85
        /// </summary>
        private int mPostSp;

        /// <summary>
        /// (Pre-Paragraph Spacing) §22.1.2.86
        /// </summary>
        private int mPreSp;

        /// <summary>
        /// (Small Fraction) §22.1.2.98
        /// </summary>
        private bool mSmallFrac;

        /// <summary>
        /// (Wrap Indent) §22.1.2.120
        /// </summary>
        private int mWrapIndent = DefaultWrapIndent;
        
        /// <summary>
        /// (Wrap Right) §22.1.2.121
        /// </summary>
        private bool mWrapRight;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultWrapIndent = 1440;
    }
}
