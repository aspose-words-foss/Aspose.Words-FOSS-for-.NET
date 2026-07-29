// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the phantom object. This object has two primary uses: adding the spacing of the phantom
    /// base element e without displaying that base; and suppressing part of the glyph for spacing considerations.
    /// </summary>
    internal class MathObjectPhantom : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Phantom; }
        }

        /// <summary>
        /// Specifies the show property of the phantom phant. 
        /// When false, the phantom base "e" <see cref="MathObjectArgumentBase"/> is hidden. 
        /// Default: true.
        /// </summary>
        internal bool IsShown
        {
            get { return (bool)FetchAttr(MathAttr.IsShown); }
            set { SetAttr(MathAttr.IsShown, value, !value); }
        }
        
        /// <summary>
        /// Specifies that the phantom is transparent for spacing. 
        /// Default: false.
        /// </summary>
        /// <remarks>
        /// This means that if the contents of the
        /// phantom are belonging to a special spacing class (such as binary operators, relational operators, differentials,
        /// etc.), the contents of that phantom are taken into consideration when laying out text. 
        /// If transparency is turned off, then the contents of the phantom are ignored during layout. 
        /// </remarks>
        internal bool IsTransparent
        {
            get { return (bool)FetchAttr(MathAttr.IsTransparent); }
            set { SetAttr(MathAttr.IsTransparent, value, value); }
        }
        
        /// <summary>
        /// Specifies that the phantom has zero ascent.
        /// If true then ascent of the contents of the phantom is not taken into account during layout.
        /// Default: false.
        /// </summary>
        internal bool IsZeroAscent
        {
            get { return (bool)FetchAttr(MathAttr.IsZeroAscent); }
            set { SetAttr(MathAttr.IsZeroAscent, value, value); }
        }
        
        /// <summary>
        /// Specifies that the phantom has zero descent. 
        /// If true, then descent of the contents of the phantom is not taken into account during layout.
        /// Default: false.
        /// </summary>
        internal bool IsZeroDescent
        {
            get { return (bool)FetchAttr(MathAttr.IsZeroDescent); }
            set { SetAttr(MathAttr.IsZeroDescent, value, value); }
        }
        
        /// <summary>
        /// Specifies that the phantom has zero width. 
        /// IF true, then width of the contents of the phantom is not taken into account during layout.
        /// Default: false.
        /// </summary>
        internal bool IsZeroWidth
        {
            get { return (bool)FetchAttr(MathAttr.IsZeroWidth); }
            set { SetAttr(MathAttr.IsZeroWidth, value, value); }
        }
    }
}
