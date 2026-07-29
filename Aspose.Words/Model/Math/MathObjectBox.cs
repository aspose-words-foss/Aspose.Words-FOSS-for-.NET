// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the box object, which is used to group components of an equation or other instance of
    /// mathematical text. 
    /// </summary>
    /// <remarks>
    /// A boxed object can (for example) serve as an operator emulator with or without an
    /// alignment point, serve as a line break point, have associated argSz, or be grouped such as not to allow line
    /// breaks within. If boxPr is omitted, all properties will be “false” by default.
    /// </remarks>
    internal class MathObjectBox : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Box; }
        }

        /// <summary>
        /// Specifies the alignment property on this box object. It is utilized only when the box is designated as
        /// an operator emulator. When true, this operator emulator serves as an alignment point; that is, designated
        /// alignment points in other equations can be aligned with it.
        /// Default: false.
        /// </summary>
        internal bool IsAlignmentPoint
        {
            get { return (bool)FetchAttr(MathAttr.IsAlignmentPoint); }
            set { SetAttr(MathAttr.IsAlignmentPoint, value, value); }
        }
        
        /// <summary>
        /// Specifies whether there is a manual line break at the start of this box.
        /// Can be null, meaning that no linebreak is associated with this run.
        /// Default: null
        /// </summary>
        internal MathLineBreak LineBreak
        {
            get { return (MathLineBreak)FetchAttr(MathAttr.LineBreak); }
            set { SetAttr(MathAttr.LineBreak, value, value != null); }
        }
        
        /// <summary>
        /// Specifies the differential property on this box. When true, the box acts as a differential (e.g., dx in an integrand), 
        /// and receives the appropriate horizontal spacing for the mathematical differential. 
        /// Defaut: false.
        /// </summary>
        internal bool IsDifferential
        {
            get { return (bool)FetchAttr(MathAttr.IsDifferential); }
            set { SetAttr(MathAttr.IsDifferential, value, value); }
        }
        
        /// <summary>
        /// Specifies the "unbreakable" property on the Box object box. When true, no line breaks can
        /// occur within the box. 
        /// Default: false
        /// </summary>
        /// <remarks>
        /// This can be important for operator emulators that consist of more than one binary
        /// operator. When this element is not specified, breaks can occur inside box.
        /// </remarks>
        internal bool IsUnbreakable
        {
            get { return (bool)FetchAttr(MathAttr.NoBreaks); }
            set { SetAttr(MathAttr.NoBreaks, value, value); }
        }

        /// <summary>
        /// Specifies the Operator Emulator property on box. When true, the box and its contents behave
        /// as a single operator and inherit the properties of an operator. This means, for example, that the character can
        /// serve as a point for a line break and can be aligned to other operators.
        /// Default: false.
        /// If this setting is set false, then <see cref="IsAlignmentPoint"/> is also set to false.
        /// </summary>
        /// <remarks>
        /// If this setting is set to true, then <see cref="IsUnbreakable"/> should be ignored.
        /// </remarks>
        internal bool IsOperatorEmulation
        {
            get { return (bool)FetchAttr(MathAttr.IsOpEmu); }
            set
            {
                SetAttr(MathAttr.IsOpEmu, value, value);
                if (!value) // we turn operator emulation off, so AlignmentPoint setting is turned off
                    SetAttr(MathAttr.IsAlignmentPoint, false, false); 
            }
        }
    }
}
